using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace VoiceChat.Behaviour
{
    public class VoiceChatRecorder : ServiceBase
    {
        public AudioClip audioClip
        {
            get { return clip; }
        }

        public event Action StartedRecording;

        [SerializeField] bool autoDetectSpeaking = false;

        [SerializeField] int autoDetectIndex = 4;

        [SerializeField] float forceTransmitTime = 4f;

        ulong packetId;
        int previousPosition = 0;
        int sampleIndex = 0;
        string device = null;
        AudioClip clip = null;
        bool transmitToggled = false;
        bool recording = false;
        float forceTransmit = 0f;
        int recordFrequency = 0;
        int recordSampleSize = 0;
        int targetFrequency = 0;
        int targetSampleSize = 0;
        float[] fftBuffer = null;
        float[] sampleBuffer = null;

        private VoiceChatCircularBuffer<float[]> previousSampleBuffer = new VoiceChatCircularBuffer<float[]>(5);

        public bool AutoDetectSpeech
        {
            get { return autoDetectSpeaking; }
            set { autoDetectSpeaking = value; }
        }

        public string Device
        {
            get { return device; }
            set
            {
                if (value != null && !Microphone.devices.Contains(value))
                {
                    Debug.LogError(value + " is not a valid microphone device");
                    return;
                }

                device = value;
            }
        }

        public bool HasDefaultDevice
        {
            get { return device == null; }
        }

        public bool HasSpecificDevice
        {
            get { return device != null; }
        }

        public bool IsTransmitting
        {
            get { return transmitToggled || forceTransmit > 0; }
        }

        public bool IsRecording
        {
            get { return recording; }
        }

        public string[] AvailableDevices
        {
            get { return Microphone.devices; }
        }

        public event System.Action<VoiceChatPacket> NewSample;

        public void Loaded()
        {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);

            string[] Devices = Microphone.devices;

            if (Devices.Length > 0)
            {
                Device = Microphone.devices[0];
            }

        }


        private void OnEnable()
        {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);

            StartRecording();
        }

        private void OnDisable()
        {
           
        }

        private void OnDestroy()
        {
        }

        private void Update()
        {
            if (!recording)
            {
                return;
            }

            forceTransmit -= Time.deltaTime;

            bool transmit = transmitToggled;

            int currentPosition = Microphone.GetPosition(Device);

            // This means we wrapped around
            if (currentPosition < previousPosition)
            {
                while (sampleIndex < recordFrequency)
                {
                    ReadSample(transmit);
                }

                sampleIndex = 0;
            }

            // Read non-wrapped samples
            previousPosition = currentPosition;

            while (sampleIndex + recordSampleSize <= currentPosition)
            {
                ReadSample(transmit);
            }
        }

        /// <summary>
        /// 重新取样
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        private void Resample(float[] src, float[] dst)
        {
            if (src.Length == dst.Length)
            {
                Array.Copy(src, 0, dst, 0, src.Length);
            }
            else
            {
                //TODO: Low-pass filter 低通滤波器
                float rec = 1.0f / dst.Length;

                for (int i = 0; i < dst.Length; ++i)
                {
                    var interp = (int)(rec * i * src.Length);

                    dst[i] = src[interp];
                }
            }
        }

        /// <summary>
        /// 读取样本
        /// </summary>
        /// <param name="transmit"></param>
        private void ReadSample(bool transmit)
        {
            // 提取数据， sampleIndex特定位置
            clip.GetData(sampleBuffer, sampleIndex);

            // 抓住一个新的样品缓冲区
            float[] targetSampleBuffer = VoiceChatFloatPool.Instance.Get();

            // 将我们的真实样本重新取样到缓冲区中
            Resample(sampleBuffer, targetSampleBuffer);

            // Forward index
            sampleIndex += recordSampleSize;

            // 最高自动检测频率
            float freq = float.MinValue;
            int index = -1;

            // 自动检测语音，但如果我们要按键传输，就不需要检测了
            if (autoDetectSpeaking && !transmit)
            {
                // 清除FFT缓冲区
                for (int i = 0; i < fftBuffer.Length; ++i)
                {
                    fftBuffer[i] = 0;
                }

                //  复制到FFT缓冲区
                Array.Copy(targetSampleBuffer, 0, fftBuffer, 0, targetSampleBuffer.Length);

                // 应用FFT
                Fourier.FFT(fftBuffer, fftBuffer.Length / 2, FourierDirection.Forward);

                // 获取最高频率
                for (int i = 0; i < fftBuffer.Length; ++i)
                {
                    if (fftBuffer[i] > freq)
                    {
                        freq = fftBuffer[i];
                        index = i;
                    }
                }
            }

            // 如果我们有一个事件，并且 
            if (NewSample != null && (transmit || forceTransmit > 0 || index >= autoDetectIndex))
            {
                // 如果我们自动检测到声音，就强行录音一会儿
                if (index >= autoDetectIndex)
                {
                    if (forceTransmit <= 0)
                    {
                        while (previousSampleBuffer.Count > 0)
                        {
                            TransmitBuffer(previousSampleBuffer.Remove());
                        }
                    }

                    forceTransmit = forceTransmitTime;
                }

                TransmitBuffer(targetSampleBuffer);
            }
            else
            {
                if (previousSampleBuffer.Count == previousSampleBuffer.Capacity)
                {
                    VoiceChatFloatPool.Instance.Return(previousSampleBuffer.Remove());
                }

                previousSampleBuffer.Add(targetSampleBuffer);
            }

        }

        private void TransmitBuffer(float[] buffer)
        {
            // Compress into packet
            var packet = VoiceChatUtils.Compress(buffer);

            packet.PacketId = ++packetId;

            // Raise event
            NewSample?.Invoke(packet);
        }

        public bool StartRecording()
        {
            if (recording)
            {
                Debug.LogError("Already recording");
                return false;
            }

            targetFrequency = VoiceChatSettings.Instance.Frequency;
            targetSampleSize = VoiceChatSettings.Instance.SampleSize;

            int minFreq;
            int maxFreq;

            Microphone.GetDeviceCaps(Device, out minFreq, out maxFreq);

            recordFrequency = minFreq == 0 && maxFreq == 0 ? 44100 : maxFreq;

            recordSampleSize = targetSampleSize * recordFrequency / targetFrequency;

            clip = Microphone.Start(Device, true, 1, recordFrequency);

            sampleBuffer = new float[recordSampleSize];

            fftBuffer = new float[VoiceChatUtils.ClosestPowerOfTwo(targetSampleSize)];

            recording = true;

            if (StartedRecording != null)
            {
                StartedRecording();
            }

            return recording;
        }

        public void StopRecording()
        {
            clip = null;
            recording = false;
        }

        public void StartTransmit()
        {
            transmitToggled = true;
        }

        public void StopTransmit()
        {
            transmitToggled = false;
        }

        public void Setup()
        {
        }

        public IEnumerator SetupAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
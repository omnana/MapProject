using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace VoiceChat.Behaviour
{
    public class VoiceChatPlayer : MonoBehaviour
    {
        public event Action PlayerStarted;

        private float lastTime = 0;
        private double played = 0;
        private double received = 0;
        private int index = 0;
        private float[] data;
        private float playDelay = 0;
        private bool shouldPlay = false;
        private float lastRecvTime = 0;

        NSpeex.SpeexDecoder speexDec = new NSpeex.SpeexDecoder(NSpeex.BandMode.Narrow);

        [SerializeField] [Range(0f, 5f)] private float playbackDelay = 0f;

        [SerializeField] [Range(1, 32)] private int packetBufferSize = 10;

        private SortedList<ulong, VoiceChatPacket> packetsToPlay = new SortedList<ulong, VoiceChatPacket>();

        public float LastRecvTime
        {
            get { return lastRecvTime; }
        }

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            var size = VoiceChatSettings.Instance.Frequency * 20;

            audioSource.clip = AudioClip.Create("VoiceChat", size, 1, VoiceChatSettings.Instance.Frequency, false);

            data = new float[size];

            audioSource.loop = true;

            if (PlayerStarted != null)
            {
                PlayerStarted();
            }
        }

        private void Update()
        {
            if (audioSource.isPlaying)
            {
                // Wrapped around
                if (lastTime > audioSource.time)
                {
                    played += audioSource.clip.length;
                }

                lastTime = audioSource.time;

                // Check if we've played to far
                if (played + audioSource.time >= received)
                {
                    Stop();
                    shouldPlay = false;
                }
            }
            else
            {
                if (shouldPlay)
                {
                    playDelay -= Time.deltaTime;

                    if (playDelay <= 0)
                    {
                        audioSource.Play();
                    }
                }
            }
        }

        public void Stop()
        {
            audioSource.Stop();
            audioSource.time = 0;
            index = 0;
            played = 0;
            received = 0;
            lastTime = 0;
        }

        public void OnNewSample(VoiceChatPacket newPacket)
        {
            // Set last time we got something
            lastRecvTime = Time.time;
            
            packetsToPlay.Add(newPacket.PacketId, newPacket);

            if (packetsToPlay.Count < 10)
            {
                return;
            }

            var pair = packetsToPlay.First();
            var packet = pair.Value;
            packetsToPlay.Remove(pair.Key);

            // Decompress
            float[] sample = null;
            int length = VoiceChatUtils.Decompress(speexDec, packet, out sample);

            // Add more time to received
            received += VoiceChatSettings.Instance.SampleTime;

            // Push data to buffer
            Array.Copy(sample, 0, data, index, length);

            // Increase index
            index += length;

            // Handle wrap-around
            if (index >= audioSource.clip.samples)
            {
                index = 0;
            }

            // Set data
            audioSource.clip.SetData(data, 0);

            //if (!audioSource.isPlaying)
            //    audioSource.Play();

            // If we're not playing
            if (!audioSource.isPlaying)
            {
                // Set that we should be playing
                shouldPlay = true;

                // And if we have no delay set, set it.
                if (playDelay <= 0)
                {
                    playDelay = (float)VoiceChatSettings.Instance.SampleTime * playbackDelay;
                }
            }

            VoiceChatFloatPool.Instance.Return(sample);
        }
    } 
}
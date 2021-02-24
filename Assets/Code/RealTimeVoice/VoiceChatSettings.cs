
namespace VoiceChat.Behaviour
{
    public class VoiceChatSettings
    {
        public static VoiceChatSettings Instance { get; set; } = new VoiceChatSettings();

        private int frequency = 16384; //8000;

        private int sampleSize = 512;

        private VoiceChatCompression compression = VoiceChatCompression.AlawZlib;

        private VoiceChatPreset preset = VoiceChatPreset.Alaw_Zlib_16k;

        public bool LocalDebug { get; set; }

        public int Frequency
        {
            get { return frequency; }
            private set
            {
                Base.VoiceChatInnerSettings.Frequency = value;
                frequency = value;
            }
        }

        public VoiceChatCompression Compression
        {
            get { return compression; }
            private set { VoiceChatInnerSettings.Compression = value; compression = value; }
        }

        public int SampleSize
        {
            get { return sampleSize; }
            private set { VoiceChatInnerSettings.SampleSize = value; sampleSize = value; }
        }

        public double SampleTime
        {
            get { return (double)SampleSize / (double)Frequency; }
        }

        public VoiceChatPreset Preset
        {
            get { return preset; }
            set
            {
                VoiceChatInnerSettings.Preset = value;
                preset = value;

                switch (preset)
                {
                    case VoiceChatPreset.Speex_8K:
                        Frequency = 8000;
                        SampleSize = 320;
                        Compression = VoiceChatCompression.Speex;
                        break;

                    case VoiceChatPreset.Speex_16K:
                        Frequency = 16000;
                        SampleSize = 640;
                        Compression = VoiceChatCompression.Speex;
                        break;

                    case VoiceChatPreset.Alaw_4k:
                        Frequency = 4096;
                        SampleSize = 128;
                        Compression = VoiceChatCompression.Alaw;
                        break;

                    case VoiceChatPreset.Alaw_8k:
                        Frequency = 8192;
                        SampleSize = 256;
                        Compression = VoiceChatCompression.Alaw;
                        break;

                    case VoiceChatPreset.Alaw_16k:
                        Frequency = 16384;
                        SampleSize = 512;
                        Compression = VoiceChatCompression.Alaw;
                        break;

                    case VoiceChatPreset.Alaw_Zlib_4k:
                        Frequency = 4096;
                        SampleSize = 128;
                        Compression = VoiceChatCompression.AlawZlib;
                        break;

                    case VoiceChatPreset.Alaw_Zlib_8k:
                        Frequency = 8192;
                        sampleSize = 256;
                        Compression = VoiceChatCompression.AlawZlib;
                        break;

                    case VoiceChatPreset.Alaw_Zlib_16k:
                        Frequency = 16384;
                        SampleSize = 512;
                        Compression = VoiceChatCompression.AlawZlib;
                        break;
                }
            }
        }
    }
}
// VoiceChat.Base.VoiceChatFloatPool

public class VoiceChatFloatPool : VoiceChatPool<float[]>
{
    public static readonly VoiceChatFloatPool Instance = new VoiceChatFloatPool();

    private VoiceChatFloatPool()
    {
    }

    protected override float[] Create()
    {
        return new float[VoiceChatInnerSettings.SampleSize];
    }
}

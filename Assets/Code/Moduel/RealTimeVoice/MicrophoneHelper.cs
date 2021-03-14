using UnityEngine;

public class MicrophoneHelper : MonoBehaviour
{
    public static MicrophoneHelper Instance { get; private set; }

    //private VoiceChatRecorder voiceChatRecorder;

    AndroidJavaClass unityPlayer;
    AndroidJavaObject activity;
    private void Awake()
    {
        Instance = this;
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //VoiceChatSettings.Instance.Preset = VoiceChatPreset.Alaw_Zlib_16k;

        //voiceChatRecorder = gameObject.AddComponent<VoiceChatRecorder>();

        //voiceChatRecorder.NewSample += packet => { SocketHelper.Instance.SendVoice(packet); };

    }

    /// 开始播放
    public bool Play()
    {
        //var startRecord = voiceChatRecorder.StartRecording();

        //if (startRecord)
        //{
        //    AudioManager.Instance.PauseBackMusic();

        //    voiceChatRecorder.StartTransmit();
        //}
        activity.Call("onStartRecond");
        return true;
    }

    /// 结束
    public void End()
    {
        activity.Call("onStopRecond");
    }
}

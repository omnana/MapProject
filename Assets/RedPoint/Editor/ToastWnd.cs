using UnityEngine;
using UnityEditor;
using System;

public class ToastWnd : EditorWindow
{
    private string showMsg;

    private Vector2 wndSize = new Vector2(200, 100);

    public Action OkAction;

    public Action CancleAction;

    public void OpenWnd(string content, Vector2 pos)
    {
        autoRepaintOnSceneChange = true;

        showMsg = content;

        Show();

        position = new Rect(pos.x, pos.y, wndSize.x, wndSize.y);
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(showMsg)) return;

        var contentLength = showMsg.Length * 15;

        EditorGUI.LabelField(new Rect((wndSize.x - contentLength) * 0.5f, wndSize.y * 0.5f - 50, contentLength, 50), new GUIContent(showMsg));

        if (GUI.Button(new Rect(wndSize.x * 0.25f - 25, wndSize.y * 0.75f, 50, 25), "确定"))
        {
            OkAction?.Invoke();

            Close();
        }

        if (GUI.Button(new Rect(wndSize.x * 0.5f + 25, wndSize.y * 0.75f, 50, 25), "取消"))
        {
            CancleAction?.Invoke();

            Close();
        }
    }
}
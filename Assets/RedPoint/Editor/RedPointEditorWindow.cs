using UnityEngine;
using UnityEditor;

public class RedPointEditorWindow : EditorWindow
{
    Rect windowRect = new Rect(500, 300, 400, 600);

    Vector3 scrollPos = Vector2.zero;

    private static float s_LineHeight = 20;
    private static float s_Indentation = 20;
    private static float s_CharacterWidth = 20;

    private GUIStyle normalGuiStyle;

    private GUIStyle chooseGuiStyle;

    private int chooseTreeNodeId;

    private void OnEnable()
    {
        normalGuiStyle = new GUIStyle();

        normalGuiStyle.normal.background = null;

        normalGuiStyle.normal.textColor = Color.black;
        
        chooseGuiStyle = new GUIStyle();

        chooseGuiStyle.normal.background = null;

        chooseGuiStyle.normal.textColor = Color.red;
    }

    private void OnDisable()
    {
        var createWnd = GetWindow<CreateTreeNodeWnd>();

        createWnd.Close();
    }

    [MenuItem("Tools/红点树编辑器")]
    private static void Open()
    {
        TreeNodeDataCtrl.Inst.LoadEditorTreeNodeData();

        RedPointEditorWindow test = GetWindow<RedPointEditorWindow>();
        
        test.CreateWnd();
    }

    private void CreateWnd()
    {
        position = windowRect;

        titleContent = new GUIContent("红点树编辑器");

        Show();
    }


    private void OnGUI()
    {
        scrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height - 30),
               scrollPos, new Rect(0, 0, windowRect.width * 0.75f, TreeNodeDataCtrl.Inst.DataRow * 25));

        var root = TreeNodeDataCtrl.Inst.Root;

        if (root == null) return;

        int depth = 0;

        int index = 0;

        DrawNode(root, ref depth, ref index);

        GUI.EndScrollView();

        if (GUI.Button(new Rect(position.width - 120, position.height - 30, 100, 25), "保存配置"))
        {
            TreeNodeDataCtrl.Inst.SaveData();
        }
    }

    private void DrawNode(ITreeNode node, ref int depth, ref int index)
    {
        //var comment = TreeNodeDataCtrl.Inst.GetRedPointComment(node.RedPoint);

        //var content = string.Empty;

        //if (!string.IsNullOrEmpty(comment))
        //{
        //    content = string.Format("{0}  ({1})", node.DisplayString, TreeNodeDataCtrl.Inst.GetRedPointComment(node.RedPoint));
        //}
        //else
        //{
        //    content = node.DisplayString;
        //}

        var content = node.DisplayString;

        var rect = new Rect(
            s_Indentation * depth,
            s_LineHeight * index,
            content.Length * s_CharacterWidth,
            s_LineHeight);

        var style = chooseTreeNodeId == node.Id ? chooseGuiStyle : normalGuiStyle;

        index++;

        var foldoutContent = chooseTreeNodeId != node.Id ? content : string.Empty;

        node.IsOpen = EditorGUI.Foldout(rect, node.IsOpen, new GUIContent(foldoutContent), true);

        rect.x += 14f;
        rect.y += 2.7f;

        var btnContent = chooseTreeNodeId == node.Id ? content : string.Empty;

        if (GUI.Button(rect, btnContent, chooseGuiStyle))
        {
            node.IsOpen = !node.IsOpen;

            chooseTreeNodeId = node.Id;

            var createWnd = GetWindow<CreateTreeNodeWnd>();

            createWnd.OpenWnd(node, this);

            createWnd.position = new Rect(position.x + 410, position.y, 400, 180);
        }

        if (node.IsOpen && node.Children != null)
        {
            depth++;
            foreach (ITreeNode child in node.Children)
            {
                DrawNode(child, ref depth, ref index);
            }
            depth--;
        }
    }
}

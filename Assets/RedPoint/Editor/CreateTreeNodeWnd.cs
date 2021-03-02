using UnityEngine;
using UnityEditor;

public class CreateTreeNodeWnd : EditorWindow
{
    public ITreeNode curNode;

    public ITreeNode parentNode;

    private string nodeName;

    private string childName;

    private string comment;

    public RedPointEditorWindow ParentWnd;

    private Vector2 lastWndSize;

    private void OnDisable()
    {
        var toastWnd = GetWindow<ToastWnd>();

        toastWnd.Close();
    }

    public void OpenWnd(ITreeNode node, RedPointEditorWindow parent)
    {
        ParentWnd = parent;

        titleContent = new GUIContent("创建子节点");

        curNode = node;

        parentNode = curNode.Parent;

        nodeName = null;

        childName = null;

        if (node.Id != 0)
        {
            childName = node.DisplayString;
        }

        comment = TreeNodeDataCtrl.Inst.GetRedPointComment(node.RedPoint);

        Show();
    }

    private void UpdateWndSize()
    {
        lastWndSize.x = position.width;

        lastWndSize.y = position.height;
    }

    private void OnGUI()
    {
        if (curNode == null) return;

        if (curNode.Id == 0)
        {
            EditorGUI.LabelField(new Rect(0, 0, 500, 20),
                new GUIContent(string.Format("当前节点：{0}", curNode.DisplayString)));
        }
        else
        {
            curNode.ReName(EditorGUILayout.TextField("当前节点：", curNode.DisplayString));

            if (!string.IsNullOrEmpty(nodeName) && !nodeName.Equals(curNode.DisplayString))
            {
                ParentWnd.Repaint();
            }

            nodeName = curNode.DisplayString;
        }

        if (!curNode.DisplayString.Equals(TreeNodeDataCtrl.RootName))
        {
            if (GUI.Button(new Rect(lastWndSize.x - 110, 25, 100, 25), "删除当前节点"))
            {
                if (!string.IsNullOrEmpty(curNode.DisplayString))
                {
                    ToastWnd toastWnd = GetWindow<ToastWnd>();

                    toastWnd.OpenWnd("是否删除当前节点", new Vector2(position.x + 100, position.y + 100));

                    toastWnd.OkAction = () =>
                    {
                        TreeNodeDataCtrl.Inst.DeleteNode(curNode);

                        ParentWnd.Repaint();

                        curNode = parentNode;

                        Repaint();
                    };
                }
            }
        }

        GUILayout.Space(50);

        childName = EditorGUILayout.TextField("子节点名称：", childName);

        if(!string.IsNullOrEmpty(childName))
        {
            GUILayout.Space(25);

            comment = EditorGUILayout.TextField("注释：", comment);

            TreeNodeDataCtrl.Inst.AddRedPointComment(curNode.RedPoint, comment);
        }

        GUILayout.Space(50);

        if (GUI.Button(new Rect(lastWndSize.x - 110, 150, 100, 25), "创建子节点"))
        {
            if (string.IsNullOrEmpty(childName))
            {
                ToastWnd toastWnd = GetWindow<ToastWnd>();
                
                toastWnd.OpenWnd("名称不能为空", new Vector2(position.x + 100, position.y + 100));

                return;
            }

            if(TreeNodeDataCtrl.Inst.ContianKey(childName))
            {
                ToastWnd toastWnd = GetWindow<ToastWnd>();

                toastWnd.OpenWnd("该名称已存在", new Vector2(position.x + 100, position.y + 100));

                return;
            }

            TreeNodeDataCtrl.Inst.CreateChildNode(curNode, childName);

            if (ParentWnd != null)
            {
                ParentWnd.Repaint();
            }
        }

        UpdateWndSize();
    }
}

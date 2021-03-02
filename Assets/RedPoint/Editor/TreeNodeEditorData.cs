using System.Collections.Generic;

public interface ITreeNode
{
    ITreeNode Parent { get; set; }

    List<ITreeNode> Children { get; set; }

    int Id { get; set; }

    RedPoint RedPoint { get; set; }

    bool IsOpen { get; set; }

    string DisplayString { get; }

    void AddChild(ITreeNode node);

    void Dispose();

    void ReName(string name);
}

public class TreeNode : ITreeNode
{
    private string m_Message;

    public ITreeNode Parent { get; set; }

    public List<ITreeNode> Children { get; set; }

    public int Id { get; set; }

    public RedPoint RedPoint { get; set; }

    public bool IsOpen { get; set; }

    public string DisplayString => m_Message;

    public TreeNode(string message)
    {
        m_Message = message;
    }

    public void AddChild(ITreeNode node)
    {
        if (Children == null)
        {
            Children = new List<ITreeNode>();
        }

        node.Parent = this;

        Children.Add(node);
    }

    public void Dispose()
    {
        if (Children == null)
        {
            return;
        }

        foreach (ITreeNode node in Children)
        {
            node.Dispose();
        }

        Children.Clear();

        Children = null;
    }

    public void ReName(string name)
    {
        m_Message = name;
    }
}
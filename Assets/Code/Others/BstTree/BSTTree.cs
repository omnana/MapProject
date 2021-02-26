using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 二叉搜索树
/// </summary>
public class BSTTree
{
    public BSTNode root;

    public BSTTree()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="val"></param>
    public void Add(int val)
    {
        if (root == null)
        {
            root = new BSTNode(val);
            return;
        }

        var node = root;

        while (node != null)
        {
            if (node.Value > val)
            {
                if (node.Left == null)
                {
                    node.Left = new BSTNode(val);
                    node.Left.Parent = node;
                    return;
                }

                node = node.Left;
            }
            else
            {
                if (node.Right == null)
                {
                    node.Right = new BSTNode(val);
                    node.Right.Parent = node;
                    return;
                }

                node = node.Right;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void MiddleOrderTraversal()
    {
        MiddleOrderTraversal(root);
    }

    /// <summary>
    /// 中序遍历
    /// </summary>
    /// <param name="node"></param>
    private void MiddleOrderTraversal(BSTNode node)
    {
        if (node == null) return;

        MiddleOrderTraversal(node.Left);

        UnityEngine.Debug.Log(node.Value);

        MiddleOrderTraversal(node.Right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public BSTNode Search(int val)
    {
        var node = root;

        while (node != null)
        {
            if (node.Value == val)
            {
                return node;
            }
            else if (node.Value > val)
            {
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }

        return node;
    }

    public void Remove(int val)
    {
        if (root == null) return;

        var node = root;

        while (node != null)
        {
            if (node.Value == val)
            {
                if (node.Left == null && node.Right == null)
                {
                    if (node.Parent == null)
                    {
                        root = null;
                    }
                    else
                    {
                        if (node.Value < node.Parent.Value)
                        {
                            var l = node.Parent.Left;
                            node.Parent.Left = null;
                            l.Parent = null;
                        }
                        else
                        {
                            var r = node.Parent.Right;
                            node.Parent.Right = null;
                            r.Parent = null;
                        }
                    }
                }
                else if (node.Left == null)
                {
                    if (node.Parent == null)
                    {
                        root = node.Right;
                        root.Parent = null;
                    }
                    else
                    {
                        if (node.Value < node.Parent.Value)
                        {
                            node.Parent.Left = node.Right;
                        }
                        else
                        {
                            node.Parent.Right = node.Right;
                        }

                        node.Right.Parent = node.Parent;
                    }
                }
                else if (node.Right == null)
                {
                    if (node.Parent == null)
                    {
                        root = node.Left;

                        root.Parent = null;
                    }
                    else
                    {
                        if (node.Value < node.Parent.Value)
                        {
                            node.Parent.Left = node.Left;
                        }
                        else
                        {
                            node.Parent.Right = node.Left;
                        }

                        node.Left.Parent = node.Parent;
                    }
                }
                else
                {
                    var m = MinBSTNode(node.Right);

                    node.Value = m.Value;

                    if (m.Right != null)
                    {
                        m.Value = m.Right.Value;

                        m.Right = null;
                    }
                    else
                    {
                        if (node.Value == m.Parent.Value)
                        {
                            node.Left = null;
                        }
                        else
                        {
                            m.Parent.Left = null;
                        }
                    }
                }

                return;
            }
            else if (node.Value < val)
            {
                node = node.Right;
            }
            else
            {
                node = node.Left;
            }
        }
    }

    private BSTNode MinBSTNode(BSTNode node)
    {
        if (node == null) return null;

        if (node.Left == null) return node;

        return MinBSTNode(node.Left);
    }
}
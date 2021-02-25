using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSTNode
{
    public BSTNode(int val)
    {
        Value = val;
    }

    public int Value;

    public BSTNode Left;

    public BSTNode Right;
}

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
        if(root == null)
        {
            root = new BSTNode(val);
            return;
        }

        var node = root;

        while(node != null)
        {
            if(node.Value > val)
            {
                if(node.Left == null)
                {
                    node.Left = new BSTNode(val);
                    break;
                }
                else
                {
                    node = node.Left;
                }
            }
            else
            {
                if (node.Right == null)
                {
                    node.Right = new BSTNode(val);

                    break;
                }
                else
                {
                    node = node.Right;
                }
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

        Debug.Log(node.Value);

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

        if(root.Value == val)
        {
            if(root.Left == null && root.Right ==  null)
            {
                root = null;
                return;
            }
            else if(root.Left != null)
            {

            }
        }

        var node = root;

        while (node != null)
        {
            if(node.Value == val)
            {
            }
            else if (node.Value > val)
            {
                node = node.Right;
            }
            else
            {
                node = node.Left;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AVLNode : MonoBehaviour
{
    public AVLNode(int val)
    {
        Value = val;
    }

    public int Value;

    public AVLNode Left;

    public AVLNode Right;

    public AVLNode Parent;

    public int BalanceFactor;
}

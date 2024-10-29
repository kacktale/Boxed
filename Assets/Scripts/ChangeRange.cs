using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRange : MonoBehaviour
{
    private CinemachineConfiner confiner;
    public PolygonCollider2D Start, Main, Card;
    public int Range;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner>();
    }

    public void ChangedRange()
    {
        if (Range == 1)
        {
            confiner.m_BoundingShape2D = Start;
        }
        else if(Range == 2)
        {
            confiner.m_BoundingShape2D = Main;
        }
        else if(Range == 3)
        {
            confiner.m_BoundingShape2D = Card;
        }
    }
}

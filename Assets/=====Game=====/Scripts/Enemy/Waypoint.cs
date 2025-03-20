using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.Analytics;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private Vector3[] points;

    [SerializeField] private Vector3 _curPos;
    public Vector3[] Points => points;
    public Vector3 CurPos => _curPos;
    private bool _gameStarted;

    private void Start()
    {
       _gameStarted = true;
        _curPos = transform.position;
    }
    public Vector3 GetWaypointPosition(int index)
    {
        return CurPos + Points[index];
    }
        


    private void OnDrawGizmos() { 

        if (!_gameStarted && transform.hasChanged)
            _curPos = transform.position;

        for (int i = 0; i<points.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(points[i] + _curPos, 0.5f);

            if (i < points.Length - 1)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(points[i] + _curPos, points[i + 1] + _curPos);
            }

        }
    }

        
}


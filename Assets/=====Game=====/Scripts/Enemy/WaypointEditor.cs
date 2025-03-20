using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor 
{
    Waypoint Waypoint => target as Waypoint;

    private void OnSceneGUI()
    {
        Handles.color = Color.cyan;
        for (int i = 0;i<Waypoint.Points.Length;i++)
        {
            EditorGUI.BeginChangeCheck();

            // Create handle
            Vector3 currentWaypointPoint = Waypoint.CurPos + Waypoint.Points[i];
            var fmh_19_85_638779429901123555 = Quaternion.identity; Vector3 newWaypointPoint = Handles.FreeMoveHandle(currentWaypointPoint, 0.7f, new Vector3(0.3f, 0.3f, 0.3f), Handles.SphereHandleCap);


            // create text
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = 16;
            textStyle.normal.textColor = Color.white;
            Vector3 textAlligment = Vector3.down * 0.35f + Vector3.right * 0.35f;
            Handles.Label(Waypoint.CurPos + Waypoint.Points[i]+textAlligment,$"{i+1}",textStyle);
            EditorGUI.EndChangeCheck();


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                Waypoint.Points[i] = newWaypointPoint - Waypoint.CurPos;
            }
        }
    }
}

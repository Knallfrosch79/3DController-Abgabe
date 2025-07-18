using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfViewEditor))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        Enemy_FieldOfView fov = (Enemy_FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);
    }
}

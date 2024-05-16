using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathfindingLinkMonoBehaviour))]
public class PathfindingLinkMonoBehaviourEditor : Editor
{
    private void OnSceneGUI() {
        PathfindingLinkMonoBehaviour pathfindingLinkMonoBehaviour = target as PathfindingLinkMonoBehaviour;

        EditorGUI.BeginChangeCheck();
        Vector3 newPositionA = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionA, Quaternion.identity);
        Vector3 newPositionB = Handles.PositionHandle(pathfindingLinkMonoBehaviour.linkPositionB, Quaternion.identity);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(pathfindingLinkMonoBehaviour, "Change Link Position");
            pathfindingLinkMonoBehaviour.linkPositionA = newPositionA;
            pathfindingLinkMonoBehaviour.linkPositionB = newPositionB;

        }
    }
}

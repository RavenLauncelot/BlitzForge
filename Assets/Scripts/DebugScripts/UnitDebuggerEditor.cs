using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitDebuggerEditor
{
    [CustomEditor(typeof(UnitDebugger))]
    public class TeamVisibilityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw default inspector (if needed)
            DrawDefaultInspector();

            UnitDebugger script = (UnitDebugger)target;

            // Show binary representation
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bitmask (Binary):", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel(script.GetBinary(), EditorStyles.textField, GUILayout.Height(18));
        }
    }
}

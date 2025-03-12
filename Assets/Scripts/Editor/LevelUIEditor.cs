using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelUI))]
public class LevelUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelUI levelUI = (LevelUI)target;

        DrawDefaultInspector();

        if (levelUI.isChangeLevelTextColor)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text Color Settings", EditorStyles.boldLabel);

            levelUI.lockTextColor = EditorGUILayout.ColorField("Lock Text Color", levelUI.lockTextColor);
            levelUI.unLockTextColor = EditorGUILayout.ColorField("Unlock Text Color", levelUI.unLockTextColor);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

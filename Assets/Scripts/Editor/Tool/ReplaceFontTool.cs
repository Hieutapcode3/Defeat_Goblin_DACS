using UnityEngine;
using UnityEditor;
using TMPro;

public class ReplaceFontTool : ToolTab
{
    public override string Name => "Replace Font";

    private TMP_FontAsset newFont;

    public override void OnGUI()
    {
        GUILayout.Label("Replace Fonts", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts in Scene"))
        {
            ReplaceFontsInScene();
        }
    }

    private void ReplaceFontsInScene()
    {
        if (newFont == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a new font.", "OK");
            return;
        }

        TextMeshProUGUI[] textComponents = Object.FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (var text in textComponents)
        {
            Undo.RecordObject(text, "Replace Font");
            text.font = newFont;
            EditorUtility.SetDirty(text);
        }

        Debug.Log($"Replaced fonts for {textComponents.Length} TextMeshProUGUI components.");
    }
}

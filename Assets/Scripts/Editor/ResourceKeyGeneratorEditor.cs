using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceKeyGenerator))]
public class ResourceKeyGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Resource Key"))
        {
            GenerateConfigKeyEnum();
        }
    }

    private void GenerateConfigKeyEnum()
    {
        ResourceKeyGenerator generator = (ResourceKeyGenerator)target;
        string enumName = "ResourceKey";
        string filePath = Path.Combine(Application.dataPath, "Scripts/Generated", $"{enumName}.cs");

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("// This file is auto-generated. Do not edit manually.");
            writer.WriteLine("public enum ResourceKey");
            writer.WriteLine("{");

            foreach (string key in generator.keys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    writer.WriteLine($"    {key},");
                }
            }

            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();
        Debug.Log($"ResourceKey enum generated at: {filePath}");
    }
}

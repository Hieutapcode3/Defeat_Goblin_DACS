using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceData))]
public class ResourceDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        if (GUILayout.Button("Generate Data"))
        {
            GenerateConfigKeyScript();
        }
    }

    private void GenerateConfigKeyScript()
    {
        ResourceData configData = (ResourceData)target;

        string filePath = Path.Combine(Application.dataPath, "Scripts/Manager", $"ResourceManager.cs");

        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("// This file is auto-generated. Do not edit manually.");
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine();
            writer.WriteLine("public class ResourceManager : MonoBehaviour");
            writer.WriteLine("{");
            writer.WriteLine("    [SerializeField] private ResourceData ResourceData;");

            foreach (var entry in configData.resourceDatas)
            {
                string typeName = entry.type.ToString();
                if (typeName == "Enum") typeName = entry.enumTypeName;

                if (!string.IsNullOrEmpty(typeName))
                {
                    writer.WriteLine($"    public static {typeName} _{entry.key} " + "{ get; private set; }");
                }
            }

            writer.WriteLine();
            writer.WriteLine("    private void Awake()");
            writer.WriteLine("    {");

            foreach (var entry in configData.resourceDatas)
            {
                string typeName = entry.type.ToString();
                if (typeName == "Enum") typeName = entry.enumTypeName;
                string type = typeof(ResourceKey).GetEnumName(entry.key);
                writer.WriteLine($"        _{entry.key} = ResourceData.GetValueByKey<{typeName}>(ResourceKey.{entry.key});");
            }

            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        AssetDatabase.Refresh();

        Debug.Log($"GameConfig script generated at: {filePath}");
    }

    private string GetCSharpTypeName(ResourceData.ResourceType type)
    {
        return type switch
        {
            ResourceData.ResourceType.String => "string",
            ResourceData.ResourceType.Int => "int",
            ResourceData.ResourceType.Float => "float",
            ResourceData.ResourceType.Bool => "bool",
            ResourceData.ResourceType.Sprite => "Sprite",
            ResourceData.ResourceType.Enum => "string",
            _ => null,
        };
    }
}

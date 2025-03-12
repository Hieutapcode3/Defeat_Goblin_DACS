using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public class MultiToolEditor : EditorWindow
{
    private int selectedTab = 0;
    private List<ToolTab> toolTabs = new List<ToolTab>();

    [MenuItem("Tools/Multi Tool")]
    public static void ShowWindow()
    {
        MultiToolEditor window = GetWindow<MultiToolEditor>("Multi Tool");
        window.InitializeTools();

        window.minSize = new Vector2(720, 540);
        window.maxSize = new Vector2(1024, 720);
        Vector2 screenSize = new Vector2(Display.main.systemWidth, Display.main.systemHeight);
        float centerX = (screenSize.x - window.minSize.x) / 2;
        float centerY = (screenSize.y - window.minSize.y) / 2;
        window.position = new Rect(centerX, centerY, 720, 540);
    }

    private void InitializeTools()
    {
        toolTabs.Clear();
        foreach (Type type in typeof(ToolTab).Assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(ToolTab)) && !type.IsAbstract)
            {
                toolTabs.Add((ToolTab)Activator.CreateInstance(type));
            }
        }
    }

    private void OnGUI()
    {
        if (toolTabs.Count == 0)
        {
            GUILayout.Label("No tools found.", EditorStyles.boldLabel);
            return;
        }

        // Render tabs
        string[] tabNames = new string[toolTabs.Count];
        for (int i = 0; i < toolTabs.Count; i++)
        {
            tabNames[i] = toolTabs[i].Name;
        }

        selectedTab = GUILayout.Toolbar(selectedTab, tabNames);

        GUILayout.Space(10);

        // Render the selected tab's GUI
        toolTabs[selectedTab].OnGUI();
    }


    [MenuItem("Tools/ClearJsonData")]
    public static void ClearJsonData()
    {
        if (File.Exists(SaveSystem.savePath))
        {
            File.Delete(SaveSystem.savePath);
            Debug.Log("Đã xóa file: " + SaveSystem.savePath);
        }
        else
        {
            Debug.LogWarning("File không tồn tại: " + SaveSystem.savePath);
        }
    }
}

public abstract class ToolTab
{
    public abstract string Name { get; }
    public abstract void OnGUI();
}

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReplaceUISpritesTool : ToolTab
{
    private string objectNames = "";
    private Dictionary<string, int> searchResults = new Dictionary<string, int>();
    private Dictionary<string, Sprite> spritesToReplace = new Dictionary<string, Sprite>();
    private Dictionary<string, bool> individualSetNativeSize = new Dictionary<string, bool>();
    private bool globalSetNativeSize = false;

    public override string Name => "Replace UI Sprites";

    public override void OnGUI()
    {
        GUILayout.Label("Replace Sprites for UI Images", EditorStyles.boldLabel);

        GUILayout.Label("Enter GameObject Names (comma-separated):", EditorStyles.label);
        objectNames = EditorGUILayout.TextField(objectNames);

        if (GUILayout.Button("Search UI Images"))
        {
            SearchUIImages();
        }

        GUILayout.Space(10);
        if (searchResults.Count > 0)
        {
            GUILayout.Label("Search Results:", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(120));
            GUILayout.Label("Sprite", GUILayout.Width(120));
            GUILayout.Space(12);
            GUILayout.Label("Set Native Size", GUILayout.Width(120));
            // GUILayout.Space(12);
            // GUILayout.Label("Position (X, Y, Z)", GUILayout.Width(150));
            // GUILayout.Label("Width", GUILayout.Width(50));
            // GUILayout.Label("Height", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            foreach (var kvp in searchResults)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{kvp.Key}: {kvp.Value} found", GUILayout.Width(120));

                if (!spritesToReplace.ContainsKey(kvp.Key))
                    spritesToReplace[kvp.Key] = null;

                spritesToReplace[kvp.Key] = (Sprite)EditorGUILayout.ObjectField(
                    spritesToReplace[kvp.Key],
                    typeof(Sprite),
                    false,
                    GUILayout.Width(120)
                );

                GUILayout.Space(12);

                if (!individualSetNativeSize.ContainsKey(kvp.Key))
                    individualSetNativeSize[kvp.Key] = true;
                //GUILayout.Label("Set Native Size", GUILayout.Width(100));
                individualSetNativeSize[kvp.Key] = EditorGUILayout.Toggle(
                    individualSetNativeSize[kvp.Key],
                    GUILayout.Width(120)
                );

                GUILayout.EndHorizontal();
            }


            GUILayout.Space(12);

            if (GUILayout.Button("Replace Sprites"))
            {
                ReplaceSprites();
            }
        }
    }

    private void SearchUIImages()
    {
        searchResults.Clear();
        spritesToReplace.Clear();

        if (string.IsNullOrEmpty(objectNames))
        {
            Debug.LogWarning("Please enter at least one GameObject name.");
            return;
        }

        string[] names = objectNames.Split(',');
        foreach (string name in names)
        {
            string trimmedName = name.Trim();
            if (string.IsNullOrEmpty(trimmedName)) continue;

            Image[] images = Object.FindObjectsOfType<Image>(true);
            int count = 0;

            foreach (var image in images)
            {
                if (image.gameObject.name == trimmedName)
                {
                    count++;
                }
            }

            if (count > 0)
            {
                searchResults[trimmedName] = count;
                spritesToReplace[trimmedName] = null;
            }
        }

        Debug.Log($"Found {searchResults.Count} unique UI elements.");
    }

    private void ReplaceSprites()
    {
        if (spritesToReplace.Count == 0)
        {
            Debug.LogWarning("No sprites to replace. Please perform a search first.");
            return;
        }

        foreach (var kvp in spritesToReplace)
        {
            if (kvp.Value == null)
            {
                Debug.LogWarning($"Sprite for {kvp.Key} is not assigned. Skipping...");
                continue;
            }

            Image[] images = Object.FindObjectsOfType<Image>(true);

            foreach (var image in images)
            {
                if (image.gameObject.name == kvp.Key)
                {
                    Undo.RecordObject(image, "Replace Sprite");
                    image.sprite = kvp.Value;

                    if (globalSetNativeSize || individualSetNativeSize[kvp.Key])
                    {
                        image.SetNativeSize();
                    }
                    EditorUtility.SetDirty(image);
                }
            }
        }

        Debug.Log("Sprites replaced successfully!");
    }

}

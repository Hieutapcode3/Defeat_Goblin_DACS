using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourceData.ResourceItem))]
public class ResourceItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var keyProp = property.FindPropertyRelative("key");
        var typeProp = property.FindPropertyRelative("type");
        var stringValueProp = property.FindPropertyRelative("stringValue");
        var intValueProp = property.FindPropertyRelative("intValue");
        var floatValueProp = property.FindPropertyRelative("floatValue");
        var boolValueProp = property.FindPropertyRelative("boolValue");
        var spriteValueProp = property.FindPropertyRelative("spriteValue");
        var transformProp = property.FindPropertyRelative("transformValue");
        var gameObjectProp = property.FindPropertyRelative("gameObjectValue");

        var enumTypeNameProp = property.FindPropertyRelative("enumTypeName");
        var enumValueProp = property.FindPropertyRelative("enumValue");

        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        var keyRect = new Rect(position.x, position.y, position.width, singleLineHeight);
        var typeRect = new Rect(position.x, position.y + singleLineHeight + 2, position.width, singleLineHeight);
        EditorGUI.PropertyField(keyRect, keyProp, new GUIContent("Key"));
        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Type"));

        var valueRect = new Rect(position.x, position.y + (singleLineHeight + 2) * 2, position.width, singleLineHeight);

        switch ((ResourceData.ResourceType)typeProp.enumValueIndex)
        {
            case ResourceData.ResourceType.String:
                EditorGUI.PropertyField(valueRect, stringValueProp, new GUIContent("Value (String)"));
                break;

            case ResourceData.ResourceType.Int:
                EditorGUI.PropertyField(valueRect, intValueProp, new GUIContent("Value (Int)"));
                break;

            case ResourceData.ResourceType.Float:
                EditorGUI.PropertyField(valueRect, floatValueProp, new GUIContent("Value (Float)"));
                break;

            case ResourceData.ResourceType.Bool:
                EditorGUI.PropertyField(valueRect, boolValueProp, new GUIContent("Value (Bool)"));
                break;

            case ResourceData.ResourceType.Sprite:
                EditorGUI.PropertyField(valueRect, spriteValueProp, new GUIContent("Value (Sprite)"));
                break;

            case ResourceData.ResourceType.Transform:
                EditorGUI.PropertyField(valueRect, transformProp, new GUIContent("Value (Transform)"));
                break;

            case ResourceData.ResourceType.GameObject:
                EditorGUI.PropertyField(valueRect, gameObjectProp, new GUIContent("Value (GameObject)"));
                break;

            case ResourceData.ResourceType.Enum:
                DrawEnumField(position, singleLineHeight, enumTypeNameProp, enumValueProp);
                break;
        }

        EditorGUI.EndProperty();
    }
    private void DrawEnumField(Rect position, float singleLineHeight, SerializedProperty enumTypeNameProp, SerializedProperty enumValueProp)
    {
        var enumTypeRect = new Rect(position.x, position.y + singleLineHeight * 2 + 4, position.width, singleLineHeight);
        var availableEnums = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => assembly.GetName().Name == "Assembly-CSharp")
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsEnum && string.IsNullOrEmpty(t.Namespace))
            .Select(t => t.FullName)
            .ToList();

        int selectedEnumIndex = Mathf.Max(0, availableEnums.IndexOf(enumTypeNameProp.stringValue));
        selectedEnumIndex = EditorGUI.Popup(enumTypeRect, "Enum Type", selectedEnumIndex, availableEnums.ToArray());

        if (selectedEnumIndex >= 0)
        {
            enumTypeNameProp.stringValue = availableEnums[selectedEnumIndex];
        }
        if (!string.IsNullOrEmpty(enumTypeNameProp.stringValue))
        {
            Type enumType = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name == "Assembly-CSharp")
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.FullName == enumTypeNameProp.stringValue);
            if (enumType != null && enumType.IsEnum)
            {
                var enumValues = Enum.GetNames(enumType);
                int selectedValueIndex = Mathf.Max(0, Array.IndexOf(enumValues, enumValueProp.stringValue));
                var enumValueRect = new Rect(position.x, position.y + singleLineHeight * 3 + 6, position.width, singleLineHeight);
                selectedValueIndex = EditorGUI.Popup(enumValueRect, "Enum Value", selectedValueIndex, enumValues);

                if (selectedValueIndex >= 0)
                {
                    enumValueProp.stringValue = enumValues[selectedValueIndex];
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("type");
        if ((ResourceData.ResourceType)typeProp.enumValueIndex == ResourceData.ResourceType.Enum)
        {
            return EditorGUIUtility.singleLineHeight * 5 + 8;
        }
        return EditorGUIUtility.singleLineHeight * 3 + 4;
    }
}

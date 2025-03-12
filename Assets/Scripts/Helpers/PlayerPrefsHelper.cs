using UnityEngine;

public static class PlayerPrefsHelper
{
    public static void SetInt(PlayerDataKey key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
    }

    public static int GetInt(PlayerDataKey key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key.ToString(), defaultValue);
    }

    public static void SetFloat(PlayerDataKey key, float value)
    {
        PlayerPrefs.SetFloat(key.ToString(), value);
    }

    public static float GetFloat(PlayerDataKey key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key.ToString(), defaultValue);
    }

    public static void SetString(PlayerDataKey key, string value)
    {
        PlayerPrefs.SetString(key.ToString(), value);
    }

    public static string GetString(PlayerDataKey key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key.ToString(), defaultValue);
    }

    public static void DeleteKey(PlayerDataKey key)
    {
        PlayerPrefs.DeleteKey(key.ToString());
    }

    public static bool HasKey(PlayerDataKey key)
    {
        return PlayerPrefs.HasKey(key.ToString());
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }
}

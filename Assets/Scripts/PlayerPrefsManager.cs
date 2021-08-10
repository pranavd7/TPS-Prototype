using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    

    public void LoadPlayerPrefs()
    {
    }

    public static void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public static int GetIfCompleted()
    {
        if (PlayerPrefs.HasKey("Completed"))
        {
            return PlayerPrefs.GetInt("Completed");
        }
        else
        {
            return 0;
        }
    }

    public static void SetIfCompleted(int completed)
    {
        PlayerPrefs.SetInt("Completed", completed);
    }

    public static int GetFullscreen()
    {
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            return PlayerPrefs.GetInt("Fullscreen");
        }
        else
        {
            return 1;
        }
    }

    public static void SetFullscreen(int fullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", fullscreen);
    }
}

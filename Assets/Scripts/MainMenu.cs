using System;
using DayGameplayScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Obsolete("Obsolete")]
    public void PlayNewGame()
    {
        NightShiftPayload.GetOrCreate();
        NightShiftPayload.Instance.ResetPayload();
        SceneManager.LoadScene("DayScene");
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("DayScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
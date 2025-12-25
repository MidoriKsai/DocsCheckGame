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
        NightShiftPayload.Instance.nightCompleted = false; 
        SceneManager.LoadScene("DayScene");
    }
    
    public void PlayGame()
    {
        AudioManager.Instance.PlaySFX("hoveringButtonMusic");
        if (NightShiftPayload.Instance != null)
        {
            NightShiftPayload.Instance.nightCompleted = false;
        }
        SceneManager.LoadScene("DayScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
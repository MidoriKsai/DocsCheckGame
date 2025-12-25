using System;
using DayGameplayScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playGameButton;
    
    [Obsolete("Obsolete")]
    
    private void Start()
    {
        if (playGameButton != null)
        {
            bool hasPlayedBefore = PlayerPrefs.HasKey("FirstLaunch");
            playGameButton.interactable = hasPlayedBefore;
        }
    }
    
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
            var payload = NightShiftPayload.Instance;
            payload.nightCompleted = false;
            payload.EnergyDrinks = payload.energyDrinksAtDayStart;
            payload.warningsToday = payload.warningsAtDayStart;
        }
        SceneManager.LoadScene("DayScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
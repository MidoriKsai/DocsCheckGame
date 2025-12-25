using System;
using System.Collections;
using DayGameplayScripts;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI timeText;

    private float realTimeDuration = 120f;
    private float displayedDuration = 7f * 3600f;
    private float timer = 0f;
    public UnityEvent passed24seconds;
    private bool nightEnded = false;
    
    void Update()
    {
        if (nightEnded) return;
        timer += Time.deltaTime;
        float t = Mathf.Clamp(timer / realTimeDuration, 0f, 1f);
        
        float displayedSeconds = t * displayedDuration;

        int hours = Mathf.FloorToInt(displayedSeconds / 3600f);
        int minutes = Mathf.FloorToInt((displayedSeconds % 3600) / 60f);

        timeText.text = $"{hours:00}:{minutes:00}";
        
        if (timer >= realTimeDuration)
        {
            nightEnded = true;
            NightShiftPayload.Instance.nightCompleted = true;
            SceneManager.LoadScene("DayScene");
        }
    }
    
    IEnumerator TimeCourutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(24f);
            passed24seconds.Invoke();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(TimeCourutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
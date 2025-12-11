<<<<<<< Updated upstream
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
=======
using TMPro;
using UnityEngine;
>>>>>>> Stashed changes
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
<<<<<<< Updated upstream
    public TextMeshProUGUI timeText;

    private float realTimeDuration = 120f;
    private float displayedDuration = 7f * 3600f;
    private float timer = 0f;
    public UnityEvent passed24seconds;
    
    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp(timer / realTimeDuration, 0f, 1f);
        
        float displayedSeconds = t * displayedDuration;

        int hours = Mathf.FloorToInt(displayedSeconds / 3600f);
        int minutes = Mathf.FloorToInt((displayedSeconds % 3600) / 60f);

        timeText.text = $"{hours:00}:{minutes:00}";
        
        if (timer >= realTimeDuration)
=======
    private float gameTimeInMinutes = 0f;
    private float timeScale = 3.5f;
    public TextMeshProUGUI timeText;
    
    void Start()
    {
        UpdateTimeDisplay();
    }
    
    void Update()
    {
        gameTimeInMinutes += Time.deltaTime * timeScale;
        UpdateTimeDisplay();
        
        if (gameTimeInMinutes >= 420f)
>>>>>>> Stashed changes
        {
            SceneManager.LoadScene("DayScene");
        }
    }
    
<<<<<<< Updated upstream
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
=======
    private void UpdateTimeDisplay()
    {
        int hours = Mathf.FloorToInt(gameTimeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(gameTimeInMinutes % 60f);
        
        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes);
    }
}
>>>>>>> Stashed changes

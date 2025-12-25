using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip dayMusic;
    public AudioClip nightMusic;

    [Header("SFX")]
    public AudioSource sfxSource;
    public List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> sfxDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Инициализируем словарь по имени клипа
            sfxDictionary = new Dictionary<string, AudioClip>();
            foreach (var clip in sfxClips)
            {
                sfxDictionary[clip.name] = clip;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "MainMenu" || sceneName == "DayScene")
            PlayMusic(dayMusic);
        else if (sceneName == "NightScene")
            PlayMusic(nightMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.ContainsKey(name) && sfxSource != null)
        {
            sfxSource.PlayOneShot(sfxDictionary[name]);
        }
        else
        {
            Debug.LogWarning("SFX not found or AudioSource missing: " + name);
        }
    }
}
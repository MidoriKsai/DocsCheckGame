using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioChanger : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button soundButton;

    [Header("Button Sprites")]
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    private bool _musicOn;
    private bool _soundOn;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AudioManager.Instance != null);
        
        _musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        _soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        
        ApplyMusicState();
        ApplySoundState();
        
        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
    }

    private void ToggleMusic()
    {
        _musicOn = !_musicOn;
        PlayerPrefs.SetInt("MusicOn", _musicOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMusicState();
    }

    private void ToggleSound()
    {
        _soundOn = !_soundOn;
        PlayerPrefs.SetInt("SoundOn", _soundOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplySoundState();
    }

    private void ApplyMusicState()
    {
        if (AudioManager.Instance?.musicSource == null) return;

        AudioManager.Instance.musicSource.mute = !_musicOn;

        var img = musicButton.GetComponent<Image>();
        img.sprite = _musicOn ? musicOnSprite : musicOffSprite;
    }

    private void ApplySoundState()
    {
        if (AudioManager.Instance?.sfxSource == null) return;

        AudioManager.Instance.sfxSource.mute = !_soundOn;

        var img = soundButton.GetComponent<Image>();
        img.sprite = _soundOn ? soundOnSprite : soundOffSprite;
    }
}

using NightGameplayScripts;
using UnityEngine;
using UnityEngine.UI;

public class CameraChanger : MonoBehaviour
{
    [Header("Камеры и кнопки")]
    public GameObject[] screens;
    public Button[] buttons;

    [Header("Спавнеры ночных предметов")]
    public NightItemSpawner[] itemSpawners;
    public NightCluesPanel nightCluesPanel;

    private int currentCameraIndex = -1;

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => SwitchCamera(index));
        }

        SwitchCamera(0);
    }
    
    
    private void SwitchCamera(int index)
    {
        AudioManager.Instance.PlaySFX("menuButtonMusic");
        if (index == currentCameraIndex) return;

        currentCameraIndex = index;

        for (int i = 0; i < screens.Length; i++)
            screens[i].SetActive(i == index);

        // уведомляем спавнеры
        nightCluesPanel?.OnCameraSwitched();

        Debug.Log($"[CameraChanger] Камера переключена на {index}");
    }
}
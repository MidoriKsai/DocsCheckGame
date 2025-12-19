using NightGameplayScripts;
using UnityEngine;
using UnityEngine.UI;

public class CameraChanger : MonoBehaviour
{
    [Header("Камеры и кнопки")]
    public GameObject[] screens;   // Все камеры
    public Button[] buttons;       // Кнопки переключения

    [Header("Спавнер ночных предметов")]
    public NightItemSpawner itemSpawner;

    private int currentCameraIndex = -1;

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // для замыкания
            buttons[i].onClick.AddListener(() => SwitchCamera(index));
        }

        // Установим первую камеру
        SwitchCamera(0);
    }

    private void SwitchCamera(int index)
    {
        if (index == currentCameraIndex) return; // если уже на этой камере, не считаем

        currentCameraIndex = index;

        // Включаем только выбранную камеру
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == index);
        }

        // Каждый раз при смене камеры вызываем спавнер
        if (itemSpawner != null)
        {
            itemSpawner.OnCameraSwitched();
            Debug.Log($"Камера переключена на {index} → переключение засчитано");
        }
    }
}
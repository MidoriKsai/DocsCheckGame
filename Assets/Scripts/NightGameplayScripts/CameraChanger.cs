using UnityEngine;
using UnityEngine.UI;

public class CameraChanger : MonoBehaviour
{
    public GameObject[] screens;
    public Button[] buttons;

    public GuestsItems itemController;

    private void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            buttons[i].onClick.AddListener(() =>
            {
                ChangeCameraView(index);

                if (itemController != null)
                    itemController.OnCameraChanged();
            });
        }

        ChangeCameraView(0);
    }

    public void ChangeCameraView(int index)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == index);
        }
    }
}
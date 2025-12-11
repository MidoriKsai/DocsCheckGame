using UnityEngine;
using UnityEngine.UI;

public class CameraChanger : MonoBehaviour
{
    public GameObject[] screens;
    public Button[] buttons;
    
<<<<<<< Updated upstream
    public GuestsItems itemController;

    void Start()
=======
    private void Start()
>>>>>>> Stashed changes
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
<<<<<<< Updated upstream
            buttons[i].onClick.AddListener(() =>
            {
                ShowScreen(index);
                if (itemController != null)
                    itemController.OnCameraChanged();
            });
        }

        ShowScreen(0);
    }

    void ShowScreen(int index)
    {
        for (int i = 0; i < screens.Length; i++)
        {
            screens[i].SetActive(i == index);
        }
    }
}
=======
            buttons[i].onClick.AddListener(() => ChangeCameraView(index));
        }

        ChangeCameraView(0);
    }
    public void ChangeCameraView(int index)
    {
        for (int i = 0; i < screens.Length; i++)
            screens[i].SetActive(i == index);
    }
}
>>>>>>> Stashed changes

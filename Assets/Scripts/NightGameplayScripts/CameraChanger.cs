using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraChanger : MonoBehaviour
{
    public Button nextButton;
    public Button previousButton;
    private int currentCameraIndex = 0;
    public TextMeshProUGUI cameraNumber;
    
    public GameObject[] cameras;
    void Start()
    {
        ChangeCameraView();
        nextButton.onClick.AddListener(ChangeNextButton);
        previousButton.onClick.AddListener(ChangePreviousButton);
    }

    void ChangeNextButton()
    {
        cameras[currentCameraIndex].SetActive(false);
        currentCameraIndex ++;
        if (currentCameraIndex >= cameras.Length)
            currentCameraIndex = 0;
        ChangeCameraView();
    }

    void ChangePreviousButton()
    {
        cameras[currentCameraIndex].SetActive(false);
        currentCameraIndex --;
        if (currentCameraIndex < 0)
            currentCameraIndex = cameras.Length - 1;
        ChangeCameraView();
    }
    
    void ChangeCameraView()
    {
        cameras[currentCameraIndex].SetActive(true);
        cameraNumber.text = (currentCameraIndex + 1).ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    public Button CameraViewButton;
    public Button ArrestButton;
    
    public GameObject CameraViewPanel;
    public GameObject WorkViewPanel;
        
    void Start()
    {
        CameraViewButton.onClick.AddListener(OpenCameraView);
    }

    void OpenCameraView()
    {
        CameraViewPanel.SetActive(true);
        WorkViewPanel.SetActive(false);
    }
    
}

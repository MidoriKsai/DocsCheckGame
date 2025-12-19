using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public Button closeButton;
    public Button journalButton;
    public Button skipNightButton;
    public GameObject journalPanel;
    public GameObject WorkViewPanel;
    public GameObject CameraViewPanel;
    void Start()
    {
        journalButton.onClick.AddListener(OpenJournal);
        closeButton.onClick.AddListener(SwitchView);
        skipNightButton.onClick.AddListener(LoadDay);
    }
    
    void OpenJournal()
    {
        journalPanel.SetActive(true);
    }

    void SwitchView()
    {
        WorkViewPanel.SetActive(true);
        CameraViewPanel.SetActive(false);
    }

    void LoadDay()
    {
        SceneManager.LoadScene("DayScene");
    }
}

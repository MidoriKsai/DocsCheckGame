using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button pauseButton;
    public Button journalButton;
    public GameObject journalPanel;
    void Start()
    {
        journalButton.onClick.AddListener(OpenJournal);
    }
    
    void OpenJournal()
    {
        journalPanel.SetActive(true);
    }
}

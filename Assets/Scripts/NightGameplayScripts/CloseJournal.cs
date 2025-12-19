using UnityEngine;

public class CloseJournal : MonoBehaviour
{
    public GameObject journalPanel;
    
    public void Close()
    {
        journalPanel.SetActive(false);
    }
}

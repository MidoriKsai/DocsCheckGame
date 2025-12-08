using UnityEngine;
using TMPro;

public class JournalManager : MonoBehaviour
{
    public TextMeshProUGUI journalText;

    public void AddRecord(string text)
    {
        journalText.text += "\n• " + text;
    }
}


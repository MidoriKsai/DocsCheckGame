using UnityEngine;
using UnityEngine.UI;

public class GuestsItems : MonoBehaviour
{
    public GuestData guest;
    public RectTransform panel;
    public Image itemImage;
    public JournalManager journal;

    private Sprite chosenItem;
    private bool itemShown = false;

    private int switchesNeeded;
    private int currentSwitchCount = 0;

    void Start()
    {
        chosenItem = guest.itemSprites[Random.Range(0, guest.itemSprites.Length)];
        switchesNeeded = Random.Range(1, 2);
        itemImage.gameObject.SetActive(false);
        itemImage.GetComponent<Button>().onClick.AddListener(OnItemClicked);
    }

    public void OnCameraChanged()
    {
        currentSwitchCount++;
        
        if (itemShown)
        {
            itemImage.gameObject.SetActive(false);
            itemShown = false;
            return;
        }
        
        if (currentSwitchCount != switchesNeeded)
            return;

        ShowItem();
    }

    void ShowItem()
    {
        itemImage.sprite = chosenItem;
        
        Vector2 randomPos = new Vector2(
            Random.Range(0, panel.rect.width),
            Random.Range(0, panel.rect.height)
        );

        itemImage.rectTransform.anchoredPosition = randomPos;

        itemImage.gameObject.SetActive(true);

        itemShown = true;
    }

    void OnItemClicked()
    {
        journal.AddRecord("вещь кого-то из гостей");
        itemImage.gameObject.SetActive(false);
    }
}

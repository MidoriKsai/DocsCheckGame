using UnityEngine;
using UnityEngine.UI;
using DayGameplayScripts;

public class NightItemSpawner : MonoBehaviour
{
    [Header("UI")]
    public RectTransform itemPanel;  // Панель для спавна предмета
    public Image itemPrefab;          // Префаб предмета
    public NightJournal journal;      // Журнал для записи найденных предметов

    [Header("Настройки")]
    public int minSwitches = 5;
    public int maxSwitches = 20;

    private Image currentItem;
    private Sprite chosenSprite;
    private int switchesNeeded;
    private int currentSwitchCount;

    private DayGameplayScripts.GuestData selectedGuest;

    void Start()
    {
        var payload = NightShiftPayload.Instance;

        if (payload == null)
        {
            Debug.LogError("NightShiftPayload отсутствует");
            return;
        }
        
        if (payload.skippedWanted != null && payload.skippedWanted.Count > 0)
        {
            selectedGuest = payload.skippedWanted[Random.Range(0, payload.skippedWanted.Count)];
        }
        else if (payload.extraWantedWithClues != null)
        {
            selectedGuest = payload.extraWantedWithClues;
            Debug.Log("Нет разыскиваемых — выбран скрытый гость");
        }

        if (selectedGuest == null)
        {
            Debug.LogWarning("Не удалось выбрать гостя для ночной улики");
            return;
        }
        NightShiftPayload.Instance.selectedGuest = selectedGuest;
        selectedGuest.LoadSprites();

        if (selectedGuest.LoadedClues == null || selectedGuest.LoadedClues.Length == 0)
        {
            Debug.LogWarning($"У гостя {selectedGuest.firstName} нет улик");
            return;
        }
        
        chosenSprite = selectedGuest.LoadedClues[Random.Range(0, selectedGuest.LoadedClues.Length)];
        
        switchesNeeded = Random.Range(minSwitches, maxSwitches + 1);
        currentSwitchCount = 0;

        Debug.Log($"Ночная улика гостя {selectedGuest.firstName} появится через {switchesNeeded} переключений камеры");
    }

    public void OnCameraSwitched()
    {
        currentSwitchCount++;
        Debug.Log($"Переключение камеры: {currentSwitchCount}/{switchesNeeded}");
        
        if (currentSwitchCount == switchesNeeded)
        {
            SpawnItem();
        }
    }

    void SpawnItem()
    {
        if (itemPrefab == null || itemPanel == null)
        {
            Debug.LogError("ItemPrefab или ItemPanel не назначены!");
            return;
        }

        currentItem = Instantiate(itemPrefab, itemPanel);
        currentItem.sprite = chosenSprite;
        
        var btn = currentItem.GetComponent<Button>();
        btn.onClick.AddListener(OnItemClicked);

        Debug.Log($"Улика гостя {selectedGuest.firstName} появилась на сцене");
    }

    private void OnItemClicked()
    {
        if (journal != null)
        {
            journal.AddClue(chosenSprite);
            Destroy(currentItem.gameObject);
            Debug.Log($"Улика {selectedGuest.firstName} добавлена в журнал");
        }
    }
}
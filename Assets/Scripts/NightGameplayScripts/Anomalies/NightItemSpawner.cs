using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DayGameplayScripts;

public class NightItemSpawner : MonoBehaviour
{
    [Header("UI")]
    public RectTransform itemPanel;
    public Image itemPrefab;
    public NightJournal journal;

    [Header("Settings")]
    public int minSwitches = 3;
    public int maxSwitches = 20;

    private Image currentItem;
    private Sprite chosenSprite;
    private int switchesNeeded;
    private int currentSwitchCount;
    private bool spawned;
    private float lastSwitchTime;
    private const float switchCooldown = 0.1f;

    public GuestData selectedGuest;

    // 🔹 Инициализация ОДНОГО гостя
    public void Init(GuestData guest)
    {
        selectedGuest = guest;

        selectedGuest.LoadSprites();
        
        

        if (selectedGuest.LoadedClues == null || selectedGuest.LoadedClues.Length == 0)
        {
            Debug.LogWarning($"[NightItemSpawner] У гостя {selectedGuest.firstName} нет улик");
            return;
        }

        // Фильтруем улики, которые уже есть в журнале
        List<Sprite> remainingClues = new List<Sprite>();
        foreach (var clue in selectedGuest.LoadedClues)
        {
            if (!NightShiftPayload.Instance.foundClueSprites.Contains(clue))
                remainingClues.Add(clue);
        }

        if (remainingClues.Count == 0)
        {
            Debug.Log($"[NightItemSpawner] Все улики гостя {selectedGuest.firstName} уже найдены");
            Destroy(gameObject); // <- убираем лишний объект
            return;
        }

        chosenSprite = remainingClues[Random.Range(0, remainingClues.Count)];

        switchesNeeded = Random.Range(minSwitches, maxSwitches + 1);
        currentSwitchCount = 0;
        spawned = false;

        Debug.Log($"[NightItemSpawner] Улика {selectedGuest.firstName} появится через {switchesNeeded} переключений");
    }
    
    public void OnCameraSwitched()
    {
        Debug.Log($"[NightItemSpawner] OnCameraSwitched вызван для {selectedGuest?.firstName}");
    
        if (spawned || selectedGuest == null) return;
        if (Time.time - lastSwitchTime < switchCooldown) return;

        lastSwitchTime = Time.time;

        currentSwitchCount++;
        Debug.Log($"[NightItemSpawner] {selectedGuest.firstName}: {currentSwitchCount}/{switchesNeeded}");

        if (currentSwitchCount >= switchesNeeded)
        {
            SpawnItem();
            spawned = true;
        }
    }

    private void SpawnItem()
    {
        if (itemPrefab == null) Debug.LogError("itemPrefab не назначен!");
        if (itemPanel == null) Debug.LogError("itemPanel не назначен!");

        if (itemPrefab == null || itemPanel == null) return;

        currentItem = Instantiate(itemPrefab, itemPanel);
        currentItem.sprite = chosenSprite;

        RectTransform rt = currentItem.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(100, 100);

        var btn = currentItem.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(OnItemClicked);

        Debug.Log($"Улика {selectedGuest.firstName} появилась: {chosenSprite?.name}");
    }

    private void OnItemClicked()
    {
        journal?.AddClue(chosenSprite);
    
        // Добавляем найденную улику в NightShiftPayload
        if (!NightShiftPayload.Instance.foundClueSprites.Contains(chosenSprite))
        {
            NightShiftPayload.Instance.foundClueSprites.Add(chosenSprite);
            NightShiftPayload.Instance.foundCluesNight++;
        }
            

        Destroy(currentItem.gameObject);

        Debug.Log($"[NightItemSpawner] Улика {selectedGuest.firstName} добавлена в журнал");
    }
}

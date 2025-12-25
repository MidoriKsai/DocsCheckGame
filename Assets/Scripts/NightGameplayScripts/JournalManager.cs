using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DayGameplayScripts;

public class NightJournal : MonoBehaviour
{
    public Transform page1;
    public Transform page2;
    public Image cluePrefab;

    private const int MaxPerPage = 6;

    private int count;
    private HashSet<Sprite> displayedClues = new HashSet<Sprite>();
    private NightShiftPayload _payload;

    private void Awake()
    {
        _payload = NightShiftPayload.Instance;
        if (_payload != null)
        {
            // Отображаем только те улики, которые ещё не были показаны в UI
            foreach (var sprite in _payload.foundClueSprites)
            {
                if (!displayedClues.Contains(sprite))
                {
                    AddClueToUI(sprite, false);
                }
            }
        }
    }

    public void AddClue(Sprite sprite)
    {
        if (displayedClues.Contains(sprite)) return; // предотвращаем дубли
        AddClueToUI(sprite, true);
    }

    private void AddClueToUI(Sprite sprite, bool saveToPayload)
    {
        Transform page = count < MaxPerPage ? page1 : page2;

        var img = Instantiate(cluePrefab, page);
        img.sprite = sprite;
        img.preserveAspect = true;
        img.gameObject.SetActive(true);

        count++;
        displayedClues.Add(sprite);

        if (saveToPayload && _payload != null)
        {
            if (!_payload.foundClueSprites.Contains(sprite))
                _payload.foundClueSprites.Add(sprite);

            _payload.foundCluesNight++;
            Debug.Log($"Найдена улика. Всего за ночь: {_payload.foundCluesNight}");
        }
    }

    // Вызывать при начале нового дня, чтобы сбросить UI, если нужно
    public void ResetJournal()
    {
        foreach (Transform page in new Transform[] { page1, page2 })
        {
            foreach (Transform child in page)
                Destroy(child.gameObject);
        }

        count = 0;
        displayedClues.Clear();
    }
}

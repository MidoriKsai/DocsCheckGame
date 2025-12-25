using UnityEngine;
using UnityEngine.UI;
using DayGameplayScripts;

public class NightJournal : MonoBehaviour
{
    public Transform page1;
    public Transform page2;
    public Image cluePrefab;

    private int count = 0;
    private const int MaxPerPage = 6;

    public void AddClue(Sprite sprite)
    {
        Transform page = count < MaxPerPage ? page1 : page2;

        var img = Instantiate(cluePrefab, page);
        img.sprite = sprite;
        img.preserveAspect = true;
        img.gameObject.SetActive(true);

        count++;
        
        var payload = NightShiftPayload.Instance;
        if (payload != null)
        {
            payload.foundCluesNight++;
            Debug.Log($"Найдена улика. Всего за ночь: {payload.foundCluesNight}");
        }
    }
}
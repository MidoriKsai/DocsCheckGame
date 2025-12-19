using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace DayGameplayScripts
{
    public class WantedListGenerator : MonoBehaviour
    {
        DayManager dayManager;
        public List<GuestData> allGuests = new();
        public List<GuestData> wantedGuests = new();

        [Range(1, 10)] public int countWanted = 6;
        public static bool IsReady { get; private set; }

        private void Awake()
        {
            StartCoroutine(LoadGuestsFromJson());
        }

        private IEnumerator LoadGuestsFromJson()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "guests.json");

            UnityWebRequest request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка загрузки гостей: " + request.error);
                yield break;
            }

            var jsonText = request.downloadHandler.text;
            var wrappedJson = "{\"guests\":" + jsonText + "}";
            var wrapper = JsonUtility.FromJson<GuestListWrapper>(wrappedJson);

            allGuests = wrapper.guests;
            IsReady = true;

            foreach (var guest in allGuests)
                guest.LoadSprites();

            GenerateWantedGuests(); // ❗ теперь вызываем ТУТ
        }


        private void GenerateWantedGuests()
        {
            wantedGuests.Clear();

            if (allGuests.Count == 0)
            {
                Debug.LogWarning("Список гостей пуст!");
                return;
            }

            while (wantedGuests.Count < countWanted)
            {
                var randomGuest = allGuests[Random.Range(0, allGuests.Count)];
                if (!wantedGuests.Contains(randomGuest))
                    wantedGuests.Add(randomGuest);
            }
            Debug.Log("Сгенерировано разыскиваемых: " + wantedGuests.Count);
        }
    }
}
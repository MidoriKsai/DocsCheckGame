using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace DayGameplayScripts
{
    public class WantedListGenerator : MonoBehaviour
    {
        public List<GuestData> allGuests = new();
        public List<GuestData> wantedGuests = new();

        [Range(1, 10)]
        public int countWanted = 6;

        public static bool IsReady { get; private set; }

        private void Awake()
        {
            IsReady = false;
            StartCoroutine(LoadGuestsFromJson());
        }

        private IEnumerator LoadGuestsFromJson()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "guests.json");

            using UnityWebRequest request = UnityWebRequest.Get(path);
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

            foreach (var guest in allGuests)
                guest.LoadSprites();

            if (NightShiftPayload.Instance == null)
            {
                yield break;
            }

            if (NightShiftPayload.Instance.wantedGuests.Count > 0)
            {
                wantedGuests = NightShiftPayload.Instance.wantedGuests;
            }
            else
            {
                GenerateWantedGuests();
                NightShiftPayload.Instance.wantedGuests = new List<GuestData>(wantedGuests);
            }

            IsReady = true;
        }

        private void GenerateWantedGuests()
        {
            wantedGuests.Clear();

            if (allGuests.Count == 0)
            {
                Debug.LogWarning("Список гостей пуст!");
                return;
            }

            var pool = new List<GuestData>(allGuests);

            while (wantedGuests.Count < countWanted && pool.Count > 0)
            {
                var randomIndex = Random.Range(0, pool.Count);
                var guest = pool[randomIndex];
                pool.RemoveAt(randomIndex);

                wantedGuests.Add(guest);
            }
            Debug.Log($"Разыскиваемых сгенерировано: {wantedGuests.Count}");
        }
    }
}
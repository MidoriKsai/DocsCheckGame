using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DayGameplayScripts
{
    public class WantedListGenerator : MonoBehaviour
    {
        DayManager dayManager;
        public List<GuestData> allGuests = new();
        public List<GuestData> wantedGuests = new();

        [Range(1, 10)] public int countWanted = 6;

        private void Awake()
        {
            LoadGuestsFromJson();
            GenerateWantedGuests();
        }

        private void LoadGuestsFromJson()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "guests.json");

            if (!File.Exists(path))
            {
                Debug.LogError("Не найден файл гостей: " + path);
                return;
            }

            var jsonText = File.ReadAllText(path);
            var wrappedJson = "{\"guests\":" + jsonText + "}";
            var wrapper = JsonUtility.FromJson<GuestListWrapper>(wrappedJson);

            allGuests = wrapper.guests;

            // Загрузка спрайтов
            foreach (var guest in allGuests)
                guest.LoadSprites();
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
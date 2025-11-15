using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DayGameplayScripts
{
    public class GuestGenerator : MonoBehaviour
    {
        public WantedListGenerator wantedListGenerator;
        public int guestsPerDay = 10;
        public int totalDays = 5;
        public DayManager dayManager; // ссылка на DayManager для проверки арестованных

        [HideInInspector] public List<GuestData> todayGuests = new();
        private Queue<GuestData> wantedQueue = new(); // очередь уникальных разыскиваемых

        private void Awake()
        {
            InitWantedQueue();
        }

        private void InitWantedQueue()
        {
            wantedQueue.Clear();
            var list = new List<GuestData>(wantedListGenerator.wantedGuests);
            Shuffle(list);
            foreach (var w in list)
                wantedQueue.Enqueue(w);
        }

        public void GenerateGuestsForDay(int dayNumber)
        {
            todayGuests.Clear();

            var allGuests = wantedListGenerator.allGuests
                .Where(g => !dayManager.IsGuestArrested(g))
                .ToList();

            if (allGuests.Count == 0)
            {
                Debug.LogError("Список всех гостей пуст или все арестованы!");
                return;
            }

            // Добавляем настоящего разыскиваемого (уникального)
            GuestData todayWanted = null;
            while (wantedQueue.Count > 0)
            {
                var candidate = wantedQueue.Dequeue();
                if (!dayManager.IsGuestArrested(candidate))
                {
                    todayWanted = candidate;
                    break;
                }
            }

            if (todayWanted != null)
            {
                todayGuests.Add(todayWanted);

                // создаём fake для него (никогда не совпадает с реальным wanted)
                var fake = CreateFakeGuest(todayWanted, allGuests);
                todayGuests.Add(fake);
            }

            // Добавляем случайных дополнительных fake
            int extraFakes = UnityEngine.Random.Range(0, 3);
            for (int i = 0; i < extraFakes; i++)
            {
                var randomWanted = wantedListGenerator.wantedGuests[
                    UnityEngine.Random.Range(0, wantedListGenerator.wantedGuests.Count)
                ];
                if (dayManager.IsGuestArrested(randomWanted)) continue;

                var fakeExtra = CreateFakeGuest(randomWanted, allGuests);
                todayGuests.Add(fakeExtra);
            }

            // Добиваем обычными гостями до guestsPerDay
            var availableGuests = new List<GuestData>(allGuests);
            availableGuests.RemoveAll(g => todayGuests.Contains(g));

            while (todayGuests.Count < guestsPerDay && availableGuests.Count > 0)
            {
                var g = availableGuests[UnityEngine.Random.Range(0, availableGuests.Count)];
                todayGuests.Add(g);
                availableGuests.Remove(g);
            }

            Shuffle(todayGuests);

            Debug.Log($"День {dayNumber}: Wanted={(todayWanted != null ? todayWanted.firstName : "нет")} Total={todayGuests.Count}");
        }

        private GuestData CreateFakeGuest(GuestData baseWanted, List<GuestData> allGuests)
        {
            var otherGuest = allGuests[UnityEngine.Random.Range(0, allGuests.Count)];

            var fake = new GuestData
            {
                id = Guid.NewGuid().ToString(),
                firstName = otherGuest.firstName,
                lastName = otherGuest.lastName,
                age = ModifyAge(baseWanted.age),
                gender = ModifyGender(baseWanted.gender),
                fullBodySprite = baseWanted.fullBodySprite,
                portraitSprite = baseWanted.portraitSprite
            };

            fake.LoadedFullBody = baseWanted.LoadedFullBody;
            fake.LoadedPortrait = baseWanted.LoadedPortrait;

            fake.isFakeByName = false;
            fake.ticket = new TicketData(fake);

            return fake;
        }

        private string ModifyAge(string age)
        {
            if (!int.TryParse(age, out int parsed)) parsed = UnityEngine.Random.Range(20, 50);
            int delta = UnityEngine.Random.Range(-3, 4);
            return Mathf.Clamp(parsed + delta, 18, 80).ToString();
        }

        private string ModifyGender(string gender)
        {
            if (UnityEngine.Random.value < 0.2f)
            {
                if (gender.ToLower().Contains("м")) return "Женский";
                else return "Мужской";
            }
            return gender;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}

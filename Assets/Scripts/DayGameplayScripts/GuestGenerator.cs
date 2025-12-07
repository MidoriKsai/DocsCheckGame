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
        public DayManager dayManager;
        [HideInInspector] public List<GuestData> todayGuests = new();
        private readonly Queue<GuestData> _wantedQueue = new();
        private List<GuestData> _allWantedGuests = new();

        private void Awake()
        {
            InitWantedData();
        }

        private void InitWantedData()
        {
            _allWantedGuests = new List<GuestData>(wantedListGenerator.wantedGuests);
            _wantedQueue.Clear();
            var availableWanted = _allWantedGuests
                .Where(w => !dayManager.IsGuestArrested(w))
                .ToList();
            
            Shuffle(availableWanted);
            foreach (var w in availableWanted)
                _wantedQueue.Enqueue(w);
        }

        public void GenerateGuestsForDay(int dayNumber)
        {
            todayGuests.Clear();
            var availableGuests = wantedListGenerator.allGuests
                .Where(g => !dayManager.IsGuestArrested(g))
                .ToList();

            if (availableGuests.Count == 0)
            {
                Debug.LogError("Нет доступных гостей!");
                return;
            }
            
            if (_wantedQueue.Count > 0)
            {
                var todayWanted = _wantedQueue.Dequeue();
                todayGuests.Add(todayWanted);
            }
            else
            {
                Debug.Log("Нет доступных разыскиваемых в очереди");
            }

            var wantedFakesToCreate = UnityEngine.Random.Range(1, 3);
            var availableForWantedFakes = _allWantedGuests.ToList();

            for (var i = 0; i < wantedFakesToCreate && availableForWantedFakes.Count > 0; i++)
            {
                var wantedForFake = availableForWantedFakes[UnityEngine.Random.Range(0, availableForWantedFakes.Count)];
                var fakeWanted = CreateFakeForWanted(wantedForFake);
                todayGuests.Add(fakeWanted);

                availableForWantedFakes.Remove(wantedForFake);
            }
            
            var regularFakesCount = UnityEngine.Random.Range(1, 3);

            var guestsForFakes = availableGuests
                .Where(g => !todayGuests.Contains(g))
                .ToList();
            
            for (var i = 0; i < regularFakesCount && guestsForFakes.Count > 0; i++)
            {
                var baseGuest = guestsForFakes[UnityEngine.Random.Range(0, guestsForFakes.Count)];
                var fakeGuest = CreateFakeForRegularGuest(baseGuest);
                todayGuests.Add(fakeGuest);
                guestsForFakes.Remove(baseGuest);
            }

            var remainingGuests = availableGuests
                .Where(g => !todayGuests.Contains(g))
                .ToList();
            
            while (todayGuests.Count < guestsPerDay && remainingGuests.Count > 0)
            {
                var guest = remainingGuests[UnityEngine.Random.Range(0, remainingGuests.Count)];
                todayGuests.Add(guest);
                remainingGuests.Remove(guest);
            }

            Shuffle(todayGuests);

            foreach (var guest in todayGuests)
            {
                guest.LoadSprites();
            }

            var wantedCount = todayGuests.Count(g => !g.IsFake && IsInWantedList(g));
            var fakeCount = todayGuests.Count(g => g.IsFake);
            var wantedFakesCount = todayGuests.Count(g => g.IsFake && _allWantedGuests.Any(w => IsSimilarToWanted(g, w)));

            Debug.Log($"=== Итоги дня {dayNumber} ===");
            Debug.Log($"Всего гостей: {todayGuests.Count}");
            Debug.Log($"Разыскиваемых: {wantedCount}");
            Debug.Log($"Фейков всего: {fakeCount}");
            Debug.Log($"Фейков разыскиваемых: {wantedFakesCount}");
            Debug.Log($"Обычных фейков: {fakeCount - wantedFakesCount}");

            foreach (var guest in todayGuests)
            {
                var type = guest.IsFake ? "ФЕЙК" : (IsInWantedList(guest) ? "РАЗЫСКИВАЕМЫЙ" : "ОБЫЧНЫЙ");
                Debug.Log($"{type}: {guest.firstName} {guest.lastName} (isFake: {guest.IsFake})");
            }
        }

        private bool IsInWantedList(GuestData guest)
        {
            return wantedListGenerator.wantedGuests.Any(w => w.id == guest.id);
        }

        private GuestData CreateFakeForWanted(GuestData baseWanted)
        {
            var fake = new GuestData
            {
                id = Guid.NewGuid().ToString(),
                firstName = AlterString(baseWanted.firstName, 1, 2),
                lastName = AlterString(baseWanted.lastName, 1, 2),
                age = baseWanted.age,
                gender = baseWanted.gender,
                fullBodySprite = baseWanted.fullBodySprite,
                portraitSprite = baseWanted.portraitSprite,
                IsFake = true
            };

            if (UnityEngine.Random.value > 0.5f)
            {
                fake.age = ModifyAge(baseWanted.age);
                Debug.Log($"Изменен возраст: {baseWanted.age} → {fake.age}");
            }

            if (UnityEngine.Random.value > 0.5f)
            {
                fake.gender = ModifyGender(baseWanted.gender);
                Debug.Log($"Изменен пол: {baseWanted.gender} → {fake.gender}");
            }

            fake.LoadSprites();
            return fake;
        }

        private GuestData CreateFakeForRegularGuest(GuestData baseGuest)
        {
            var fake = new GuestData
            {
                id = Guid.NewGuid().ToString(),
                firstName = AlterString(baseGuest.firstName, 1, 5),
                lastName = AlterString(baseGuest.lastName, 1, 5),
                age = ModifyAge(baseGuest.age),
                gender = ModifyGender(baseGuest.gender),
                fullBodySprite = baseGuest.fullBodySprite,
                portraitSprite = baseGuest.portraitSprite,
                IsFake = true
            };

            fake.LoadSprites();
            return fake;
        }

        private string AlterString(string input, int minChanges, int maxChanges)
        {
            if (string.IsNullOrEmpty(input) || input.Length < 2)
                return GenerateRandomName(5);

            var chars = input.ToCharArray();
            var changes = UnityEngine.Random.Range(minChanges, maxChanges + 1);
            changes = Math.Min(changes, chars.Length);

            var changedIndexes = new HashSet<int>();
            for (var i = 0; i < changes; i++)
            {
                int index;
                do
                {
                    index = UnityEngine.Random.Range(0, chars.Length);
                } while (changedIndexes.Contains(index));

                chars[index] = GetRandomCyrillicChar();
                changedIndexes.Add(index);
            }

            return new string(chars);
        }

        private static bool IsSimilarToWanted(GuestData guest, GuestData wanted)
        {
            if (wanted == null) return false;

            var firstNameSimilar = guest.firstName.Length >= 2 && wanted.firstName.Length >= 2 &&
                                   guest.firstName[..2] == wanted.firstName[..2];
            var lastNameSimilar = guest.lastName.Length >= 2 && wanted.lastName.Length >= 2 &&
                                  guest.lastName[..2] == wanted.lastName[..2];

            return firstNameSimilar || lastNameSimilar;
        }

        private static string GenerateRandomName(int length)
        {
            var chars = new char[length];
            for (var i = 0; i < length; i++)
                chars[i] = GetRandomCyrillicChar();
            return new string(chars);
        }

        private static char GetRandomCyrillicChar()
        {
            return (char)('а' + UnityEngine.Random.Range(0, 32));
        }

        private static string ModifyAge(string age)
        {
            return int.TryParse(age, out var parsed) ? Mathf.Clamp(parsed + UnityEngine.Random.Range(-2, 3), 18, 80).ToString() : UnityEngine.Random.Range(20, 50).ToString();
        }

        private static string ModifyGender(string gender)
        {
            return gender switch
            {
                "Муж" when UnityEngine.Random.value > 0.5f => "Жен",
                "Жен" when UnityEngine.Random.value > 0.5f => "Муж",
                _ => gender
            };
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var j = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
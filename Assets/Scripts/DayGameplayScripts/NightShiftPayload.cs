using System.Collections.Generic;
using UnityEngine;
using DayGameplayScripts;

namespace DayGameplayScripts
{
    public class NightShiftPayload : MonoBehaviour
    {
        public static NightShiftPayload Instance;

        public List<GuestData> skippedWanted = new List<GuestData>();
        public GuestData extraWantedWithClues;

        public int visitorsToday;
        public int arrestedWantedToday;
        public int warningsToday;
        public int foundCluesNight;
        public int warningBonusPoints;
        public List<GuestData> wantedGuests = new List<GuestData>();
        public GuestData selectedGuest;

        public List<Sprite> foundClueSprites = new List<Sprite>();

        public bool nightCompleted;
        public int currentDay = 1;
        public bool guestDiedTonight = false;

        private int _energyDrinks = 2;
        private const int MaxEnergyDrinks = 2;

        public int EnergyDrinks
        {
            get => _energyDrinks;
            set => _energyDrinks = Mathf.Clamp(value, 0, MaxEnergyDrinks);
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public bool AddEnergyDrink()
        {
            if (_energyDrinks >= MaxEnergyDrinks) return false;
            _energyDrinks++;
            return true;
        }

        public bool UseEnergyDrink()
        {
            if (_energyDrinks <= 0) return false;
            _energyDrinks--;
            return true;
        }

        [System.Obsolete("Obsolete")]
        public static void GetOrCreate()
        {
            if (Instance == null)
            {
                var existing = FindObjectOfType<NightShiftPayload>();
                if (existing != null)
                {
                    Instance = existing;
                }
                else
                {
                    var go = new GameObject("NightShiftPayload");
                    Instance = go.AddComponent<NightShiftPayload>();
                    DontDestroyOnLoad(go);
                }
            }
        }

        public void AddArrestedGuest(GuestData guest)
        {
            if (!wantedGuests.Contains(guest))
                wantedGuests.Add(guest);

            arrestedWantedToday++;
        }

        public void RecordSkippedGuest(GuestData guest)
        {
            if (!skippedWanted.Contains(guest))
                skippedWanted.Add(guest);
        }

        /// <summary>
        /// Переход на следующий день.
        /// Сбрасывает только временные данные текущего дня.
        /// Список разыскиваемых гостей остаётся.
        /// </summary>
        public void NextDay()
        {
            currentDay++;

            // Очистка временных данных дня
            skippedWanted.Clear();
            extraWantedWithClues = null;

            visitorsToday = 0;
            arrestedWantedToday = 0;
            warningsToday = 0;
            foundCluesNight = 0;
            warningBonusPoints = 0;
            foundClueSprites.Clear();
            selectedGuest = null;

            nightCompleted = false;
            guestDiedTonight = false;

            EnergyDrinks = MaxEnergyDrinks;
        }

        /// <summary>
        /// Полный сброс всех данных (например при старте новой игры)
        /// </summary>
        public void ResetPayload()
        {
            skippedWanted.Clear();
            extraWantedWithClues = null;

            visitorsToday = 0;
            arrestedWantedToday = 0;
            warningsToday = 0;
            foundCluesNight = 0;
            warningBonusPoints = 0;

            selectedGuest = null;
            nightCompleted = false;
            guestDiedTonight = false;
        }
    }
}

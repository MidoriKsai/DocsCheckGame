using System;
using System.Collections.Generic;
using UnityEngine;

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
        public int warningsAtDayStart;
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
        public int energyDrinksAtDayStart;
        public event Action OnEnergyDrinksChanged;
        
        public static HashSet<string> ArrestedGuestIds = new();

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
            OnEnergyDrinksChanged?.Invoke();
            return true;
        }

        public bool UseEnergyDrink()
        {
            if (_energyDrinks <= 0) return false;
            _energyDrinks--;
            OnEnergyDrinksChanged?.Invoke();
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

        public void ResetPayload()
        {
            skippedWanted.Clear();
            extraWantedWithClues = null;

            visitorsToday = 0;
            arrestedWantedToday = 0;
            warningsToday = 0;
            foundCluesNight = 0;
            warningBonusPoints = 0;
            
            wantedGuests.Clear();
            foundClueSprites.Clear();
            ArrestedGuestIds.Clear();

            selectedGuest = null;
            nightCompleted = false;
            guestDiedTonight = false;

            currentDay = 1;
            EnergyDrinks = 2;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DayGameplayScripts
{
    public class NightShiftPayload : MonoBehaviour
    {
        public static NightShiftPayload Instance;

        public List<GuestData> skippedWanted = new();
        public GuestData extraWantedWithClues;

        public int visitorsToday;
        public int arrestedWantedToday;
        public int warningsToday;
        public int foundCluesNight;
        public int warningBonusPoints;
        public List<GuestData> wantedGuests = new();
        public GuestData selectedGuest;
        public bool nightCompleted;
        public int currentDay = 1;
        
        // Добавляем энергетики
        private int _energyDrinks = 2; // Начальное количество
        private const int MaxEnergyDrinks = 2;
        
        public int EnergyDrinks
        {
            get => _energyDrinks;
            set => _energyDrinks = Mathf.Clamp(value, 0, MaxEnergyDrinks);
        }

        private void Awake()
        {
            // Убедимся, что Instance существует при старте игры
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
        
        // Метод для добавления энергетика с проверкой лимита
        public bool AddEnergyDrink()
        {
            if (_energyDrinks >= MaxEnergyDrinks)
                return false;
                
            _energyDrinks++;
            return true;
        }
        
        // Метод для использования энергетика
        public bool UseEnergyDrink()
        {
            if (_energyDrinks <= 0)
                return false;
                
            _energyDrinks--;
            return true;
        }
        
        // Статический метод для получения Instance (создает при необходимости)
        [Obsolete("Obsolete")]
        public static void GetOrCreate()
        {
            if (Instance == null)
            {
                // Ищем существующий объект
                var existing = FindObjectOfType<NightShiftPayload>();
                if (existing != null)
                {
                    Instance = existing;
                }
                else
                {
                    // Создаем новый
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
            selectedGuest = null;

            nightCompleted = false;
            currentDay = 1;

            EnergyDrinks = 2;
        }

    }
}
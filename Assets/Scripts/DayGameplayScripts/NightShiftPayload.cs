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
        public static NightShiftPayload GetOrCreate()
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
            return Instance;
        }
    }
}
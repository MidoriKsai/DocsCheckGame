using System.Collections.Generic;
using UnityEngine;

namespace DayGameplayScripts
{
    public partial class NightShiftPayload : MonoBehaviour
    {
        public static NightShiftPayload Instance;

        public List<GuestData> skippedWanted = new();
        public GuestData extraWantedWithClues;

        public int visitorsToday;
        public int arrestedWantedToday;
        public int warningsToday;
        public int foundCluesNight;
        public int warningBonusPoints;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
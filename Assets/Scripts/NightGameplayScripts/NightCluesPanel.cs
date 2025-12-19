using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DayGameplayScripts;

namespace NightGameplayScripts
{
    public class NightCluesPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI dateText;
        public WarningUIController warningUI; // тот же тип, что используется днем


        private NightShiftPayload _payload;
        
        private int _currentDay = 1;
    
        
        private void Start()
        {
            _payload = NightShiftPayload.Instance;
            NightStaticManager.nightCluesPanel = this;
            if (_payload == null)
            {
                Debug.LogError("NightShiftPayload не найден");
                return;
            }

            UpdateDateUI();
            UpdateWarningUI();
        }

        private void UpdateDateUI()
        {
            if (dateText != null)
                dateText.text = $"{_currentDay}";
        }

        private void UpdateWarningUI()
        {
            if (warningUI != null)
                warningUI.SetWarnings(_payload.warningsToday);
            Debug.Log(warningUI);
        }

        
    }
}

using System.Collections.Generic;
using UnityEngine;
using DayGameplayScripts;

namespace DayGameplayScripts
{
    public class ArrestManager : MonoBehaviour
    {
        [SerializeField] private WarningUIController warningUI;
        [SerializeField] private ArrestConfirmPanel confirmPanel;

        public void OnArrestButtonClicked(GuestData guest)
        {
            if (guest == null) return;
            if (NightShiftPayload.ArrestedGuestIds.Contains(guest.id))
            {
                Debug.Log("Гость уже арестован");
                return;
            }

            confirmPanel.Show(guest, this);
        }

        public void ConfirmArrest(GuestData guest)
        {
            var payload = NightShiftPayload.Instance;
            if (payload == null || guest == null) return;

            bool eligibleForBonus =
                (payload.skippedWanted != null && payload.skippedWanted.Contains(guest)) ||
                (payload.extraWantedWithClues != null && payload.extraWantedWithClues == guest);

            if (eligibleForBonus)
            {
                payload.warningsToday = Mathf.Max(0, payload.warningsToday - 2);
                payload.AddEnergyDrink();
                Debug.Log($"{guest.firstName} — бонус: -2 предупреждения");
            }
            else
            {
                payload.warningsToday += 2;
                Debug.Log($"{guest.firstName} — штраф: +2 предупреждения");
            }

            payload.arrestedWantedToday += 1;
            NightShiftPayload.ArrestedGuestIds.Add(guest.id);

            if (warningUI != null)
                warningUI.SetWarnings(payload.warningsToday);
        }
    }
}
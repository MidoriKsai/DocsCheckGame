using UnityEngine;

namespace DayGameplayScripts
{
    public class ArrestManager : MonoBehaviour
    {
        public WarningUIController warningUI;

        public void Arrest(GuestData guest)
        {
            var payload = NightShiftPayload.Instance;
            if (payload == null || guest == null) return;

            bool wasInPark =
                payload.skippedWanted != null &&
                payload.skippedWanted.Contains(guest);

            if (wasInPark)
            {
                payload.warningsToday = Mathf.Max(0, payload.warningsToday - 2);
                Debug.Log($"{guest.firstName} — был в парке → -2 предупреждения");
            }
            else
            {
                payload.warningsToday += 2;
                Debug.Log($"{guest.firstName} — не был в парке → +2 предупреждения");
            }

            UpdateWarningUI(payload);
        }

        private void UpdateWarningUI(NightShiftPayload payload)
        {
            if (warningUI != null)
            {
                // Показываем актуальные предупреждения
                warningUI.SetWarnings(payload.warningsToday + payload.warningBonusPoints);
            }
            else
            {
                Debug.LogWarning("WarningUI не назначен!");
            }
        }
    }
}
using DayGameplayScripts;
using TMPro;
using UnityEngine;

namespace NightGameplayScripts
{
    public class ArrestManager : MonoBehaviour
    {
        [SerializeField] private WarningUIController warningUI;
        [SerializeField] private ArrestConfirmPanel confirmPanel;
        [SerializeField] private ArrestUIManager arrestUIManager;
        public GameObject gameOverPanel;
        public TextMeshProUGUI gameOverText;
        public TextMeshProUGUI gameOverDescriptionText;

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
            if (payload == null && guest == null) return;

            bool wasInParkAtNight =
                payload.skippedWanted.Exists(g => g.id == guest.id) ||
                (payload.extraWantedWithClues != null && payload.extraWantedWithClues.id == guest.id);

            if (wasInParkAtNight)
            {
                payload.warningsToday = Mathf.Max(0, payload.warningsToday - 2);
                payload.AddEnergyDrink();

                payload.skippedWanted.RemoveAll(g => g.id == guest.id);
                payload.arrestedWantedToday += 1;
                Debug.Log($"{guest.firstName} — ВЕРНЫЙ ночной арест");
                NightShiftPayload.ArrestedGuestIds.Add(guest.id);
            }
            else
            {
                payload.warningsToday += 2;
                Debug.Log($"{guest.firstName} — НЕВЕРНЫЙ ночной арест");
                
                if (payload.warningsToday >= 5)
                {
                    GameOver();
                    return;
                }
            }
            warningUI?.SetWarnings(payload.warningsToday);
            arrestUIManager?.ShowArrest(guest.LoadedFullBody);
        }
        
        private void GameOver() 
        {
            gameOverPanel.SetActive(true);
            gameOverDescriptionText.text =
                "Вы получили 5 из 5 предупреждений и не справились со своей служебной задачей.\n" +
                "Парк развлечений омрачен чудовищными инцидентами.\n" +
                "А также он временно закрыт по решению МКА. Ваш доступ аннулирован, Вы уволены!";
            
            gameOverText.text = $"Смен: {NightShiftPayload.Instance.currentDay}/5 \n" +
                                $"Гостей: {NightShiftPayload.Instance.totalGuests}  \n" +
                                $"Выявлено сущностей: {NightShiftPayload.ArrestedGuestIds.Count} \n" +
                                $"Предупреждений: {NightShiftPayload.Instance.warningsToday}/5 \n";
        }
    }
}
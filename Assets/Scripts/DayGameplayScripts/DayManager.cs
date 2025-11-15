using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DayGameplayScripts
{
    public class DayManager : MonoBehaviour
    {
        public GuestGenerator generator;
        public Transform guestSpawnParent;
        public GameObject guestPrefab;
        public GuestTicketUI ticketUI;
        public ActionButtonsUI buttonsUI;
        public TextMeshProUGUI warningText;
        public TextMeshProUGUI endDayStatsText;
        public TextMeshProUGUI dayText;
        public GameObject gameOverPanel;
        public GameObject endDayPanel;
        public GameObject arrestConfirmationPanel;

        private int _currentGuestIndex;
        private int _currentDay = 1;
        private const int _totalDays = 5;

        private int _correctCountDay;       // верные решения за текущий день
        private int _wrongCountTotal;       // суммарные ошибки (только неверное отклонение)
        private int _warnings;              // предупреждения за неверный арест
        private bool _gameOver;

        private GuestController _currentGuestController;
        private GuestData _currentGuest;
        private TicketData _currentTicket;

        public HashSet<string> arrestedGuestIds = new HashSet<string>();

        private void Start()
        {
            buttonsUI.Init(this);
            generator.dayManager = this; // передаём ссылку генератору
            StartDay(_currentDay);
        }

        private void StartDay(int dayNumber)
        {
            Debug.Log($"▶ Начало дня {dayNumber}");

            _currentGuestIndex = 0;
            _correctCountDay = 0; // сброс верных решений за день
            _gameOver = false;

            endDayPanel.SetActive(false);
            gameOverPanel.SetActive(false);

            if (dayText != null)
                dayText.text = $"День {dayNumber}/{_totalDays}";

            generator.GenerateGuestsForDay(dayNumber);
            SpawnNextGuest();
            UpdateWarningUI();
        }

        private void SpawnNextGuest()
        {
            if (_currentGuestIndex >= generator.todayGuests.Count)
            {
                EndDay();
                return;
            }

            var guestData = generator.todayGuests[_currentGuestIndex];
            var guestObj = Instantiate(guestPrefab, guestSpawnParent);
            var guestController = guestObj.GetComponent<GuestController>();

            buttonsUI.SetButtonsInteractable(false);

            guestController.Initialize(guestData);
            _currentGuestController = guestController;

            guestController.OnReadyForDecision = (gc) =>
            {
                _currentGuest = gc.guestData;

                if (_currentGuest.ticket == null)
                    _currentGuest.ticket = new TicketData(_currentGuest);

                _currentTicket = _currentGuest.ticket;

                ticketUI.Show(_currentTicket);

                buttonsUI.SetButtonsInteractable(true);
            };
        }

        public void OnAllowClick() => EvaluateDecision(GuestDecision.Allow);
        public void OnDenyClick() => EvaluateDecision(GuestDecision.Deny);
        public void OnArrestClick()
        {
            if (_gameOver || _currentGuest == null || _currentTicket == null) return;
            arrestConfirmationPanel.SetActive(true);
        }

        public void OnConfirmArrestYes()
        {
            arrestConfirmationPanel.SetActive(false);
            EvaluateDecision(GuestDecision.Arrest);
        }

        public void OnConfirmArrestNo()
        {
            arrestConfirmationPanel.SetActive(false);
        }

        private void EvaluateDecision(GuestDecision decision)
        {
            if (_gameOver || _currentGuest == null || _currentTicket == null) return;

            bool isWanted = generator.wantedListGenerator.wantedGuests.Contains(_currentGuest);
            bool correct = false;

            switch (decision)
            {
                case GuestDecision.Allow:
                    if (!isWanted && !_currentTicket.isFake) correct = true;
                    break;

                case GuestDecision.Deny:
                    if (!isWanted && _currentTicket.isFake) correct = true;
                    break;

                case GuestDecision.Arrest:
                    if (isWanted)
                    {
                        correct = true;
                        arrestedGuestIds.Add(_currentGuest.id);
                    }
                    else
                    {
                        // неверный арест → предупреждение
                        _warnings++;
                        UpdateWarningUI();
                        if (_warnings >= 5)
                        {
                            GameOver();
                            return;
                        }
                    }
                    break;
            }

            // Если решение верное → верные решения за день
            if (correct)
            {
                _correctCountDay++;
            }
            else
            {
                // Ошибка учитывается **только для неверного отклонения** (Allow/Deny)
                if (decision == GuestDecision.Allow || decision == GuestDecision.Deny)
                {
                    _wrongCountTotal++;
                    if (_wrongCountTotal >= 5)
                    {
                        GameOver();
                        return;
                    }
                }
            }

            Debug.Log(correct
                ? $"Решение верное: {decision} для {_currentGuest.firstName} {_currentGuest.lastName}"
                : $"Решение неверное: {decision} для {_currentGuest.firstName} {_currentGuest.lastName}");

            ticketUI.Hide();

            _currentGuestController.Despawn(() =>
            {
                _currentGuestIndex++;
                SpawnNextGuest();
            });
        }

        private void UpdateWarningUI()
        {
            warningText.text = $"⚠ Предупреждения: {_warnings}/5";
        }

        private void EndDay()
        {
            Debug.Log($"День {_currentDay} завершён!");
            endDayPanel.SetActive(true);

            endDayStatsText.text =
                $"День {_currentDay} окончен!\n" +
                $"Верных решений: {_correctCountDay}\n" +
                $"Ошибок (неверное отклонение): {_wrongCountTotal}\n" +
                $"⚠ Предупреждений (неверный арест): {_warnings}/5";

            _currentDay++;
            if (_currentDay <= _totalDays)
                Invoke(nameof(StartNextDay), 3f);
            else
                ShowFinalResults();
        }

        private void StartNextDay()
        {
            endDayPanel.SetActive(false);
            StartDay(_currentDay);
        }

        private void ShowFinalResults()
        {
            endDayStatsText.text += "\n\nИгра завершена! Все дни отработаны.";
        }

        private void GameOver()
        {
            _gameOver = true;
            gameOverPanel.SetActive(true);
            endDayStatsText.text = "Игра окончена. Достигнут лимит ошибок или предупреждений.";
        }

        public bool IsGuestArrested(GuestData guest)
        {
            return arrestedGuestIds.Contains(guest.id);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace DayGameplayScripts
{
    public class DayManager : MonoBehaviour
    {
        public GuestGenerator generator;
        public Transform guestSpawnParent;
        public GameObject guestPrefab;
        public GuestTicketUI ticketUI;
        public ActionButtonsUI buttonsUI;
        
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI dateText;
        public GameObject gameOverPanel;
        public GameObject endDayPanel;
        public GameObject tiredPanel;
        public GameObject startNightPanel;
        public GameObject arrestConfirmationPanel;
        public WarningUIController warningUI;
        public Button nextDayButton;
        
        public TextMeshProUGUI visitorsText;
        public TextMeshProUGUI arrestedText;
        public TextMeshProUGUI cluesText;
        public TextMeshProUGUI warningBonusText;
        public TextMeshProUGUI totalWarningsText;
        public TextMeshProUGUI endShiftWarningsText;

        private int _currentGuestIndex;
        private int _currentDay = 1;
        private const int TotalDays = 5;

        private int _warnings;
        private int _visitorsToday;
        private int _arrestedWantedToday;
        private int _warningBonusPoints;

        private GuestController _currentGuestController;
        private GuestData _currentGuest;
        private TicketData _currentTicket;

        private readonly HashSet<string> _arrestedGuestIds = new();
        private readonly List<GuestData> _missedWantedToday = new();

        private void Start()
        {
            buttonsUI.Init(this);
            generator.dayManager = this;

            // Если данные ночной смены есть — показать сводку
            if (NightShiftPayload.Instance != null && NightShiftPayload.Instance.foundCluesNight > 0)
            {
                ShowDailySummary();
            }
            else
            {
                StartDay(_currentDay);
            }
        }
        
        private void StartDay(int dayNumber)
        {
            UpdateDateUI();

            _currentGuestIndex = 0;
            _warnings = 0;
            _visitorsToday = 0;
            _arrestedWantedToday = 0;
            _warningBonusPoints = 0;
            _missedWantedToday.Clear();

            endDayPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            nextDayButton.gameObject.SetActive(false);

            if (dayText != null)
                dayText.text = $"День {dayNumber}/{TotalDays} завершён!";

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
            guestObj.transform.SetSiblingIndex(1);
            var guestController = guestObj.GetComponent<GuestController>();

            buttonsUI.SetButtonsInteractable(false);

            guestController.Initialize(guestData);
            _currentGuestController = guestController;

            guestController.OnReadyForDecision = (gc) =>
            {
                _currentGuest = gc.guestData;

                _currentGuest.Ticket ??= new TicketData(_currentGuest);

                _currentTicket = _currentGuest.Ticket;

                ticketUI.Show(_currentTicket);
                buttonsUI.SetButtonsInteractable(true);
            };
        }

        public void OnAllowClick() => EvaluateDecision(GuestDecision.Allow);
        public void OnDenyClick() => EvaluateDecision(GuestDecision.Deny);

        public void OnArrestClick()
        {
            if (_currentGuest == null || _currentTicket == null) return;
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
            if (_currentGuest == null || _currentTicket == null) return;

            var isWanted = generator.wantedListGenerator.wantedGuests.Contains(_currentGuest);
            var correct = false;
            var isExpired = _currentTicket.validUntilDay < _currentDay;

            _visitorsToday++;

            switch (decision)
            {
                case GuestDecision.Allow:
                    if (!isWanted && !_currentTicket.isFake && !isExpired)
                        correct = true;
                    else
                    {
                        _warnings++;
                        if (isWanted) _missedWantedToday.Add(_currentGuest);
                    }
                    break;

                case GuestDecision.Deny:
                    if (_currentTicket.isFake || isExpired)
                        correct = true;
                    else
                        _warnings++;
                    break;

                case GuestDecision.Arrest:
                    if (isWanted)
                    {
                        correct = true;
                        _arrestedGuestIds.Add(_currentGuest.id);
                        _arrestedWantedToday++;
                        _warnings = Mathf.Max(0, _warnings - 2);
                        _warningBonusPoints += 2;
                    }
                    else
                        _warnings++;
                    break;
            }

            if (correct && decision != GuestDecision.Arrest)
            {
            }

            UpdateWarningUI();
            ticketUI.Hide();

            _currentGuestController.Despawn(() =>
            {
                _currentGuestIndex++;
                SpawnNextGuest();
            });
        }

        private void UpdateDateUI()
        {
            if (dateText != null)
                dateText.text = $"{_currentDay}";
        }

        private void UpdateWarningUI()
        {
            warningUI.SetWarnings(_warnings);
        }

        private void EndDay()
        {
            Debug.Log($"День {_currentDay} завершён!");

            if (NightShiftPayload.Instance != null)
            {
                var payload = NightShiftPayload.Instance;

                payload.skippedWanted = new List<GuestData>(_missedWantedToday);
                payload.visitorsToday = _visitorsToday;
                payload.arrestedWantedToday = _arrestedWantedToday;
                payload.warningsToday = _warnings;
                payload.warningBonusPoints = _warningBonusPoints;
                payload.foundCluesNight = 0;
                

                if (_missedWantedToday.Count == 0)
                {
                    var wantedGuests = generator.wantedListGenerator.wantedGuests;
                    if (wantedGuests.Count > 0)
                    {
                        GuestData randomExtraWanted;
                        do
                        {
                            randomExtraWanted = wantedGuests[Random.Range(0, wantedGuests.Count)];
                        } while (_arrestedGuestIds.Contains(randomExtraWanted.id)); // чтобы не добавлять уже арестованного

                        payload.extraWantedWithClues = randomExtraWanted;
                    }
                    
                    StartCoroutine(ShowPanelsAndLoadScene(showTired: true));
                    return;
                }
                
                payload.extraWantedWithClues = null;
            }
            StartCoroutine(ShowPanelsAndLoadScene(showTired: false));
        }

        private void ShowDailySummary()
        {
            var payload = NightShiftPayload.Instance;
            if (payload == null) return;

            endDayPanel.SetActive(true);

            visitorsText.text = $"Посетителей\n{payload.visitorsToday}";
            arrestedText.text = $"Сущностей\n{payload.arrestedWantedToday}";
            cluesText.text = $"Улик\n{payload.foundCluesNight}";
            warningBonusText.text = $"Получено:\n{payload.warningBonusPoints}";
            totalWarningsText.text = $"Снято:\n{payload.warningsToday + payload.warningBonusPoints}";
            endShiftWarningsText.text = $"Итог:\n{payload.warningsToday}";

            nextDayButton.gameObject.SetActive(true);
            nextDayButton.onClick.RemoveAllListeners();
            nextDayButton.onClick.AddListener(() =>
            {
                nextDayButton.gameObject.SetActive(false);
                _currentDay++;
                if (_currentDay <= TotalDays)
                    StartDay(_currentDay);
                else
                    Debug.Log("Игра завершена: все дни пройдены!");
            });
        }

        public bool IsGuestArrested(GuestData guest)
        {
            return _arrestedGuestIds.Contains(guest.id);
        }
        private IEnumerator ShowPanelsAndLoadScene(bool showTired)
        {
            if (showTired)
            {
                tiredPanel.SetActive(true);     
                yield return new WaitForSeconds(3f);
                tiredPanel.SetActive(false);
            }
         
            startNightPanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("NightScene");
        }
    }
    
    
}

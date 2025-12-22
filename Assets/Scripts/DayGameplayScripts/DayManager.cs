using System;
using System.Collections;
using System.Collections.Generic;
using NightGameplayScripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace DayGameplayScripts
{
    public class DayManager : MonoBehaviour
    {
        public GuestGenerator generator;
        public WantedListGenerator wantedListGenerator;
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
        
        public GameObject energyDrinkUIPrefab;
        public Transform energyDrinksParent;
        private List<GameObject> _energyDrinkUIInstances = new();
        
        public TextMeshProUGUI visitorsText;
        public TextMeshProUGUI arrestedText;
        public TextMeshProUGUI cluesText;
        public TextMeshProUGUI warningBonusText;
        public TextMeshProUGUI totalWarningsText;
        public TextMeshProUGUI endShiftWarningsText;
        public TextMeshProUGUI endGameText;

        private int _currentGuestIndex;
        private const int TotalDays = 5;
        private int _totalGuests = 0;

        private int _warnings;
        private int _visitorsToday;
        private int _arrestedWantedToday;
        private int _warningBonusPoints;

        private GuestController _currentGuestController;
        private GuestData _currentGuest;
        private TicketData _currentTicket;

        private readonly HashSet<string> _arrestedGuestIds = new();
        private readonly List<GuestData> _missedWantedToday = new();

        [Obsolete("Obsolete")]
        private IEnumerator Start()
        {
            // Убеждаемся, что Payload существует
            NightShiftPayload.GetOrCreate();

            buttonsUI.Init(this);
            generator.dayManager = this;

            UpdateEnergyDrinksUI();

            // ⏳ ЖДЁМ, пока загрузятся гости
            yield return new WaitUntil(() => WantedListGenerator.IsReady);

            // Если данные ночной смены есть — показать сводку
            if (NightShiftPayload.Instance != null &&
                NightShiftPayload.Instance.nightCompleted)
            {
                ShowDailySummary();
                NightShiftPayload.Instance.nightCompleted = false; // сброс
            }
            else
            {
                StartDay(NightShiftPayload.Instance.currentDay);
            }
        }
        
        private void StartDay(int dayNumber)
        {
            UpdateDateUI();
            UpdateEnergyDrinksUI();

            _currentGuestIndex = 0;
            _visitorsToday = 0;
            _arrestedWantedToday = 0;
            _warningBonusPoints = 0;
            _missedWantedToday.Clear();
            
            if (NightShiftPayload.Instance != null)
                _warnings = NightShiftPayload.Instance.warningsToday;
            else
                _warnings = 0;

            endDayPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            nextDayButton.gameObject.SetActive(false);

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

            _totalGuests += 1;
            
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
            var isExpired = _currentTicket.validUntilDay < NightShiftPayload.Instance.currentDay;

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
                    {
                        correct = true;
                        _warnings = Mathf.Max(0, _warnings - 1);
                        _warningBonusPoints += 1;
                    }
                    
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
                        
                        if (NightShiftPayload.Instance != null)
                        {
                            bool added = NightShiftPayload.Instance.AddEnergyDrink();
                            if (added)
                            {
                                UpdateEnergyDrinksUI();
                                Debug.Log("Получен энергетик за правильное задержание!");
                            }
                        }
                    }
                    else
                        _warnings++;
                    break;
            }

            if (!correct && _warnings == 5)
            {
                GameOver();
                return;
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
            dateText.text = $"{NightShiftPayload.Instance.currentDay}";
        }

        private void UpdateWarningUI()
        {
            warningUI.SetWarnings(_warnings);
        }

        private void EndDay()
        {
            Debug.Log($"День {NightShiftPayload.Instance.currentDay} завершён!");

            if (NightShiftPayload.Instance != null)
            {
                var payload = NightShiftPayload.Instance;

                payload.skippedWanted = new List<GuestData>(_missedWantedToday);
                payload.visitorsToday = _visitorsToday;
                payload.arrestedWantedToday = _arrestedWantedToday;
                payload.warningsToday = _warnings;
                payload.warningBonusPoints = _warningBonusPoints;
                payload.foundCluesNight = 0;
                payload.wantedGuests = wantedListGenerator.wantedGuests;
                

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
            NightStaticManager.nightShiftPayload = NightShiftPayload.Instance;
            StartCoroutine(ShowPanelsAndLoadScene(showTired: false));
        }
        
        private void UpdateEnergyDrinksUI()
        {
            if (NightShiftPayload.Instance == null || energyDrinksParent == null || energyDrinkUIPrefab == null)
                return;

            foreach (var instance in _energyDrinkUIInstances)
            {
                Destroy(instance);
            }
            _energyDrinkUIInstances.Clear();

            for (var i = 0; i < NightShiftPayload.Instance.EnergyDrinks; i++)
            {
                var energyDrinkUI = Instantiate(energyDrinkUIPrefab, energyDrinksParent);
                _energyDrinkUIInstances.Add(energyDrinkUI);
            }
        }

        private void ShowDailySummary()
        {
            var payload = NightShiftPayload.Instance;
            if (payload == null) return;

            endDayPanel.SetActive(true);
            dayText.text = $"День {payload.currentDay}/5 завершён!";
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
                payload.currentDay++;
                if (payload.currentDay <= TotalDays)
                    StartDay(payload.currentDay);
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
        public void GameOver() 
        {
            gameOverPanel.SetActive(true);
            endGameText.text = $"Смен: {NightShiftPayload.Instance.currentDay}/5 \n" +
                          $"Гостей: {_totalGuests}  \n" +
                          $"Выявлено сущеностей: {_arrestedGuestIds.Count} \n" +
                          $"Предупреждений: {_warnings}/5 \n";
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

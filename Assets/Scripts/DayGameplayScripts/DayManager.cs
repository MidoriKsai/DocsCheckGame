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
        public GameObject endGamePanel;
        public GameObject endDayPanel;
        public GameObject tiredPanel;
        public GameObject startNightPanel;
        public GameObject arrestConfirmationPanel;
        public WarningUIController warningUI;
        public Button nextDayButton;
        
        public GameObject energyDrinkUIPrefab;
        public Transform energyDrinksParent;
        private readonly List<GameObject> _energyDrinkUIInstances = new();
        
        public TextMeshProUGUI visitorsText;
        public TextMeshProUGUI arrestedText;
        public TextMeshProUGUI cluesText;
        public TextMeshProUGUI warningBonusText;
        public TextMeshProUGUI totalWarningsText;
        public TextMeshProUGUI endShiftWarningsText;
        public TextMeshProUGUI gameOverText;
        public TextMeshProUGUI endGameText;
        public TextMeshProUGUI endGameDescriptionText;

        private int _currentGuestIndex;
        private const int TotalDays = 5;
        private int _totalGuests;

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

            _warnings = NightShiftPayload.Instance != null ? NightShiftPayload.Instance.warningsToday : 0;

            endDayPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            nextDayButton.gameObject.SetActive(false);
            
            generator.GenerateGuestsForDay(dayNumber);

            // --- Проверка первого запуска ---
            if (!PlayerPrefs.HasKey("FirstLaunch"))
            {
                PlayerPrefs.SetInt("FirstLaunch", 1);
                PlayerPrefs.Save();

                if (TutorialManager.Instance != null)
                {
                    // Запускаем туториал поверх подгруженных гостей
                    TutorialManager.Instance.StartTutorial();
                    SpawnNextGuest(); 
                    UpdateWarningUI();
                    
                }

                // Не спавним гостей сразу
                return;
            }

            // --- обычный запуск дня ---
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
            AudioManager.Instance.PlaySFX("menuButtonMusic");
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
                    AudioManager.Instance.PlaySFX("menuButtonMusic");
                    if (!isWanted && !_currentTicket.isFake && !isExpired)
                        correct = true;
                    else
                    {
                        _warnings++;
                        if (isWanted) _missedWantedToday.Add(_currentGuest);
                    }
                    break;

                case GuestDecision.Deny:
                    AudioManager.Instance.PlaySFX("dontPassMusic");
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
                    AudioManager.Instance.PlaySFX("arrestMusic");
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

        // ReSharper disable Unity.PerformanceAnalysis
        private void EndDay()
        {
            Debug.Log($"День {NightShiftPayload.Instance.currentDay} завершён!");
            
            var payload = NightShiftPayload.Instance;
            bool perfectShift = _missedWantedToday.Count == 0;
            payload.currentDay++;

            if (perfectShift && payload.wantedGuests.Count > 0)
            {
                GuestData extra;
                do
                {
                    extra = payload.wantedGuests[Random.Range(0, payload.wantedGuests.Count)];
                }
                while (_arrestedGuestIds.Contains(extra.id));

                payload.extraWantedWithClues = extra;
                Debug.Log($"[DayManager] ExtraWanted добавлен: {extra.firstName}");
            }
            else
            {
                payload.extraWantedWithClues = null;
            }
            
            if (payload != null)
            {
                // сохраняем пропущенных разыскиваемых
                payload.skippedWanted = new List<GuestData>(_missedWantedToday);

                payload.visitorsToday = _visitorsToday;
                payload.arrestedWantedToday += _arrestedWantedToday;
                payload.warningsToday += _warnings;
                payload.warningBonusPoints += _warningBonusPoints;

                // сохраняем найденные улики
                if (NightStaticManager.nightShiftPayload != null)
                {
                    payload.foundCluesNight += NightStaticManager.nightShiftPayload.foundCluesNight;
                    payload.foundClueSprites.AddRange(NightStaticManager.nightShiftPayload.foundClueSprites);
                }

                payload.wantedGuests = wantedListGenerator.wantedGuests;

                // логика ночи
                CalculateNightClues();
                UpdateWantedNights();
                CheckLoseCondition();

                payload.nightCompleted = true;
            }

            // сохраняем итоговый payload для следующей сцены
            NightStaticManager.nightShiftPayload = NightShiftPayload.Instance;
            payload.wantedGuests = wantedListGenerator.wantedGuests;
            StartCoroutine(ShowPanelsAndLoadScene(showTired: perfectShift));
        }
        
        private void CalculateNightClues()
        {
            var payload = NightShiftPayload.Instance;

            int wantedCount = payload.wantedGuests.Count;
            payload.foundCluesNight = wantedCount;
        }
        
        private void UpdateWantedNights()
        {
            var payload = NightShiftPayload.Instance;

            foreach (var guest in payload.wantedGuests)
            {
                guest.nightsOnTerritory++;

                if (guest.nightsOnTerritory >= 3)
                {
                    payload.guestDiedTonight = true;
                }
            }
        }
        
        private void CheckLoseCondition()
        {
            var payload = NightShiftPayload.Instance;

            if (payload.guestDiedTonight)
            {
                Debug.Log("ПОРАЖЕНИЕ: гость погиб ночью");
                // загрузка экрана поражения
                // SceneManager.LoadScene("LoseScene");
            }
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
                if (payload.currentDay <= TotalDays && _arrestedGuestIds.Count != 6)
                    StartDay(payload.currentDay);
                else
                    EndGame();
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

        private void GameOver() 
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = $"Смен: {NightShiftPayload.Instance.currentDay}/5 \n" +
                          $"Гостей: {_totalGuests}  \n" +
                          $"Выявлено сущностей: {_arrestedGuestIds.Count} \n" +
                          $"Предупреждений: {_warnings}/5 \n";
        }
        
        private void EndGame() 
        {
            endGamePanel.SetActive(true); 
            endGameText.text = $"Смен: {NightShiftPayload.Instance.currentDay-1}/5 \n" +
                               $"Гостей: {_totalGuests}  \n" +
                               $"Выявлено сущностей: {_arrestedGuestIds.Count} \n" +
                               $"Предупреждений: {_warnings}/5 \n";
            
            if (_arrestedGuestIds.Count == 6)
            {
                AudioManager.Instance.PlaySFX("winButtonMusic");
                endGameDescriptionText.text = "Вы выполнили свой долг. Парк и его гости в безопасности...\n" +
                                              "Насколько это вообще возможно. МКА высылает Вам премию и почётные награды. Но конец ли это?";
            }
            else
            {
                AudioManager.Instance.PlaySFX("lossButtonMusic");
                endGameDescriptionText.text =
                    "Музыка умолкла, гирлянды погасли. Пять дней прошли, но не всех сущностей удалось поймать.\n" +
                    "Парк закрыт, но тишина обманчива. Что-то всё ещё бродит среди аттракционов.. И забирает простых горожан\n" +
                    "Звенит сирена МКА...За недостаточную эффективность будут последствия...";
            }
        }

        public void LoadMainMenu()
        {
            AudioManager.Instance.PlaySFX("menuButtonMusic");
            NightShiftPayload.Instance.ResetPayload();
            SceneManager.LoadScene("MainMenu");
        }
    }
}

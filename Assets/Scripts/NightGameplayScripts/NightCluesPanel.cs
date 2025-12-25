using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DayGameplayScripts;

namespace NightGameplayScripts
{
    public class NightCluesPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI dateText;
        public TextMeshProUGUI cluesText;
        public TextMeshProUGUI arrestedText;
        public WarningUIController warningUI;

        [Header("Night Clues Logic")]
        public NightItemSpawner spawnerPrefab;     // префаб спавнера
        public Transform spawnerParent;            // куда их инстансить
        public CameraChanger cameraChanger;        // камера

        private NightShiftPayload _payload;
        private List<NightItemSpawner> spawners = new();

        private void Start()
        {
            NightStaticManager.nightCluesPanel = this;
            _payload = NightShiftPayload.Instance;
            if (!PlayerPrefs.HasKey("FirstLaunch"))
            {
                PlayerPrefs.SetInt("FirstLaunch", 1);
                PlayerPrefs.Save();

                Debug.Log(PlayerPrefs.HasKey("FirstLaunch"));
                // Не спавним гостей сразу
                return;
            }

            if (_payload == null)
            {
                Debug.LogError("NightShiftPayload не найден");
                return;
            }

            StartCoroutine(WaitForPayloadAndSpawn());
            
            IEnumerator WaitForPayloadAndSpawn()
            {
                while (NightShiftPayload.Instance == null || NightShiftPayload.Instance.skippedWanted == null)
                    yield return null;

                CreateNightSpawners();
            }
            
            RefreshUI();
        }

        private void CreateNightSpawners()
        {
            var payload = NightShiftPayload.Instance;
            var guestsForNight = new List<GuestData>();

            if (payload.skippedWanted != null) guestsForNight.AddRange(payload.skippedWanted);

            if (payload.extraWantedWithClues != null) guestsForNight.Add(payload.extraWantedWithClues);

            if (guestsForNight.Count == 0)
            {
                Debug.Log("Нет гостей для ночных улик");
                return;
            }

            foreach (var guest in guestsForNight)
            {
                Debug.Log($"[NightCluesPanel] Гостей для ночи: {guestsForNight.Count}");
                // Проверяем есть ли у гостя ещё улики для добавления
                bool hasNewClues = false;
                foreach (var clue in guest.LoadedClues)
                {
                    if (!payload.foundClueSprites.Contains(clue))
                    {
                        hasNewClues = true;
                        break;
                    }
                }

                if (!hasNewClues)
                {
                    Debug.Log($"Все улики гостя {guest.firstName} уже найдены, спавнер не создаётся");
                    continue;
                }

                var spawner = Instantiate(spawnerPrefab, spawnerParent);
                spawner.Init(guest);
                spawners.Add(spawner);
            }

            Debug.Log($"Создано ночных спавнеров: {spawners.Count}");
        }

        public void RefreshUI()
        {
            if (dateText != null)
                dateText.text = $"{_payload.currentDay}";

            if (warningUI != null)
                warningUI.SetWarnings(
                    _payload.warningsToday
                );

            if (cluesText != null)
                cluesText.text = $"Улик: {_payload.foundCluesNight}";

            if (arrestedText != null)
                arrestedText.text = $"Сущностей: {_payload.arrestedWantedToday}";
        }
        
        public void OnCameraSwitched()
        {
            Debug.Log("[NightCluesPanel] Камера переключена, уведомляем спавнеры");
            foreach (var spawner in spawners)
            {
                if (spawner != null)
                {
                    Debug.Log($"[NightCluesPanel] Уведомляем спавнер: {spawner.selectedGuest.firstName}");
                    spawner.OnCameraSwitched();
                }
            }
        }
    }
}

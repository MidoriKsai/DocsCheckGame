using NightGameplayScripts;
using UnityEngine;

namespace DayGameplayScripts
{
    public class WantedListNightUI : MonoBehaviour
    {
        public GameObject wantedCardNightPrefab;
        public Transform contentParent;
        public ArrestManager arrestManager;

        private void Start()
        {
            DisplayWantedList();
        }

        private void DisplayWantedList()
        {
            var payload = NightShiftPayload.Instance;
            if (payload == null)
            {
                Debug.LogError("NightShiftPayload.Instance == NULL");
                return;
            }
            
            var wantedList = NightShiftPayload.Instance.wantedGuests;
            Debug.Log(wantedList);
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            foreach (var guest in wantedList)
            {
                var card = Instantiate(wantedCardNightPrefab, contentParent);
                var ui = card.GetComponent<WantedCardNightUI>();

                ui.Setup(guest, arrestManager);
            }

            Debug.Log("Разыскиваемые ночной смены отображены");

            Debug.Log("Разыскиваемые отображены");
        }
    }
}
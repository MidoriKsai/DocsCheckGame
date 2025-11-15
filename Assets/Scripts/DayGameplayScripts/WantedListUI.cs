
using UnityEngine;

namespace DayGameplayScripts
{
    public class WantedListUI : MonoBehaviour
    {
        public WantedListGenerator wantedListGenerator;
        public GameObject wantedCardPrefab;
        public Transform contentParent;
        public WantedDetailUI detailUI;

        private void Start()
        {
            DisplayWantedList();
        }

        private void DisplayWantedList()
        {
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            foreach (var guest in wantedListGenerator.wantedGuests)
            {
                var card = Instantiate(wantedCardPrefab, contentParent);
                var ui = card.GetComponent<WantedCardUI>();
                ui.Setup(guest, this);
                
            }

            Debug.Log("Разыскиваемые отображены");
        }
        
        public void ShowDetail(GuestData wanted)
        {
            detailUI.Show(wanted);
        }
    }
}
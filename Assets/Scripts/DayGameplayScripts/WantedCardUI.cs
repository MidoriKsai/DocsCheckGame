using UnityEngine; 
using UnityEngine.UI; 
using TMPro; 

namespace DayGameplayScripts 
{ 
    public class WantedCardUI : MonoBehaviour 
    { 
        public Image portraitImage;
        public TextMeshProUGUI nameText; 
        private WantedListUI _wantedListUI; 
        private GuestData _wantedData;

        public void Setup(GuestData wantedData, WantedListUI wantedListUI)
        {
            _wantedData = wantedData; 
            _wantedListUI = wantedListUI; 
            nameText.text = $"{wantedData.firstName} {wantedData.lastName}";

            if (portraitImage && wantedData.LoadedPortrait != null)
            {
                portraitImage.sprite = wantedData.LoadedPortrait;
                portraitImage.preserveAspect = true;
            }    
                
            
            GetComponent<Button>().onClick.AddListener(OnCardClicked);
        }

        private void OnCardClicked()
        {
            _wantedListUI.ShowDetail(_wantedData);
        } 
    } 
}

using NightGameplayScripts;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DayGameplayScripts
{
    public class WantedCardNightUI : MonoBehaviour
    {
        public Image portraitImage;
        public TextMeshProUGUI nameText;

        private GuestData _guestData;
        private WantedListNightUI _listUI;
        private NightShiftPayload _payload;
        private ArrestManager _arrestManager;

        public void Setup(GuestData guestData, ArrestManager arrestManager)
        {
            _guestData = guestData;
            _arrestManager = arrestManager;

            nameText.text = $"{guestData.firstName} {guestData.lastName}";

            if (portraitImage && guestData.LoadedPortrait != null)
            {
                portraitImage.sprite = guestData.LoadedPortrait;
                portraitImage.preserveAspect = true;
            }

            var btn = GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            AudioManager.Instance.PlaySFX("menuButtonMusic");
            if (_arrestManager == null || _guestData == null) return;

            _arrestManager.OnArrestButtonClicked(_guestData);
        }

    }
}
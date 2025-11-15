using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DayGameplayScripts
{
    public class WantedDetailUI : MonoBehaviour
    {
        public Image portraitImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI ageText;
        public TextMeshProUGUI genderText;
        public Button closeButton;
        
        private void Start()
        {
            closeButton.onClick.AddListener(Hide);
        }
        public void Show(GuestData wanted)
        {
            gameObject.SetActive(true);
            nameText.text = $"{wanted.firstName} {wanted.lastName}";
            ageText.text = $"Возраст: {wanted.age}";
            genderText.text = $"Пол: {wanted.gender}";
            Debug.Log("Подробная информация отображена");
            if (portraitImage && wanted.LoadedPortrait != null)
                portraitImage.sprite = wanted.LoadedPortrait;
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
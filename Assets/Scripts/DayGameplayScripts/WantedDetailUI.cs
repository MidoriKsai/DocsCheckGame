using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DayGameplayScripts
{
    public class WantedDetailUI : MonoBehaviour
    {
        public Image portraitImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI lastNameText;
        public TextMeshProUGUI ageText;
        public TextMeshProUGUI genderText;
        public Button closeButton;

        private RectTransform _rectTransform;
        private Vector2 _initialPosition;
        private bool _initialized;

        private void OnEnable()
        {
            if (!_initialized)
            {
                _rectTransform = GetComponent<RectTransform>();
                _initialPosition = _rectTransform.anchoredPosition;

                if (closeButton != null)
                    closeButton.onClick.AddListener(Hide);

                _initialized = true;
            }
        }

        public void Show(GuestData wanted)
        {
            if (wanted == null)
            {
                Debug.LogError("WantedDetailUI.Show: wanted == null");
                return;
            }

            // Возвращаем окно в исходную позицию
            if (_rectTransform != null)
                _rectTransform.anchoredPosition = _initialPosition;

            gameObject.SetActive(true);

            nameText.text = $"И: {wanted.firstName}";
            lastNameText.text = $"Ф: {wanted.lastName}";
            ageText.text = $"Возраст: {wanted.age}";
            genderText.text = $"Пол: {wanted.gender}";

            if (portraitImage && wanted.LoadedPortrait != null)
                portraitImage.sprite = wanted.LoadedPortrait;
        }

        private void Hide()
        {
            AudioManager.Instance.PlaySFX("menuButtonMusic");
            gameObject.SetActive(false);
        }
    }
}
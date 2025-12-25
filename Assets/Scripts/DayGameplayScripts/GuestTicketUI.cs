using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DayGameplayScripts
{
    public class GuestTicketUI : MonoBehaviour
    {
        public Image portraitImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI lastNameText;
        public TextMeshProUGUI ageText;
        public TextMeshProUGUI genderText;
        public TextMeshProUGUI dateText;

        private TicketData _ticket;

        private RectTransform _rectTransform;
        private Vector2 _initialPosition;
        private bool _initialized;

        private void OnEnable()
        {
            if (_initialized) return;

            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.anchoredPosition;
            _initialized = true;
        }

        public void Show(TicketData ticket)
        {
            AudioManager.Instance.PlaySFX("ticketOpenMusic");
            if (ticket == null)
            {
                Debug.LogError("GuestTicketUI.Show: ticket == null");
                return;
            }

            _ticket = ticket;

            // 🔁 всегда возвращаем билет в исходное место
            if (_rectTransform != null)
                _rectTransform.anchoredPosition = _initialPosition;

            gameObject.SetActive(true);

            nameText.text = $"И: {_ticket.firstName}";
            lastNameText.text = $"Ф: {_ticket.lastName}";
            ageText.text = $"Возраст: {_ticket.age}";
            genderText.text = $"Пол: {_ticket.gender}";
            dateText.text = $"Действителен до: {_ticket.validUntil}";

            if (portraitImage && _ticket.portrait != null)
                portraitImage.sprite = _ticket.portrait;
        }

        public void Hide()
        {
            AudioManager.Instance.PlaySFX("menuButtonMusic");
            gameObject.SetActive(false);
        }
    }
}
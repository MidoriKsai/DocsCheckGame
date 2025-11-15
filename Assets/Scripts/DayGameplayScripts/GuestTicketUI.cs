using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DayGameplayScripts
{
    public class GuestTicketUI : MonoBehaviour
    {
        public Image portraitImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI ageText;
        public TextMeshProUGUI genderText;
        public TextMeshProUGUI dateText;

        private TicketData _ticket;

        // Показывает существующий билет
        public void Show(TicketData ticket)
        {
            _ticket = ticket;

            gameObject.SetActive(true);

            nameText.text = $"{_ticket.firstName} {_ticket.lastName}";
            ageText.text = $"Возраст: {_ticket.age}";
            genderText.text = $"Пол: {_ticket.gender}";
            dateText.text = $"Действителен до: {_ticket.validUntil}";

            if (portraitImage && _ticket.portrait != null)
                portraitImage.sprite = _ticket.portrait;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
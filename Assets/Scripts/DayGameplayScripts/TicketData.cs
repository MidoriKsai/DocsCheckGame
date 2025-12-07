using System;
using UnityEngine;

namespace DayGameplayScripts
{
    [Serializable]
    public class TicketData
    {
        public string firstName;
        public string lastName;
        public string age;
        public string gender;
        public string validUntil;
        public int validUntilDay;
        public bool isFake;
        public Sprite portrait;

        public TicketData(GuestData guest)
        {
            firstName = guest.firstName;
            lastName = guest.lastName;
            age = guest.age;
            gender = guest.gender;
            portrait = guest.LoadedPortrait;

            isFake = guest.IsFake;

            if (!isFake && UnityEngine.Random.value < 0.2f)
            {
                isFake = true;
                RandomizeError();
            }
            else
            {
                validUntilDay = UnityEngine.Random.Range(1, 6);
                var validDate = new DateTime(DateTime.Now.Year, 8, validUntilDay);
                validUntil = validDate.ToString("dd.MM.yyyy");
            }
        }

        private void RandomizeError()
        {
            int badDay = UnityEngine.Random.Range(1, 32);
            var invalidDate = new DateTime(DateTime.Now.Year, UnityEngine.Random.Range(1, 8), badDay);
            validUntil = invalidDate.ToString("dd.MM.yyyy");
        }
    }
}
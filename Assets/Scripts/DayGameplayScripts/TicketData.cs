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
        public bool isFake;
        public Sprite portrait;

        public TicketData(GuestData guest)
        {
            firstName = guest.firstName;
            lastName = guest.lastName;
            age = guest.age;
            gender = guest.gender;
            portrait = guest.LoadedPortrait;

            isFake = guest.isFakeByName;

            if (!isFake && UnityEngine.Random.value < 0.2f)
            {
                isFake = true;
                RandomizeError();
            }
            else
            {
                var validDate = new DateTime(DateTime.Now.Year, 8, UnityEngine.Random.Range(1, 6));
                validUntil = validDate.ToString("dd.MM.yyyy");
            }
        }

        private void RandomizeError()
        {
            int badDay = UnityEngine.Random.Range(1, 32);
            var invalidDate = new DateTime(DateTime.Now.Year, 7, badDay);
            validUntil = invalidDate.ToString("dd.MM.yyyy");
        }
    }
}
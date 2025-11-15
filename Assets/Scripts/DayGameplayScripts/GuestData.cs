using UnityEngine;
using System;

namespace DayGameplayScripts
{
    [Serializable]
    public class GuestData
    {
        public string id;
        public string firstName;
        public string lastName;
        public string age;
        public string gender;
        public string fullBodySprite;
        public string portraitSprite;
        public string thingSprite;

        [NonSerialized] public Sprite LoadedFullBody;
        [NonSerialized] public Sprite LoadedPortrait;
        [NonSerialized] public bool isFakeByName;
        [NonSerialized] public TicketData ticket;

        public void LoadSprites()
        {
            if (!string.IsNullOrEmpty(fullBodySprite))
                LoadedFullBody = Resources.Load<Sprite>(fullBodySprite);

            if (!string.IsNullOrEmpty(portraitSprite))
                LoadedPortrait = Resources.Load<Sprite>(portraitSprite);
        }
    }

    [Serializable]
    public class GuestListWrapper
    {
        public System.Collections.Generic.List<GuestData> guests;
    }
}
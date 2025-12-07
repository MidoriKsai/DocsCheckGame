using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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
        public string[] clueSprites;

        [NonSerialized] public Sprite LoadedFullBody;
        [NonSerialized] public Sprite LoadedPortrait;

        [NonSerialized] public Sprite[] LoadedClues;

        [NonSerialized] public bool IsFake;
        [NonSerialized] public TicketData Ticket;

        public void LoadSprites()
        {
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>("UI");
            if (atlas == null)
            {
                Debug.LogError("Не удалось загрузить атлас UI");
                return;
            }

            LoadedFullBody ??= atlas.GetSprite(fullBodySprite);
            if (LoadedFullBody == null)
                Debug.LogWarning($"Не найден спрайт {fullBodySprite} в атласе");

            LoadedPortrait ??= atlas.GetSprite(portraitSprite);
            if (LoadedPortrait == null)
                Debug.LogWarning($"Не найден спрайт {portraitSprite} в атласе");

            if (clueSprites is not { Length: > 0 }) return;
            LoadedClues ??= new Sprite[clueSprites.Length];
            for (var i = 0; i < clueSprites.Length; i++)
            {
                LoadedClues[i] = atlas.GetSprite(clueSprites[i]);
                if (LoadedClues[i] == null)
                    Debug.LogWarning($"Не найден спрайт подсказки {clueSprites[i]} в атласе");
            }
        }
    }

    [Serializable]
    public class GuestListWrapper
    {
        public List<GuestData> guests;
    }
}
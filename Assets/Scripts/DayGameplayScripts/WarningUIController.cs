using UnityEngine;
using UnityEngine.UI;

namespace DayGameplayScripts
{
    public class WarningUIController : MonoBehaviour
    {
        public Image[] dots;
        public Color normalColor = Color.gray;
        public Color warningColor = Color.red;

        public void SetWarnings(int count)
        {
            for (var i = 0; i < dots.Length; i++)
            {
                dots[i].color = (i < count) ? warningColor : normalColor;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace DayGameplayScripts
{
    public class ActionButtonsUI : MonoBehaviour
    {
        public Button allowButton;
        public Button denyButton;
        public Button arrestButton;

        public void SetButtonsInteractable(bool value)
        {
            allowButton.interactable = value;
            denyButton.interactable = value;
            arrestButton.interactable = value;
        }

        public void Init(DayManager manager)
        {
            allowButton.onClick.AddListener(manager.OnAllowClick);
            denyButton.onClick.AddListener(manager.OnDenyClick);
            arrestButton.onClick.AddListener(manager.OnArrestClick);
        }
    }
}
using UnityEngine;

namespace DayGameplayScripts
{
    public class ArrestConfirmPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;

        private GuestData _pendingGuest;
        private ArrestManager _arrestManager;

        public void Show(GuestData guest, ArrestManager arrestManager)
        {
            AudioManager.Instance.PlaySFX("wantedButtonMusic");
            _pendingGuest = guest;
            _arrestManager = arrestManager;
            panelRoot.SetActive(true);
        }

        public void OnConfirm()
        {
            AudioManager.Instance.PlaySFX("arrestMusic");
            _arrestManager.ConfirmArrest(_pendingGuest);
            Close();
        }

        public void OnCancel()
        {
            AudioManager.Instance.PlaySFX("menuButtonMusic");
            Close();
        }

        private void Close()
        {
            _pendingGuest = null;
            panelRoot.SetActive(false);
        }
    }
}
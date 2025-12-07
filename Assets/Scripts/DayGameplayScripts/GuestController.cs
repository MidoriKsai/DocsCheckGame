using UnityEngine;
using UnityEngine.UI;

namespace DayGameplayScripts
{
    public class GuestController : MonoBehaviour
    {
        public Image guestImage;
        public GuestData guestData;

        private Button _guestButton;

        public System.Action<GuestController> OnReadyForDecision;

        private void Awake()
        {
            _guestButton = GetComponent<Button>();
            _guestButton.onClick.AddListener(() => OnReadyForDecision?.Invoke(this));
        }

        public void Initialize(GuestData data)
        {
            guestData = data;
            if (!guestImage || data.LoadedFullBody == null) return;
            guestImage.sprite = data.LoadedFullBody;
            guestImage.preserveAspect = true;
        }

        public void Despawn(System.Action onComplete)
        {
            Destroy(gameObject);
            onComplete?.Invoke();
        }
    }
}
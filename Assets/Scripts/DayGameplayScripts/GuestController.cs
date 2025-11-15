using UnityEngine;
using UnityEngine.UI;

namespace DayGameplayScripts
{
    public class GuestController : MonoBehaviour
    {
        public Image guestImage;
        public GuestData guestData;

        private Button guestButton;

        public System.Action<GuestController> OnReadyForDecision;

        private void Awake()
        {
            guestButton = GetComponent<Button>();
            guestButton.onClick.AddListener(() => OnReadyForDecision?.Invoke(this));
        }

        public void Initialize(GuestData data)
        {
            guestData = data;
            if (guestImage && data.LoadedFullBody != null)
                guestImage.sprite = data.LoadedFullBody;
        }

        public void Despawn(System.Action onComplete)
        {
            Destroy(gameObject);
            onComplete?.Invoke();
        }
    }
}
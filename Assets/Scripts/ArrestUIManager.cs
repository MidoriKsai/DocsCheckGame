using UnityEngine;
using UnityEngine.UI;

public class ArrestUIManager : MonoBehaviour
{
    public GameObject arrestPanel; // панель с арестом
    public Image characterImage;// спрайт персонажа
    public Button closeButton;// закрытие панели

    private void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            arrestPanel.SetActive(false);
        });
    }

    public void ShowArrest(Sprite guestSprite)
    {
        characterImage.sprite = guestSprite;
        arrestPanel.SetActive(true);
    }
}
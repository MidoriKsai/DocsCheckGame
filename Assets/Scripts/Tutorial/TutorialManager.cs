using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    
    public GameObject tutorialPanel1;
    public GameObject tutorialPanel2;
    public Button nextStepButton; 
    public Button WantedButton;
    public Button WantedDetailButton;
    public Button GuestButton;
    public Button GuestDetailCloseButton;
    public Button CloseDetailButton;
    public Button CloseWantedButton;

    public GameObject tutorialInfoPanel;
    public TextMeshProUGUI tutorialInfoText;
    
    public GameObject CalendarPointer;
    public GameObject WantedPointer;
    public GameObject WantedDetailPointer;
    public GameObject EnergyPointer;
    public GameObject GuestPointer;
    public GameObject GuestDetailPointer;
    public GameObject MKAPointer;
    public GameObject WarningsPointer;
    public GameObject PassPointer;
    public GameObject DontPassPointer;

    public GameObject GuestPrefab;
    public GameObject parentTransform;
    
    
    
    public GameObject nightWorkPanel;
    public GameObject NightCameraPanel;
    public GameObject JournalPanel;
    
    public GameObject DrinkPointer;
    public GameObject EnergyCountPointer;
    public GameObject ArrestPointer;
    public GameObject WarningPanelPointer;
    public GameObject CameraViewSwitchPointer;
    
    
    public GameObject CameraSwitchPointer;
    public GameObject CluePointer;
    
    
    
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Добавляем параметр onTutorialComplete
    public void StartTutorial()
    {
        Debug.Log("Туториал запущен!");
        // Здесь твоя логика шагов туториала
        // В конце вызываем коллбек:
        StartCoroutine(TutorialCoroutine());
    }

    private IEnumerator TutorialCoroutine()
    {
        if (tutorialPanel1 != null) tutorialPanel1.SetActive(true);

        // Ждём первый клик
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        // Скрываем первую панель и показываем вторую
        if (tutorialPanel1 != null) tutorialPanel1.SetActive(false);
        if (tutorialPanel2 != null) tutorialPanel2.SetActive(true);

        // Ждём второй клик
        nextStepButton.onClick.AddListener(() => StartCoroutine(StartGameTutorial()));
        
    }

    private IEnumerator StartGameTutorial()
    {
        // Первая панель
        if (tutorialPanel2 != null) tutorialPanel2.SetActive(false);
        tutorialInfoPanel.SetActive(true);

        Step1();
        
        BlockAllButtons();
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        
        Step2();
        
        
    }

    void Step1()
    {
        BlockAllButtons();
        CalendarPointer.SetActive(true);
        tutorialInfoText.text = "Календарь смен: 5 дней до завершения";
    }

    void Step2()
    {
        BlockAllButtonsExcept(WantedButton);
        CalendarPointer.SetActive(false);
        WantedPointer.SetActive(true);
        tutorialInfoText.text = "Это список разыскиваемых преступников.";
        WantedButton.onClick.AddListener(Step3);
    }

    void Step3()
    {
        WantedButton.onClick.RemoveListener(Step3);
        UnblockAllButtons();
        BlockAllButtonsExcept(WantedDetailButton);
        BlockAllButtonsExcept(CloseWantedButton);
        WantedPointer.SetActive(false);
        WantedDetailPointer.SetActive(true);
        tutorialInfoText.text = "Опасные лица занесены в базу. Кликайте на них, чтобы узнать больше.";
        CloseWantedButton.onClick.AddListener(() => StartCoroutine(Step4Coroutine()));
    }

    private IEnumerator Step4Coroutine()
    {
        CloseWantedButton.onClick.RemoveListener(OnCloseWantedClicked);
        BlockAllButtons();
        EnergyPointer.SetActive(true);
        WantedDetailPointer.SetActive(false);
        tutorialInfoText.text = "Если вы поймали разыскиваемого — вы получаете 1 энергетик! Он пригодится вам для ночной смены. Всего их у вас в запасе может быть не больше двух.";

        // Ждём клик
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);

        // Здесь можешь запускать следующий шаг
        StartCoroutine(Step5Coroutine());
    }
    
    void OnCloseWantedClicked()
    {
        CloseWantedButton.onClick.RemoveListener(OnCloseWantedClicked);
        StartCoroutine(Step4Coroutine());
    }

    private IEnumerator Step5Coroutine()
    {
        // Блокируем все кнопки
        BlockAllButtons();

        // Активируем указатель
        EnergyPointer.SetActive(false);
        GuestPointer.SetActive(true);

        // Обновляем текст туториала
        tutorialInfoText.text = "Нажмите на посетителя, чтобы увидеть его билет. Проверяйте данные как можно тщательнее. Нет ли человека в списке розыска? Корректные ли имя и фамилия? Правильно ли указаны пол и дата?";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(Step6Coroutine());
        
    }

    private IEnumerator Step6Coroutine()
    {
        // Блокируем все кнопки
        BlockAllButtons();

        // Активируем указатель
        GuestPointer.SetActive(false);
        MKAPointer.SetActive(true);

        // Обновляем текст туториала
        tutorialInfoText.text = "Если перед вами человек в розыске, вызывайте МКА для его поимки.";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(Step7Coroutine());
    }

    private IEnumerator Step7Coroutine()
    {
        BlockAllButtons();

        // Активируем указатель
        MKAPointer.SetActive(false);
        PassPointer.SetActive(true);

        // Обновляем текст туториала
        tutorialInfoText.text = "Если все данные корректны, то вы должны пропустить гостя.";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(Step8Coroutine());
    }
    
    private IEnumerator Step8Coroutine()
    {
        BlockAllButtons();

        // Активируем указатель
        PassPointer.SetActive(false);
        DontPassPointer.SetActive(true);

        // Обновляем текст туториала
        tutorialInfoText.text = "Если не все данные корректны, то вы не должны пропускать гостя.";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(Step9Coroutine());
    }
    
    private IEnumerator Step9Coroutine()
    {
        BlockAllButtons();

        // Активируем указатель
        DontPassPointer.SetActive(false);
        WarningsPointer.SetActive(true);

        // Обновляем текст туториала
        tutorialInfoText.text = "Здесь находится панель с предупреждениями. За неверный арест, неверный пропуск или отклонение вы получаете +1. За верное отклонение -1. За верное задержание -2. Если вы получите 5 предупреждений, вам грозит увольнение. ";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        
        StartCoroutine(Step10Coroutine());
    }
    
    private IEnumerator Step10Coroutine()
    {
        BlockAllButtons();

        // Активируем указатель
        WarningsPointer.SetActive(false);

        // Обновляем текст туториала
        tutorialInfoText.text = "Хорошей смены";

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        yield return new WaitForSeconds(0.2f);
        tutorialInfoPanel.SetActive(false);
        UnblockAllButtons();
    }
    
    public void BlockAllButtons()
    {
        // Находим все кнопки на сцене
        var allButtons = GameObject.FindObjectsOfType<Button>();

        foreach (var btn in allButtons)
        {
            btn.interactable = false;
        }
    }
    
    
    public void BlockAllButtonsExcept(Button allowedButton)
    {
        // Находим все кнопки на сцене
        var allButtons = GameObject.FindObjectsOfType<Button>();

        foreach (var btn in allButtons)
        {
            // Разрешаем только нужную кнопку
            btn.interactable = (btn == allowedButton);

            // Подсветка Outline
            var outline = btn.GetComponent<Outline>();
            if (outline != null) outline.enabled = (btn == allowedButton);
        }
    }
    
    public void UnblockAllButtons()
    {
        var allButtons = GameObject.FindObjectsOfType<Button>();
        foreach (var btn in allButtons)
        {
            btn.interactable = true;
            var outline = btn.GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
        }
    }
}
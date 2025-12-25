using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DayGameplayScripts;

public class DrinkManager : MonoBehaviour
{
    [SerializeField] private GameObject drinkPrefab;

    public UnityEvent drinkPressed;

    private readonly List<GameObject> _drinkList = new();
    private NightShiftPayload _payload;

    private void Start()
    {
        _payload = NightShiftPayload.Instance;
        if (_payload == null)
        {
            Debug.LogError("NightShiftPayload не найден");
            return;
        }
        _payload.OnEnergyDrinksChanged += RefreshUI;
        RefreshUI();
    }
    
    private void OnDestroy()
    {
        if (_payload != null)
            _payload.OnEnergyDrinksChanged -= RefreshUI;
    }
    
    private void RefreshUI()
    {
        // очищаем старые кнопки
        foreach (var drink in _drinkList)
            Destroy(drink);

        _drinkList.Clear();

        // создаём кнопки по данным payload
        for (int i = 0; i < _payload.EnergyDrinks; i++)
        {
            var drink = Instantiate(drinkPrefab, transform);
            var button = drink.GetComponent<Button>();

            button.onClick.AddListener(OnDrinkPressed);
            _drinkList.Add(drink);
        }
    }

    private void OnDrinkPressed()
    {
        if (_payload.UseEnergyDrink())
        {
            AudioManager.Instance.PlaySFX("energyDrink");
            drinkPressed?.Invoke();
        }
    }
}

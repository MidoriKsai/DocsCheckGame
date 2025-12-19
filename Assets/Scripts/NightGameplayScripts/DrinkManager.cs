using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DrinkManager : MonoBehaviour
{
    [SerializeField] private int drinkCount;
    [SerializeField] private GameObject drinkPrefab;
    private List<GameObject> drinkList ;
    
    public UnityEvent drinkPressed;
    
    void Start()
    {
        drinkList =  new List<GameObject>();
        for (int i = 0; i < drinkCount; i++)
        {
            var drink = Instantiate(drinkPrefab, transform);
            var button = drink.GetComponent<Button>();
            button.onClick.AddListener(ButtonPressed);
            drinkList.Add(drink);
        }   
    }

    private void ButtonPressed()
    {
        drinkPressed.Invoke();
    }
}

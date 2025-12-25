using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnergyManager : MonoBehaviour
{
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private List<DrinkManager> drinkManagers;

    [SerializeField] private int energyCount;
    [SerializeField] private GameObject energyPrefab;
    private List<GameObject> energyList;
    
    void Start()
    {
        energyList =  new List<GameObject>();
        timeManager.passed24seconds.AddListener(DeleteEnergy);
        foreach (var dm in drinkManagers)
        {
            dm.drinkPressed.AddListener(AddEnergy);
        }

        
        for (int i = 0; i < energyCount; i++)
        {
            CreateEnergy();
        }
    }
    
    private void DeleteEnergy()
    {
        if (CanDeleteEnergy())
        {
            Destroy(energyList[energyList.Count - 1]);
            energyList.RemoveAt(energyList.Count - 1);
            if (energyList.Count == 0)
            {
                SceneManager.LoadScene("DayScene");
            }
        }
    }

    private void CreateEnergy()
    {
        var energy = Instantiate(energyPrefab, transform);
        energyList.Add(energy);
    }

    private void AddEnergy()
    {
        if (energyList.Count < energyCount)
        {
            CreateEnergy();
        }
    }

    private bool CanDeleteEnergy()
    {
        return energyList.Count > 0;
    }
    
}

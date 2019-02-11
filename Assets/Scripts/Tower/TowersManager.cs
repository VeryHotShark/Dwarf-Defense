using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowersManager : MonoBehaviour {
    public static TowersManager Instance;

    public List<Transform> towers = new List<Transform>();

    private void Awake()
    {
    	// tutaj po prostu jest singleton i to jest po to żebyśmy mogli się w innych skryptach zwracać do tego skryptu za pomocą stworzonej zmiennej statycznej czyli nie musimy mieć do niej referencji
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    public void AddToTowersManager(Transform tower) // metoda żeby dodać Wieże do listy celów
    {
        if (!towers.Contains(tower)) // jeśli lista jeszcze nie zawiera tego celu to dodaje
        {
            towers.Add(tower);
        }
    }
    public void RemoveFromTowersManager(Transform tower) // to samo tlko odejmuje
    {
        if (towers.Contains(tower))
        {
            towers.Remove(tower);
        }
    }

    //Tymczasowa metoda na wybieranie najbliszej wiezy
    public Transform GetClosestTower(Transform from)
    {
        float distance;
        float minDistance = 100000f; //tutaj mozna by podac jakis zasieg wyszukiwania wiez
        Transform closestTower = null;
        for (int i = 0; i < towers.Count; i++)
        {
            distance = Vector3.Distance(from.position, towers[i].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTower = towers[i].transform;
            }
        }
        return closestTower;
    }
    
}

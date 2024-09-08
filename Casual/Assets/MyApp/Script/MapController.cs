using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance;
    [Range(2,8)][SerializeField] public int maxObjectActive = 4;
    [SerializeField] public List<GameObject> housePrefabs;

    [SerializeField] public int defaultZPos;
    [SerializeField] public int rangeBetweenZPos;

    [HideInInspector] public int currentMultiValue;
    [HideInInspector] public int currentHousePrefValue;

    [HideInInspector] public List<GameObject> housePool;

    private void OnEnable()
    {
        Instance = this;
        housePool = new List<GameObject>();
        for (int i = 0; i < maxObjectActive; i++)
        {
            OnInitHouse(i);
        }
    }

    private void OnInitHouse(int index)
    {
        currentMultiValue = index;
        currentHousePrefValue++;
        if (!housePrefabs.IndexInRange(currentHousePrefValue))
        {
            currentHousePrefValue = 0;
        }
        var house = Instantiate(housePrefabs[currentHousePrefValue], 
            new Vector3(12, -5, defaultZPos + rangeBetweenZPos*currentMultiValue), new Quaternion(0,180,0,0));
        housePool.Add(house);
    }

    public void OnUpdatePosition(HouseData item)
    {
        currentMultiValue++;
        item.transform.parent.position = new Vector3(12, -5, defaultZPos + rangeBetweenZPos * currentMultiValue);
        housePool.SetFirstIndexToLastIndex();
    }
}

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FenceInit : MonoBehaviour
{
    [SerializeField] public int InitLenght;
    [SerializeField] public GameObject firstFence;
    [HideInInspector] public List<GameObject> fenceList;

    private int multiCount;
    
    [Button("Init")]
    void OnFenceInit()
    {
        multiCount = 0;
        if (fenceList != null)
        {
            foreach (var item in fenceList)
            {
                DestroyImmediate(item);
            }
            fenceList.Clear();
        }
        fenceList = new List<GameObject>();
        for (int i = 0; i < InitLenght; i++)
        {
            var fence = Instantiate(firstFence, new Vector3(firstFence.transform.position.x,
                firstFence.transform.position.y,
                firstFence.transform.position.z + multiCount++),Quaternion.identity);
            fenceList.Add(fence);
        }
    }
    [Button("Reset")]
    void OnFenceReset()
    {
        foreach (var item in fenceList)
        {
            DestroyImmediate(item);
        }
        fenceList.Clear();
    }
}

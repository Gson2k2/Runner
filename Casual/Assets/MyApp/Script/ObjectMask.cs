using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectMask : MonoBehaviour
{
    [SerializeField] private List<GameObject> maskObject;

    [Button("Mask")]
    void Start()
    {
        maskObject.Add(GameObject.FindGameObjectWithTag("Road"));
        maskObject.Add(GameObject.FindGameObjectWithTag("Arrow"));

        for (int i = 0; i < maskObject.Count; i++)
        {
            if (maskObject[i].GetComponent<MeshRenderer>() != null)
            {
                maskObject[i].GetComponent<MeshRenderer>().material.renderQueue = 3002;
            }
            else
            {
                maskObject[i].GetComponent<SpriteRenderer>().material.renderQueue = 3002;
            }
        }
    }
}

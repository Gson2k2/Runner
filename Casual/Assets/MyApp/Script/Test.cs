using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Button("Test")]
    public void TestFunc()
    {
        Transitioner.Instance.TransitionToScene(1);
    }
}

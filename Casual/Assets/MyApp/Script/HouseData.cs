using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseData : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        Debug.Log(MapController.Instance.defaultZPos + MapController.Instance.rangeBetweenZPos * MapController.Instance.currentMultiValue);
        MapController.Instance.OnUpdatePosition(this);
    }
}

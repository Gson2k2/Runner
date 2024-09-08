using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    [Button("Shake")]
    public void OnObjectShake()
    {
        DOTween.Sequence()
            .Append(gameObject.transform.DOShakeScale(1f,0.25f))
            .Join(gameObject.transform.DOShakeRotation(1f,20f))
            .SetLoops(-1).WithCancellation(this.GetCancellationTokenOnDestroy());
    }
}

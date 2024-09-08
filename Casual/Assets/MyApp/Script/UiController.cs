using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public enum UiAction
{
    Open,Close
}
public class UiController : MonoBehaviour
{
    public static UiController Instance;
    [SerializeField] public GameObject SettingUI;
    [SerializeField] public RectTransform ExtraUnitUI;
    [SerializeField] public RectTransform ExtraEffortUI;
    [SerializeField] public GameObject CashUI;
    [SerializeField] public GameObject MoneyUI;
    [SerializeField] public GameObject ChanceUI;

    public void OnExtraUnitAndExtraEffortAnim(UiAction uiAction)
    {
        if (uiAction == UiAction.Close)
        {
            ExtraUnitUI.DOAnchorPosX(-212, 0.5f);
            ExtraEffortUI.DOAnchorPosX(-212, 0.5f);
        }
        else
        {
            ExtraUnitUI.DOAnchorPosX(212, 0.5f);
            ExtraEffortUI.DOAnchorPosX(212, 0.5f);
        }
    }

    void Start()
    {
        Instance = this;
    }
}



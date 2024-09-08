using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;
    [SerializeField] private CanvasGroup settingBlurPanel;
    [SerializeField] private RectTransform settingPanel;
    [SerializeField] private TextMeshProUGUI textFPS;
    [SerializeField] private Canvas canvasEndGame;

    [Header("----------------Gameplay Controller Script----------------")]
    [SerializeField] public GameDataManager gameDataManager;
    [SerializeField] public EndGameController endGameController;

    private bool isSettingEnable;

    private CancellationTokenSource _cancellationTokenSource;
    [HideInInspector] public int playerLv = 1;

    void OnEnable()
    {
        Instance = this;
    }
    
    private void Start()
    {
        if(!PlayerPrefs.HasKey("Playerlv")){
            PlayerPrefs.SetInt("PlayerLv",playerLv);
        }else{
            playerLv = PlayerPrefs.GetInt("PlayerLv");
        }
        _cancellationTokenSource = new CancellationTokenSource();
        if (Debug.isDebugBuild)
        {
            textFPS.gameObject.SetActive(true);
        }
    }
    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("PlayerLv",playerLv);
    }
    
    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            textFPS.text ="FPS: " + (1f / Time.unscaledDeltaTime);
        }
    }

    public void OnEndGameConfig(){
        canvasEndGame.enabled = true;
    }

    private void OnDestroy()
    {
        _cancellationTokenSource.Cancel();
    }

    public void OnSettingMenuListener()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        
        void ToggleSettingsPanel(bool isActive)
        {
            settingBlurPanel.gameObject.SetActive(isActive);
            settingBlurPanel.DOFade(isActive ? 0.8f : 0, 1f);
            settingPanel.DOSizeDelta(isActive ? new Vector2(3000, 3000) : new Vector2(100, 100), 0.5f)
                .SetEase(isActive ? Ease.OutBounce : Ease.InBack)
                .WithCancellation(cancellationToken:_cancellationTokenSource.Token);
        }
        
        
        ToggleSettingsPanel(!isSettingEnable);
        isSettingEnable = !isSettingEnable;

    }
}

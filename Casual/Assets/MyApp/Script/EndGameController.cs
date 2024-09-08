using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EndGameController : MonoBehaviour
{
    public static EndGameController Instance;
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject rewardSpin;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI rewardCauculator;
    [SerializeField] private Button watchAdsBtn;
    [SerializeField] private Button PlayAgainBtn;
    [SerializeField] private Button CountinueBtn;
    [SerializeField] private RectTransform moneyImg;
    private int _slideValue = 0;
    private int _cashValue = 0;
    private float _startCashValue = 0f;

    [HideInInspector] public int cubeFinalNum;

    private CancellationTokenSource cts;

    [Button("RewardCauculatorTest")]
    public async void OnRewardCauculator(){

        await DOTween.Sequence()
        .Append(moneyImg.DOSizeDelta(new Vector2(175f, 175f),0.5f))
        .SetEase(Ease.OutBounce)
        .WithCancellation(this.GetCancellationTokenOnDestroy());


        endGamePanel.SetActive(true);
        cts = new CancellationTokenSource();

        _startCashValue = 0f;
        if(cubeFinalNum == 0){
            _cashValue = Random.Range(0,11);
        }else{
            _cashValue = cubeFinalNum;
        }

        await OnCashCounting(_startCashValue,0);

        if(SettingManager.Instance.gameDataManager.PlayerChance > 0){
            if(ObjectMoving.Instance.gameStatus == GameStatus.Complete){
                cts = new CancellationTokenSource();
                rewardSpin.SetActive(true);
                
                await DOTween.To(() => _slideValue, x => _slideValue = x,(int) slider.maxValue, 1f)
                .OnUpdate(() =>
                {
                    slider.value = _slideValue;
                })
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine).WithCancellation(cancellationToken:cts.Token);
                return;
            }
        }else{
            if(ObjectMoving.Instance.gameStatus == GameStatus.Fail){
                PlayAgainBtn.gameObject.SetActive(true);
            }
            if(ObjectMoving.Instance.gameStatus == GameStatus.Complete){
                watchAdsBtn.gameObject.SetActive(true);
                OnContinueShowUp();
            }

        }
    }

    public async UniTask OnCashCounting(float startValue,int plusValue){

        if (cubeFinalNum == 0)
        {
            await DOTween.To(() => startValue, x => startValue = x, 
                    _cashValue, 1f)
                .OnUpdate(() =>{
                    var convertToInt = Convert.ToInt32(startValue);
                    _startCashValue = startValue;
                    rewardCauculator.text = "<color=green>+" + convertToInt.ToString() + "$</color>";
                })
                .WithCancellation(cts.Token);
        }
        else
        {
            await DOTween.To(() => startValue, x => startValue = x, 
                    _cashValue/5f +plusValue, 1f)
                .OnUpdate(() =>{
                    var convertToInt = Convert.ToInt32(startValue);
                    _startCashValue = startValue;
                    rewardCauculator.text = "<color=green>+" + convertToInt.ToString() + "$</color>";
                })
                .WithCancellation(cts.Token);
        }

        cts.Cancel();
    }

    public void OnWatchADS(){
        //TODO ADS
    }

    public async void OnReplay(){
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        await SceneManager.LoadSceneAsync(sceneIndex,LoadSceneMode.Single);
    }
    public async void OnContinue(Button button){
        cts.Cancel();
        button.interactable = false;
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        await SettingManager.Instance.gameDataManager.OnCashChange(Convert.ToInt32(_startCashValue));
        await SceneManager.LoadSceneAsync(sceneIndex,LoadSceneMode.Single);
        
    }
    async void OnContinueShowUp(){

        await UniTask.Delay(TimeSpan.FromSeconds(3f));
        CountinueBtn.gameObject.SetActive(true);
    }

    [Button("Clamp Test")]
    private void OnClampTest(){
        var floatTest = 101f;
        var test =(int) floatTest/5;
        Debug.Log(test);
    }

    public async void RewardEvent(Button button){
        cts.Cancel();
        button.enabled = false;
        var plus = 0;
        if(slider.value > 0 && slider.value <= 43){
            Debug.Log(slider.value + "+5");
            plus = 5;
        }
        if(slider.value > 43 && slider.value <= 73){
            Debug.Log(slider.value + "+10");
             plus = 15;
        }
        if(slider.value > 73 && slider.value <= 90){
            Debug.Log(slider.value + "+25");
             plus = 30;
        }
        if(slider.value > 90 && slider.value <= 100){
            Debug.Log(slider.value + "+30");
             plus = 50;
        }

        cts = new CancellationTokenSource();
        await OnCashCounting(_startCashValue,plus);
        OnContinueShowUp();
    }
}

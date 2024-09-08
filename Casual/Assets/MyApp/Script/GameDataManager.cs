using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerCashUI;
    [SerializeField] private TextMeshProUGUI playerDiamondUI;
    [SerializeField] private TextMeshProUGUI playerChanceUI;

    //************************** Upgrade System **************************
    [Header("-----------------System-----------------")]
    [SerializeField] private TextMeshProUGUI playerInitCubeUpgradeUI;
    [SerializeField] private TextMeshProUGUI playerInitCubeUpgradeCostUI;
    [SerializeField] private TextMeshProUGUI playerEffortUI;
    [SerializeField] private TextMeshProUGUI playerEffortCostUI;

    private int _playerCash;
    private int _playerDiamond;
    private int _playerChance;
    private int _playerLv;
    private int _playerInitCubeUpgrade;
    private int _playerInitCubeCost;
    private int _playerEffortUpgrade;
    private int _playerEffortCostUpgrade;

    private ExtraUnitCost extraUnitCost = new ExtraUnitCost();
    private ExtraEffortCost extraEffortCost = new ExtraEffortCost();


    [Button("Clear Pref")]
    void OnClearPref(){
        PlayerPrefs.DeleteAll();
    }

    [Button("Check Data")]
    public void OnDataCheck()
    {
        Debug.LogWarning(PlayerLv);
    }

    public void OnExtraUnitCostUpgrade(){
        if(PlayerCash >= PlayerEffortCostUpgrade){
            PlayerInitCubeUpgrade++;
            PlayerInitCubeCost = extraUnitCost.extraUnitCostList[PlayerInitCubeUpgrade - 1];
        }

    }
    public void OnExtraEffortCostUpgrade(){
        if(PlayerCash >= PlayerEffortCostUpgrade){
            PlayerEffortUpgrade++;
            PlayerEffortCostUpgrade = extraEffortCost.extraEffortCostList[PlayerEffortUpgrade - 1];
        }

    }
    public async UniTask OnCashChange(int value){

        var cts = new CancellationTokenSource();

        await DOTween.To(() => SettingManager.Instance.gameDataManager.PlayerCash, 
        x => SettingManager.Instance.gameDataManager.PlayerCash = x, 
        value, 1f)
        .WithCancellation(cts.Token);

        cts.Cancel();
    }

    public int PlayerInitCubeUpgrade{
        get{ return _playerInitCubeUpgrade; }
        set{ 
            if(value > 7) return;
            _playerInitCubeUpgrade = value; 
            PlayerPrefs.SetInt("PlayerInitCubeUpgrade",value);
            playerInitCubeUpgradeUI.text = "Lv" + _playerInitCubeUpgrade.ToString();
            }
    }
    public int PlayerInitCubeCost{
        get{ return _playerInitCubeCost; }
        set{ 
            _playerInitCubeCost = value; 
            PlayerPrefs.SetInt("PlayerInitCubeCost",value);
            playerInitCubeUpgradeCostUI.text = _playerInitCubeCost.ToString();
            }
    }
    public int PlayerEffortUpgrade{
        get{ return _playerEffortUpgrade; }
        set{ 
            if(value > 5) return;
            _playerEffortUpgrade = value; 
            PlayerPrefs.SetInt("PlayerEffort",value);
            playerEffortUI.text = "Lv" + _playerEffortUpgrade.ToString();
            }
    }
    public int PlayerEffortCostUpgrade{
        get{ return _playerEffortCostUpgrade; }
        set{ 
            _playerEffortCostUpgrade = value; 
            PlayerPrefs.SetInt("PlayerEffortCost",value);
            playerEffortCostUI.text = _playerEffortCostUpgrade.ToString();
            }
    }
    
        public int PlayerCash{
        get{ return _playerCash; }
        set{ 
            _playerCash = value; 
            PlayerPrefs.SetInt("PlayerCash",value);
            playerCashUI.text = _playerCash.ToString();
            }
    }
        public int PlayerDiamond{
        get{ return _playerDiamond; }
        set{ 
            _playerDiamond = value; 
            PlayerPrefs.SetInt("PlayerDiamond",value);
            playerDiamondUI.text = _playerDiamond.ToString();
            }
    }
        public int PlayerChance{
        get{ return _playerChance; }
        set{ 
            _playerChance = value; 
            PlayerPrefs.SetInt("PlayerChance",value);
            playerChanceUI.text = _playerChance.ToString();
            }
    }
        public int PlayerLv{
        get{ return _playerLv; }
        set{ 
            _playerLv = value; 
            PlayerPrefs.SetInt("PlayerLv",value);
            }
    }

    void OnEnable()
    {
        PlayerCashConfig();
        PlayerDiamondConfig();
        PlayerLvConfig();
        PlayerChanceConfig();
        PlayerInitCubeUpgradeConfig();
        PlayerInitCubeCostConfig();
        PlayerEffortConfig();
        PlayerEffortCostConfig();

        PlayerCash = PlayerPrefs.GetInt("PlayerCash");
        PlayerDiamond = PlayerPrefs.GetInt("PlayerDiamond");
        PlayerChance = PlayerPrefs.GetInt("PlayerChance");
        PlayerLv = PlayerPrefs.GetInt("PlayerLv");
        PlayerInitCubeUpgrade = PlayerPrefs.GetInt("PlayerInitCubeUpgrade");
        PlayerInitCubeCost = PlayerPrefs.GetInt("PlayerInitCubeCost");
        PlayerEffortUpgrade = PlayerPrefs.GetInt("PlayerEffortUpgrade");
        PlayerEffortCostUpgrade = PlayerPrefs.GetInt("PlayerEffortCost");
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("PlayerCash",_playerCash);
        PlayerPrefs.SetInt("PlayerDiamond",_playerDiamond);
        PlayerPrefs.SetInt("PlayerLv",_playerLv);
        PlayerPrefs.SetInt("PlayerChance",_playerChance);
        PlayerPrefs.SetInt("PlayerInitCubeUpgrade",_playerInitCubeUpgrade);
        PlayerPrefs.SetInt("PlayerInitCubeCost",PlayerInitCubeCost);
        PlayerPrefs.SetInt("PlayerEffortUpgrade",_playerEffortUpgrade);
        PlayerPrefs.SetInt("PlayerEffortCost",PlayerEffortCostUpgrade);

    }

    public void PlayerCashConfig(){
        if(!PlayerPrefs.HasKey("PlayerCash")){
            PlayerPrefs.SetInt("PlayerCash",0);
        }
    }

    public void PlayerDiamondConfig(){
        if(!PlayerPrefs.HasKey("PlayerDiamond")){
            PlayerPrefs.SetInt("PlayerDiamond",0);
        }
    }

    public void PlayerLvConfig(){
        if(!PlayerPrefs.HasKey("PlayerLv")){
            PlayerPrefs.SetInt("PlayerLv",0);
        }
    }

    public void PlayerChanceConfig(){
        if(!PlayerPrefs.HasKey("PlayerChance")){
            PlayerPrefs.SetInt("PlayerChance",0);
        }
    }

    public void PlayerInitCubeUpgradeConfig(){
        if(!PlayerPrefs.HasKey("PlayerInitCubeUpgrade")){
            PlayerPrefs.SetInt("PlayerInitCubeUpgrade",1);
        }
    }
    public void PlayerInitCubeCostConfig(){
        if(!PlayerPrefs.HasKey("PlayerInitCubeCost")){
            PlayerPrefs.SetInt("PlayerInitCubeCost",extraUnitCost.extraUnitCostList[0]);
        }
    }

    public void PlayerEffortConfig(){
        if(!PlayerPrefs.HasKey("PlayerEffortUpgrade")){
            PlayerPrefs.SetInt("PlayerEffortUpgrade",1);
        }
    }
    public void PlayerEffortCostConfig(){
        if(!PlayerPrefs.HasKey("PlayerEffortCost")){
            PlayerPrefs.SetInt("PlayerEffortCost",extraEffortCost.extraEffortCostList[0]);
        }
    }
    

}
public class ExtraUnitCost{
    public List<int> extraUnitCostList = new List<int>(){100,250,500,1000,1500,2500,3500,5000,7000,10000};
}
public class ExtraEffortCost{
    public List<int> extraEffortCostList = new List<int>(){100,250,400,650,800,1250,1750,2500,5000,10000};
}

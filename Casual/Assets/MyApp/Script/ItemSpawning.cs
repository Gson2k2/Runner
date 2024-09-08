using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Status
    {
        Dynamic,
        Static
    }

    public enum Direction
    {
        Unset,
        Both,
        Left,
        Right
    }
    public enum MathCube
    {
        Unset,
        Plus,
        Multi
    }

public class ItemSpawning : MonoBehaviour
{

    public bool isEvent;

    [SerializeField] public Transform eventTrans;
    [SerializeField] public List<Transform> transformsPos;
    [SerializeField] public Transform transformsRight;
    [SerializeField] public Transform transformsLeft;

    [SerializeField] public List<ItemData> itemData;

    [ShowIf("isEvent"),EnumToggleButtons]public ObjectEventType objectEventType;
    [HideIf("objectEventType",ObjectEventType.Unset)] public GameObject eventCheckPoint;

    [ShowIf("objectEventType",ObjectEventType.FallHole),HideLabel, EnumToggleButtons]public Direction currentDirection;
    [HideIf("objectEventType",ObjectEventType.Unset),EnumToggleButtons] public Status EventPreconfig;
    [ShowIf("objectEventType",ObjectEventType.SpaceGravity),HideIf("EventPreconfig",Status.Dynamic)] public int gravityReq;
    [ShowIf("objectEventType",ObjectEventType.EnemyOutPost),HideIf("EventPreconfig",Status.Dynamic)] public int enemyOutPostReq;

    [HideIf("isEvent"),EnumToggleButtons] public Status Preconfig;


    private List<int> cubePlusValueList = new List<int>();
    private List<int> cubeMultiValueList = new List<int>();
    
    [ShowIf("Preconfig",Status.Static),EnumToggleButtons] public Direction DirectionSpawn;
    [ShowIf("Preconfig",Status.Static),HideIf("DirectionSpawn",Direction.Both),HideIf("DirectionSpawn",Direction.Unset),EnumToggleButtons] public MathCube math;
    [ShowIf("DirectionSpawn",Direction.Both),EnumToggleButtons] public MathCube leftMath;
    [HideIf("Preconfig",Status.Dynamic),ShowIf("leftMath",MathCube.Plus),ValueDropdown("cubePlusValueList")] public int leftPlusValue = 0;
    [HideIf("Preconfig",Status.Dynamic),ShowIf("leftMath",MathCube.Multi),ValueDropdown("cubeMultiValueList")] public int leftMultiValue = 0;

    [ShowIf("DirectionSpawn",Direction.Both),EnumToggleButtons] public MathCube rightMath;
    [HideIf("Preconfig",Status.Dynamic),ShowIf("rightMath",MathCube.Plus),ValueDropdown("cubePlusValueList")] public int rightPlusValue = 0;
    [HideIf("Preconfig",Status.Dynamic),ShowIf("rightMath",MathCube.Multi),ValueDropdown("cubeMultiValueList")] public int rightMultiValue = 0;

    [HideIf("Preconfig",Status.Dynamic),ShowIf("math",MathCube.Plus),ValueDropdown("cubePlusValueList")] public int PlusValue = 0;
    [HideIf("Preconfig",Status.Dynamic),ShowIf("math",MathCube.Multi),ValueDropdown("cubeMultiValueList")] public int MultiValue = 0;

    public void OnValidate()
    {
        transformsPos.Clear();
        transformsPos.Add(transformsLeft);
        transformsPos.Add(transformsRight);
        if(!isEvent){
            objectEventType = ObjectEventType.Unset;
            EventPreconfig = Status.Dynamic;
        }
        if(isEvent){
            Preconfig = Status.Dynamic;
            if(objectEventType is ObjectEventType.FallHole or ObjectEventType.Unset){
                EventPreconfig = Status.Dynamic;
            }
        }
        if(Preconfig == Status.Dynamic){
            DirectionSpawn = Direction.Unset;
            leftMath = MathCube.Unset;
            rightMath = MathCube.Unset;
            math = MathCube.Unset;
        }
        if(DirectionSpawn is Direction.Unset or Direction.Both){
            math = MathCube.Unset;
            PlusValue = 0;
            MultiValue = 0;
        }
        if(DirectionSpawn is Direction.Left or Direction.Right){
            leftMath = MathCube.Unset;
            rightMath = MathCube.Unset;
            leftPlusValue = 0;
            leftMultiValue = 0;
            rightPlusValue = 0;
            rightMultiValue = 0;
        }
        if(EventPreconfig == Status.Dynamic){
            gravityReq = 1;
            enemyOutPostReq = 1;
        }
        cubePlusValueList = ItemData.CubeValue.cubePlusValueList;
        cubeMultiValueList = ItemData.CubeValue.cubeMultiValueList;
    }

    void Start()
    {
        if(isEvent){
            eventCheckPoint.SetActive(true);
            ObjectEvent.Instance.OnSpawnEventItem(this,gravityReq);
            return;
        }
        if(Preconfig == Status.Dynamic){
            OnDynamicSpawn();
            return;
        }
        if(Preconfig == Status.Static){
            OnStaticSpawn();
        }

    }
    public void OnStaticSpawn(){
        switch (DirectionSpawn)
        {
            case Direction.Unset:{
                OnDynamicSpawn();
                break;
            }
            case Direction.Left:{
                ObjectManager.Instance.SingleSpawnCylinder(this,transformsLeft);
                break;
            }
            case Direction.Right:{
                ObjectManager.Instance.SingleSpawnCylinder(this,transformsRight);
                break;
            }
            case Direction.Both:{
                ObjectManager.Instance.MultiSpawnCyinder(this,transformsPos);
                break;
            }
        }
    }
    public void OnDynamicSpawn(){
        Preconfig = Status.Dynamic;
        var random = Random.Range(0, transformsPos.Count - 1);
        if(!FiftyFiftyChance()){
            foreach (var item in transformsPos)
            {
                ObjectManager.Instance.SingleSpawnCylinder(this,item.transform);
            }
        }else{
            ObjectManager.Instance.SingleSpawnCylinder(this,transformsPos[random].transform);
        }
    }
    bool FiftyFiftyChance() 
    {
        return Random.value < 0.5f;
    }
}

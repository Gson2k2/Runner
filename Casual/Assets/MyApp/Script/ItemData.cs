using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum DataType{
    Cash,Chance,Cube,Event
}
public enum CashValueEnum{
    Green,Red
}

public enum CubeSpawnType{
    None,Plus,Multi
}

public class ItemData : MonoBehaviour
{
    public DataType dataType;
    [ShowIf(("dataType"), DataType.Event)] public ObjectEventType objectEventType;

    [HideIf(("dataType"), DataType.Chance)]
    public TextMeshProUGUI textMeshValue;

    private string _textValue;
    [HideInInspector] public ItemSpawning itemSpawning;

    [ShowIf(("dataType"), DataType.Cash)] public CashValueEnum cashValueEnum;

    [ShowIf(("dataType"), DataType.Cash)] [SerializeField]
    public CashData cashData;

    [ShowIf(("dataType"), DataType.Cube)] [SerializeField]
    public CubeData cubeData;

    [ShowIf(("dataType"), DataType.Cube)] [SerializeField]
    public CubeSpawnType CubeSpawnType;


    [Header("--------------Gravity--------------")] [ShowIf(("objectEventType"), ObjectEventType.SpaceGravity)]
    public int gravityHoleReq;

    [HideInInspector] public int gravityHoleReqTemp;
    [HideInInspector] public int ranDelay;

    [Header("--------------Enemy OutPos--------------")] [ShowIf(("objectEventType"), ObjectEventType.EnemyOutPost)]
    public int enemyOutPostReq;

    [ShowIf(("objectEventType"), ObjectEventType.EnemyOutPost)]
    public Transform enemyInitPos;

    [ShowIf(("objectEventType"), ObjectEventType.EnemyOutPost)]
    public TextMeshProUGUI enemyNumPos;

    [ShowIf(("objectEventType"), ObjectEventType.EnemyOutPost)]
    public List<EnemyController> enemyList;

    [SerializeField] public bool isDynamic;
    [SerializeField] private bool isVisible;

    private int curGravityHoleReq;
    private int maxGravityHoleReq;

    private Collider objectCol;

    private float distanFactor = 0.2f;
    private float Radius = 0.5f;

    void Start()
    {
        objectCol = GetComponent<Collider>();
        OnPreConfig();
    }

    void OnPreConfig()
    {
        if (objectEventType == ObjectEventType.SpaceGravity)
        {
            maxGravityHoleReq = gravityHoleReq;
        }

        if (objectEventType == ObjectEventType.EnemyOutPost)
        {
            enemyNumPos.text = enemyOutPostReq.ToString();
        }
    }

    void LateUpdate()
    {
        if (dataType == DataType.Event && objectEventType == ObjectEventType.SpaceGravity)
        {
            textMeshValue.text = curGravityHoleReq + "/" + maxGravityHoleReq;
        }
        if (dataType == DataType.Event && objectEventType == ObjectEventType.EnemyOutPost)
        {
            enemyNumPos.text = enemyOutPostReq.ToString();
        }

        if (gravityHoleReqTemp > maxGravityHoleReq && objectEventType == ObjectEventType.SpaceGravity)
        {
            objectCol.enabled = false;
        }

        if (gravityHoleReq <= 0 && objectEventType == ObjectEventType.SpaceGravity)
        {
            ObjectMoving.Instance.speed = 10;
            ObjectMoving.Instance.limitValue = 5;
            gameObject.SetActive(false);
        }

        if (enemyOutPostReq <= 0 && objectEventType == ObjectEventType.EnemyOutPost)
        {
            ObjectMoving.Instance.speed = 10;
            ObjectMoving.Instance.limitValue = 5;
            gameObject.SetActive(false);
        }
    }

    [Button("Enemy Format")]
    public void CubeFormat(float time)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            var x = distanFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = distanFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var newPos = new Vector3(x, 0, z);

            enemyList[i].transform.DOLocalMove(newPos, time).SetEase(Ease.OutBack);
        }
    }

    private void OnBecameInvisible()
    {
        if (objectEventType == ObjectEventType.EnemyOutPost)
        {
            Debug.LogWarning("Invisible");
        }
        
    }

    void OnBecameVisible()
    {
        if (objectEventType == ObjectEventType.EnemyOutPost)
        {
            Debug.LogWarning("Visible");
        }
        

        if (!isDynamic) return;
        if (!isVisible)
        {
            isVisible = true;
            OnDynamicDataReConfig();
        }

    }

    void EnemyDynamicSpawn(int req)
    {
        Debug.LogWarning("Win" +req);
        for(int i = 0;i < req;i++){
                
            Debug.LogWarning("EnemySpawn" + i);
            var enemyClone = Instantiate(ObjectManager.Instance.enemyCube,enemyInitPos);

            enemyList.Add(enemyClone);
            enemyClone.itemData = this;
        }
        CubeFormat(0);
    }

    [Button("Force Dynamic")]
    public void OnDynamicDataReConfig()
    {
        List<int> value = new List<int>();
        if (objectEventType == ObjectEventType.EnemyOutPost)
        {
            Debug.LogWarning("Enemy Outpost sign");
            var req = ObjectMoving.Instance.numberofCube;
            var finalEnemyPosReq = 0;
            if (Random.value < 0.25f)
            {
                value = new List<int>() { 2,3 };
                finalEnemyPosReq = req / value.GetRandomItem();
                EnemyDynamicSpawn(finalEnemyPosReq);
            }
            else
            {
                List<int> valueMin = new List<int>();
                List<int> valueMax = new List<int>();
                
                valueMin = new List<int>() { 20,30,40};
                valueMax = new List<int>() { 60,70,80};
                if (ObjectMoving.Instance.numberofCube > 50)
                {
                    finalEnemyPosReq = valueMax.GetRandomItem();
                    EnemyDynamicSpawn(finalEnemyPosReq);
                }
                else
                {
                    finalEnemyPosReq = valueMin.GetRandomItem();
                    EnemyDynamicSpawn(finalEnemyPosReq);
                }
            }

            enemyOutPostReq = finalEnemyPosReq;
            enemyNumPos.text = finalEnemyPosReq.ToString();
        }
        if (objectEventType == ObjectEventType.SpaceGravity)
        {
            var req = ObjectMoving.Instance.numberofCube;
            if (Random.value < 0.25f)
            {
                Debug.LogWarning("SpaceGravity Spawn");

                value = new List<int>() { 10, 15, 20 };
                if (ObjectMoving.Instance.numberofCube > 50)
                {
                    gravityHoleReq = req - value.GetRandomItem() * 2;
                }
                else
                {
                    gravityHoleReq = req + value.GetRandomItem();
                }
            }
            else
            {
                Debug.LogWarning("SpaceGravity Spawn");

                value = new List<int>() { 2};
                gravityHoleReq = req / value.GetRandomItem();
            }

            OnPreConfig();
        }

        if (dataType == DataType.Cube)
        {
            if (ObjectMoving.Instance.numberofCube > 50)
            {
                CubeSpawnType = CubeSpawnType.Plus;
            }

            if (CubeSpawnType == CubeSpawnType.Plus)
            {
                cubeData.cubeValue = DynamicDataAutoConfig();
                textMeshValue.text = "+" + cubeData.cubeValue.ToString();
            }

            if (CubeSpawnType == CubeSpawnType.Multi)
            {
                cubeData.cubeValue = CubeValue.cubeMultiValueList.GetRandomItem();
                textMeshValue.text = "x" + cubeData.cubeValue.ToString();
            }

            Debug.Log(cubeData.cubeValue);
        }
    }

    int DynamicDataAutoConfig()
    {
        var value = 1;
        var data = ObjectMoving.Instance.numberofCube;

        if (CubeSpawnType == CubeSpawnType.Plus)
        {
            if (ObjectMoving.Instance.numberofCube > 100)
            {
                var randomDivide = Random.Range(3, 6);
                value = (int)data / randomDivide;
                return value;
            }

            if (ObjectMoving.Instance.numberofCube > 50)
            {
                var randomDivide = Random.Range(2, 3);
                value = (int)data / randomDivide;
                return value;
            }

            if (ObjectMoving.Instance.numberofCube <= 50)
            {
                value = CubeValue.cubePlusValueList.GetRandomItem();
                return value;
            }
        }

        return value;
    }

    async void OnGravityDataChange()
    {
        if (ObjectMoving.Instance.numberofCube < maxGravityHoleReq)
        {
            var maxCubeIndex = ObjectMoving.Instance.subPlayersList.Count;
            for (int i = 0; i < maxCubeIndex; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
                DataSync();
            }
        }

        if (ObjectMoving.Instance.numberofCube > maxGravityHoleReq)
        {
            for (int i = 0; i < maxGravityHoleReq; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
                DataSync();
            }
        }
    }

    void DataSync()
    {
        var randomChild = ObjectMoving.Instance.subPlayersList.GetRandomItem();
        ObjectMoving.Instance.subPlayersList.Remove(randomChild);
        randomChild.GetComponent<SubPlayerEvent>().EventTask(this);

        curGravityHoleReq++;
        gravityHoleReq--;
    }

    public void OnEnemyOutpostDataChange(EnemyController enemyController)
    {
        enemyOutPostReq--;
        Destroy(enemyController.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (dataType == DataType.Event)
        {
            if (objectEventType == ObjectEventType.SpaceGravity)
            {
                if (other.GetComponent<ObjectMoving>() == null) return;
                OnGravityDataChange();
            }

            if (objectEventType == ObjectEventType.EnemyOutPost)
            {
                if (other.GetComponent<ObjectMoving>() == null) return;
                OnCheck();
                enemyList.Shuffle();
                foreach (var item in enemyList)
                {
                    try
                    {
                        item.OnEnemyForceCharge();
                    }
                    catch (System.Exception)
                    {
                        Debug.LogWarning("Non Index");
                    }
                }
            }

            return;
        }

        if (other.GetComponent<SubPlayerEvent>() != null) return;
        foreach (Transform item in transform)
        {
            if (item != transform)
            {
                item.gameObject.SetActive(false);
            }
        }
        if(itemSpawning == null) return;
        foreach (var item in itemSpawning.itemData)
        {
            item.GetComponent<Collider>().enabled = false;
        }
    }

    void OnCheck()
    {
        foreach (EnemyController enemy in enemyList)
        {
            float minDistance = float.MaxValue;
            SubPlayerEvent closestObject = null;
            foreach (SubPlayerEvent subPlayer in ObjectMoving.Instance.subPlayersList)
            {
                if (!subPlayer.isTarget)
                {
                    float distance = Vector3.Distance(enemy.transform.position, subPlayer.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestObject = subPlayer;
                    }
                }
            }

            if (closestObject != null)
            {
                closestObject.isTarget = true;
                enemy.subPlayer = closestObject;
            }
            else
            {
                enemy.subPlayer = null;
            }

        }
    }

    [Serializable]
    public struct CubeData
    {
        public int cubeValue;
    }

    [Serializable]
    public struct CashData
    {
        public int cashValue;
    }

    public static class CashValue
    {
        public static List<int> cashValueList = new List<int>() { 5, 15, 20, 30 };
    }

    public static class CubeValue
    {
        public static List<int> cubeMultiValueList = new List<int>() { 2, 3 };
        public static List<int> cubePlusValueList = new List<int>() { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
    }
}

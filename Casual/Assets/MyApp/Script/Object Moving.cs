using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using GraphicRaycaster = UnityEngine.UI.GraphicRaycaster;

public enum ObjectType{
    Player,NPC
}
public enum GameStatus{
    Playing,Fail,Complete,Pause
}

public class ObjectMoving : MonoBehaviour
{

    public static ObjectMoving Instance;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public RectTransform countCanvas;
    [SerializeField] public GameObject cube;
    [SerializeField] public ObjectType objectType;
    [SerializeField] public float speed = 10f;
    [SerializeField] public float limitValue;

    [Range(0, 5f)] [SerializeField] private float distanFactor, Radius;

    public GraphicRaycaster graphicRaycaster;
    public TextMeshProUGUI numberOfCubeText;
    [HideInInspector] public EventSystem eventSystem;


    [HideInInspector] public bool isMoving;
    [HideInInspector] public int numberofCube;

    private bool isEndGame;
    [SerializeField] public GameStatus gameStatus;
    [SerializeField] public ParticleSystem teleParticle;

    public List<SubPlayerEvent> subPlayersList = new List<SubPlayerEvent>();

    private bool isStarted;

    private void OnEnable()
    {
        Instance = this;
        eventSystem = FindObjectOfType<EventSystem>();
    }

    private void LateUpdate()
    {
        ObjectMoving.Instance.numberOfCubeText.text = ObjectMoving.Instance.numberofCube.ToString();
    }


    private void Start()
    {
        ObjectSpawning(SettingManager.Instance.gameDataManager.PlayerInitCubeUpgrade);
    }


    void Update()
    {
        numberofCube = subPlayersList.Count;

        countCanvas.transform.position = Vector3.SmoothDamp(countCanvas.transform.position,
            new Vector3(transform.localPosition.x, -3f, transform.localPosition.z), ref canvasVelocity, 0.05f, 1000f);

        if (gameStatus == GameStatus.Complete)
        {
            teleParticle.transform.position = transform.position;
        }

        if (numberofCube <= 0 && gameStatus != GameStatus.Complete)
        {
            isMoving = false;
            teleParticle.Stop();

            SettingManager.Instance.endGameController.cubeFinalNum = numberofCube;
            if (!isEndGame)
            {
                OnEndGameInvoke(1f);
                gameStatus = GameStatus.Fail;
                isEndGame = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isStarted)
            {
                isMoving = true;
            }

            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);
            if (!isMoving)
            {
                if (results.First().gameObject.CompareTag("ClickToPlay"))
                {
                    isStarted = true;
                    results.First().gameObject.gameObject.SetActive(false);

                    UiController.Instance.OnExtraUnitAndExtraEffortAnim(UiAction.Close);
                }
            }
        }

        if (!isMoving) return;
        Vector3 movement = new Vector3(0, 0, 1);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * Time.deltaTime);

        mainCamera.transform.position = new Vector3(
            transform.position.x / 1.5f,
            mainCamera.transform.position.y,
            transform.position.z - 8.75f
        );


        if (objectType == ObjectType.Player)
        {
            if (Input.GetMouseButton(0))
            {
                MovePlayer();
            }
        }
    }

    public void OnTest()
    {

    }

    async void OnEndGameInvoke(float time)
    {
        if (gameStatus == GameStatus.Complete)
        {
            teleParticle.Stop();
            teleParticle.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(time / 2));
            SettingManager.Instance.endGameController.cubeFinalNum = numberofCube;
            foreach (var item in subPlayersList)
            {
                Destroy(item.gameObject);
            }

        }


        await UniTask.Delay(TimeSpan.FromSeconds(time / 2));
        Debug.Log("End Game");
        SettingManager.Instance.endGameController.OnRewardCauculator();
    }

    private CancellationTokenSource CubeFormatToken;

    void CubeFormat(float time)
    {
        if (CubeFormatToken != null)
        {
            CubeFormatToken.Cancel();
        }

        CubeFormatToken = new CancellationTokenSource();
        for (int i = 0; i < transform.childCount; i++)
        {
            var x = distanFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = distanFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var newPos = new Vector3(x, 0, z);

            transform.GetChild(i).DOLocalMove(newPos, time).SetEase(Ease.OutBack)
                .WithCancellation(CubeFormatToken.Token);
        }
    }

    [Button("Spawn Cube")]
    public void ObjectSpawning(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var cubeClone = Instantiate(cube, transform.position, quaternion.identity, transform);

            DOTween.Sequence().AppendInterval(i % 4)
                .Append(cubeClone.transform
                    .DOLocalRotate(
                        new Vector3(cubeClone.transform.localRotation.x, -90, cubeClone.transform.localRotation.z), 1f))
                .SetLoops(-1).WithCancellation(cubeClone.GetCancellationTokenOnDestroy());
        }

        CubeFormat(1f);
    }

    private Vector3 playerVelocity;
    private Vector3 canvasVelocity;

    void MovePlayer()
    {
        var halfScreen = Screen.width / 2;
        var xPos = (Input.mousePosition.x - halfScreen) / halfScreen;
        var finalXpos = Mathf.Clamp(xPos * limitValue, -limitValue, limitValue);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
            new Vector3(finalXpos, -4.875f, -5),
            ref playerVelocity, 0.2f, 20f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            CubeFormat(0.5f);
        }

        if (other.CompareTag("Complete"))
        {
            gameStatus = GameStatus.Complete;
            DOTween.Sequence()
                .AppendInterval(2f)
                .AppendCallback(() =>
                {
                    isMoving = false;
                    OnEndGameInvoke(2f);
                }).WithCancellation(this.GetCancellationTokenOnDestroy());
        }

        if (other.GetComponent<ItemData>() != null)
        {
            Debug.Log("Trigger Item");
            var otherComp = other.GetComponent<ItemData>();
            if (otherComp.dataType == DataType.Cash)
            {
                if (otherComp.cashValueEnum == CashValueEnum.Green)
                {
                    var tempCash = SettingManager.Instance.gameDataManager.PlayerCash + otherComp.cashData.cashValue;
                    SettingManager.Instance.gameDataManager.OnCashChange(tempCash);
                    Debug.Log(otherComp.cashData.cashValue + "/" + tempCash);
                }
                else if (otherComp.cashValueEnum == CashValueEnum.Red)
                {
                    var tempCash = SettingManager.Instance.gameDataManager.PlayerCash - otherComp.cashData.cashValue;
                    if (tempCash <= 0)
                    {
                        SettingManager.Instance.gameDataManager.PlayerCash = 0;
                        return;
                    }

                    SettingManager.Instance.gameDataManager.OnCashChange(tempCash);

                    Debug.Log(otherComp.cashData.cashValue + "/" + tempCash);
                }
            }

            if (otherComp.dataType == DataType.Event)
            {
                if (otherComp.objectEventType == ObjectEventType.SpaceGravity ||
                    otherComp.objectEventType == ObjectEventType.EnemyOutPost)
                {
                    speed = 1;
                    limitValue = 0;
                }
            }

            if (otherComp.dataType == DataType.Cube)
            {
                if (otherComp.CubeSpawnType == CubeSpawnType.Plus)
                {
                    ObjectSpawning(otherComp.cubeData.cubeValue);
                }

                if (otherComp.CubeSpawnType == CubeSpawnType.Multi)
                {
                    if (numberofCube <= 1)
                    {
                        ObjectSpawning(otherComp.cubeData.cubeValue);
                    }
                    else
                    {
                        Debug.Log("Plus");
                        ObjectSpawning((numberofCube * otherComp.cubeData.cubeValue) - numberofCube);
                    }

                }
            }

        }
    }
}

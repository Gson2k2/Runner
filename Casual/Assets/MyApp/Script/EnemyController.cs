using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public Collider _collider;
    [SerializeField] public ItemData itemData;
    [HideInInspector] public SubPlayerEvent subPlayer;

    private bool isCharge;
    void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public async void OnEnemyForceCharge(){
        var randomDelay = Random.Range(0f, 0.5f);
        await UniTask.Delay(TimeSpan.FromSeconds(randomDelay));

        isCharge = true;
        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        // if(subPlayer != null){
        //     OnEnemyDestroy();
        //     Destroy(subPlayer.gameObject);
        // }
    }

    void OnTriggerEnter(Collider other)
    {
        if(subPlayer == null) return;
        if(other.gameObject == subPlayer.gameObject){
                        OnEnemyDestroy();
            Destroy(subPlayer.gameObject);
        }
    }

    private Vector3 velocity;
    void Update()
    {
        if(isCharge && subPlayer != null){
            transform.position = Vector3.SmoothDamp(transform.position,
            new Vector3(subPlayer.transform.position.x, 
            subPlayer.transform.position.y,
            subPlayer.transform.position.z),
            ref velocity,0.25f,500f);
        }
    }
    public void OnEnemyDestroy(){
        ObjectManager.Instance.OnFlashControl(itemData.enemyInitPos);
        OnDataUpdate();
    }
    void OnDataUpdate(){
        itemData.OnEnemyOutpostDataChange(this);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubPlayerEvent : MonoBehaviour
{

    [SerializeField] private ConstantForce _constantForce;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

    [HideInInspector] public float randomDelay;

    [HideInInspector] public bool isTarget;
    void OnEnable()
    {
        isTarget = false;
    }

    void Start()
    {
        ObjectMoving.Instance.subPlayersList.Add(this);
    }

    void OnDestroy()
    {
        if(ObjectMoving.Instance.subPlayersList.Contains(this)){
            ObjectMoving.Instance.subPlayersList.Remove(this);
        }
    }
    public void EventTask(ItemData itemData){
            itemData.gravityHoleReqTemp++;

            if(itemData.itemSpawning.objectEventType == ObjectEventType.SpaceGravity){
                _constantForce.force = new Vector3(0,30,0);
                DestroyObjectDelay(3f,true);
            }

    }
    public async void FallHoleEventTask(ItemData itemData){
        randomDelay = UnityEngine.Random.Range(0f, 1f);
        int delayInMilliseconds = (int)(randomDelay * 1000);

        if(ObjectMoving.Instance.subPlayersList.Contains(this)){
            ObjectMoving.Instance.subPlayersList.Remove(this);
        }

        if(itemData.itemSpawning.objectEventType == ObjectEventType.FallHole){
            await UniTask.Delay(TimeSpan.FromMilliseconds(delayInMilliseconds/4));
            _constantForce.force = new Vector3(0,-10,0);
            DestroyObjectDelay(2f,true);
            Debug.Log("Fall");
        }

    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EventItem") && other.GetComponent<ItemData>() != null){
            var otherComp = other.GetComponent<ItemData>();
            if(otherComp.objectEventType == ObjectEventType.FallHole){
                Debug.Log("Trigger FallHole");
                FallHoleEventTask(otherComp);
            }
        }
    }
    public async void DestroyObjectDelay(float time,bool useRB){
        if(ObjectMoving.Instance.subPlayersList.Contains(this)){
            ObjectMoving.Instance.subPlayersList.Remove(this);
        }
        if(useRB){
            transform.SetParent(null);
            _collider.enabled = false;
            _rigidbody.isKinematic = false;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(time));
        Destroy(gameObject);
    }
}

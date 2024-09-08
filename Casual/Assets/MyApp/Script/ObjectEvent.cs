using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public enum ObjectEventType{
    Unset,SpaceGravity,FallHole,EnemyOutPost
}
public class ObjectEvent : MonoBehaviour
{
    public static ObjectEvent Instance;
    void OnEnable()
    {
        Instance = this;
    }

    public void OnSpawnEventItem(ItemSpawning itemSpawning,int value){
        var objectEventType = itemSpawning.objectEventType;
        if(itemSpawning.objectEventType == ObjectEventType.EnemyOutPost){
            var enemyCubeClone = Instantiate(ObjectManager.Instance.enemyOutPost,itemSpawning.eventTrans);
            var enemyOutPosComp = enemyCubeClone.GetComponent<ItemData>();
            enemyOutPosComp.itemSpawning = itemSpawning;
            enemyOutPosComp.enemyOutPostReq = itemSpawning.enemyOutPostReq;
            if (enemyOutPosComp.itemSpawning.EventPreconfig == Status.Dynamic)
            {
                enemyOutPosComp.isDynamic = true;
                enemyOutPosComp.objectEventType = objectEventType;
                return;
            }
            for(int i = 0;i < itemSpawning.enemyOutPostReq;i++){
                
                Debug.Log(i);
                var enemyClone = Instantiate(ObjectManager.Instance.enemyCube,enemyOutPosComp.enemyInitPos);

                enemyOutPosComp.enemyList.Add(enemyClone);
                enemyClone.itemData = enemyOutPosComp;
            }
            enemyOutPosComp.CubeFormat(0.5f);

        }
        if(itemSpawning.objectEventType == ObjectEventType.SpaceGravity){
            var gravityCylinder = Instantiate(ObjectManager.Instance.gravityCyinder,itemSpawning.eventTrans);
            var gravityCylinderComp = gravityCylinder.GetComponent<ItemData>();

            gravityCylinderComp.itemSpawning = itemSpawning;
            if(gravityCylinderComp.itemSpawning.EventPreconfig == Status.Dynamic){
                gravityCylinderComp.isDynamic = true;
                gravityCylinderComp.gravityHoleReq = value;
            }else{
                gravityCylinderComp.gravityHoleReq = value;
            }
            gravityCylinderComp.objectEventType = objectEventType;
        }
        if(itemSpawning.objectEventType == ObjectEventType.FallHole){
            if (itemSpawning.currentDirection == Direction.Unset || itemSpawning.currentDirection == Direction.Both)
            {
                var fallHole = Instantiate(ObjectManager.Instance.fallHole,itemSpawning.transformsPos.GetRandomItem());
                var fallHoleComp = fallHole.GetComponent<ItemData>();
                fallHoleComp.itemSpawning = itemSpawning;
                fallHoleComp.objectEventType = objectEventType;
                return;
            }
            if(itemSpawning.currentDirection == Direction.Left){
                var fallHole = Instantiate(ObjectManager.Instance.fallHole,itemSpawning.transformsPos.First());
                var fallHoleComp = fallHole.GetComponent<ItemData>();
                fallHoleComp.itemSpawning = itemSpawning;
                fallHoleComp.objectEventType = objectEventType;
                return;
            }
            if(itemSpawning.currentDirection == Direction.Right){
                var fallHole = Instantiate(ObjectManager.Instance.fallHole,itemSpawning.transformsPos.Last());
                var fallHoleComp = fallHole.GetComponent<ItemData>();
                fallHoleComp.itemSpawning = itemSpawning;
                fallHoleComp.objectEventType = objectEventType;
                return;
            }
            
        }
    }
}


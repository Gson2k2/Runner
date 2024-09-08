using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance;

    public List<ItemData> listCylinder;
    public ItemData greyClinder;
    public EnemyController enemyCube;

    public GameObject gravityCyinder;
    public GameObject fallHole;
    public GameObject enemyOutPost;
    public GameObject flash;

    public List<ParticleSystem> flashParticle;

    void OnEnable()
    {
        Instance = this;
    }

    public void OnFlashControl(Transform pos)
    {
        flash.transform.position = pos.position;
        foreach (var item in flashParticle)
        {
            item.Stop();
            item.Play();
        }
    }

    void StaticConfig(ItemSpawning itemSpawning, Transform cylinderTrans)
    {
        ItemData cylinder = Instantiate(greyClinder, cylinderTrans);
        itemSpawning.itemData.Add(cylinder);
        cylinder.itemSpawning = itemSpawning;
        CylinderStaticDataSetup(cylinder);
    }

    void DynamicConfig(ItemSpawning itemSpawning, Transform transform)
    {
        int cylinderChance = Random.Range(0, 101);
        ItemData cylinder;

        cylinder = Instantiate(cylinderChance < 1 ? listCylinder.GetRandomItem() : greyClinder, transform);

        cylinder.itemSpawning = itemSpawning;

        itemSpawning.itemData.Add(cylinder);
        CylinderDynamicDataSetUp(cylinder);
    }

    public void MultiSpawnCyinder(ItemSpawning itemSpawning, List<Transform> transforms)
    {
        switch (itemSpawning.Preconfig)
        {
            case Status.Static when itemSpawning.leftMath == MathCube.Unset || itemSpawning.rightMath == MathCube.Unset:
                itemSpawning.Preconfig = Status.Dynamic;
                DynamicConfig(itemSpawning, transforms.First());
                DynamicConfig(itemSpawning, transforms.Last());
                return;
            case Status.Static:
            {
                ItemData cylinderOne = Instantiate(greyClinder, transforms.First());
                ItemData cylinderTwo = Instantiate(greyClinder, transforms.Last());
                itemSpawning.itemData.Add(cylinderOne);
                itemSpawning.itemData.Add(cylinderTwo);

                switch (itemSpawning.leftMath)
                {
                    case MathCube.Plus:
                        cylinderOne.cubeData.cubeValue = itemSpawning.leftPlusValue;
                        cylinderOne.textMeshValue.text = "+" + cylinderOne.cubeData.cubeValue.ToString();
                        break;
                    case MathCube.Multi:
                        cylinderOne.cubeData.cubeValue = itemSpawning.leftMultiValue;
                        cylinderOne.textMeshValue.text = "x" + cylinderOne.cubeData.cubeValue.ToString();
                        break;
                }

                switch (itemSpawning.rightMath)
                {
                    case MathCube.Plus:
                        cylinderTwo.cubeData.cubeValue = itemSpawning.rightPlusValue;
                        cylinderTwo.textMeshValue.text = "+" + cylinderTwo.cubeData.cubeValue.ToString();
                        break;
                    case MathCube.Multi:
                        cylinderTwo.cubeData.cubeValue = itemSpawning.rightMultiValue;
                        cylinderTwo.textMeshValue.text = "x" + cylinderTwo.cubeData.cubeValue.ToString();
                        break;
                }

                break;
            }
        }
    }

    public void SingleSpawnCylinder(ItemSpawning itemSpawning, Transform cylinderTrans)
    {
        switch (itemSpawning.Preconfig)
        {
            case Status.Dynamic:
                DynamicConfig(itemSpawning, cylinderTrans);
                return;
            case Status.Static when itemSpawning.DirectionSpawn == Direction.Unset:
                itemSpawning.Preconfig = Status.Dynamic;
                SingleSpawnCylinder(itemSpawning, cylinderTrans.transform);
                return;
            case Status.Static:
            {
                if (itemSpawning.DirectionSpawn is Direction.Left or Direction.Right)
                {
                    if (itemSpawning.math == MathCube.Unset)
                    {
                        itemSpawning.Preconfig = Status.Dynamic;
                        SingleSpawnCylinder(itemSpawning, cylinderTrans.transform);
                        return;
                    }
                }

                StaticConfig(itemSpawning, cylinderTrans);
                return;
            }
        }
    }

    void ItemCashSetUp(ItemData itemData, string color)
    {
        itemData.cashData.cashValue = ItemData.CashValue.cashValueList.GetRandomItem();
        if (color.Equals("Green", StringComparison.OrdinalIgnoreCase))
        {
            itemData.textMeshValue.text =
                "<color=" + color + ">+" + itemData.cashData.cashValue.ToString() + "$</color>";
        }

        if (color.Equals("Red", StringComparison.OrdinalIgnoreCase))
        {
            itemData.textMeshValue.text =
                "<color=" + color + ">-" + itemData.cashData.cashValue.ToString() + "$</color>";
        }

    }

    void SingleItemCubeSetUp(ItemData itemData, CubeSpawnType cubeSpawnType)
    {
        itemData.CubeSpawnType = cubeSpawnType;
        switch (cubeSpawnType)
        {
            case CubeSpawnType.Plus:
            {
                switch (itemData.itemSpawning.PlusValue)
                {
                    case <= 0:
                        itemData.cubeData.cubeValue = ItemData.CubeValue.cubePlusValueList.GetRandomItem();
                        break;
                    case > 0:
                        itemData.cubeData.cubeValue = itemData.itemSpawning.PlusValue;
                        break;
                }

                itemData.textMeshValue.text = "+" + itemData.cubeData.cubeValue.ToString();
                break;
            }
            case CubeSpawnType.Multi:
            {
                switch (itemData.itemSpawning.PlusValue)
                {
                    case <= 0:
                        itemData.cubeData.cubeValue = ItemData.CubeValue.cubeMultiValueList.GetRandomItem();
                        break;
                    case > 0:
                        itemData.cubeData.cubeValue = itemData.itemSpawning.MultiValue;
                        break;
                }

                itemData.cubeData.cubeValue = itemData.itemSpawning.MultiValue;
                itemData.textMeshValue.text = "x" + itemData.cubeData.cubeValue.ToString();
                break;
            }
        }
    }

    void DynamicItemCubeSetUp(ItemData itemData, CubeSpawnType cubeSpawnType)
    {
        itemData.CubeSpawnType = cubeSpawnType;
        itemData.isDynamic = true;
    }

    public void CylinderStaticMultiDataSetup(ItemData itemData, MathCube mathCube)
    {
        switch (mathCube)
        {
            case MathCube.Plus:
                SingleItemCubeSetUp(itemData, CubeSpawnType.Plus);
                break;
            case MathCube.Multi:
                SingleItemCubeSetUp(itemData, CubeSpawnType.Multi);
                break;
        }
    }

    void CylinderStaticDataSetup(ItemData itemData)
    {
        switch (itemData.itemSpawning.math)
        {
            case MathCube.Plus:
                SingleItemCubeSetUp(itemData, CubeSpawnType.Plus);
                break;
            case MathCube.Multi:
                SingleItemCubeSetUp(itemData, CubeSpawnType.Multi);
                break;
        }
    }

    void CylinderDynamicDataSetUp(ItemData itemData)
    {
        switch (itemData.dataType)
        {
            case DataType.Cash:
            {
                ItemCashSetUp(itemData, itemData.cashValueEnum == CashValueEnum.Green ? "Green" : "Red");
                break;
            }
            case DataType.Cube:
            {
                var randomCubeData = Random.Range(0, 2);
                switch (randomCubeData)
                {
                    case 0:
                        DynamicItemCubeSetUp(itemData, CubeSpawnType.Plus);
                        break;
                    case 1:
                        DynamicItemCubeSetUp(itemData, CubeSpawnType.Multi);
                        break;
                }

                break;
            }
        }
    }

}

using System;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtensionCode{
    private static System.Random rng = new System.Random();  

    public static T GetRandomItem<T>(this List<T> list)
    {
        int index = rng.Next(list.Count);
        return list[index];
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
    public static bool IndexInRange<T>(this List<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    public static void SetFirstIndexToLastIndex<T>(this List<T> list)
    {
        T firstItem = list[0];
        list.RemoveAt(0);
        list.Add(firstItem);
    }
}

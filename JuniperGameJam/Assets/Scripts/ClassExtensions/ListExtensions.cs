using UnityEngine;
using System.Collections.Generic;

public static class ListExtensions
{
    /// <summary>
    /// Shuffles the elements of a given list, modifies base list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i.IsValidIndex(list))
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
    }

    /// <summary>
    /// Checks if an index is within the bounds of the given list and if the list is not null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <param name="list"></param>
    /// <returns> a bool if the above conditions are all true</returns>
    public static bool IsValidIndex<T>(this int index, IList<T> list)
    {
        return list != null && index >=0 && index < list.Count;
    }

    /// <summary>
    /// Gets and returns the last item in a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns> the last item in a list, type agnostic</returns>
    public static T GetLastItem<T>(this List<T> list)
    {
        return list[list.Count - 1];
    }

    public static T GetRandomItem<T>(this List<T> list)
    {
        int rand = Random.Range(0, list.Count); 
        return list[rand];
    }
    
}

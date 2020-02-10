using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling<T> : MonoBehaviour
{
    private T[] pool;
    private GameObject[] poolObject;
    private int pointer = 0;
    private int size;

    public void MakePool(Transform parent, GameObject prefab, int size)
    {
        this.size = size;
        pool = new T[size];
        poolObject = new GameObject[size];

        for (int i = 0; i < size; ++i)
        {
            poolObject[i] = Instantiate(prefab, parent);
            pool[i] = poolObject[i].GetComponent<T>();
            poolObject[i].SetActive(false);
        }

        pointer = 0;
    }

    public T GetObject()
    {
        poolObject[pointer].SetActive(true);
        T obj = pool[pointer];

        pointer = pointer + 1 >= size ? 0 : pointer + 1; 

        return obj;
    }
}

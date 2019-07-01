using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// A generic spawner that re-instantiates objects when the 
/// currentItem has been taken for a random amount of time.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    public T prefab;
    public float respawnMin;
    public float respawnMax;

    private T currentItem;
    private float respawnTimer;

    private void Awake()
    {
        respawnMin = Mathf.Min(respawnMin, respawnMax);
        respawnMax = Mathf.Max(respawnMin, respawnMax);
    }

    private void Update()
    {
        if (currentItem != null && prefab != null)
            return;

        respawnTimer += Time.deltaTime;

        if(respawnTimer >= Random.Range(respawnMin, respawnMax))
            Spawn();
    }

    private void Spawn()
    {

        respawnTimer = 0f;

        currentItem = Instantiate(prefab, transform.position, Quaternion.identity);
        
        if (currentItem.gameObject != null)
            currentItem.transform.SetParent(transform);
    }

    public T Take()
    {
        T item = currentItem;
        currentItem = null;

        return item;
    }

    public T CurrentItem { get { return currentItem; } }
}


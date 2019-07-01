using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntityManager : MonoBehaviour {

    public static MovingEntityManager instance;

    public float separation;
    public float cohesion;
    public float alignment;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

}

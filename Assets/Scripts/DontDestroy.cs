using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class DontDestroy : MonoBehaviour {

    public Action<GameObject> callback;

    // Use this for initialization
    void Awake()
    {
        callback = null;
        DontDestroyOnLoad(transform.gameObject);
    }

    void DoDestroy()
    {
        if (callback != null)
        {
            callback(transform.gameObject);
        }
        //Destroy(transform.gameObject);
    }
}

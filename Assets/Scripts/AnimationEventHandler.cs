using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
    public Action<GameObject> callback;

    void Awake()
    {
        callback = null;
    }

    void DoDestroy()
    {
        if (callback != null)
        {
            callback(transform.gameObject);
        }
    }
}

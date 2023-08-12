using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; set; }

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        Instance = this as T;
    }
}

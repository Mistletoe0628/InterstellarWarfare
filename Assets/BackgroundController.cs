using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    Vector2 _scrollVelocity;

    Material _material;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _material.mainTextureOffset += _scrollVelocity * Time.deltaTime;
    }
}

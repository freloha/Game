using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionScrollder : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float scrollRange = 9.9f;
    [SerializeField]
    private float moveSpeed = 3.0f;
    [SerializeField]
    private Vector3 moveDirection = Vector3.down;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if(transform.position.y <= -scrollRange)
        {
            transform.position = target.position + Vector3.up * scrollRange;
        }
    }
}

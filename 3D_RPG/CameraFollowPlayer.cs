using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    public Transform target;
    [SerializeField]
    public Vector3 offset; // offset은 보정값

    void Update()
    {
        transform.position = target.position + offset;
    }
}

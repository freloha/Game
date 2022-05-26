using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    public Transform target;
    [SerializeField]
    public Vector3 offset; // offset�� ������

    void Update()
    {
        transform.position = target.position + offset;
    }
}

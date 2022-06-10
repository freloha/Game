using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;    // 근접인지 체크
    public bool isRock;

    void OnCollisionEnter(Collision collision)
    {
        if(!isRock && collision.gameObject.tag == "Floor") // 탄피가 바닥에 떨어지면
        {
            Destroy(gameObject, 3);     // 3초 후 사라짐
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == "Wall")    //총알이 벽에 닿으면
        {
            Destroy(gameObject);     // 바로 사라짐
        }
    }
}

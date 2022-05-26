using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage; // 총알 데미지

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3); // 땅바닥에 닿으면 3초 후 삭제
        }
        if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject); // 땅바닥에 닿으면 3초 후 삭제
        }
    }
}

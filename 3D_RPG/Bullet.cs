using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage; // �Ѿ� ������

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3); // ���ٴڿ� ������ 3�� �� ����
        }
        if(collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject); // ���ٴڿ� ������ 3�� �� ����
        }
    }
}

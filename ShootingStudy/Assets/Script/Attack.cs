using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 부딫힌 오브젝트 사망처리 (적)
            collision.GetComponent<Enemy>().OnDie();
            // 내 오브젝트 삭제 (발사체)
            Destroy(gameObject);
        }
    }
}

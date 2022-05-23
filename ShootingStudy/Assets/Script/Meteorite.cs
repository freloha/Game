using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [SerializeField]
    private int damage = 5;

    //운석에 부딪힌 오브젝트의 태그가 "Player"이면
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 운석의 데미지 만큼 플레이어 체력 감소
            collision.GetComponent<PlayerHP>().TakeDamage(damage);

            // 운석 제거
            Destroy(gameObject);
        }
    }
}

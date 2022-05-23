using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private int scorePoint = 100; // 적 처치시 획득 점수
    private PlayerController playerController;

    private void Awake()
    {
        // 현재 코드에서는 한번만 호출하기 때문에 OnDie()에서 바로 호출해도 되지만
        // 오브젝트 폴링을 이용해 오브젝트를 재사용할 경우에는 최초 한번만 Find를 이용해
        // PlayerController의 정보를 저장해두고 사용하는 것이 연산에 효율적이다.
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // 적에게 부딪힌 오브젝트의 태그가 "Player" 이면
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 적 공격력만큼 플레이어 체력 감소
            collision.GetComponent<PlayerHP>().TakeDamage(damage);

            // 적 사망
            OnDie();
        }
    }

    public void OnDie() // 적이 죽었을 시 호출되는 함수
    {
        //플레이어의 점수를 scorePoint만큼 증가시킨다
        playerController.Score += scorePoint;
        // 적 오브젝트 삭제
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;  // 터지는 순간 폭탄 멈춤
        rigid.angularVelocity = Vector3.zero;   // 구르는 것도 멈춤

        meshObj.SetActive(false);   // 터지므로 수류탄은 사라짐
        effectObj.SetActive(true);  // 동시에 폭발 이펙트 활성화

        /*
         * 폭발한 시점에서 주변 적들에게 데미지를 주어야 함
         */
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
            15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));    // 수류탄 폭발범위부터 15간격에 있는 적들 모두 레이어마스크로 받아와서 처리

        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5); // 파티클(이펙트)가 사라지는 것까지 기다려줘야 하므로 딜레이를 둠
    }
}

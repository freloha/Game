using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range}; // Melee : 부딫히다, 근접공격을 의미, Range : 원거리 공격
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;       // 근접공격 범위
    public TrailRenderer trailEffect;   // 잔상을 그려주는 컴포넌트

    public Transform bulletPos;         // 총알 위치
    public GameObject bullet;           // 총알 프리펩을 저장할 변수로 사용
    public Transform bulletCasePos;     // 총알 케이스 위치
    public GameObject bulletCase;       // 총알 케이스 프리펩을 저장할 변수로 사용

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--; // 총알 하나씩 제거
            StartCoroutine("Shot"); // 코루틴으로 총 발사
        }
    }

    IEnumerator Swing() // 휘두르면 Collider와 TrailRenderer를 키고 끄면 됨, 코루틴은 yield 무조건 하나 이상 있어야 함
    {
        yield return new WaitForSeconds(0.1f); // 0.1 초 대기, 1프레임 쉬고 싶으면 yield return null, 사용 멈추고 싶으면 yield break;
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        trailEffect.enabled = false;
        
    }

    // Use() 메인루틴 -> Swing() 서브 루틴 -> Use() 메인루틴
    // 코루틴 사용 시 --> Use() 메인루틴 + Swing() 코루틴 (동시 실행, Co-Op)

    IEnumerator Shot()
    {
        // 총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        // 탄피 배출
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 위쪽으로 토크를 추가, 나선형을 토크
    }
}

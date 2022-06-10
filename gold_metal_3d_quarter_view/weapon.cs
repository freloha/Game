using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; // Melee : 근접 공격, Range : 원거리 공격
    public Type type;   // 해당 무기는 어떤 공격 타입인가
    public int Damage;  // 해당 무기의 데미지
    public float rate;  // 공격 속도
    public int maxAmmo; // 총알 최대 소지 개수
    public int curAmmo; // 총알 현재 소지 개수

    public BoxCollider meleeArea;   // 공격 범위
    public TrailRenderer trailEffect; // 트레일 렌더러는 충돌체가 이동하는 장면에서 뒤의 흔적(궤적)을 만드는데 사용

    public Transform bulletPos;     // 총알의 위치
    public GameObject bullet;       // 총알 오브젝트
    public Transform bulletCasePos; // 총알 케이스의 위치
    public GameObject bulletCase;   // 총알 케이스 오브젝트

    public void Use()
    {
        if(type == Type.Melee)
        {            
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0)  // 무기 타입이 원거리면서 총알이 있을 때
        {
            curAmmo--;  // 총알이 하나씩 줄어듦
            StartCoroutine("Shot");
        }
    }

    /*
     * Use() 메인루틴 -> Swing() 서브 루틴 -> Use() 메인루틴
     * 
     * 만일 Swing이 coroutine(코루틴)이면
     * Use() 메인루틴 + Swing 코루틴 (Co-Op)
     * 시간 차 로직의 작성이 가능해짐
     * 
     * 코루틴은 yield return 무조건 하나 있어야함
     */
    IEnumerator Swing()
    {
        // 0.1초 쉬고 공격 범위와 이펙트를 킴
        yield return new WaitForSeconds(0.1f); // 1프레임 대기
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        // 0.3초 쉬고 공격 범위 삭제
        yield return new WaitForSeconds(0.3f); // 1프레임 대기
        meleeArea.enabled = false;

        // 0.3초 쉬고 이펙트 삭제
        yield return new WaitForSeconds(0.5f); // 1프레임 대기
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        // 1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation); // 총알 생성, Initiate는 오브젝트 만드는 함수
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();    // 총알도 물체이므로 리지드바디
        bulletRigid.velocity = bulletPos.forward * 50;  // 50의 속도로 총알이 앞으로 나감

        yield return null;

        // 2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation); // 총알 생성, Initiate는 오브젝트 만드는 함수
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();    // 총알도 물체이므로 리지드바디
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse); // 케이스에 힘을 가함, 즉발적으로 나와야하므로 ForceMode.Impulse
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);    // 오브젝트가 돌면서 나감
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;

    Vector3 lookVec;
    Vector3 tauntVec;   // 보스가 어디에 점프해서 내려찍을건지
    

    void Awake()    // Awake는 자식 스크립트만 단독 시행, 즉 상속 받은 부모의 Awake는 실행이 안됨
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();    // 메테리얼 초기화하는 방법, 주의
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;

        StartCoroutine(Think());
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;   // 보스가 스스로 도는 현상 막음
    }

    void FixedUpdate()
    {
        FreezeRotation();
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();    // 죽으면 모든 코루틴을 정지하고 끝
            return;
        }

        if (isLook) // 플레이어를 바라보고 있다면
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;    // 예측 범위
            transform.LookAt(target.position + lookVec);    // 예측 범위를 보스가 대략적으로 쳐다봄, target은 Enemy에 있기 때문에 별도의 선언이 필요 없음 
        }
        else
        {
            nav.SetDestination(tauntVec);   //
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f); // 보스가 패턴을 생각하는 시간

        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;

            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;

            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);  // 0.2초 후 미사일 하나 발사
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.8f);  // 0.3초 후 미사일 하나 발사
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        isLook = false; // 피하기 쉽도록 look은 잠시 끔
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);    // 생성만 하면 나머지는 돌이 알아서 함
        yield return new WaitForSeconds(3f);
        isLook = true;  // 돌 굴렸으니까 다시 켜줘야 함

        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;   // 플레이어가 있는 방향으로 점프

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;    // 점프하는 동안은 플레이어를 밀면 안되므로 꺼둠
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true; // 원상복귀


        StartCoroutine(Think());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameManager manager;
    public enum Type { A, B, C, D}; // D는 보스
    public Type enemyType;

    public bool isDead;
    public int maxHealth;   // 최대 체력
    public int curHealth;   // 현재 체력
    
    public BoxCollider meleeArea; // 적의 근접공격
    public bool isAttack;         // 지금 공격을 하는 중인가?
    
    public Transform target;    // Enemy가 쫒아갈 대상, 플레이어
    public NavMeshAgent nav;           // 유니티 엔진에서 제공하는 네비게이션, UnityEngine.AI

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;

    public GameObject bullet;   // C타입 몬스터의 공격

    public Animator anim;

    public bool isChase;

    public int score;
    public GameObject[] coin;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();    // 메테리얼 초기화하는 방법, 주의
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)     // 보스가 아닐때만 chaseStart 실행
            Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if(nav.enabled && enemyType != Type.D) //네비게이션이 활성화 되어 있을때만
        {
            nav.SetDestination(target.position);    // 타겟(플레이어)를 따라감
            nav.isStopped = !isChase;               // 12장 8분
        }
    }

    void FreezeRotation() // 캐릭터가 오브젝트에 부딫혔을 때 자동으로 회전하는 것을 방지
    {
        if (isChase) { 
            rigid.angularVelocity = Vector3.zero;  // 회전 값을 0으로 만들기 때문에 돌지 않음
            rigid.velocity = Vector3.zero;         // 속도를 0으로 조절 
        }
    }

    void Targeting()
    {
        if(!isDead && enemyType != Type.D)
        {
            float targetRadius = 1.5f;
            float targetRange = 3f;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 6f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;

            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));    // 근처에 플레이어가 있는지 감지

            if (rayHits.Length > 0 && !isAttack)  // 플레이어가 감지되었음을 의미, 플레이어를 공격중이 아닌 경우
            {
                StartCoroutine(Attack());
            }
        }        
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true); // 공격 애니메이션을 활성화


        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);  // 애니메이션에 딜레이가 있으므로 그에 맞춰 딜레이를 넣음
                meleeArea.enabled = true;   // 공격 범위 활성화

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;   // 공격 범위 비활성화

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.2f);  // 애니메이션에 딜레이가 있으므로 그에 맞춰 딜레이를 넣음
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;   // 공격 범위 활성화

                yield return new WaitForSeconds(1f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;   // 공격 범위 비활성화

                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);

                break;
        }

        

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false); // 공격 애니메이션을 활성화


    }

    // 플레이어랑 부딫히면 이상한 움직임(물리 충돌에 의해)을 방지하기 위한 움직임 방지 함수
    void FixedUpdate()
    {
        Targeting();
        FreezeRotation();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.Damage;

            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;

            Destroy(other.gameObject);// 총알이 적과 충돌이 생겼다면 총알 삭제

            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    /*
     * 수류탄 맞았을 때 리액트 함수
     */
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    /*
     * 적이 피격당했음을 인지할 수 있도록 이펙트 지정
     */
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);  // 피격 시 0.1초동안 빨갛게 변함

        if(curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;

            gameObject.layer = 12;  // Enemy 상태였던 Enemy의 레이어를 EnemyDead인 12번으로 교체

            isDead = true; // 죽음
            isChase = false;
            nav.enabled = false;    // AI를 꺼주어야 죽었을 때 위로 튕기는 모션이 실행됨

            anim.SetTrigger("doDie");   // 죽는 모션

            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0, 3);
            Instantiate(coin[ranCoin], transform.position, Quaternion.identity);    // 랜덤으로 코인드랍

            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
            }

            /*
             * 죽는 순간 적이 넉백이 됨
             */
            if (isGrenade)  // 수류탄 사망 시
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false;   //돌면서 사망
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else // 일반 무기 피격 사망
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            
            Destroy(gameObject, 2); // 회색으로 2초 변하고 그 후 사라짐
        }
    }
}

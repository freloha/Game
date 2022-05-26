using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ammo; // 총알 변수
    public int coin; // 재화
    public int health; // 체력 아이템
    public int hasGrenades; // 수류탄

    public Camera followCamera; // 메인 카메라 가지고 옴

    public int maxammo; // 총알 변수
    public int maxcoin; // 재화
    public int maxhealth; // 체력 아이템
    public int maxhasGrenades; // 수류탄

    public GameObject[] grenades; // 플레이어 주변에서 공전할 수류탄


    [SerializeField]
    float speed;
    float fireDelay;

    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;    // 캐릭터의 수평 움직임
    float vAxis;    // 캐릭터의 수직 움직임
    bool wDown;     // 캐릭터가 walk를 하기 위한 조건 변수
    bool jDown;     // 캐릭터가 jump를 하기 위한 조건 변수
    bool fDown;     // 공격키 조건 변수
    bool rDown;     // 재장전
    bool sDown1;    // 무기 1번 스왑 창
    bool sDown2;    // 무기 2번 스왑 창
    bool sDown3;    // 무기 2번 스왑 창
    bool iDown;     // 상호 작용에 관련된 조건 변수

    bool isReload;  // 현재 상태가 장전 중인지?
    bool isJump;    // 현재 상태가 jump 인지
    bool isDodge;   // 현재 상태가 dodge 인지
    bool isSwap;    // 지금 무기 스왑중인지?
    bool isFireReady = true;   // 공격할 수 있는가?

    Vector3 moveVec;    // 캐릭터 움직임에 관한 3차원 벡터
    Vector3 dodgeVec;   // 회피에 관련된 3차원 벡터

    Animator anime;     // 에니메이터 호출 변수
    Rigidbody rb;       // 리지드바디(물리적현상)에 대한 변수

    GameObject nearObject;  // 오브젝트를 받아오기 위한 변수, 무기, 재화 등등
    Weapon equipWeapon; // 장착중인 무기는 어떤거인가

    int equipWeaponIndex = -1;   // 현재 들고있는 무기의 인덱스, Hammer의 Index가 0이기 때문에 -1로 해주어야 먹고 바로 교체가 가능해짐

    void Awake()
    {
        anime = GetComponentInChildren<Animator>(); // 자식 컴포넌트이기 때문에 GetComponent는 동작 X
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload();
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");            // walk는 shift 키
        jDown = Input.GetButtonDown("Jump");        // jump는 스페이스바
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");      // r 버튼 누르면 장전
        iDown = Input.GetButtonDown("Interaction"); // edit - project setting - Input Manager에서 각각 변수에 대한 키를 할당해주어야 함, Interaction은 e 키
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    private void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 대각선의 이동에서도 동일한 보정값을 가지게 하기 위함

        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        if (isSwap || !isFireReady || isReload) // 플레이어가 움직이지 못하는 상태 조건
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; // 각 컴퓨터마다 프레임이 다르기 때문에 이를 보정, wDown 이 true면 0.3, 아니면 1

        anime.SetBool("isRun", moveVec != Vector3.zero);
        anime.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); // 나아가는 방향으로 캐릭터가 바라봄

        /*
         * Ray 설명 : https://kukuta.tistory.com/391
         * Ray는 직선의 시작점(Origin)과 방향(Direction)을 가지고 있는 단순한 구조체이다.
         * Raycast는 Ray가 다른 씬의 다른 객체와 충돌하는지 여부를 판별
         */
        // 마우스에 의한 회전
        if (fDown) // 마우스 클릭을 했을 때
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // 카메라 뷰의 모든 선은 Ray 오브젝트로 제공, 스크린에서 마우스 포인터의 위치를 카메라의 레이로 지정함
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : return처럼 반환 값을 주어진 변수에 저장하는 키워드
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }        
    }

    void Jump()
    {
        if (jDown && !isJump && moveVec == Vector3.zero && !isDodge && !isSwap)
        {
            rb.AddForce(Vector3.up * 15, ForceMode.Impulse); // ForceMode.Impulse 는 즉발적인 힘
            anime.SetBool("isJump", true);
            anime.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime; // 공격 딜레이를 더해주고 공격 가능한지 확인
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap) // 공격 키 누르고 공격 가능하면서 회피중이 아니면서 스왑하지 않는중이면
        {
            equipWeapon.Use(); // player는 공격 했다는 조건만 걸어주고 실 행동은 weapon에서 동작
            anime.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); // 삼항연산자
            fireDelay = 0;
        }
    }

    /*
     * 장전 조건 : 손에 무기가 있고, 원거리 무기이며, 총알이 없는 경우
     */
    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady) // 장전키 누르면서 점프는 X, 회피도 X, 스왑도 X, 총 쏠 준비는 되어있음
        {
            anime.SetTrigger("doReload"); // 애니메이션에서 장전 실행
            isReload = true;

            Invoke("ReloadOut", 3f); // 장전하는동안 장전 외 모든 움직임 중지
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo; // 플레이어가 가진 탄창이 최대 탄창보다 적으면, 가진 탄창을 장전하고, 많은 경우 최대 탄창 장전
        equipWeapon.curAmmo = reAmmo;       // 현재 탄창에 reAmmo 값을 장전해줌
        ammo -= reAmmo; // 장전한 만큼 가지고 있는 값을 빼줌
        isReload = false;
    }

    void Dodge()
    {
        if (jDown && !isJump && moveVec != Vector3.zero && !isJump && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anime.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f); // 시간차 함수 호출
        }
    }

    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }

    void SwapOut()
    {
        isSwap = false;
    }

    private void OnCollisionEnter(Collision collision) // 충돌이 일어난 경우
    {
        if(collision.gameObject.tag == "Floor")
        {
            anime.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) // 오브젝트와 충돌에 대한 체크, 닿았을 때
    {        
        if (other.tag == "Item")
        {
            Debug.Log(hasGrenades);
            item item = other.GetComponent<item>();
            switch (item.type)
            {
                case item.Type.Ammo:
                    ammo += item.value;
                    if(ammo > maxammo)
                        ammo = maxammo;
                    break;
                case item.Type.Coin:
                    coin += item.value;
                    if (coin > maxcoin)
                        coin = maxcoin;
                    break;
                case item.Type.Heart:
                    ammo += item.value;
                    if (health > maxhealth)
                        health = maxhealth;
                     break;
                case item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if(hasGrenades > maxhasGrenades)
                        hasGrenades = maxhasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) // 오브젝트와 계속 닿고 있는 중
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
            Debug.Log(nearObject.name);
        }
        
    }

    void OnTriggerExit(Collider other) // 오브젝트와 닿았다가 떨어졌을 때
    {
        if(other.tag == "Weapon")
        {
            nearObject = null;
        }
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1)
        {
            weaponIndex = 0;
        }
        if (sDown2) 
        { 
            weaponIndex = 1;
        }
        if (sDown3)
        {
            weaponIndex = 2;
        }

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if (equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false); // 지금 끼고 있는 무기를 먼저 비활성화
            }

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); // object.SetActive(true) : 비활성화된 오브젝트를 활성화시킴(눈에 보이게 함) 

            anime.SetTrigger("doSwap"); // 트리거가 발생했을 때, 해당 동작 진행

            isSwap = true;

            Invoke("SwapOut", 0.4f); // 시간차 함수 호출
        }
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isJump && !isDodge) // 점프나 회피상태에서는 상호 작용 X, 현재 오브젝트 장착 X
        {
            if(nearObject.tag == "Weapon")
            {
                item item = nearObject.GetComponent<item>();
                int weaponIndex = item.value; // item 스크립트에서 무기 별 열거형 value를 가져옴
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }
}

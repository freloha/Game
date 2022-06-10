using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager manager;

    public GameObject[] weapons;
    public bool[] hasWeapons;

    public GameObject[] grenades;
    public GameObject grenadeObj;

    public Camera followCamera;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    public int score;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    [SerializeField]
    float speed;    // 캐릭터의 이동 속도
    float hAxis;    // 캐릭터 수평 위치 값
    float vAxis;    // 캐릭터 수직 위치 값

    bool isDead;

    bool wDown;     // 캐릭터가 걷는 것
    bool jDown;     // 캐릭터가 점프
    bool iDown;     // 근처 오브젝트와 상호작용 중?
    bool sDown1;    // 1번 장비 스왑
    bool sDown2;    // 2번 장비 스왑
    bool sDown3;    // 3번 장비 스왑
    bool fDown;     // 공격키
    bool gDown;     // 폭탄 던지는 것
    bool rDown;     // 장전키

    bool isJump;    // 점프 중인지
    bool isDodge;   // 회피 중인지
    bool isSwap;    // 스왑 중인지
    bool isReload;  // 장전 중인지
    bool isFireReady = true;   // 공격 가능?
    bool isBorder;  // 벽에 닿았는가?
    bool isDamage;  // 데미지 입었는가?
    public bool isShop;    // 쇼핑중인가?

    Vector3 moveVec;    // 캐릭터의 이동 관련
    Vector3 dodgeVec;   // 회피 시 백터값

    Rigidbody rigid;

    GameObject nearObject;  // 근처에 있는 오브젝트, Trigger위해
    public Weapon equipWeapon; // 장착중인 무기

    MeshRenderer[] meshs;   // 캐릭터의 전신 색 변환을 위한 변수

    int equipWeaponIndex = -1;

    float fireDelay;    // 공격 딜레이

    Animator anim;

    void Awake() // 변수들을 초기화하는 Awake
    {
        rigid = GetComponent<Rigidbody>();          // rigidbody 초기화
        anim = GetComponentInChildren<Animator>();  // Player 아래 애니메이터가 있으므로 Children
        meshs = GetComponentsInChildren<MeshRenderer>();    // 플레이어의 모든 자식 매쉬를 가져옴

        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 588229); // 유니티에서 제공하는 간단한 저장 기능
    }

    void Update()
    {
        GetInput();     // 입력을 받음
        Move();         // 캐릭터가 움직임
        Turn();         // 캐릭터가 움직이는 방향으로 캐릭터 시선 고정
        Jump();         // 캐릭터가 점프
        Dodge();        // 캐릭터가 회피
        Interaction();  // 아이템과 무언가를 할 수 있음
        Swap();         // 무기변경
        Attack();       // 공격
        Reload();       // 재장전
        Grenade();      // 수류탄 던지기
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // edit - InputManager에 있는 Horizontal
        vAxis = Input.GetAxisRaw("Vertical");   // edit - InputManager에 있는 Vertical
        wDown = Input.GetButton("Walk");        // edit - InputManager에 Walk 등록하여 shift누르면 동작
        jDown = Input.GetButtonDown("Jump");        // edit - InputManager에 Jump 등록하여 space bar 누르면 동작
        iDown = Input.GetButtonDown("Interaction"); // edit - InputManager에 Interaction 등록하여 e 키 누르면 동작
        sDown1 = Input.GetButtonDown("Swap1"); // 1번무기 스왑
        sDown2 = Input.GetButtonDown("Swap2"); // 1번무기 스왑
        sDown3 = Input.GetButtonDown("Swap3"); // 1번무기 스왑
        rDown = Input.GetButtonDown("Reload"); // 총알 장전
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 대각선 이동시에도 상하좌우와 동일한 값을 보장

        if (isDodge)
        {
            moveVec = dodgeVec; // 회피 중, 공격 중에는 moveVec의 값을 받지 않고 회피하는 순간의 moveVec 값인 dodgeVec으로 동작
        }

        if (isSwap || !isFireReady || isReload || isDead)
        {
            moveVec = Vector3.zero;
        }

        if (!isBorder)
        {
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; // 3항연산자 사용, wDown이 true이면 0.3f 아니면 1f
        }
               
        anim.SetBool("isRun", moveVec != Vector3.zero); // 캐릭터가 가만히 있는 것이 아니면 isRun은 True가 됨
        anim.SetBool("isWalk", wDown); // wDown이 true이면 isWalk 상태
    }

    void Dodge()
    {
        if (jDown && !isDodge && moveVec != Vector3.zero && !isDodge && !isSwap)   // 점프 중이 아니고 점프 버튼이 눌렸을 때, 점프
        {
            dodgeVec = moveVec; // 회피 중에는 
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);   // 0.4초 대기했다가 DodgeOut 함수 실행            
        }
    }

    void Turn()
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); // 캐릭터의 현재 위치에 무브벡터를 더하고 나아가는 방향으로 캐릭터가 바라봄(회전함)

        // 마우스에 의한 회전(단, 총을 발사하는 경우에)
        if (fDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);   // 마우스는 Input에서 가지고 있음
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))   // 레이가 특정 오브젝트에 닿았다면, out을 통해 해당 정보를 rayHit에 저장, 길이(강도)는 100으로 세게 줌
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }        
    }

    void Grenade()
    {
        if(hasGrenades == 0)    // 폭탄 없으면 못던짐
        {
            return;
        }

        if(gDown && !isReload && !isSwap)   // 폭탄 키 눌렸으면서 재장전 중이 아니고 스왑중이 아닐 때
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);   // 마우스는 Input에서 가지고 있음
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))   // 레이가 특정 오브젝트에 닿았다면, out을 통해 해당 정보를 rayHit에 저장, 길이(강도)는 100으로 세게 줌
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 2;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);    // 폭탄 생성
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();      // 폭탄의 리지드바디 생성
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
            }
        }
    }
    void Jump()
    {
        if (jDown && isJump && moveVec == Vector3.zero && !isDodge && !isSwap && !isDead)   // 점프 중이 아니고 점프 버튼이 눌렸을 때, 점프
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse); // 캐릭터에게 힘을 주기 위해서는 AddForce함수를 사용, ForceMode.Impulse는 즉발적인 힘
            isJump = true;  // 점프중이다
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    

    void OnCollisionEnter(Collision collision)  // 충돌감지, Trigger에 들어갔을 때
    {
        if(collision.gameObject.tag == "Floor") // 땅에 캐릭터가 닿으면
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;        // 특정 프레임 시간 당 공격 딜레이가 차오름
        isFireReady = equipWeapon.rate < fireDelay; // 공격 속도보다 fireDelay가 크면 true

        if(fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead) // 스왑중 아니면서 회피중도 아니면서 공격키는 눌렀고 공격 가능상태이면
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
            Debug.Log(isFireReady);
        }
    }

    void Reload()
    {
        if(equipWeapon == null) // 손에 들린 무기가 없으면 장전 안함
        {
            return;
        }

        if(equipWeapon.type == Weapon.Type.Melee) // 근접무기는 장전 X
        {
            return;
        }

        if(ammo == 0)   // 소지 총알 없음
        {
            return;
        }

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead) // 장전을 하기 위한 조건들
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;   // 현재 가진 탄창이 무기 최대 탄창보다 적으면 현재 플레이어가 가진 탄창 장전해주고 아니면 무기 최대 탄창을 장전
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) // 1번 눌렀는데 해당 무기가 없거나 이미 그 무기를 끼고 있으면 아무런 동작하지 않음
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if(sDown1) weaponIndex = 0;
        if(sDown2) weaponIndex = 1;
        if(sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDead)
        {            
            if(equipWeapon != null)                 // 무기를 아무것도 안차고 있으면
                equipWeapon.gameObject.SetActive(false);       // 낀 무기 안보이게 비활성화

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();     // 착용 무기를 현재 무기로
            equipWeapon.gameObject.SetActive(true);   // 착용 무기 보이게 활성화

            anim.SetTrigger("doSwap");
            isSwap = true;

            Invoke("SwapOut", 1.3f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isJump && !isDead) // 점프중일 땐 먹지 못하고, 해당 키 누른 상태로 근처에 아이템이 있을 때
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                
                Destroy(nearObject);    // 근처에 있던 아이템은 삭제
            }
            else if (nearObject.tag == "Shop")
            {
                Debug.Log("Shop Enter");
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    /*
     * Collider에 isTrigger가 체크되어 있으면 Collider는 더 이상 충돌체가 아닌 트리거로 동작
     * 즉, Trigger로 동작한다는 것은 충돌하지 않고 그냥 통과하게 된다는 것
     * OnTriggerEnter = 통과 시작한 순간, OnTriggerStay = 머무르는 동안, OnTriggerExit = 나가는 순간
     */
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type){
                case Item.Type.Ammo:
                    ammo += item.value;
                    if(ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if(coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if(health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    hasGrenades += item.value;
                    if(hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }

            if (other.GetComponent<Rigidbody>() != null)     // 미사일이 몸에 닿으면 미사일 삭제
                Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow; // 피격 당하면 캐릭터의 모든 매쉬를 노랑으로 변환
        }

        if(isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse); // 보스 점프에 맞으면 넉백

        if (health <= 0 != isDead)
            OnDie();

        yield return new WaitForSeconds(1f);    // 1초 대기

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white; // 피격 끝났으므로 원래 색깔로 변환
        }

        if(isBossAtk)
            rigid.velocity = Vector3.zero;  // 날라간 이후 멈추게 하기

        
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerStay(Collider other) // 통과 시작
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject; // 무기와 접촉했으면 근처 오브젝트 안에 해당 트리거된 오브젝트 집어넣음
            Debug.Log(nearObject.name);
        }
    }

    void OnTriggerExit(Collider other) // 머무르는 동안
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = null; // 떼어졌으니 근처 오브젝트는 없는 것
        }
        else if(other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            isShop = false;
            shop.Exit();    // 퇴장 함수 호출
            nearObject = null;
            
        }
    }

    void FreezeRotation() // 캐릭터가 오브젝트에 부딫혔을 때 자동으로 회전하는 것을 방지
    {
        rigid.angularVelocity = Vector3.zero; // 회전 값을 0으로 만들기 때문에 돌지 않음
    }

    void StopToWall()   // 벽 뚫고 나가는 것을 방지
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));    // Wall이라는 레이어 마스크랑 닿으면 true로 바뀜, 이를 Move에 제한사항으로 걸음
    }

    /*
     * FixedUpdate = 프레임을 기반으로 호출되는 업데이트와 다르게 fixed timestep에 설정된 값에 따라 일정 간격으로 호출
     * 물리 효과가 적용된 rigidbody 오브젝트 조정할 때 사용
     * update는 불규칙한 호출임으로 물리엔진 충돌검사가 제대로 안될 수 있음
     */
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
}

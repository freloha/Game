using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ammo; // �Ѿ� ����
    public int coin; // ��ȭ
    public int health; // ü�� ������
    public int hasGrenades; // ����ź

    public Camera followCamera; // ���� ī�޶� ������ ��

    public int maxammo; // �Ѿ� ����
    public int maxcoin; // ��ȭ
    public int maxhealth; // ü�� ������
    public int maxhasGrenades; // ����ź

    public GameObject[] grenades; // �÷��̾� �ֺ����� ������ ����ź


    [SerializeField]
    float speed;
    float fireDelay;

    public GameObject[] weapons;
    public bool[] hasWeapons;

    float hAxis;    // ĳ������ ���� ������
    float vAxis;    // ĳ������ ���� ������
    bool wDown;     // ĳ���Ͱ� walk�� �ϱ� ���� ���� ����
    bool jDown;     // ĳ���Ͱ� jump�� �ϱ� ���� ���� ����
    bool fDown;     // ����Ű ���� ����
    bool rDown;     // ������
    bool sDown1;    // ���� 1�� ���� â
    bool sDown2;    // ���� 2�� ���� â
    bool sDown3;    // ���� 2�� ���� â
    bool iDown;     // ��ȣ �ۿ뿡 ���õ� ���� ����

    bool isReload;  // ���� ���°� ���� ������?
    bool isJump;    // ���� ���°� jump ����
    bool isDodge;   // ���� ���°� dodge ����
    bool isSwap;    // ���� ���� ����������?
    bool isFireReady = true;   // ������ �� �ִ°�?

    Vector3 moveVec;    // ĳ���� �����ӿ� ���� 3���� ����
    Vector3 dodgeVec;   // ȸ�ǿ� ���õ� 3���� ����

    Animator anime;     // ���ϸ����� ȣ�� ����
    Rigidbody rb;       // ������ٵ�(����������)�� ���� ����

    GameObject nearObject;  // ������Ʈ�� �޾ƿ��� ���� ����, ����, ��ȭ ���
    Weapon equipWeapon; // �������� ����� ����ΰ�

    int equipWeaponIndex = -1;   // ���� ����ִ� ������ �ε���, Hammer�� Index�� 0�̱� ������ -1�� ���־�� �԰� �ٷ� ��ü�� ��������

    void Awake()
    {
        anime = GetComponentInChildren<Animator>(); // �ڽ� ������Ʈ�̱� ������ GetComponent�� ���� X
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
        wDown = Input.GetButton("Walk");            // walk�� shift Ű
        jDown = Input.GetButtonDown("Jump");        // jump�� �����̽���
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");      // r ��ư ������ ����
        iDown = Input.GetButtonDown("Interaction"); // edit - project setting - Input Manager���� ���� ������ ���� Ű�� �Ҵ����־�� ��, Interaction�� e Ű
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    private void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // �밢���� �̵������� ������ �������� ������ �ϱ� ����

        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        if (isSwap || !isFireReady || isReload) // �÷��̾ �������� ���ϴ� ���� ����
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime; // �� ��ǻ�͸��� �������� �ٸ��� ������ �̸� ����, wDown �� true�� 0.3, �ƴϸ� 1

        anime.SetBool("isRun", moveVec != Vector3.zero);
        anime.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); // ���ư��� �������� ĳ���Ͱ� �ٶ�

        /*
         * Ray ���� : https://kukuta.tistory.com/391
         * Ray�� ������ ������(Origin)�� ����(Direction)�� ������ �ִ� �ܼ��� ����ü�̴�.
         * Raycast�� Ray�� �ٸ� ���� �ٸ� ��ü�� �浹�ϴ��� ���θ� �Ǻ�
         */
        // ���콺�� ���� ȸ��
        if (fDown) // ���콺 Ŭ���� ���� ��
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ī�޶� ���� ��� ���� Ray ������Ʈ�� ����, ��ũ������ ���콺 �������� ��ġ�� ī�޶��� ���̷� ������
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : returnó�� ��ȯ ���� �־��� ������ �����ϴ� Ű����
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
            rb.AddForce(Vector3.up * 15, ForceMode.Impulse); // ForceMode.Impulse �� ������� ��
            anime.SetBool("isJump", true);
            anime.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime; // ���� �����̸� �����ְ� ���� �������� Ȯ��
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap) // ���� Ű ������ ���� �����ϸ鼭 ȸ������ �ƴϸ鼭 �������� �ʴ����̸�
        {
            equipWeapon.Use(); // player�� ���� �ߴٴ� ���Ǹ� �ɾ��ְ� �� �ൿ�� weapon���� ����
            anime.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot"); // ���׿�����
            fireDelay = 0;
        }
    }

    /*
     * ���� ���� : �տ� ���Ⱑ �ְ�, ���Ÿ� �����̸�, �Ѿ��� ���� ���
     */
    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if(rDown && !isJump && !isDodge && !isSwap && isFireReady) // ����Ű �����鼭 ������ X, ȸ�ǵ� X, ���ҵ� X, �� �� �غ�� �Ǿ�����
        {
            anime.SetTrigger("doReload"); // �ִϸ��̼ǿ��� ���� ����
            isReload = true;

            Invoke("ReloadOut", 3f); // �����ϴµ��� ���� �� ��� ������ ����
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo; // �÷��̾ ���� źâ�� �ִ� źâ���� ������, ���� źâ�� �����ϰ�, ���� ��� �ִ� źâ ����
        equipWeapon.curAmmo = reAmmo;       // ���� źâ�� reAmmo ���� ��������
        ammo -= reAmmo; // ������ ��ŭ ������ �ִ� ���� ����
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

            Invoke("DodgeOut", 0.4f); // �ð��� �Լ� ȣ��
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

    private void OnCollisionEnter(Collision collision) // �浹�� �Ͼ ���
    {
        if(collision.gameObject.tag == "Floor")
        {
            anime.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other) // ������Ʈ�� �浹�� ���� üũ, ����� ��
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

    void OnTriggerStay(Collider other) // ������Ʈ�� ��� ��� �ִ� ��
    {
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;
            Debug.Log(nearObject.name);
        }
        
    }

    void OnTriggerExit(Collider other) // ������Ʈ�� ��Ҵٰ� �������� ��
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
                equipWeapon.gameObject.SetActive(false); // ���� ���� �ִ� ���⸦ ���� ��Ȱ��ȭ
            }

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true); // object.SetActive(true) : ��Ȱ��ȭ�� ������Ʈ�� Ȱ��ȭ��Ŵ(���� ���̰� ��) 

            anime.SetTrigger("doSwap"); // Ʈ���Ű� �߻����� ��, �ش� ���� ����

            isSwap = true;

            Invoke("SwapOut", 0.4f); // �ð��� �Լ� ȣ��
        }
    }

    void Interaction()
    {
        if(iDown && nearObject != null && !isJump && !isDodge) // ������ ȸ�ǻ��¿����� ��ȣ �ۿ� X, ���� ������Ʈ ���� X
        {
            if(nearObject.tag == "Weapon")
            {
                item item = nearObject.GetComponent<item>();
                int weaponIndex = item.value; // item ��ũ��Ʈ���� ���� �� ������ value�� ������
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }
}

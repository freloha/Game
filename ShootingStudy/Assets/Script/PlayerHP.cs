using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    public float maxHP = 10; // �ִ� ü��
    public float currentHP; // ���� ü��
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private void Awake()
    {
        currentHP = maxHP; // ���� ü���� �ִ� ü�°� ���� ����
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage; // ü�� ����

        StopCoroutine("HitColorAnimation");
        StartCoroutine("HitColorAnimation");

        // ü���� 0 ���� = �÷��̾� ĳ���� ���
        if(currentHP <= 0)
        {
            Debug.Log("Player HP : 0.. Die");

            //�÷��̾ �׾�����
            playerController.OnDie();
        }
    }

    private IEnumerator HitColorAnimation()
    {
        // �÷��̾��� ������ ����������
        spriteRenderer.color = Color.red;
        //0.1�� ���� ���
        yield return new WaitForSeconds(0.1f);
        // �÷��̾��� ������ ���� ������ �Ͼ������
        // ���� ������ �Ͼ���� �ƴ� ��� ���� ���� ���� ����
        spriteRenderer.color = Color.white;
        
    }
}

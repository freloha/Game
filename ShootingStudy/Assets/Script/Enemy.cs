using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private int scorePoint = 100; // �� óġ�� ȹ�� ����
    private PlayerController playerController;

    private void Awake()
    {
        // ���� �ڵ忡���� �ѹ��� ȣ���ϱ� ������ OnDie()���� �ٷ� ȣ���ص� ������
        // ������Ʈ ������ �̿��� ������Ʈ�� ������ ��쿡�� ���� �ѹ��� Find�� �̿���
        // PlayerController�� ������ �����صΰ� ����ϴ� ���� ���꿡 ȿ�����̴�.
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // ������ �ε��� ������Ʈ�� �±װ� "Player" �̸�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // �� ���ݷ¸�ŭ �÷��̾� ü�� ����
            collision.GetComponent<PlayerHP>().TakeDamage(damage);

            // �� ���
            OnDie();
        }
    }

    public void OnDie() // ���� �׾��� �� ȣ��Ǵ� �Լ�
    {
        //�÷��̾��� ������ scorePoint��ŭ ������Ų��
        playerController.Score += scorePoint;
        // �� ������Ʈ ����
        Destroy(gameObject);
    }
}

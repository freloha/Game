using UnityEngine;

public class Attack : MonoBehaviour
{
    /*
     * OnTriggerEnter2D : ������Ʈ�� �浹�� �Ͼ ��, ó�� �ѹ��� ȣ��Ǵ� �Լ�
     * OnTriggerStay2D : ������Ʈ�� �浹�� �Ͼ�� ���� ���������� ȣ��Ǵ� �Լ�
     * OnTriggerExit2D : ������Ʈ�� �浹���� ��� ��, �ѹ� ȣ��Ǵ� �Լ�
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // �΋H�� ������Ʈ�� Tag�� Enemy���
        {
            // �΋H�� ������Ʈ ���ó�� (��)
            collision.GetComponent<Enemy>().OnDie();
            // �� ������Ʈ ���� (�߻�ü)
            Destroy(gameObject);
        }
    }
}
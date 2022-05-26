using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range}; // Melee : �΋H����, ���������� �ǹ�, Range : ���Ÿ� ����
    public Type type;
    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;

    public BoxCollider meleeArea;       // �������� ����
    public TrailRenderer trailEffect;   // �ܻ��� �׷��ִ� ������Ʈ

    public Transform bulletPos;         // �Ѿ� ��ġ
    public GameObject bullet;           // �Ѿ� �������� ������ ������ ���
    public Transform bulletCasePos;     // �Ѿ� ���̽� ��ġ
    public GameObject bulletCase;       // �Ѿ� ���̽� �������� ������ ������ ���

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--; // �Ѿ� �ϳ��� ����
            StartCoroutine("Shot"); // �ڷ�ƾ���� �� �߻�
        }
    }

    IEnumerator Swing() // �ֵθ��� Collider�� TrailRenderer�� Ű�� ���� ��, �ڷ�ƾ�� yield ������ �ϳ� �̻� �־�� ��
    {
        yield return new WaitForSeconds(0.1f); // 0.1 �� ���, 1������ ���� ������ yield return null, ��� ���߰� ������ yield break;
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        trailEffect.enabled = false;
        
    }

    // Use() ���η�ƾ -> Swing() ���� ��ƾ -> Use() ���η�ƾ
    // �ڷ�ƾ ��� �� --> Use() ���η�ƾ + Swing() �ڷ�ƾ (���� ����, Co-Op)

    IEnumerator Shot()
    {
        // �Ѿ� �߻�
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        // ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody bulletCaseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // �������� ��ũ�� �߰�, �������� ��ũ
    }
}

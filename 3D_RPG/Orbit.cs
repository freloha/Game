using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offSet;

    // Start is called before the first frame update
    void Start()
    {
        offSet = transform.position - target.position; // ����ź�� �÷��̾�� �Ÿ����� ����ؼ� offSet�� ����
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offSet; // ����ź�� ������ ����
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime); // 1��° �Ű����� : Ÿ���� ��ġ, 2��° �Ű����� : ȸ�� ��, 3��°�� ȸ�� �ӵ�
        offSet = transform.position - target.position; // �ֱ������� �Ÿ� �� ������Ʈ
    }
}

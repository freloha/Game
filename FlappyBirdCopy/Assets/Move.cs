using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    void Start()
    {
        //Transform �� �⺻������ ���Ե� ������Ʈ�̹Ƿ� GetComponent�� �Ƚᵵ �ȴ�.
    }

    // Update is called once per frame
    void Update()
    {
        // time.deltaTime = ���� �������� �Ϸ����� �ɸ� �ð�
        transform.position += Vector3.left * speed * Time.deltaTime; //(-1,0,0) �� �����Ӹ��� ������, �� �������� �ϵ��� ���ɿ� ���� �������� �ٸ��� �����ϹǷ� �����������
        //Debug.Log(transform.position);        
    }
}

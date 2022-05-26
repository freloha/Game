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
        offSet = transform.position - target.position; // 수류탄과 플레이어간의 거리차를 계산해서 offSet에 저장
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offSet; // 수류탄의 포지션 변경
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime); // 1번째 매개변수 : 타겟의 위치, 2번째 매개변수 : 회전 축, 3번째는 회전 속도
        offSet = transform.position - target.position; // 주기적으로 거리 차 업데이트
    }
}

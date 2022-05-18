using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseMove : MonoBehaviour
{
    Transform tr; // Transform은 오브젝트의 움직임에 관련된 클래스

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tr = GetComponent<Transform>();

        if (Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Camera의 메인 부분에 마우스 포인터 값을 잢아서 Vector2 값으로 받아옴
            tr.position = Vector2.MoveTowards(tr.position, mousePosition, Time.deltaTime * 5f); // 첫번째 인자는 현재 위치, 두 번째 인자는 움직일 목표 위치, 세 번째는 속도
            // 해당 값을 Transform을 통하여 움직임에 적용
            // Time.deltaTime : 이전 프레임에서 현재 프레임으로 넘어올 때 걸린 시간을 측정한 것, 컴퓨터마다 성능이 다르니 컴퓨터 환경에 상관없이 프레임 동기화를 위한 값

            //게임 화면에서 마우스 왼쪽 버튼을 쭉 누르면 해당 방향으로 오브젝트가 이동해옴
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Transform tr;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tr = GetComponent<Transform>();

        float x = Input.GetAxis("Horizontal") * 0.01f; // 가로축에 대한 값을 가져오며 키보드 이동 방향키에 적용, 움직이는 단위는 1씩
        float y = Input.GetAxis("Vertical") * 0.01f; // 세로축에 대한 값을 가져오며 키보드 이동 방향키에 적용, 움직이는 단위는 1씩

        tr.Translate(new Vector2(x,y));

        /*
        if (Input.GetKey(KeyCode.RightArrow))
        {
            tr.position = new Vector2(tr.position.x + 0.01f, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            tr.position = new Vector2(tr.position.x - 0.01f, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            tr.position = new Vector2(tr.position.y + 0.01f, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            tr.position = new Vector2(tr.position.y - 0.01f, 0);
        }
        */
    }
}

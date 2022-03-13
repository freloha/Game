using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    void Start()
    {
        //Transform 은 기본적으로 포함된 컴포넌트이므로 GetComponent를 안써도 된다.
    }

    // Update is called once per frame
    void Update()
    {
        // time.deltaTime = 지난 프레임이 완료기까지 걸린 시간
        transform.position += Vector3.left * speed * Time.deltaTime; //(-1,0,0) 한 프레임마다 움직임, 단 프레임은 하드의 성능에 따라 프레임이 다르게 동작하므로 통일해줘야함
        //Debug.Log(transform.position);        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePipe : MonoBehaviour
{
    public GameObject pipe;
    float timer = 0;
    public float timeDiff;
    float dummy = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeDiff) // 매 timeDiff초마다 파이프 생성
        {
            GameObject newpipe = Instantiate(pipe); // 파이프를 생성
            newpipe.transform.position = new Vector3(BirdJump.location.x + Random.Range(2f, 3.7f), Random.Range(-1.7f, 3.7f), 0); // 랜덤출력
            timer = 0;
            Destroy(newpipe, 10.0f); // 5초 후에 사라짐
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BirdJump : MonoBehaviour
{
    Rigidbody2D rb;
    public static Vector2 location;
    public float jumpPower;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start 입니다.");
        //jumpPower = 3;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        location = this.gameObject.transform.position;
        Debug.Log("Update 입니다.");
        if(Input.GetMouseButtonDown(0))
        {
            rb.velocity = Vector2.up * jumpPower;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision) // Collider들끼리 부딫혔을 때 실행
    {
        if(Score.score > Score.bestScore)
        {
            Score.bestScore = Score.score;
        }
        SceneManager.LoadScene("GameOverScene");
    }
}

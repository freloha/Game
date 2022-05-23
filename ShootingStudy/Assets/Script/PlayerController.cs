using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;

    [SerializeField]
    private StageData stageData;
    private Movement2D movement2D;

    [SerializeField]
    private KeyCode keyCodeAttack = KeyCode.Space;
    private Weapon weapon;

    private int score;
    public int Score
    {
        set => score = Mathf.Max(0,value); // 스코어 값이 음수로 설정되지 않도록
        get => score;
    }

    private void Awake()
    {
        movement2D = GetComponent<Movement2D>();
        weapon = GetComponent<Weapon>();
    }

    void Update()
    {
        // 방향 키를 눌러 이동 방향 설정
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        movement2D.MoveTo(new Vector3(x,y,0));

        // 공격 키를 Down/Up으로 공격 시작/종료
        if (Input.GetKeyDown(keyCodeAttack))
        {
            weapon.StartFiring();
        }
        else if (Input.GetKeyUp(keyCodeAttack))
        {
            weapon.StopFiring();
        }
    }

    public void OnDie()
    {
        // 디바이스에 획득한 점수를 저장
        PlayerPrefs.SetInt("Score", score);

        //플레이어 사망 시 nextSceneName 씬으로 이동
        SceneManager.LoadScene(nextSceneName);
    }

    private void LateUpdate()
    {
        // 플레이어 캐릭터가 화면 범위 바깥으로 나가지 못하도록 함
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, stageData.LimitMin.x, stageData.LimitMax.x),
            Mathf.Clamp(transform.position.y, stageData.LimitMin.y, stageData.LimitMax.y));
    }
}

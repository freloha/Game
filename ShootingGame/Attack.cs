using UnityEngine;

public class Attack : MonoBehaviour
{
    /*
     * OnTriggerEnter2D : 오브젝트간 충돌이 일어날 때, 처음 한번만 호출되는 함수
     * OnTriggerStay2D : 오브젝트간 충돌이 일어나는 동안 지속적으로 호출되는 함수
     * OnTriggerExit2D : 오브젝트간 충돌에서 벗어날 때, 한번 호출되는 함수
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // 부딫힌 오브젝트의 Tag가 Enemy라면
        {
            // 부딫힌 오브젝트 사망처리 (적)
            collision.GetComponent<Enemy>().OnDie();
            // 내 오브젝트 삭제 (발사체)
            Destroy(gameObject);
        }
    }
}
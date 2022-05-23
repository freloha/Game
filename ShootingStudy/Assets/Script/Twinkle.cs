using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twinkle : MonoBehaviour
{
    private float fadeTime = 0.1f;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine("TwinkleLoop");
    }

    private IEnumerator TwinkleLoop()
    {
        while(true)
        {
            // Alpha 값을 1에서 0으로 : Fade Out
            yield return StartCoroutine(FadeEffect(1, 0));
            // Alpha 값을 0에서 1로 : Fade In
            yield return StartCoroutine(FadeEffect(0, 1));
        }
    }

    private IEnumerator FadeEffect(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while(percent < 1)
        {
            //fadeTime 시간 동안 while() 반복문 실행
            currentTime += Time.deltaTime;
            percent = currentTime / fadeTime;

            // 유니티 클래스에 설정되어 있는 spriteRenderer.color, transform.position은 프로퍼티로
            // spriteRenderer.color.a = 1.0f 과 같이 설정이 불가능하다
            // spriteRenderer.color = new Color(spriteRenderer.color., .., .., 1.0f); 과 같이 설정해야 한다.
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(start, end, percent);
            // Mathf.Lerf(start, end, percent); == start와 end 사이의 값 중 percent 위치에 있는 값을 반환, start가 0이고 end가 100일 때, percent가 0.3이면 30을 반환
            spriteRenderer.color = color;

            yield return null; // yield return은 컬렉션 데이터를 하나씩 리턴하는데 사용
            /*
             * yield return 1;
             * yield return 2;
             * 라고 되어있으면 첫 번째 호출때는 1이 return되고 두 번째 호출때는 2가 return 된다.
             */

        }
    }
}

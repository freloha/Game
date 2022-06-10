using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;// 아이템
    public int[] itemPrice;     // 아이템 가격
    public Transform[] itemPos; // 아이템 위치

    public string[] talkData;
    public Text talkText;       // 대사를 바꾸기 위함


    Player enterPlayer; // 상점에 입장한 플레이어

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        anim.SetTrigger("doHello"); // 인사를 해서 캐릭터가 나가는 것을 함
        uiGroup.anchoredPosition = Vector3.down * 1000; // UI가 원래 있던 곳으로 돌아감
        enterPlayer.isShop = false;
    }

    // 구매 버튼을 누르면 아이템이 구매가되고 스폰지역에 해당 아이템이 나옴
    public void Buy(int index)
    {
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());  // 다중 클릭하면 꼬일 수 있으므로 한번 끔
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
            + Vector3.forward * Random.Range(-3, 3);

        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}

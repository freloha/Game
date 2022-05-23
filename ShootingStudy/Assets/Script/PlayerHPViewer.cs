using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPViewer : MonoBehaviour
{
    [SerializeField]
    private PlayerHP playerHP;
    private Slider sliderHP;

    private void Awake()
    {
        sliderHP = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Slider UI에 현재 체력 정보를 업데이트
        sliderHP.value = playerHP.currentHP / playerHP.maxHP;
    }
}

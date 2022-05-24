using UnityEngine;
using UnityEngine.UI;

public class PlayerHPViewer : MonoBehaviour
{
    [SerializeField]
    private PlayerHP playerHP;
    private Slider sliderHP; // UI�� �����̴�

    private void Awake()
    {
        sliderHP = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Slider UI�� ���� ü�� ������ ������Ʈ
        sliderHP.value = playerHP.currentHP / playerHP.maxHP;
    }
}

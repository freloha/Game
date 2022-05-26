using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };// 열거형, 순차적으로 번호가 부여
    public Type type;
    public int value;

}

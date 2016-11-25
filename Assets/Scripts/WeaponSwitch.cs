using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour {


    [HideInInspector]
    public static bool canSwitch = false;

    public static bool showOnce = true;

    PlayerWeapon currentWeapon = new PlayerWeapon();

        

}

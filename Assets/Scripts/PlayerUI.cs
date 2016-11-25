using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {


    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject scoreboard;

    [SerializeField]
    private GameObject toggleWeapon;

    private IEnumerator coroutine;

    

	// Use this for initialization
	void Start () {
        PauseMenu.isOn = false;
        
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseMenu();

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(false);
        }
        if(WeaponSwitch.canSwitch)
        {
            if (WeaponSwitch.showOnce)
            {
                StartCoroutine(WeaponSwitchEnabled());
                WeaponSwitch.showOnce = false;
            }
        }

	}

    
    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        PauseMenu.isOn = pauseMenu.activeSelf;
    }


    private IEnumerator WeaponSwitchEnabled()
    {
        toggleWeapon.SetActive(true);
        yield return new WaitForSeconds(4f);
        toggleWeapon.SetActive(false);
    }
}

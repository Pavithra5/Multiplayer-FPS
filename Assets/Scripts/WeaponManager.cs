using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;
    
    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;

    [SerializeField]
    private PlayerWeapon advancedWeapon;

    private WeaponGraphics currentGraphics;

    public bool isReloading=false;
    private bool isUpgradedWeapon = true;
    
    // Use this for initialization
	void Start () {

        EquipWeapon(primaryWeapon);
	}

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.T))
        {
            if (isLocalPlayer)
            {
                if (!WeaponSwitch.canSwitch)
                    return;
                DestroyCurrentWeapon();
                if (isUpgradedWeapon)
                {
                    EquipWeapon(advancedWeapon);
                    isUpgradedWeapon = false;
                }
                else
                {
                    EquipWeapon(primaryWeapon);
                    isUpgradedWeapon = true;
                }
            }
            

            

            
        }
    }

	void EquipWeapon(PlayerWeapon _weapon)
    {
        
        currentWeapon = _weapon;
        GameObject weaponIns=(GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);
       
        currentGraphics = weaponIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.LogError("No weapon graphics component on the weapon object:" +weaponIns.name);

        if(isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }

        Debug.Log("current weapon" + currentWeapon.name);
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    public void Reload()
    {
        if (isReloading)
            return;
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        Debug.Log("Reloading...");

        isReloading = true;
        CmdOnReload(); 

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        currentWeapon.bullets = currentWeapon.maxBullets;

        isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim=currentGraphics.GetComponent<Animator>();
        if(anim!=null)
        {
            anim.SetTrigger("Reload");
        }
    }

    void DestroyCurrentWeapon()
    {
        foreach (Transform child in weaponHolder)
            Destroy(child.gameObject);
    }
}
  
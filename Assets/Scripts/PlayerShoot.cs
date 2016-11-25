using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    
    private PlayerWeapon currentWeapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;//Allows us to control whatwe hit

    private WeaponManager weaponmanager;

    // Use this for initialization
    void Start() {

        weaponmanager = GetComponent<WeaponManager>();
        if (cam == null)
        {
            Debug.LogError("Playershoot: No camera referenced");
            this.enabled = false;
        }

            
	}

    // Update is called once per frame
    void Update()
    {

        currentWeapon = weaponmanager.GetCurrentWeapon();

        //Don't shoot if the pause menu is being displayed
        if (PauseMenu.isOn)
            return;

        if (currentWeapon.bullets < currentWeapon.maxBullets)
        {

            if (Input.GetButtonDown("Reload"))
            {
                weaponmanager.Reload();
                return;
            }
        }


        if(currentWeapon.fireRate<=0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
             
                Shoot();
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot",0f,1f/currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
        
    }


    [Command]//Executed only on the server
    //Calls RpcShootEffect to perform muzzle flash on all clients.
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }


    [ClientRpc]//Executed on all clients
    //Perform the muzzle flash effect
    void RpcDoShootEffect()
    {
        weaponmanager.GetCurrentGraphics().muzzleFlash.Play();
    }

    
    //Called on the server to do the hit effect
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal);
    }

    //Called on all clients to perform hit effects
    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject hitEffect=(GameObject)Instantiate(weaponmanager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(hitEffect, 2f);
    }


    //Shoot
    [Client]//Because it is a local method, not on the server
    void Shoot()
    {
        if (!isLocalPlayer && !weaponmanager.isReloading) 
            return;

        
        
        if(currentWeapon.bullets<=0)
        {
           
            weaponmanager.Reload();
            return;
        }

        currentWeapon.bullets--;
        Debug.Log("Remaining bullets " + currentWeapon.bullets);

        //We are shooting, call the Onshoot method on the server
        CmdOnShoot();
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position,cam.transform.forward,out hit,currentWeapon.range,mask))
        {
            //We hit something
            Debug.Log("We hit " + hit.collider.name); 
            if(hit.collider.tag==PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage,transform.name);
                Debug.Log(hit.collider.name);
            }

            //We hit something. Call the onHitMethod on the server
            CmdOnHit(hit.point, hit.normal);


        }
    }


    [Command]//Called on the server
    void CmdPlayerShot(string _playerID,int _damage,string sourceId)
    {
        Debug.Log(_playerID + " has been shot");

        Player _player =GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage,sourceId);
        
    }
}

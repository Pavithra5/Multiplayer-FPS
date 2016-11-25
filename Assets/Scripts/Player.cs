using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    
    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SyncVar]
    public string username="LOADING...";

    public int kills;

    public int deaths;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;


    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool firstSetup = true;

    
    public bool canSwitch = false;

    
    public bool showOnce = true;

    public void SetupPlayer()
    {
        //switch cameras
        if(isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(false);
        CmdBroadCastNewPlayerSetup();
    }
    
    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            firstSetup = false;
        }
        
        SetDefaults();
    }


    public void SetDefaults()
    {

         

        currentHealth = maxHealth;
        isDead = false;

        //Enable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable game objects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

     

        //Create spwan effect
        GameObject spawnInstance = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(spawnInstance, 3f);
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount,string sourceId)
    {
        if(_isDead)
            return;
        currentHealth -= _amount;
        Debug.Log(transform.name+" now has "+currentHealth+" health");

        if (currentHealth <= 0)
        {
            
            Die(sourceId);
        }
    }
    void Update()
    {
        if (!isLocalPlayer)
            return;
        /*if (Input.GetKeyDown(KeyCode.K))
            RpcTakeDamage(9999);*/
    }
    private void Die(string sourceId)
    {
        isDead = true;

        deaths++;

        Player sourcePlayer = GameManager.GetPlayer(sourceId);

        if(sourcePlayer!=null)
        {
            sourcePlayer.kills++;
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }

        

        //Disable components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        
        //Disable game objects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //Disable the collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;



        //Spawn a death effect
        GameObject deathInstance =(GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathInstance, 3f);

        //switch cameras
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);

        }

        Debug.Log(transform.name + " is deead");

        //Call respawn method
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " respawned");
    }
}
     
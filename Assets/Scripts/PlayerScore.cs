using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    private int lastKills=0;
    private int lastDeaths = 0;
    Player player;
    PlayerUI playerUI = new PlayerUI();
    // Use this for initialization
	void Start () {

        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
	}


    void OnDestroy()
    {
        if(player!=null)
            SyncNow();  
    }

    IEnumerator SyncScoreLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);
            SyncNow();

            
        }
        
    }


    void SyncNow()
    {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.GetData(OnDataReceived);
    }

	
    void OnDataReceived(string data)
    {

        if (player.kills <= lastKills && player.deaths <= lastDeaths) 
            return;
        
        int killsSinceLast = player.kills - lastKills;
        int deathsSinceLast = player.deaths - lastDeaths;


        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        int newKills = killsSinceLast + kills;
        int newDeaths = deathsSinceLast + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);

        Debug.Log("Syncing " + newData);

        lastKills = player.kills;
        lastDeaths = player.deaths;

        if (player.kills == 1)
        {
            WeaponSwitch.canSwitch = true;
            //Debug.Log("Weapon switch enabled");
            

        }
          

        UserAccountManager.instance.SendData(newData);

    }
	
}

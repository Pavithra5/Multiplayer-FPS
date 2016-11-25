using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    private GameObject playerScoreBoardPrefab;

    [SerializeField]
    private Transform playerScoreboardList;
    void OnEnable()
    {
        //Get an array of players 
        Player[] players = GameManager.GetAllPlayers();

        foreach (Player player in players)
        {
            GameObject itemGO=(GameObject) Instantiate(playerScoreBoardPrefab);
            itemGO.transform.SetParent(playerScoreboardList);

            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if(item!=null)
            {
                item.Setup(player.username,player.kills,player.deaths);
            }
            


        }
        
        
    }


    void OnDisable()
    {
        //Clean up list of items
        foreach(Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}

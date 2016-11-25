using UnityEngine;
using System.Collections;

public class KillFeed : MonoBehaviour {



    [SerializeField]
    GameObject killFeedItemPRefab;
	// Use this for initialization
	void Start () {

        GameManager.instance.onPlayerKilledCallback += OnKill;
	}
	
    public void OnKill(string player,string source)
    {
        
        GameObject go = (GameObject)Instantiate(killFeedItemPRefab);
        go.transform.SetParent(this.transform);
        go.GetComponent<KillFeedItem>().Setup(player, source);

        Destroy(go, 4f);
    }
	
}

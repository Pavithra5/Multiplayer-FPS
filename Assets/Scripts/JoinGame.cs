using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    private NetworkManager networkManager;

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;
    

	// Use this for initialization
	void Start () {
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();

        RefreshRoomList();
	}
	
	public void RefreshRoomList()
    {
        ClearRoomList();

        if (networkManager == null)
            networkManager.StartMatchMaker();


        networkManager.matchMaker.ListMatches(0, 20, "", OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(ListMatchResponse matchList)
    {
        status.text = "";
        if(matchList==null)
        {
            status.text = "Couldn't get room list.";
            return;
        }

        

        foreach(MatchDesc match in matchList.matches)
        {
            GameObject roomListItemGO = Instantiate(roomListItemPrefab);
            roomListItemGO.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = roomListItemGO.GetComponent<RoomListItem>();
            if(_roomListItem!=null)
            {
                _roomListItem.Setup(match,JoinRoom);
            }
            



            //as well as setting up a callback function that will join the game.


            roomList.Add(roomListItemGO);
        }

        if (roomList.Count == 0)
            status.text = "No rooms at the moment";


    }


    private void ClearRoomList()
    {
        for(int i=0;i<roomList.Count;i++)
        {
            Destroy(roomList[i]);
        }


        roomList.Clear();

        
    }


    public void JoinRoom(MatchDesc _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }

    IEnumerator WaitForJoin()
    {
        ClearRoomList();
        

        int countDown=10;
        while(countDown>0)
        {
            status.text = "Joining...("+countDown+")";
            yield return new WaitForSeconds(1);
            countDown--;
        }

        //Failed to connect
        status.text = "Failed to connect";
        yield return new WaitForSeconds(1);

        MatchInfo matchInfo = networkManager.matchInfo;
        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, OnMatchDropConnection);
            networkManager.StopHost();
        }

        RefreshRoomList();

    }

    public void OnMatchDropConnection(BasicResponse response)
    {

    }


}

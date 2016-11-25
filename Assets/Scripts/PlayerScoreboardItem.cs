using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScoreboardItem : MonoBehaviour {

    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private Text killsText;

    [SerializeField]
    private Text deathsText;
	

    public void Setup(string username, int kills, int deaths)
    {
        usernameText.text = username;
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
    }
}

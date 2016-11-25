using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour {

    [SerializeField]
    private Text text;

    public void Setup(string player,string source)
    {
       
        text.text = "<b>" + source + "</b>"+" killed <i>"+ player+"</i>";
    }
}

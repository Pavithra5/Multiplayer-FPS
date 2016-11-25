using UnityEngine;
using UnityEngine.UI;

public class UserAccount_Lobby : MonoBehaviour {

    public Text userNameText;

    void Start()
    {
        if(UserAccountManager.isLoggedIn)
            userNameText.text = UserAccountManager.LoggedIn_Username;
    }

    public void Logout()
    {
        if(UserAccountManager.isLoggedIn)
            UserAccountManager.instance.Logout();
    }


}

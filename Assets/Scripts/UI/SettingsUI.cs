using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{

    public Button LogoutButton;

    private void Start()
    {
        LogoutButton.onClick.AddListener(LogOutClicked);
    }

    private void LogOutClicked()
    {
        FirebaseLogin.instance.LogOut();
    }

}

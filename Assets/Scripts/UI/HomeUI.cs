using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class HomeUI : MonoBehaviour
{
    private bool settingsOpened;
    public Button Settings;

    public Button startGame;
    public TMP_Dropdown taskDropdown;
    
    private PlayerController player;
    private GameManager gameManager;

    void Start()
    {
        settingsOpened = false;
        player = PlayerController.instance;
        gameManager = GameManager.instance;

        Settings.onClick.AddListener(toggleSettings);
        startGame.onClick.AddListener(onStartGame);
        //taskDropdown.onValueChanged.AddListener(delegate { PlayerPrefs.SetString("GameMode", taskDropdown.captionText.text); });
    }

    private void onStartGame()
    {
        PlayerPrefs.SetString("GameMode", taskDropdown.captionText.text);
        new WaitForEndOfFrame();
        gameManager.UpdateAction(GameManager.Action.GAME);
    }

    private void toggleSettings()
    {
        if (!settingsOpened) GameManager.instance.UpdateAction(GameManager.Action.SETTINGS);
        else GameManager.instance.ReturnToMainAction();

        settingsOpened = !settingsOpened;
    }
}

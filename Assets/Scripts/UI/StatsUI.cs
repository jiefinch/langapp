using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatsUI : MonoBehaviour
{
    public TMP_Text username;
    public TMP_Text coins;
    public TMP_Text rank;

    private PlayerController player;

    private void Start()
    {
        player = PlayerController.instance;
    }

    // make this only called on player update
    void Update()
    {
        username.text = player.username;
        coins.text = player.coins.ToString();
        rank.text = player.rank.ToString();
    }


}

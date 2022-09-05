using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public HashSet<string> seenQuestions = new HashSet<string>(); // can only see unique questions between sessions


    [SerializeField]
    private PlayerData _playerData;

    public PlayerData playerData => _playerData;

    public string username => _playerData.username;
    public string lastActive => _playerData.lastActive;
    public string nativeLang => _playerData.nativeLang;
    public string targetLang => _playerData.targetLang;
    public int coins => _playerData.coins;
    public int rank => _playerData.rank;

    internal void UpdatePlayer(Task<PlayerData> playerData)
    {
        throw new NotImplementedException();
    }

    public UnityEvent OnPlayerUpdated = new UnityEvent();

    void Awake()
    {
        if (instance == null) { instance = this; }
        else
        {
            instance.transform.position = transform.position;
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);


    }

    public void SetLastActive()
    {
       
        _playerData.lastActive = DateTime.Today.ToString("d");
        Debug.Log($"UPDATING LAST ACTVTIVE: {_playerData.lastActive}");
        OnPlayerUpdated.Invoke();
    }

    public void SetUsername(string user)
    {
        _playerData.username = user;
        GameManager.instance.OnUsernameUpdated.Invoke(user); // call this to also update in database
        OnPlayerUpdated.Invoke();
    }

    public void SetCoins(int c)
    {
        if (c != coins)
        {
            _playerData.coins = c;
            OnPlayerUpdated.Invoke();
        }
    }

    public void SetRank(int r)
    {
        _playerData.rank = r;
        OnPlayerUpdated.Invoke();
    }

    public void ActivatePlayer()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        gameObject.transform.Find("character sprite").gameObject.SetActive(true);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void DectivatePlayer()
    {
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.transform.Find("character sprite").gameObject.SetActive(false);
    }



    public void UpdatePlayer(PlayerData playerData)
    {
        if (!playerData.Equals(_playerData))
        {
            Debug.Log("updating player");
            if (playerData.username != _playerData.username)
                GameManager.instance.OnUsernameUpdated.Invoke(playerData.username); // call this to also update in database

            _playerData = playerData;
            OnPlayerUpdated.Invoke();
        }
    }
    public void UpdatePlayer(Equipped equip)
    {
        if (!equip.Equals(InventoryManager.Instance.equipped))
        {
            Debug.Log("updating equip");
            InventoryManager.Instance.equipped = equip;
            InventoryManager.Instance.onSpriteUpdated.Invoke(equip);
        }    
        
    }



}

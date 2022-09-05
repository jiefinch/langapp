using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System;

//using FBTask = Firebase.Task;
//using SYTask = System.Threading.Tasks;

public class SaveManager : MonoBehaviour
{

    private string PLAYER_KEY;
    private DatabaseReference root;
    private DatabaseReference _playerRef;
    private DatabaseReference _inventoryRef;
    private DatabaseReference _equippedRef;

    public PlayerData LastPlayerData { get; private set; }
    public Equipped LastEquipData { get; private set; }

    public PlayerUpdatedEvent OnPlayerUpdated = new PlayerUpdatedEvent();

    private bool initialized = false;

    public void IntializeSaveManager()
    {
        PLAYER_KEY = FirebaseLogin.PLAYER_KEY;
        if (PLAYER_KEY != null && !initialized)
        {
            Debug.Log($"Savemanager enabled --- KEY: {PLAYER_KEY}");
            root = FirebaseManager.instance.root;
            _playerRef = root.Child("users").Child(PLAYER_KEY);
            _inventoryRef = root.Child("inventory").Child(PLAYER_KEY);
            _equippedRef = root.Child("equipped").Child(PLAYER_KEY);

            // register for events from DATABASE
            _playerRef.ValueChanged += HandlePlayerValueChanged;
            _equippedRef.ValueChanged += HandleEquipValueChanged;

            initialized = true;
        }
        else { Debug.Log($"SAVE MANAGER HAS ALREADY BEEN INITLIAZED: {initialized}"); }
    }

    private void HandlePlayerValueChanged(object sender, ValueChangedEventArgs e)

    {
        // get a callback whenever OUR USER updates in database
        var json = e.Snapshot.GetRawJsonValue();
        if (!string.IsNullOrEmpty(json))
        {
            var playerData = JsonUtility.FromJson<PlayerData>(json);
            // set last player data
            LastPlayerData = playerData;
            // notify theres new data 2 be had
            OnPlayerUpdated.Invoke(playerData);
        }
    }
    private void HandleEquipValueChanged(object sender, ValueChangedEventArgs e)
    {
        // get a callback whenever OUR USER updates in database
        var json = e.Snapshot.GetRawJsonValue();
        if (!string.IsNullOrEmpty(json))
        {
            var equipData = JsonUtility.FromJson<Equipped>(json);
            // set last player data
            LastEquipData = equipData;
            // notify theres new data 2 be had
            InventoryManager.Instance.onSpriteUpdated.Invoke(equipData);
        }
    }


    public void SavePlayer(PlayerData player)
    {
        if (!player.Equals(LastPlayerData))
        {
            var json = JsonUtility.ToJson(player);
            _playerRef.SetRawJsonValueAsync(json);
            Debug.Log($"Saving player {PLAYER_KEY}");
        }
    }
    public void SavePlayer(Equipped equip)
    {
        if (!equip.Equals(LastEquipData))
        {
            var json = JsonUtility.ToJson(equip);
            _equippedRef.SetRawJsonValueAsync(json);
            Debug.Log($"Saving player equip {PLAYER_KEY}");
        }
    }


    //TODO: this also freezes fuck me
    // async Task<PlayerData?> LoadPlayer() || async Task<bool> SaveExists()


    public void EraseSave()
    {
        _playerRef.RemoveValueAsync();
    }

    private void OnDestroy() // unregister
    {
        if (_playerRef != null) _playerRef.ValueChanged -= HandlePlayerValueChanged;
        if (_equippedRef != null) _equippedRef.ValueChanged -= HandleEquipValueChanged;
        _playerRef = null;
        _equippedRef = null;
        //root = null;
    }

    [System.Serializable]
    public class PlayerUpdatedEvent : UnityEvent<PlayerData> // from playercontroller
    {

    }

}

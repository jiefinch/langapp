using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayerToSave : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private SaveManager saveManager;

    private InventoryManager inventory;
    private bool initialized = false;

    public void InitializeSync()
    {
        Debug.Log("initializing sync");
        if (PlayerController.instance == null)
        {
            Debug.Log("No player found, cancelling");
            return;
        }
        if (initialized)
        {
            Debug.Log("Already Intializied");
            return;
        }

        player = PlayerController.instance;
        saveManager = FirebaseManager.instance.GetComponent<SaveManager>();
        inventory = InventoryManager.Instance;

        // heard from inside our DB
        saveManager.OnPlayerUpdated.AddListener(HandlePlayerSaveUpdated);
        inventory.onSpriteUpdated.AddListener(HandlePlayerSaveUpdated);



        // heard from game
        player.OnPlayerUpdated.AddListener(HandlePlayerUpdated);
        inventory.onSpritePartUpdated.AddListener(HandleSpriteUpdated);
        inventory.onSpriteUpdated.AddListener(HandleSpriteUpdated);


        // just in case DB event was missed
        if (saveManager.LastPlayerData != null)
            player.UpdatePlayer(saveManager.LastPlayerData);
        if (saveManager.LastEquipData != null)
            player.UpdatePlayer(saveManager.LastEquipData);

        initialized = true;
        Debug.Log("Sync activated");

    }

    private void HandlePlayerSaveUpdated(PlayerData playerData)
    {
        Debug.Log("recieving player update from DB -- updating");
        player.UpdatePlayer(playerData);
    }
    private void HandlePlayerSaveUpdated(Equipped equip)
    {
        Debug.Log("recieving equip update from DB -- updating");
        player.UpdatePlayer(equip);
    }



    private void HandlePlayerUpdated()
    {
        Debug.Log("recieving player update from game -- saving");
        saveManager.SavePlayer(player.playerData);
    }
    private void HandleSpriteUpdated(Equipped equip, InventoryManager.Item item)
    {
        Debug.Log("recieving equip update from game -- saving");
        saveManager.SavePlayer(equip);
    }
    private void HandleSpriteUpdated(Equipped equip)
    {
        Debug.Log("recieving equip update from game -- saving");
        saveManager.SavePlayer(equip);
    }

}

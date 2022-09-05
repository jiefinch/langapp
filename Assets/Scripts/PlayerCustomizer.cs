using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizer : MonoBehaviour
{
    public SpriteRenderer eyes;
    public SpriteRenderer hair;
    public SpriteRenderer hairback;

    public SpriteRenderer armf;
    public SpriteRenderer armb;
    public SpriteRenderer outfitBase;

    public GameObject accessories;

    private InventoryManager inventory;
    private EquipItems catalogue;

    private void Start()
    {
        inventory = GetComponent<InventoryManager>();
        inventory.onSpriteUpdated.AddListener(UpdatePlayerSprite);
        inventory.onSpritePartUpdated.AddListener(UpdatePlayerSpritePart);
        inventory.onAccessoryRemoved.AddListener(RemoveAccessory);
        inventory.onAccessoryAdded.AddListener(AddAccessory);
        catalogue = inventory.equipItems;

    }

    private void UpdatePlayerSpritePart(Equipped equipped, InventoryManager.Item item)
    {
        switch (item)
        {
            case InventoryManager.Item.EYES:
                UpdateEyes(equipped.eyes);
                break;

            case InventoryManager.Item.OUTFIT:
                UpdateOutfit(equipped.outfit);
                break;

            case InventoryManager.Item.HAIR:
                UpdateHair(equipped.hair);
                break;
        }
    }

    private void UpdatePlayerSprite(Equipped equipped)
    {
        UpdateEyes(equipped.eyes);
        UpdateOutfit(equipped.outfit);
        UpdateHair(equipped.hair);
        foreach (var acc in equipped.accessories) AddAccessory(acc);
    }


    private void UpdateEyes(int i)
    {
        eyes.sprite = catalogue.Eyes[i].sprite;
        //eyes.color = items.Eyes[equipped.eyes].color;     TODO COLOR LATER
    }
    private void UpdateOutfit(int i)
    {
        armf.sprite = catalogue.Outfits[i].armf;
        armb.sprite = catalogue.Outfits[i].armb;
        outfitBase.sprite = catalogue.Outfits[i].outfitbase;
    }
    private void UpdateHair(int i)
    {
        hair.sprite = catalogue.Hairs[i].front;
        hairback.sprite = catalogue.Hairs[i].back;
    }

    private void RemoveAccessory(int acc)
    {
        Destroy(accessories.transform.Find(acc.ToString()).gameObject);
    }

    private void AddAccessory(int acc)
    {
        // create the accessory / child
        GameObject child = new GameObject();
        child.name = acc.ToString();

        // make it the child of the accessories parent object
        child.transform.parent = accessories.transform;

        // rescale / reposition to parent
        child.transform.localScale = accessories.transform.localScale;
        child.transform.position = accessories.transform.position;

        // add a sprite renderer to the child
        SpriteRenderer sr = child.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;

        // get the sprite from catalogue
        Sprite sp = catalogue.Accessories[acc].sprite;

        if (sp.name.Contains("back")) { sr.sortingOrder = 0; } // draw on back layer
        else { sr.sortingOrder = 9; } // default front

        // change child sprite to "glasses"
        sr.sprite = sp;

    }


}

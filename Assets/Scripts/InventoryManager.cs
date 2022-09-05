using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public EquipItems equipItems;
    public Equipped equipped;
    [SerializeField]public Dictionary<string, List<int>> Owned; // to implement later

    public enum Item { EYES, HAIR, OUTFIT, ACCESSORY }

    public SpriteUpdatedEvent onSpriteUpdated = new SpriteUpdatedEvent();
    public SpritePartUpdatedEvent onSpritePartUpdated = new SpritePartUpdatedEvent();
    public AccessoryRemovedEvent onAccessoryRemoved = new AccessoryRemovedEvent();
    public AccessoryAddedEvent onAccessoryAdded = new AccessoryAddedEvent();

    private void Start()
    {
        Instance = this;
    }




    public void UpdateSprite(Equipped equip)
    {
        equipped = equip;
        onSpriteUpdated.Invoke(equip);
    }

    public void ChangeSprite(Item item, int i)
    {
        switch(item)
        {
            case Item.EYES:
                equipped.eyes = i;
                onSpritePartUpdated.Invoke(equipped, Item.EYES);
                break;

            case Item.OUTFIT:
                equipped.outfit = i;
                onSpritePartUpdated.Invoke(equipped, Item.OUTFIT);
                break;

            case Item.HAIR:
                equipped.hair = i;
                onSpritePartUpdated.Invoke(equipped, Item.HAIR);
                break;

            case Item.ACCESSORY:
                if (equipped.accessories.Contains(i))
                {
                    equipped.accessories.Remove(i);
                    onAccessoryRemoved.Invoke(i);
                }
                    
                else
                {
                    equipped.accessories.Add(i);
                    onAccessoryAdded.Invoke(i);
                }
                break;
        }
    }

    [System.Serializable]
    public class SpriteUpdatedEvent : UnityEvent<Equipped> // from playercontroller
    {

    }

    public class SpritePartUpdatedEvent : UnityEvent<Equipped, Item> // from playercontroller
    {

    }

    public class AccessoryRemovedEvent : UnityEvent<int>
    {

    }

    public class AccessoryAddedEvent : UnityEvent<int>
    {

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpriteSwapper : MonoBehaviour
{

    public string _PART;
    private InventoryManager.Item Part;
    private int currentOption = 0;

    private void Start()
    {
        string[] words = _PART.Split(' ');
        _PART = words[0];

        bool tryParse = Enum.TryParse(_PART, out Part);
        if (!tryParse) Debug.Log($"PARSING {_PART} as INVENTORY ENUM ITEM FAILED");

        if (words.Length > 1) { currentOption = int.Parse(words[1]); }

    }

    // for things u can loop thru >>> CHANGE THIS SO THE LOOP IS FOR "OWNED" ITEM SET:: AKA {1,4,6,7} swap thru it


    public void UpdateOption()
    {
        InventoryManager.Instance.ChangeSprite(Part, currentOption); //update sprite
    }


    public void NextOption() 
    { 
        currentOption++;
        ClampOptions(Part);
        UpdateOption(); 
    }
    public void PrevOption() 
    { 
        currentOption--;
        ClampOptions(Part);
        UpdateOption(); 
    }

    private void ClampOptions(InventoryManager.Item item)
    {
        int options = 0;
        EquipItems equipItems = InventoryManager.Instance.equipItems;

        if (item == InventoryManager.Item.EYES)
            options = equipItems.Eyes.Count;

        else if (item == InventoryManager.Item.OUTFIT)
            options = equipItems.Outfits.Count;

        else if (item == InventoryManager.Item.HAIR)
            options = equipItems.Hairs.Count;

        currentOption %= options; // options.count
        if (currentOption == -1) { currentOption = options - 1; } // options.count

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class CreationUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerData creatorData;

    private PlayerMovement playerMovement;

    [Header("Interactables")]
    public TMP_InputField nameInput;
    public TMP_Dropdown dropNative;
    public TMP_Dropdown dropTarget;

    public TMP_Text confirm;
    public Button confirmButton;

    public UnityEvent onConfirmed = new UnityEvent();


    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.instance;
        creatorData = new PlayerData(FirebaseLogin.instance.user.DisplayName, player.lastActive, "native","target",0,0);

        playerMovement = player.gameObject.GetComponent<PlayerMovement>();
        playerMovement.enabled = false;

        //dropNative.onValueChanged.AddListener(delegate { SetNative(); }); (the next button sets it for us)
        //dropTarget.onValueChanged.AddListener(delegate { SetTarget(); });
        nameInput.onEndEdit.AddListener(delegate { SetName(); });
        confirmButton.onClick.AddListener(confirmButtonClicked);
    }


    public void SetName() 
    {
        string username = nameInput.text;
        if (creatorData.username.Contains("(Guest)"))
            username += " (Guest)";

        creatorData = new PlayerData(username, creatorData.lastActive, creatorData.nativeLang, creatorData.targetLang, 0, 0);
    }

    public void SetNative()
    {
        string nativeLang = dropNative.captionText.text;
        creatorData = new PlayerData(creatorData.username, creatorData.lastActive, nativeLang, creatorData.targetLang, 0, 0);
    }
    public void SetTarget()
    {
        string targetLang = dropTarget.captionText.text;
        creatorData = new PlayerData(creatorData.username, creatorData.lastActive, creatorData.nativeLang, targetLang, 0, 0);
    }

    public void UpdateConfirmText()
    {

        if (creatorData.username == "" || creatorData.username == " (Guest)" || creatorData.username == "(Guest)")
        {
            confirm.text = "Please enter a name!";
            confirmButton.gameObject.SetActive(false);
        }
        else
        {
            confirm.text =
            "\nName: " + creatorData.username
            + "\nNative Language: " + creatorData.nativeLang
            + "\nTarget Language: " + creatorData.targetLang;
            confirmButton.gameObject.SetActive(true);
        }
    }

    public void confirmButtonClicked()
    {
        SaveManager saveManager = FirebaseManager.instance.GetComponent<SaveManager>();
        // create in database
        saveManager.SavePlayer(creatorData);
        saveManager.SavePlayer(InventoryManager.Instance.equipped);

        GameManager.instance.UpdateAction(GameManager.Action.HOME);
    }



}

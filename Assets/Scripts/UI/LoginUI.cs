using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class LoginUI : MonoBehaviour
{
    //Login variables
    public TMP_Text statusText;
    public Button exit;
    public Button topLogin;
    public Button topRegister;
    public Button guestLogin;

    [Header("Login")]
    public GameObject loginCanvas;
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public Button LoginButton;


    //Register variables
    [Header("Register")]
    public GameObject registerCanvas;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public Button RegisterButton;

    [SerializeField]private bool loginActivated;


    private FirebaseLogin FBlogin;

    private void Start()
    {
        FBlogin = FirebaseLogin.instance;
        loginDefaults();

        topLogin.onClick.AddListener( delegate { loginActivated = true; } );
        topRegister.onClick.AddListener(delegate { loginActivated = false; });

        LoginButton.onClick.AddListener(delegate { StartCoroutine(LoginClicked()); });
        RegisterButton.onClick.AddListener(delegate { StartCoroutine(RegisterClicked()); });

        exit.onClick.AddListener(GameManager.instance.ReturnToMainAction);
        guestLogin.onClick.AddListener(delegate { guestLogin.GetComponent<TMP_Text>().text = "Loading..."; StartCoroutine(FBlogin.AnonLogin());  });

    }

    private void loginDefaults()
    {
        bool starting = GameManager.instance.prevMainAction == GameManager.Action.START;

        loginCanvas.SetActive(loginActivated);
        registerCanvas.SetActive(!loginActivated);

        if (loginActivated)
            topLogin.Select();
        else
            topRegister.Select();

        exit.gameObject.SetActive(!starting);
        guestLogin.gameObject.SetActive(starting);

    }
    
    public IEnumerator LoginClicked()
    {
        yield return StartCoroutine(FBlogin.Login(emailLoginField.text, passwordLoginField.text));
        // make this wait
        statusText.text = FBlogin.getMessage();
    }

    public IEnumerator RegisterClicked()
    {
        // convert acc
        yield return StartCoroutine(FBlogin.Register(emailRegisterField.text, passwordRegisterField.text, passwordRegisterVerifyField.text));
        // make this wait
        statusText.text = FBlogin.getMessage();

    }

}

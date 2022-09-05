using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.Events;
using System.Threading.Tasks;

public class FirebaseLogin : MonoBehaviour
{
    // passwords need to be at least 6 characters
    // test account: email: test@test.com pass: 123456

    //Firebase variables
    [Header("Firebase")]
    public UnityEvent OnLogInCompleted = new UnityEvent();
    public UnityEvent OnGuestOrRegisterCompleted = new UnityEvent();
    public UnityEvent OnLogOutCompleted = new UnityEvent();

    public FirebaseAuth auth;
    public FirebaseUser user;
    public static string PLAYER_KEY;

    public static FirebaseLogin instance;
    private string message;

    [SerializeField] private bool useAutoLogin;

    void Awake()
    {
        instance = this;
    }


    public void InitializeAuth()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance; //Set the authentication instance object

        GameManager.instance.OnUsernameUpdated.AddListener( newUsername => StartCoroutine(UpdateUsername(newUsername)) );

        if (useAutoLogin)
            StartCoroutine(CheckAutoLogin()); 

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

    }


    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();
        if (user != null)
        {
            var reloadTask = user.ReloadAsync();
            yield return new WaitUntil(predicate: () => reloadTask.IsCompleted);
            // get data
            AutoLogin();
        }
    }

    public IEnumerator AnonLogin() 
    {
        yield return StartCoroutine(AnonCreate());
        yield return StartCoroutine(UpdateUsername("(Guest)"));

        
        OnGuestOrRegisterCompleted.Invoke();
        //yield return StartCoroutine(UpdateUsername(PlayerController.instance.username + " (Guest)"));
    }

    public IEnumerator RegisterAnon(string _email, string _password, string _vpassword)
    {
        // (Anonymous user is signed in at that point.)
        user = auth.CurrentUser;

        if (_password != _vpassword)
        {
            message = "Password Does Not Match!";
        }
        else
        {
            // 1. Create the email and password credential, to upgrade the anon user
            Credential credential = EmailAuthProvider.GetCredential(_email, _password);
            // 2. Links the credential to the currently signed in user anon user
            var RegisterAnonTask = user.LinkWithCredentialAsync(credential);
            yield return new WaitUntil(predicate: () => RegisterAnonTask.IsCompleted);

            if (RegisterAnonTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {RegisterAnonTask.Exception}");
                FirebaseException firebaseEx = RegisterAnonTask.Exception.GetBaseException() as FirebaseException;
                message = "Registration Failed!\n" + firebaseEx.Message;

            }
            else
            {
                // remove (Guest) from username
                yield return StartCoroutine(UpdateUsername(PlayerController.instance.username.Replace(" (Guest)", "")));
                Debug.LogFormat("Anonymous account successfully upgraded {0} {1}", user.DisplayName, user.UserId);
            }
        }

    }

    private IEnumerator AnonCreate()
    {
        //Call the Firebase auth signin function passing the email and password
        var AnonLoginTask = auth.SignInAnonymouslyAsync();
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => AnonLoginTask.IsCompleted);

        if (AnonLoginTask.Exception != null)
        {
            Debug.LogWarning(AnonLoginTask.Exception);
        }
        else
        {
            user = AnonLoginTask.Result;
            Debug.LogFormat("ANON User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            PLAYER_KEY = user.UserId;
        }
    }

    public IEnumerator Register(string _email, string _password, string _vpassword)
    {
        user = auth.CurrentUser;

        if (_password != _vpassword)
        {
            message = "Password Does Not Match!";
        }
        else if (user != null && user.DisplayName == "(Guest)")
        {
            StartCoroutine(RegisterAnon(_email, _password, _vpassword));
            GameManager.instance.ReturnToMainAction();
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            yield return new WaitUntil(() => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                message = "Register Failed!\n" + firebaseEx.Message;

            }
            else
            {
                //User is now logged in
                //Now get the result
                user = RegisterTask.Result;
                Debug.LogFormat("User Registered successfully: {0} ({1})", user.DisplayName, user.Email);
                message = "Registered";
                PLAYER_KEY = user.UserId;
                OnGuestOrRegisterCompleted.Invoke();

            }
        }
            

    }

    public IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            message = "Login Failed!\n" + firebaseEx.Message;

        }
        else
        {
            //User is now logged in
            //Now get the result
            user = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            message = "Logged In";
            PLAYER_KEY = user.UserId;
            OnLogInCompleted.Invoke();

        }
    }


    private IEnumerator UpdateUsername(string _username)
    {
        // UPDATE THE CURRENT USER
        user = auth.CurrentUser;

        if (user != null)
        {
            UserProfile profile = new UserProfile { DisplayName = _username };
            var UpdateUsernameTask = user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => UpdateUsernameTask.IsCompleted);

            if (UpdateUsernameTask.Exception != null)
            {
                Debug.LogWarning(UpdateUsernameTask.Exception);
            }
            else
            {
                Debug.LogFormat("AUTH Username updated successfully: {0} ({1})", user.DisplayName, user.UserId);
            }
        }
        else { Debug.Log("No User Found"); }
    }

    public string getMessage()
    {
        if (message != null)
            return message;
        else return "";
    }

    private void AutoLogin()
    {
        if (user != null)
        {
            Debug.LogFormat("AUTO signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
            PLAYER_KEY = user.UserId;

            if (user.DisplayName == "(Guest)" || user.DisplayName == "")
                OnGuestOrRegisterCompleted.Invoke(); // havent completed character creation yet
            else
                OnLogInCompleted.Invoke();
        }
    }

    public void LogOut()
    {
        auth.SignOut();
        if (auth != null && user != null)
            Debug.LogFormat("User signed out: {0} ({1})", user.DisplayName, user.UserId);

        Destroy(GameObject.Find("Player"));
        OnLogOutCompleted.Invoke();

    }

    // Track state changes of the auth object. https://firebase.google.com/docs/auth/unity/manage-users "get currently signed in"
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log($"STATE CHANGE: Signed out {user.UserId} \n {user.DisplayName}");
                OnLogOutCompleted.Invoke();

            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"STATE CHANGE: Signed in {user.UserId} \n {user.DisplayName}");
            }
        }
    }

    //it does not directly log the user out but invalidates the auth
    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
        
    }

}

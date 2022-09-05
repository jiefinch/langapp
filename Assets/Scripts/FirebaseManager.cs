using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Database;


public class FirebaseManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public UnityEvent OnFirebaseInitialized = new UnityEvent();
    public DatabaseReference root;

    public static FirebaseManager instance;
    void Awake()
    {
        instance = this;
        StartCoroutine(CheckAndFixDependencies());

        OnFirebaseInitialized.AddListener(delegate {
            root = FirebaseDatabase.DefaultInstance.RootReference; 
        });
    }

    private IEnumerator CheckAndFixDependencies()
    {
        Debug.Log("Checking firebase dependancies.");
        //Check that all of the necessary dependencies for Firebase are present on the system
        var CheckAndFixDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(predicate: () => CheckAndFixDependenciesTask.IsCompleted);
        var dependencyResult = CheckAndFixDependenciesTask.Result;

        if (dependencyResult == DependencyStatus.Available)
        {
            Debug.Log("Firebase dependancies avaliable.");
            OnFirebaseInitialized.Invoke();
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyResult);
        }

    }




}

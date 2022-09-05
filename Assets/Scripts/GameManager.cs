using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

//DatabaseReference reference = FirebseDatabase.DefaultInstance.RootReference;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public UsernameUpdatedEvent OnUsernameUpdated = new UsernameUpdatedEvent();

    [Serializable] class ActionItem
    {
        //public List<GameObject> UI;
        public string SceneName;
        public bool IsAdditive;

    }

    [SerializeField] private List<ActionItem> ActionItems;
    [SerializeField] private Camera Camera;

    // USE THIS TO TOGGLE UI in "settings" canvas
    public enum Action
    {
        START,
        LOGIN,
        CREATION,

        HOME,
        GAME,

        STATS,
        SHOP,
        CUSTOMIZE,
        SETTINGS
    }

    public Action action;
    public Action prevMainAction;

    void Awake()
    {
        instance = this;

    }

    
    public void UpdateAction(Action a)
    {
        if (a == action) return; // no change

        ActionItem nextAction = ActionItems[(int)a];
        ActionItem prevAction = ActionItems[(int)action];

        if (!prevAction.IsAdditive)
            prevMainAction = action; // store previous to return to AKA {START, CREATION, HOME, GAME}

        Debug.Log($"Prev Main Action: {prevMainAction},  Next Action: {a}\nComing from {prevAction}, loading {nextAction}.");


        Scene[] loadedScenes = LoadedScenes();
        bool alreadyLoaded = false;
        foreach (Scene s in loadedScenes)
        {
            if (s.name == nextAction.SceneName)
            { alreadyLoaded = true; break; }
        }

        // load the corresponding scene if not already loaded
        if (!alreadyLoaded)
        {
            IEnumerator loadScene()
            {
                yield return SceneManager.LoadSceneAsync(nextAction.SceneName, LoadSceneMode.Additive);
                yield return new WaitForEndOfFrame();
                SwitchState(a); // wait for scene to load before settng up scene specific params
            }
            StartCoroutine(loadScene());

        }

        if (!nextAction.IsAdditive)
        {
            // if it isnt the main scene or the scene we're going to, unload
            foreach (Scene s in loadedScenes)
            {
                if (!s.name.Equals(nextAction.SceneName) && !s.name.Equals("Main")) 
                    SceneManager.UnloadSceneAsync(s);
            }
        }
        
        action = a;
    }

    public void UpdateAction(string s)
    {
        UpdateAction((Action)Enum.Parse(typeof(Action), s));
    }

    public void ReturnToMainAction()
    {
        UpdateAction(prevMainAction);
    }

    private Scene[] LoadedScenes()
    {
        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
        }
        return loadedScenes;

    }

    void SwitchState(Action a)
    {
        SaveManager saveManager = FirebaseManager.instance.GetComponent<SaveManager>();
        Camera.GetComponent<PanZoom>().enabled = false;
        switch (a)
        {
            case Action.START:
                break;
            case Action.CREATION:
                saveManager.IntializeSaveManager();
                break;
            case Action.HOME:
                Camera.GetComponent<PanZoom>().enabled = true;
                saveManager.IntializeSaveManager();
                FirebaseManager.instance.GetComponent<SyncPlayerToSave>().InitializeSync();
                PlayerController.instance.ActivatePlayer();
                break;
            case Action.GAME:
                PlayerController.instance.SetLastActive();
                break;
            default:
                break;
        }

    }


    [System.Serializable]
    public class UsernameUpdatedEvent : UnityEvent<String> // from playercontroller
    {

    }
}

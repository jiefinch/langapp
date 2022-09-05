using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Firebase;

public class StartUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text startText;
    private bool ready;

    private void Update()
    {
        ready = FirebaseManager.instance.dependencyStatus.Equals(DependencyStatus.Available);

        if (ready)
        {
            startText.text = "Press anywhere to begin!";
            if (Input.GetMouseButtonUp(0))
                GameManager.instance.UpdateAction(GameManager.Action.LOGIN);
        }
       

    }


}

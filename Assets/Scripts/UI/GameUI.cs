using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Button Management")]
    public Button exit1;
    public Button exit2;
    public Button submit;
    public Button next;

    [Header("Game UI Objects")]
    public TMP_Dropdown dropdown;
    public TMP_Text directions;
    public TMP_Text content;
    public TMP_Text progress;

    public GameObject rewards;

    public static GameUI Instance;
    private QuestionManager questionManager;

    [Header("POPUP TEXTS")]
    public TMP_Text Results;
    public TMP_Text Rewards;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        questionManager = QuestionManager.instance;
        ToggleSubmitButton();

        exit1.onClick.AddListener(ExitGame);
        exit2.onClick.AddListener(ExitGame);
        submit.onClick.AddListener(questionManager.ParseQuestionAnswer);
        next.onClick.AddListener(delegate { QuestionManager.instance.Next(); });


        dropdown.onValueChanged.AddListener(delegate { ToggleSubmitButton(); });

        QuestionManager.instance.onNextQuestion.AddListener(NewQuestion);
        QuestionManager.instance.onQuestionReady.AddListener(ToggleSubmitButton);

    }

    void ExitGame()
    {
        GameManager.instance.UpdateAction(GameManager.Action.HOME);
    }

    void ToggleSubmitButton()
    {
        submit.interactable = dropdown.captionText.text != "Select an option";
    }
    void NewQuestion()
    {
        progress.text = QuestionManager.instance.QuestionCount.ToString() + "/5";
    }



}

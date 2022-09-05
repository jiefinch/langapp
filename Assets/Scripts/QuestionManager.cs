using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Events;
using System.Reflection;

public class QuestionManager : MonoBehaviour
{
    private DatabaseReference root;

    [SerializeField] private List<QuestionTemplate> questionTemplates;


    public UnityEvent onQuestionLoaded = new UnityEvent();
    public UnityEvent onQuestionReady= new UnityEvent();
    public UnityEvent onNextQuestion = new UnityEvent();

    public static QuestionManager instance;
    private PlayerController player;
    public bool isCorrect;
    public string QuestionID;
    public int QuestionCount = 0;



    private int CorrectCount;
    private long startingRank;
    private float startTime;

    public enum QUESTIONS
    {
        //random,
        reading,    // machine reading comprehension
        labeling,   // named entity recognition
        inference,  // naturla language inference
        emotion,     // sentiment analysis
        random
    }

    public QUESTIONS questionType;


    Type questionClass;
    public object questionObject;
    public MethodInfo ParseAnswer;

    private void Start()
    {
        instance = this;
        player = PlayerController.instance;
        root = FirebaseManager.instance.root;

        CorrectCount = 0;
        startingRank = player.rank;
        startTime = Time.time;

        questionType = Enum.Parse<QUESTIONS>(PlayerPrefs.GetString("GameMode"));
        Debug.Log(questionType);
        StartCoroutine(ServeQuestion()); // start off first Q
    }

    public void UpdateResultsText()
    {
        MethodInfo ShowResults = questionClass.GetMethod("ShowResults");
        ShowResults.Invoke(questionObject, null);
    }

    public void Next()
    {
        QuestionCount++;

        if (QuestionCount >= 5)
        {
            // bring up "completion"
            float elapsedTime = Time.time - startTime;
            int coins = 10 + 10 * player.rank + 20 * CorrectCount;
            long rankDifference = player.rank - startingRank;


            GameUI.Instance.exit1.interactable = false;
            GameUI.Instance.submit.interactable = false;
            GameUI.Instance.rewards.SetActive(true);

            SetRewards(elapsedTime, coins, rankDifference);
            
        }
        else
        {
            StartCoroutine(ServeQuestion());
        }
        
        onNextQuestion.Invoke();
    }



    public IEnumerator ServeQuestion()
    {
        if (PlayerPrefs.GetString("GameMode") == "random")
        {
            questionType = GetRandomQuestionType();
        }

        List<Tuple<long, string>> QuestionIDs = new List<Tuple<long, string>>(); // <complexity, ID>
        QuestionTemplate currentQuestion = questionTemplates[(int)questionType];

        yield return StartCoroutine(SetRandomQuestion(currentQuestion.taskCode, QuestionIDs));
        yield return new WaitForEndOfFrame();

        questionClass = Type.GetType(currentQuestion.Class);
        questionObject = Activator.CreateInstance(questionClass, QuestionID);
        ParseAnswer = questionClass.GetMethod("ParseAnswer");//, BindingFlags.NonPublic | BindingFlags.Instance);

    }


    private IEnumerator SetRandomQuestion(string taskCode, List<Tuple<long, string>> QuestionIDs)
    {
        string lang = Data.Language[player.targetLang];
        var task = root.Child("complexity").Child(taskCode).Child(lang).GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);

        Debug.Log(taskCode);
        if (task.IsFaulted) { Debug.LogWarning(task.Exception); }
        else if (task.IsCompleted)
        {
            Debug.Log("TASK COMPLETED");
            DataSnapshot Questions = task.Result;
            Debug.Log(Questions);
            foreach (var Q in Questions.Children)
            {
                Debug.Log($"LOOKING AT {Q.Key}");
                if (!player.seenQuestions.Contains(Q.Key))
                {
                    QuestionIDs.Add(new Tuple<long, string>(long.Parse(Q.GetValue(false).ToString()), Q.Key));
                }
            }
            
        }

        Debug.Log(string.Join("\n\r", QuestionIDs));
        QuestionID = GetRandomQuestion(QuestionIDs);
    }

    private string GetRandomQuestion(List<Tuple<long, string>> QuestionIDs)
    {
        QuestionIDs.Sort((x, y) => y.Item1.CompareTo(x.Item1)); // sorted in order of complexity
        int mid; // middle index

        for (mid = 0; mid < QuestionIDs.Count; mid++)
        {
            if (player.rank <= QuestionIDs[mid].Item1)
            {
                break; // found the median complexity
            }
        }

        HashSet<string> possibilities = new HashSet<string>();

        int len = QuestionIDs.Count;
        for (int i = 0; i <= len; i++)  // outward search
        {
            bool canAdd = (mid + i) >= 0 && (mid + i) < len;
            bool canSubtract = (mid - i) >= 0 && (mid - i) < len;

            if (possibilities.Count > 6)
                break;

            if (canAdd)
                possibilities.Add(QuestionIDs[mid + i].Item2);

            if (canSubtract)
                possibilities.Add(QuestionIDs[mid - i].Item2);
        }

        //Debug.Log(string.Join("\n\r", possibilities));

        return possibilities.ElementAt(UnityEngine.Random.Range(0, possibilities.Count));

    }

    public void ParseQuestionAnswer()
    {
        //Debug.Log(ParseAnswer);
        //Debug.Log(questionObject);
        ParseAnswer.Invoke(questionObject, null);
    }


    public void AdjustRank(bool _isCorrect)
    {
        if (_isCorrect)
            CorrectCount++;


        if (!isCorrect && _isCorrect)
            isCorrect = true;

        else if (isCorrect && _isCorrect) // two right in a row - reset
        {
            player.SetRank(player.rank + 1);
            isCorrect = false;
        }
        else if (!_isCorrect)
            player.SetRank(Math.Max(0, player.rank - 1));
    }

    private void SetRewards(float elapsedTime, int coins, long rankDifference)
    {
        GameUI.Instance.Rewards.text = $"RESULTS: ({CorrectCount}/5)\n\n";

        if (rankDifference < 0)
            GameUI.Instance.Rewards.text += $"RANK: {rankDifference}";
        else
            GameUI.Instance.Rewards.text += $"RANK: +{rankDifference}";

        player.SetCoins(player.coins + coins);
        GameUI.Instance.Rewards.text  += $"\nCOINS +{coins} \nTIME {elapsedTime}sec";
    }


    private QUESTIONS GetRandomQuestionType()
    {
        return (QUESTIONS)UnityEngine.Random.Range(0, Enum.GetValues(typeof(QUESTIONS)).Length - 1);
    }


}
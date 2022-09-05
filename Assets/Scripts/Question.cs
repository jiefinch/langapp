using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System;



[Serializable]
public class Question
{

    public DatabaseReference _questionRef;
    public DatabaseReference _complexityRef;


    public static string Instructions { set; get; }
    public GameUI ui = GameUI.Instance;

    // listen for data changed and autoupdate answers / label / complexity
    public string QuestionID { get; private set; }
    public string task { get; private set; }
    public string language { get; private set; }


    public class Data
    {
        public string content; // grab as json first and foremost, can parse in individual classes
        public List<int> answer;
        public List<int> labels;
        public long complexity;
    }
    public Data data;


    public bool isLabeled()
    {
        return data.answer != null && data.answer.Count > 0;
    }

    public Question(string questionID)
    {
        QuestionID = questionID; // sa-ko-001
        string[] sections = questionID.Split("-");
        task = sections[0];
        language = sections[1];

        _complexityRef = FirebaseManager.instance.root.Child("complexity").Child(task).Child(language).Child(QuestionID);
        _questionRef = FirebaseManager.instance.root.Child("questions").Child(QuestionID);


        FirebaseManager.instance.root.Child("questions").Child(QuestionID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted) Debug.LogWarning(task.Exception);
            else if (task.IsCompleted)
            {
                string json = task.Result.GetRawJsonValue();
                if (!string.IsNullOrEmpty(json))
                {
                    data = JsonUtility.FromJson<Data>(json);

                    ui.directions.text = Instructions;
                    ui.content.text = data.content;

                    QuestionManager.instance.onQuestionLoaded.Invoke();

                }
            }
        });
    }

    public void UploadResponse(Data data, long complexity, bool isCorrect)     // we know the answer
    {
        UploadResponse(data);
        QuestionManager.instance.AdjustRank(isCorrect);
        long newComplexity = AdjustedComplexity(isCorrect, complexity);
        UploadComplexity(newComplexity, complexity);
    }
    public void UploadResponse(Data data)     // we dont know correct answer yet
    {
        string json = JsonUtility.ToJson(data);
        FirebaseManager.instance.root.Child("questions").Child(QuestionID).SetRawJsonValueAsync(json);
    }

    
    private long AdjustedComplexity(bool isCorrect, long complexity)
    {
        int playerRank = PlayerController.instance.rank;
        float difference = Mathf.Abs(playerRank - complexity);
        long newComplexity = 0;

        if (playerRank < complexity)
        {
            if (isCorrect) // question was answered correctly by a lower player
                newComplexity = (long)(complexity - (difference/2f));
        }
        else if (playerRank >= complexity)
        {
            if (!isCorrect) // question was answered wrong by a same/higher player
                newComplexity = (long)(complexity + (difference/2f));
        }

        Debug.Log($"COMPLEXITY {complexity}, DIFFERENCE {difference}, NEW COMPLEXITY {newComplexity}");

        return newComplexity;
    }

    private void UploadComplexity(long newComplexity, long oldComplexity)
    {
        if (newComplexity != oldComplexity)
        {
            _complexityRef.SetValueAsync(newComplexity);
            _questionRef.Child("complexity").SetValueAsync(newComplexity);
        }
    }
}




[Serializable]
public class MultipleChoice : Question
{
    
    public int choice { get; set; }
    public List<string> DropOptions = new List<string> { "Select an option" };


    public MultipleChoice(string questionID) : base(questionID)
    {
    }

    //List<string> DropOptions = new List<string> { "Select an option", "Positive", "Negative", "I don't know" };
    public void Initialize()
    {
        ui.dropdown.ClearOptions();
        ui.dropdown.AddOptions(DropOptions);
        QuestionManager.instance.onQuestionReady.Invoke();
    }

    string option;
    public void ParseAnswer()
    {
        //Debug.Log("PARSING ANSWER");
        option = ui.dropdown.captionText.text;

        for (int i = 0; i < DropOptions.Count; i++)
        {
            if (DropOptions[i] == option)
            {
                choice = i - 1;
            }
        }
        if (choice >= 0 && choice < DropOptions.Count-1) // discard options "select an answer" and "i dont know"
        {
            data.labels[choice] += 1;
            if (isLabeled())
                UploadResponse(data, data.complexity, isCorrect());
            else
                UploadResponse(data);
        }
    }

    public void ShowResults()
    {
        float total = 0;
        foreach (float i in data.labels)
            total += i;

        GameUI.Instance.Results.text = "RESULTS: \n";

        for(int i = 1; i < DropOptions.Count-1; i++)
        {
            string percentage = (data.labels[i - 1] / total * 100).ToString(); // POSITIVE: 30.22% (5) // 33.333333333333
            percentage = percentage.Substring(0, Math.Min(5, percentage.Length ));
            GameUI.Instance.Results.text += $"\n{DropOptions[i]}: {percentage}% ({data.labels[i-1]})";
        }

        if (isLabeled() && option != "I don't know")
        {
            if (isCorrect())
                GameUI.Instance.Results.text += "\n\nCORRECT";
            else
                GameUI.Instance.Results.text += "\n\nWRONG";
        }
    }
    public bool isCorrect()
    {
        return choice == data.answer[0];
    }

}
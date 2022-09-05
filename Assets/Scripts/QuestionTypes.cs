using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using System;

[Serializable]
public class QuestionTemplate
{
    public string name;
    public string taskCode;
    public string Class;
}



[Serializable]
public class SentimentAnalysis : MultipleChoice
{
    public SentimentAnalysis(string questionID) : base(questionID)
    {
        Instructions = "Judge the sentiment/emotion of the sentence.";
        DropOptions.AddRange(new List<string> { "Negative", "Positive", "I don't know"});
        Initialize();
    }
    
}

public class NaturalLanguageInference : MultipleChoice
{
    public NaturalLanguageInference(string questionID) : base(questionID)
    {
        Instructions = "Based on the given sentence, judge how strongly the next sentence follows.";
        DropOptions.AddRange(new List<string> { "Follows", "Neutral", "Contradictory", "I don't know" });
        Initialize();
    }

}

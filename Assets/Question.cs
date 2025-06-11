using UnityEngine;

/// <summary>
/// A simple data structure to hold a question and its answer.
/// The [System.Serializable] attribute allows us to see and edit this in the Inspector.
/// </summary>
[System.Serializable]
public class Question
{
    public string questionText;
    public int correctAnswer;
}
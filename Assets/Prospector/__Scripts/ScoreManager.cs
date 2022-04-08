using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// An enum to handle all the possible scoring events 
public enum eScoreEvent
{
    draw,
    mine,
    mineGold,
    gameWin,
    gameLoss
}

// SocreManager handles all of the scoring
public class ScoreManager : MonoBehaviour
{
    static private ScoreManager S;

    static public int SCORE_FROM_PREV_ROUND = 0;
    static public int HIGH_SCORE = 0;

    [Header("Set Dynamically")]
    // Fields to track score info
    public int chain = 0;
    public int scoreRun = 0;
    public int score = 0;
    private void Awake()
    {
        if (S == null)
        {
            S = this; // set the private singleton
        }
        else
        {
            Debug.LogError("ERROR: ScoreManager.Awake(): S is already set!");
        }
        //check for a high score in PlayerPrefs
        if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
            HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
        }
        // Add the score from last round, which will be >0 if it was a win
        score += SCORE_FROM_PREV_ROUND;
        // And reset the SCORE_FROM_PREV_ROUND
        SCORE_FROM_PREV_ROUND = 0;
    }
    static public void EVENT(eScoreEvent evt)
    {
        try
        { // try-catch stops an error from breaking your program
            S.Event(evt);
        } catch(System.NullReferenceException nre)
        {
            Debug.LogError("ScoreManager:EVENT() called while S=null.\n" + nre);
        }
    }

    void Event(eScoreEvent evt)
    {
        switch (evt)
        {
            // Same things need to happen wheterh it's a draw, a win or a loss
            case eScoreEvent.draw: // Drawing a card 
            case eScoreEvent.gameWin:  // Won the round
            case eScoreEvent.gameLoss: // Lost the round
                chain = 0; // reset the score chain
                score += scoreRun; // add scoreRun to total score
                scoreRun = 0; // reset scoreRun
                break;

            case eScoreEvent.mine: // remove a mine card 
                chain++;
                score += scoreRun; // add scoreRun to total score
                break;
        }
        // This second switch statemet handles round wins and losses
        switch (evt)
        {
            case eScoreEvent.gameWin:
                // if its a win, add the score to the next round
                // static fields are NOT reset vy SceneManager.LoadScene()
                SCORE_FROM_PREV_ROUND = score;
                print("You won this round! Round score:" + score);
                break;

            case eScoreEvent.gameLoss:
                // If its a loss check against the high score
                if (HIGH_SCORE <= score)
                {
                    print("You got the high score!\n High score: " + score);
                    HIGH_SCORE = score;
                    PlayerPrefs.SetInt("ProspectorHighScore", score);
                } else
                {
                    print("Your final score for the game was: " + score);
                }
                break;

            default:
                print("score: " + score + " scoreRun:" + scoreRun + " chain" + chain);
                break;
        }
    }
        static public int CHAIN { get { return S.chain; } }
        static public int SCORE { get { return S.score; } }
        static public int SCORE_RUN { get { return S.scoreRun;} }
}

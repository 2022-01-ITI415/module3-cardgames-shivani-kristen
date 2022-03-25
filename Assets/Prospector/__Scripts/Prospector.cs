using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


// An enum to handle all the possible scoring events
public enum ScoreEvent {
	draw, 
	mine,
	mineGold,
	gameWin,
	GameLoss,
 }

public class Prospector : MonoBehaviour {

	static public Prospector 	S;
	static public int SCORE_FROM_PREV_ROUND = 0;
	static public int HIGH_SCORE = 0;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;


	[Header("Set Dynamically")]
	public Deck					deck;

	void Awake(){
		S = this;
		if (PlayerPrefs.HasKey("ProspectorHighScore"))
        {
			HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
        }
		// Add the score from last round, which will be >0 if it was a win 
		//Score += SCORE_FROM_PREV_ROUND;
		//And rest the SCORE_FROM_PREV_ROUND
		SCORE_FROM_PREV_ROUND = 0;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
	}

}

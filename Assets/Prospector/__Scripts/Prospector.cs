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
	public TextAsset deckXML;
	public TextAsset layoutXML;


	[Header("Set Dynamically")]
	public Deck	deck;
	public Layout layout;

	void Awake(){
		S = this;
	}
	void Start() {
		deck = GetComponent<Deck> (); // get the Deck 
		deck.InitDeck (deckXML.text); // pass DeckXML to it 
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>(); // Get the Layout component
		layout.ReadLayout(layoutXML.text); // Pass LayoutXML to it
	}

}
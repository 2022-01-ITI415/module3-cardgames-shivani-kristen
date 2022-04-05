using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


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
	public float xoffset = 3;
	public float yOffeset = -2.5f;
	public Vector3 layoutCenter;


	[Header("Set Dynamically")]
	public Deck	deck;
	public Layout layout;
	public List<CardProspector> drawPile;
	public Transform layoutAnchor;
	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;

	void Awake(){
		S = this;
	}
	void Start() {
		deck = GetComponent<Deck> (); // get the Deck 
		deck.InitDeck (deckXML.text); // pass DeckXML to it 
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>(); // Get the Layout component
		layout.ReadLayout(layoutXML.text); // Pass LayoutXML to it
		drawPile = ConvertListCardsToListCardProspectors(deck.cards);
		LayoutGame();
	}
     List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
    {
        List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach( Card tCD in lCD){
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
        }
		return(lCP);
    }
	// this Draw function will pull a single card from the drawPile and retuen it 
	CardProspector Draw()
    {
		CardProspector Draw()
        {
			CardProspector cd = drawPile[0]; // pull the 0th CardProspector 
			drawPile.RemoveAt(0);
			return (cd);
        }
    }
	// LayoutGame() positions the initial tableau of cards, a.k.a the "mine"
	void LayoutGame()
    {
		// Create an empty GameObject to serve as an anchor for the tableau
		if(layoutAnchor == null)
        {
			GameObject tGo = new GameObject("_layoutAnchor");
			// ^ Create an empty GameObject named _layoutAnchor in the Hierarchy
			layoutAnchor = tGo.transform;
			layoutAnchor.transform.position = layoutCenter;
        }

		CardProspector cp; 
		// Follow the layout
		foreach (SlotDef tSD in layout.slotDefs)
        {
			// ^ Iterate through all the SlotDefs in the layout.SlotDefs as tSD
			cp = Draw(); // Pull a card from the top (beginning) of the draw pile 
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor; //Make its parents layoutAnchor
												// This replaces the previous parent: deck.deckAnchor, which
												// apperas as _Deck in the Hierarchy when the scene is playing 
			cp.transform.localPosition = new Vector3(
				layout.multiplier.x * tSD.x,
				layout.multiplier.y * tSD.y,
				-tSD.layerID);
			// ^ Set the localPosition of the Card based on SlotDef
			cp.layoutID - tSD.id;
			cp.slotDef = tSD;
			// CardProspectors in the tableau have the state CardState.tableu 
			cp.state - eCardState.tableau;

			tableau.Add(cp); // Add this CardProspector to the List<> tableau

        }
    }
}
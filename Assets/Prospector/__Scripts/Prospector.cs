using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Prospector : MonoBehaviour {

	static public Prospector 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;
	public TextAsset			layoutXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;


	[Header("Set Dynamically")]
	public Deck					deck;
	public Layout					layout;
	public List<CardProspector> drawPile;
	public Transform layoutAnchor;
	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;

	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> ();
		deck.InitDeck (deckXML.text);
		Deck.Shuffle(ref deck.cards); // this shuffles the deck

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutXML.text);

		drawPile = ConvertListCardstoListCardProspectors(deck.cards);

		LayoutGame();
	}
	
	List<CardProspector> ConvertListCardstoListCardProspectors(List<Card> lCD) {
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach(Card tCD in lCD) {
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
		}
		return(lCP);
	}

	// the Draw function will pull a single card from the drawpile and return it
	CardProspector Draw()
	{
		CardProspector cd = drawPile[0]; // pull the 0th cardprospector
		drawPile.RemoveAt(0); // then remove it from list<> drawPile
		return(cd); // and return it
	}

	void LayoutGame()
	{
		// create an empty gameobject to serve as an anchor for the tableau
		if(layoutAnchor == null) {
			GameObject tGO = new GameObject("_LayoutAnchor"); // create an empty GameObject named _LayoutAnchor in the hierarchy
			layoutAnchor = tGO.transform; // grab its transform
			layoutAnchor.transform.position = layoutCenter; // position it
		}

		CardProspector cp;
		// follow the layout
		foreach(SlotDef tSD in layout.slotDefs) { // iterate through all the SLotDefs in the layout.slotDefs as tSD
			cp = Draw();
			cp.faceUp = tSD.faceUp; // set its faceup to the value in SlotDef
			cp.transform.parent = layoutAnchor; // make its parent layoutAnchor
			// this replaces the previous parent: deck.deckAnchour, which appears as _Deck in the hierarchy when the scene is playing
			cp.transform.localPosition = new Vector3(
				layout.multiplier.x * tSD.x,
				layout.multiplier.y * tSD.y,
				-tSD.layerID); // set the localPosition of the card based on slotDef
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			// CardProspectors in the tableau have the state CardState.tableau
			cp.state = eCardState.tableau;

			tableau.Add(cp); // add this CardProspector to the List<> tableau
		}
	}

}

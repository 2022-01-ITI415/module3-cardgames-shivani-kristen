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

public class Golf : MonoBehaviour {

	static public Prospector S;
	static public int SCORE_FROM_PREV_ROUND = 0;
	static public int HIGH_SCORE = 0;

	[Header("Set in Inspector")]
	public TextAsset deckXML;
	public TextAsset layoutXML;
	public float xoffset = 3;
	public float yoffeset = -2.5f;
	public Vector3 layoutCenter;
	public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
	public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
	public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
	public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
	public float reloadDelay = 2f; // 2 sec delay between rounds
	public Text gameOverText, roundResultText, highScoreText;



	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;
	public List<CardProspector> drawPile;
	public Transform layoutAnchor;
	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;
	public FloatingScore fsRun;
	private Text HighScoreText;
	private Text GameOverText;
	private Text RoundResultText;

	void Awake() {
		S = this;
        SetUpUITexts();
	}
	void SetUpUITexts()
    {
		// Set up the HighScore UI Text
		GameObject go = GameObject.Find("HighScore");
		if (go != null)
        {
			highScoreText = go.GetComponent<Text>();
        }
		int highScore = ScoreManager.HIGH_SCORE;
		string hScore = "High Score: " + Utils.AddCommasToNumber(highScore);

		//Set up the UI Texts that show at the end of the round
		go = GameObject.Find("GameOver");
		if (go != null)
        {
			gameOverText = go.GetComponent<Text>();
        }
		go = GameObject.Find("RoundResult");
		if(go != null)
        {
			roundResultText = go.GetComponent<Text>();
        }
		// Make the end cf round texts invisible
		ShowResultsUI(false);
    }
	void ShowResultsUI(bool show)
    {
		gameOverText.gameObject.SetActive(show);
		roundResultText.gameObject.SetActive(show);
    }
	void Start() {
		deck = GetComponent<Deck>(); // get the Deck 
		deck.InitDeck(deckXML.text); // pass DeckXML to it 
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>(); // Get the Layout component
		layout.ReadLayout(layoutXML.text); // Pass LayoutXML to it
		drawPile = ConvertListCardsToListCardProspectors(deck.cards);
		LayoutGame();
		Scoreboard.S.score = ScoreManager.SCORE;
		deck = GetComponent<Deck>(); // Get the Deck
	}
	List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
	{
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach (Card tCD in lCD) {
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
		}
		return (lCP);
	}
	// this Draw function will pull a single card from the drawPile and retuen it 
	CardProspector Draw()
	{
		CardProspector cd = drawPile[0]; // pull the 0th CardProspector 
		drawPile.RemoveAt(0);
		return (cd);
	}

	// LayoutGame() positions the initial tableau of cards, a.k.a the "mine"
	void LayoutGame()
	{
		// Create an empty GameObject to serve as an anchor for the tableau
		if (layoutAnchor == null)
		{
			GameObject tGo = new GameObject("_LayoutAnchor");
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
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			// CardProspectors in the tableau have the state CardState.tableu 
			cp.state = eCardState.tableau;
			// CardProspectors in the tableau have the state CardState.tableau
			cp.SetSortingLayerName(tSD.layerName); // Set the sorting layers

			tableau.Add(cp); // Add this CardProspector to the List<> tableau
		}
		// Set which cards are hiding others
		foreach(CardProspector tCP in tableau)
        {
			foreach(int hid in tCP.slotDef.hiddenBy)
            {
				cp = FindCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
            }
        }
		// set up the initail target card
		MoveToTarget(Draw());
		// Set up the Draw pile 
		UpdateDrawPile();
	}
	// Convert from the layoutID int to the CardProspector with that ID
	CardProspector FindCardByLayoutID(int layoutID)
    {
		foreach(CardProspector tCP in tableau)
        {
			// Search through all cards in the tableau List<>
			if (tCP.layoutID == layoutID)
            {
				// If the card has the same ID , return it 
				return (tCP);
            }
        }
		// If it's not found, return null
		return (null);
    }
	// This turns cards in the Mine face-up or face-down
	void SetTabeauFaces()
    {
		foreach (CardProspector cd in tableau)
        {
			bool faceUp = true; // Assume the card will be face-up
			foreach(CardProspector cover in cd.hiddenBy)
            {
				// If either of the covering cards are in the tableau
				if (cover.state == eCardState.tableau)
                {
					faceUp = false; // then this card is face-down
                }
            }
			cd.faceUp = faceUp; // set the value on the card
        }
    }
	//Moves the current target to the discardPile
	void MoveToDiscard(CardProspector cd)
	{
		// set the sate of the card to discard 
		cd.state = eCardState.discard;
		discardPile.Add(cd); // Add it to the discardPile List<>
		cd.transform.parent = layoutAnchor; // Updtae its transform parent 

		//Position this card on the discardPile
		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID + 0.5f);
		cd.faceUp = true;
		// Place it on top of the pile for depth sorting
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
	}
	//Make cd the new target card
	void MoveToTarget(CardProspector cd)
	{
		// If there is currently a target card, move it to discardpile
		if (target != null) MoveToDiscard(target);
		target = cd; // cd is new target 
		cd.state = eCardState.target;
		cd.transform.parent = layoutAnchor;
		// Move to the target positiion 
		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID);
		cd.faceUp = true; // Make iy face-up 
						  // Set the depth sorting 
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}
	// Arranges all the cards of the drawPile to show how many are left
	void UpdateDrawPile()
	{
		CardProspector cd;
		//Go through all the cards of the drawPile 
		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;
			// Position it correctly with the layout.drawPile.stagger
			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
				layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
				-layout.drawPile.layerID + 0.1f * 1);

			cd.faceUp = false; // Make them all face-down
			cd.state = eCardState.drawpile;
			//Set depth sorting
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10 * i);
		}
	}
	// CardCliked is called any time a card in the game is clicked
	public void CardClicked(CardProspector cd)
	{
		// The reaction is determined by the state of the clciked card 
		switch (cd.state)
		{
			case eCardState.target:
				// Clicking the target card does nothing 
				break;

			case eCardState.drawpile:
				// Clicking any card in the drawPile will draw the next card
				MoveToDiscard(target); // Moves the target to the discardpile
				MoveToTarget(Draw()); // Moves the next draw card to the target 
				UpdateDrawPile(); // restacks the drawpile
				ScoreManager.EVENT(eScoreEvent.draw);
                FloatingScoreHandler(eScoreEvent.draw);
				break;

			case eCardState.tableau:
				//Clicking a card in the tableau will check if it's a valid play
				bool validMatch = true;
				if (!cd.faceUp)
                {
					// If the card is face-down, its not valid 
					validMatch = false;
                }
				if (!AdjacentRank(cd, target))
                {
					// If it's not an adjacent rank, it's not valid
					validMatch = false;
                }
				if (!validMatch) return; // return if not valid 

				// If we got here, then : yay! It's a valid card.
				tableau.Remove(cd); // Remove it form tableau List 
				MoveToTarget(cd); // Make it the target card
				SetTabeauFaces(); // Update tableau card face-ups
				ScoreManager.EVENT(eScoreEvent.mine);
				FloatingScoreHandler(eScoreEvent.mine);
				break;
		}
		// Check to see whether the game is over or not
		CheckForGameOver();
	}
	// Test whether the is over 
	void CheckForGameOver()
    {
		// if the tableau is empty , the game is over
		if (tableau.Count == 0)
        {
			// Call GameOver() with a win 
			GameOver(true);
			return;
        }
        // If there are still cards in the draw pile, the game's not over 
        if (drawPile.Count > 0)
        {
			return;
        }
		// Check for remaining vaild plays
		foreach (CardProspector cd in tableau)
        {
			if (AdjacentRank(cd, target))
            {
				// If there is a valid play, the game's not over
				return;
            }
        }
		// since there are no valid plays, the game is over
		// call GameOver with a loss
		GameOver(false);
	}
	// Called when the game is over.Simple for now, but expandable
	void GameOver(bool won)
    {
		int score = ScoreManager.SCORE;
		if (fsRun != null) score += fsRun.score;
		if (won) {
			gameOverText.text = "Round Over";
			roundResultText.text = "you won this round!\nRound Score: " + score;
			ShowResultsUI(true);
			// print("Game Over. You Won! :)");
			ScoreManager.EVENT(eScoreEvent.gameWin);
			FloatingScoreHandler(eScoreEvent.gameWin);
        } else {
			gameOverText.text = "Game Over;";
			if (ScoreManager.HIGH_SCORE <= score)
            {
				string str = "You got the high score!nHigh score: " + score;
				roundResultText.text = str;
            }
            else
            {
				roundResultText.text = "Your final score was: " + score;
            }
			ShowResultsUI(true);
			//print("Game Over. You Lost.(Hehehe) :(");
			ScoreManager.EVENT(eScoreEvent.gameLoss);
			FloatingScoreHandler(eScoreEvent.gameLoss);
		}
		// Reload the screne, resetting the game
		// SceneManager.LoadScene("__Prospector_Scence_0"); //

		// Reload the screne in reloadDelay seconds
		// This will give score a moment to travel
		Invoke("ReloadLevel", reloadDelay);
	}
	void ReloadLevel()
    {
		// reload the scene, resrtting the game 
		SceneManager.LoadScene("__Prospector_Scene_0");
    }
	// Return true if the two cards are adjacent in the rank (A & K wrap around)
	public bool AdjacentRank(CardProspector c0, CardProspector c1)
    {
		// If either card is face-down, it's not adjacent.
		if (!c0.faceUp || !c1.faceUp) return (false);

		// If they are 1 apart, they are adjacent
		if (c0.rank == 1 && c1.rank == 13) return (true);
		if (c0.rank == 13 && c1.rank == 1) return (true);
		if (Mathf.Abs(c0.rank - c1.rank) == 1) return (true);

		// otherwise, return fasle
		return (false);
    }
	// Handle FloatingScore movement 
	void FloatingScoreHandler(eScoreEvent evt)
    {
		List<Vector2> fsPts;
		switch (evt)
        {
			// Same things need to happen whether it's a draw a win, or a loss
			case eScoreEvent.draw: // Drawing a card 
			case eScoreEvent.gameWin: // won the round
            case eScoreEvent.gameLoss: // loss the round 
				// Add fsRun to the Scoreboard score
				if (fsRun != null)
                {
					// create pointsfor the Bezier curve 
					fsPts = new List<Vector2>();
					fsPts.Add(fsPosRun);
					fsPts.Add(fsPosMid2);
					fsPts.Add(fsPosEnd);
					fsRun.reportFinishTo = Scoreboard.S.gameObject;
					fsRun.Init(fsPts, 0, 1);
					// Also adjust the fontSize
					fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
					fsRun = null; // Clear fsRun so it's created again
                }
				break;
			case eScoreEvent.mine: // Remove a mine card
								   // Create a FloatingScore for this score
				FloatingScore fs;
				// Move it from the mousePosition to fsPosRun
				Vector2 p0 = Input.mousePosition;
				p0.x /= Screen.width;
				p0.y /= Screen.height;
				fsPts = new List<Vector2>();
				fsPts.Add(fsPosMid);
				fsPts.Add(fsPosRun);
				fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
				fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
				if(fsRun == null)
                {
					fsRun = fs;
					fsRun.reportFinishTo = null;
                }
                else
                {
					fs.reportFinishTo = fsRun.gameObject;
                }
				break;
        }
    }
}
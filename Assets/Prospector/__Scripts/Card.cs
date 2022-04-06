using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour {

	public string suit;
	public int rank;
	public Color color = Color.black;
	public string colS = "Black";  // or "Red"

	public List<GameObject> decoGOs = new List<GameObject>();
	public List<GameObject> pipGOs = new List<GameObject>();

	public GameObject back;  // back of card;
	public CardDefinition def;  // from DeckXML.xml	


	//List of the SpriteRenderer Components of this GameObject and its children
	public SpriteRenderer[] spriteRenderers;

	void Start()
	{
		SetSortOrder(0); // Ensure that the card state properly depth sorted 
	}

	// If spriteRenderes is not yet defined, this function defines it 
	public void PopulateSpriteRenderers()
	{
		// if spriteRenderers is null or empty
		if (spriteRenderers == null || spriteRenderers.Length == 0)
		{
			//Get SpriteRenderer Components of this GameObject and its childern
			spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		}
	}
	//Sets the sortingLayerName of all SpriteRenderer Components
	public void SetSortingLayerName (string tSLN)
    {
		PopulateSpriteRenderers();
		foreach (SpriteRenderer tSR in spriteRenderers)
		{
			tSR.sortingLayerName = tSLN;
		}
	}
	
	//Sets the sortingOrder of all SpriteRenderer Components
	public void SetSortOrder(int sOrd)
    {
		PopulateSpriteRenderers();

		//Iterate throught all the spriteRenderers as tSR
		foreach(SpriteRenderer tSR in spriteRenderers)
        {
			if(tSR.gameObject == this.gameObject)
            {
				// If the GameObject is this.gameObject,it's the background
				tSR.sortingOrder = sOrd; //set its to sOrd
				continue; //Add continue to the next iteration of the loop
            }
		// Each of the childern of this GameObject are Named 
		//switch based onthe names
		switch (tSR.gameObject.name)
            {
			case "back":// if the name is "back"
							//Set it to the highest layer to cover the other sprites
				tSR.sortingOrder = sOrd + 2;
				break;
			case "face": //if the name is "face"
				default: // or if it's anthing else
				// set it to the middle layer to be above the backgroud
				tSR.sortingOrder = sOrd + 1;
				break;

            }
        }
    }
	// Sets the sortingLayerName on all SpriteRenderer Components
	
	public bool faceUp {
		get {
			return (!back.activeSelf);
		}

		set {
			back.SetActive(!value);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
} // class Card

[System.Serializable]
public class Decorator{
	public string	type;			// For card pips, tyhpe = "pip"
	public Vector3	loc;			// location of sprite on the card
	public bool		flip = false;	//whether to flip vertically
	public float 	scale = 1.0f;
}

[System.Serializable]
public class CardDefinition{
	public string	face;	//sprite to use for face cart
	public int		rank;	// value from 1-13 (Ace-King)
	public List<Decorator>	
					pips = new List<Decorator>();  // Pips Used
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An enum defines a varibale type with a few prenamed values
public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}
public class CardGolf: Card
{ // Make sure CardProspector extends Card 
    [Header("Set Dynamically : CardGolf")]
    // This is how you use enum eCardState
    public eCardState state = eCardState.drawpile;
    // The hiddenBy list stores which other cards will keep this one face down 
    public List<CardGolf> hiddenBy = new List<CardGolf>();
    // the layoutID matches this card to the tableau XML if it's a tableau card
    public int layoutID;
    // The SlotDef class stores info pulled in from LayoutXML <slot>
    public SlotDef slotDef;
    //This allows the card to react to being clciked 
    public override void OnMouseUpAsButton()
    {
        // Call the CardClicked method on the Prospector singleton
        Prospector.S.CardClicked(this);
        // Also call the base class (Card.cs)version of this method
        base.OnMouseUpAsButton();
    }
}
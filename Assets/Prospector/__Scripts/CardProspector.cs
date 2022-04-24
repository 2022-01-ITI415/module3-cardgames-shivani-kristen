using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an enum defines a variable type with a few prenamed values
public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card
{
    [Header("Set Dynamically: CardProspector")]
    // this is how you use the enum eCardState
    public eCardState state = eCardState.drawpile;
    // the hiddenBy list stores which other cards will keep this one face down
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    // the layoutID matches this card to the tableau xml if it's a tableau card
    public int layoutID;
    // the SlotDef class stores info pulled from the LayoutXML <slot>
    public SlotDef slotDef;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The SlotDef class is not a subclass of MomoBehavior , so it doesn't need 
// a separate c# file.
[System.Serializable] // make SlotDefs visible in Unity Inspector pane 
public class SlotDef {
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}

public class Layout : MonoBehaviour
{
    public PT_XMLReader xmlr; // like Deck has a PT_XMLReader 
    public PT_XMLHashtable xml; // variable for faster xml access 
    public Vector2 multiplier; //offset of the tableau's center 
    // SlotDef references 
    public List<SlotDef> slotDefs; //all the SlotDefs for Row0-Row3
    public SlotDef drawPile;
    public SlotDef discardPile;
    // This holds all of the possible names for the layers set by layerID
    public string[] sortingLayerName = new string[] {"Row0","Row1",
                        "Row2","Row3","Discard","Draw"};
// This function is called to read in the LayoutXML.xml file
public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);   // The XMl is parsed 
        xml = xmlr.xml["xml"][0];  // And xml is set as a shortcut to the XMl
    }
}

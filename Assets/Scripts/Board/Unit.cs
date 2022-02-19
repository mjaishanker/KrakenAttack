using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
    // Dictonary to find mineLines and see if they are alive
    [SerializeField]
    public Dictionary<string, MineLine> tileLines = new Dictionary<string, MineLine>();
    public bool unitDestroyed = false;

    public bool checkUnitTilesStatus()
    {
        //Debug.Log("Tile Lines in Unit Class: " + tileLines.Count);
        var keys = tileLines.Where(item => item.Value.tileLineDestroyed.Equals(true)).Select(item => item.Key);

        //Debug.Log("CheckUnitTilesStatus: Key count: " + keys.Count());

        //foreach (var k in keys)
        //{
        //    Debug.Log("Unit Keys: " + k);
        //}

        if (keys.Count() == tileLines.Count)
        {
            //Debug.Log("KEY COUNT IS EQUAL TO TILE COUNT, SO DELETE UNIT!");
            unitDestroyed = true;
        }
        else
        {
            //Debug.Log("KEY COUNT IS NOT EQUAL TO TILE COUNT!");
            unitDestroyed = false;
        }


        //foreach (KeyValuePair<string, MineLine> kvp in tileLines)
        //{
        //    //Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value);

        //    if (kvp.Value.tileLineDestroyed == true)
        //        unitDestroyed = true;
        //    else
        //        unitDestroyed = false;

        //}

        return unitDestroyed;
    }

    public void clearTileLines()
    {
        tileLines.Clear();
    }
}

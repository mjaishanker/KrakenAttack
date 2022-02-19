using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // 0 = empty, 1 = ship, 2= hit, 3=miss,
    public Dictionary<(int, int), int> player1WorldBoard = new Dictionary<(int, int), int>();
    public Dictionary<(int, int), int> player2WorldBoard = new Dictionary<(int, int), int>();

    public Dictionary<(int, int), MineTile> newPlayer1WorldBoard = new Dictionary<(int, int), MineTile>();
    public Dictionary<(int, int), MineTile> newPlayer2WorldBoard = new Dictionary<(int, int), MineTile>();

    // Start is called before the first frame update
    void Start()
    {

        //foreach (KeyValuePair<(int, int), int> kvp in player1WorldBoard)
        //    Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value);

    }

    private void Update()
    {
        //if (Input.GetButtonDown("CheckBoardLoc"))
        //{
        //    foreach (KeyValuePair<(int, int), int> kvp in player1WorldBoard)
        //        Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value);
        //}
        //if (Input.GetButtonDown("CheckP2BoardLoc"))
        //{
        //    foreach (KeyValuePair<(int, int), int> kvp in player2WorldBoard)
        //        Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value);
        //}
        if (Input.GetButtonDown("CheckBoardLoc"))
        {
            foreach (KeyValuePair<(int, int), MineTile> kvp in newPlayer1WorldBoard)
                Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value.emptyOrOccupied);
        }
        if (Input.GetButtonDown("CheckP2BoardLoc"))
        {
            foreach (KeyValuePair<(int, int), MineTile> kvp in newPlayer2WorldBoard)
                Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value.emptyOrOccupied);
        }
        if (Input.GetButtonDown("FreeNodesMapButton"))
        {
            Debug.Log("Number of free nodes: " + gameObject.GetComponentInChildren<BattleSystem>().availableNodes.Count);
            foreach (var k in gameObject.GetComponentInChildren<BattleSystem>().availableNodes)
            {
                Debug.Log("Key: " + k);
            }
            //Debug.Log("Checking the Updated Board!!!");
            //foreach (KeyValuePair<(int, int), int> kvp in gameObject.GetComponentInChildren<BattleSystem>().freeNodeLocations)
            //{
            //    Debug.Log("Free Node Locations");
            //    Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value);

            //}
        }
    }
}

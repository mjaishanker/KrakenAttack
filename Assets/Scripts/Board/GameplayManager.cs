using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public enum GameState { minePlacement, Battle }

    public GameState state;

    public Dictionary<string, GameObject> player1Battleships = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> player2Battleships = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        state = GameState.minePlacement;
        if (state == GameState.minePlacement)
        {
            this.gameObject.transform.GetComponent<MineLineSpawner>().enabled = true;

        }



    }

}

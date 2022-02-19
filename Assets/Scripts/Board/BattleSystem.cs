using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattleState { Start, Player1Turn, KrakenTurn, Won, Lost }

public class BattleSystem : MonoBehaviour
{

    public GameObject player1;
    [SerializeField]
    private GameObject krakenObject, krakenMissed;

    [SerializeField]
    private ParticleSystem krakenSplash, shipExplosion, krakenHurt;

    // Prefabs of all the mine lines
    [SerializeField]
    public List<GameObject> player2MineLines = new List<GameObject>();

    public Dictionary<(int, int), int> freeNodeLocations = new Dictionary<(int, int), int>();

    // New attempt
    //public List<(int, int)> availableNodes = new List<(int, int)>();
    public Dictionary<(int, int), int> availableNodes = new Dictionary<(int, int), int>();

    public BattleState state;

    public bool allPlayer2MinesPlaced = false;
    private int localCruiserPosX, localCruiserPosZ, rotationFlip;


    // Player turn variables
    public bool playerClicked = false;

    // Kraken's Lines
    [SerializeField]
    public Dialogue krakenDialogue;

    [SerializeField]
    public Dialogue newQMDialogue;

    [SerializeField]
    public DialogueManager DM;

    [SerializeField]
    public Image shipInfoImage;

    public TextMeshProUGUI text;
    public TMP_FontAsset m_Font;

    public TMP_Text gameBeginText;
    public Animator gameStateAnimator;
    public Button restartGameButton;

    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager.Play("The Kraken");
        state = BattleState.Start;
        StartCoroutine(SetupBattle());
    
    }

    // Instantiate Enemy Mines
    IEnumerator SetupBattle()
    {

        // Set Up player UI
        foreach (var ships in player1.GetComponent<Unit>().tileLines)
        {
            GameObject textGO = new GameObject();
            textGO.transform.parent = shipInfoImage.transform;
            textGO.AddComponent<TextMeshProUGUI>();
            textGO.transform.name = ships.Value.mineLineName + "Info";

            // Set Text component properties.
            text = textGO.GetComponent<TextMeshProUGUI>();
            text.SetText(ships.Value.mineLineName + " : " + ships.Value.gameObject.transform.position.x / 2 + ", " + ships.Value.gameObject.transform.position.z / 2);
            text.font = m_Font;
            text.color = Color.black;
            text.fontSize = 15;
            text.alignment = TextAlignmentOptions.Center;

            RectTransform rectTransform;
            rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(114, 15);
        }

        // Reset the board and its free nodes to 0, so 64 available nodes
        resetFreeNodes();

        // Instatiate the Kraken Tiles first
        List<GameObject> krakenTilesInstatiated = new List<GameObject>();
        for (int p = 0; p < player2MineLines.Count; p++)
        {
            krakenTilesInstatiated.Add(instantiateKrakenTile(player2MineLines[p]));
            //Debug.Log("Before Select RANDOME PLAYER 2 POS: " + krakenTilesInstatiated[p].name + " Pos: " + krakenTilesInstatiated[p].transform.position);
        }

        for (int p = 0; p < krakenTilesInstatiated.Count; p++)
        {
            selectRandomPlayer2Loc(krakenTilesInstatiated[p]);
        }

        yield return new WaitForSeconds(2f);


        Debug.LogWarning("Checking Player Ship status");
        // Player's ships have all been destroyed before the game even began
        if (player1.GetComponent<Unit>().checkUnitTilesStatus())
        {
            Debug.Log("Kraken destroyed all you ships!");

            foreach (Transform child in shipInfoImage.transform)
            {
                child.transform.GetComponent<TextMeshProUGUI>().SetText("Sunk");
                child.transform.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            newQMDialogue.listSentences.Clear();
            newQMDialogue.listSentences.Add("The Kraken has destroyed all our ships! It's O'er...");
            DM.StartDialogue(newQMDialogue);

            state = BattleState.Lost;
            EndBattle();
        }
        else
        {
            Debug.Log("Attack the Kraken!");

            state = BattleState.Player1Turn;
            player1Turn();
        }
    }

    public void selectRandomPlayer2Loc(GameObject player2MineLineToPlace)
    {

        //Debug.LogWarning("SELECTING RANDOM LOCATIONS FOR KRAKEN TILE LINE: " + player2MineLineToPlace.transform.name);

        // Second try
        //Debug.Log("Number of free nodes: " + availableNodes.Count());

        //foreach (var k in availableNodes)
        //{
        //    Debug.Log("Tile Line: " + player2MineLineToPlace.name + " : node: " + k);
        //}

        int randomElementPick = Random.Range(0, availableNodes.Count());
        //Debug.Log("Random Number picked: " + randomElementPick);

        //Debug.Log("Random XY Coordinate: " + availableNodes.ElementAt((randomElementPick))); //availableNodes[randomElementPick]);// availableNodes.ElementAt(randomElementPick));

        // Spawn head tile at the random element of keyes picked
        //Debug.Log("X rand Elem Pick: " + availableNodes.ElementAt(randomElementPick).Key.Item1);
        //Debug.Log("Y rand Elem Pick: " + availableNodes.ElementAt(randomElementPick).Key.Item2);

        int headTileXPos = availableNodes.ElementAt(randomElementPick).Key.Item1;
        int headTileYPos = availableNodes.ElementAt(randomElementPick).Key.Item2;
        // Second try

        //var keys = freeNodeLocations.Where(item => item.Value.Equals(0)).Select(item => item.Key);



        //Debug.Log("HeadTileXPos: " + headTileXPos + " headTileYPos: " + headTileYPos);

        //freeNodeLocations[(headTileXPos, headTileYPos)] = 1;

        //int tempCheck;
        //freeNodeLocations.TryGetValue((headTileXPos, headTileYPos), out tempCheck);

        rotationFlip = Random.Range(1, 3);

        // Spawn the object
        int localLinePosX = headTileXPos * 2;
        int localLinePosZ = headTileYPos * 2;

        //Debug.Log("Before: " + player2MineLineToPlace.transform.position);

        // Horizontal
        if (rotationFlip == 1)
        {
            //Debug.Log("Rotation: " + rotationFlip);
            player2MineLineToPlace.transform.position = new Vector3(localLinePosX, 42, localLinePosZ);
            player2MineLineToPlace.transform.rotation = Quaternion.identity;
            player2MineLineToPlace.transform.Rotate(0.0f, 0.0f, 0.0f);
        }
        // Vertical
        else if (rotationFlip == 2)
        {
            //Debug.Log("Rotation: " + rotationFlip);
            player2MineLineToPlace.transform.position = new Vector3(localLinePosX, 42, localLinePosZ);
            player2MineLineToPlace.transform.rotation = Quaternion.identity;
            player2MineLineToPlace.transform.Rotate(0.0f, 90.0f, 0.0f);
        }

        //Debug.Log("After: " + player2MineLineToPlace.transform.position);

        // Add each tile to the kraken world board
        foreach (Transform child in player2MineLineToPlace.transform)
        {
            //Debug.Log("Kraken Tile: " + child.name + " Position: " + child.position);

            // Offset world tile location to match the board data structure location for updating
            int tempChildBoardX;
            int tempChildBoardZ;

            if (child.position.x == 0 && child.position.z == 0)
            {
                tempChildBoardX = 0;
                tempChildBoardZ = 0;
            }
            else if (child.position.x == 0)
            {
                tempChildBoardX = 0;
                tempChildBoardZ = Mathf.CeilToInt(child.position.z / 2);
            }
            else if (child.position.z == 0)
            {
                tempChildBoardX = Mathf.CeilToInt(child.position.x / 2);
                tempChildBoardZ = 0;
            }
            else
            {
                tempChildBoardX = Mathf.CeilToInt(child.position.x / 2);
                tempChildBoardZ = Mathf.CeilToInt(child.position.z / 2);
            }

            child.GetComponent<MineTile>().offsetTileLocationX = tempChildBoardX;
            child.GetComponent<MineTile>().offsetTileLocationZ = tempChildBoardZ;
            child.GetComponent<MineTile>().emptyOrOccupied = 1;


            // Assigning the tiles of the tile line to the world board dictionary
            this.gameObject.GetComponentInParent<Board>().newPlayer2WorldBoard[(tempChildBoardX, tempChildBoardZ)] = child.GetComponent<MineTile>();

        }

        foreach (Transform child in player2MineLineToPlace.transform)
        {
            // Set the child head position and its neighbors to 1 in the free nodes dictionary
            removeAvailableNodes(child.GetComponent<MineTile>().offsetTileLocationX, child.GetComponent<MineTile>().offsetTileLocationZ);
        }

        // Check if the placed kraken tile has colided with a player's ship
        checkKrakenShipCollsion(player2MineLineToPlace);


    }

    public GameObject instantiateKrakenTile(GameObject krakenTileToSpawn)
    {
        GameObject krakenTileLine = null;
        if (krakenTileToSpawn != null)
        {
            krakenTileLine = Instantiate(krakenTileToSpawn, new Vector3(100, 42, 100), Quaternion.Euler(0, 0, 0));

            foreach(Transform child in krakenTileLine.transform)
            {
                child.GetComponent<MineTile>().setOGLocalPos();
            }
            // Add Kraken Tile Line to kraken unit
            krakenObject.GetComponent<Unit>().tileLines.Add(krakenTileLine.GetComponent<MineLine>().mineLineName, krakenTileLine.GetComponent<MineLine>());
        }
        return krakenTileLine;
    }

    // public void checkKrakenShipCollsion(GameObject krakenTileLine)
    public void checkKrakenShipCollsion(GameObject krakenTileLine)
    {
        foreach (Transform child in krakenTileLine.transform)
        {
            //Debug.Log("Kraken Tile: " + child.name + " Position: " + child.position);

            // Offset world tile location to match the board data structure location for updating
            int tempChildBoardX = child.GetComponent<MineTile>().offsetTileLocationX;
            int tempChildBoardZ = child.GetComponent<MineTile>().offsetTileLocationZ;

            // When placing the kraken tiles, check if the tiles are colliding with any of players tiles

            MineTile tempTile;

            // Look for tile state, if the tile is found at this x,z location that means theres a ship time where a kraken tile is
            this.gameObject.GetComponentInParent<Board>().newPlayer1WorldBoard.TryGetValue((tempChildBoardX, tempChildBoardZ), out tempTile);
            //Debug.Log("2nd_tempChildX: " + tempChildBoardX + " 2nd_tempChildZ: " + tempChildBoardZ);

            // The hit ship tile is reterived for the dictionary and the ship object is active meaning it was not hit before 
            if (tempTile != null && tempTile.transform.parent.gameObject.activeSelf)
            {
                Debug.Log("Hit Ship Tile: " + tempTile);
                //Debug.Log("TempChildBoardX: " + tempChildBoardX + " TempChildBoardZ: " + tempChildBoardZ);
                //Debug.Log("tempTileOffsetX: " + tempTile.offsetTileLocationX + " tempTileOffsetZ: " + tempTile.offsetTileLocationZ);
                if (tempChildBoardX == tempTile.offsetTileLocationX && tempChildBoardZ == tempTile.offsetTileLocationZ)
                {
                    Debug.Log("The Kraken has hit the Ship!!!" + tempTile.transform.GetComponentInParent<MineLine>().mineLineName);

                    // Update the ship info on UI
                    string shipName = tempTile.transform.GetComponentInParent<MineLine>().mineLineName.ToString();
                    shipName = shipName + "Info";
                    GameObject child1 = shipInfoImage.transform.Find(shipName).gameObject;

                    child1.transform.GetComponent<TextMeshProUGUI>().SetText("Sunk" +
                        " : " + tempTile.gameObject.GetComponentInParent<MineLine>().gameObject.transform.position.x / 2 + ", " +
                        tempTile.gameObject.GetComponentInParent<MineLine>().gameObject.transform.position.z / 2);
                    child1.transform.GetComponent<TextMeshProUGUI>().color = Color.red;

                    tempTile.isHit = true;
                    tempTile.gameObject.GetComponentInParent<MineLine>().tileLineDestroyed = true;

                    // Play Ship Explosion Effect
                    //ParticleSystem tempShipExplosion = Instantiate(shipExplosion, new Vector3(tempChildBoardX * 2, 1, tempChildBoardZ * 2), Quaternion.Euler(-90, 0, 0));
                    //Destroy(tempShipExplosion);
                    spawnAndDestroyParticle(shipExplosion, tempChildBoardX, tempChildBoardZ);

                    tempTile.gameObject.transform.parent.gameObject.SetActive(false);

                    // Set the player1 world board location to empty
                    this.gameObject.GetComponentInParent<Board>().newPlayer1WorldBoard.Remove((tempChildBoardX, tempChildBoardZ));

                    newQMDialogue.listSentences.Clear();
                    newQMDialogue.listSentences.Add("The kraken has destroyed one o' our ships!!!");
                    DM.StartDialogue(newQMDialogue);
                    //StartCoroutine(completeQMDialogue("The kraken has destroyed one o' our ships!!!"));

                    // brak out of the foreach loop to iterate through the children
                    break;
                }
            }
            // The kraken missed the player
            else
            {
                //Debug.LogWarning("The Kraken missed the player!!!");
                //ParticleSystem tempKrakenMiss = Instantiate(krakenSplash, new Vector3(tempChildBoardX * 2, 0, tempChildBoardZ * 2), Quaternion.Euler(-90, 0, 0));
                //Destroy(tempKrakenMiss);
                spawnAndDestroyParticle(krakenSplash, tempChildBoardX, tempChildBoardZ);
            }
        }
    }


    public void player1Turn()
    {
        if (state != BattleState.Player1Turn)
            return;

        StartCoroutine(WaitForPlayer());


    }

    IEnumerator completeQMDialogue(string sentence)
    {
        newQMDialogue.listSentences.Clear();
        newQMDialogue.listSentences.Add(sentence);
        DM.StartDialogue(newQMDialogue);
        while (true)
        {
            if (DM.startDialogue == false)
            {
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator completeKrakenDialogue(string sentence)
    {
        krakenDialogue.listSentences.Clear();
        krakenDialogue.listSentences.Add(sentence);
        DM.StartDialogue(krakenDialogue);
        while (true)
        {
            if (DM.startDialogue == false)
            {
                yield break;
            }
            yield return null;
        }
    }

    // Coroutine that acts like an update function
    IEnumerator WaitForPlayer()
    {
        audioManager.Play("PlayerTurn");
        yield return StartCoroutine(completeQMDialogue("Now's our chance!! Attack The kraken!!"));

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Player is clicking...");
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.transform.gameObject.tag == "Player1BoardNode")
                    {
                        // Check player select location
                        int tempHitLocX = (int)hit.transform.position.x / 2;
                        int tempHitLocZ = (int)hit.transform.position.z / 2;

                        Debug.Log("I hit a node" + hit.transform.position + " : " + tempHitLocX + ", " + tempHitLocZ);

                        StartCoroutine(OnPlayerClickBoard(tempHitLocX, tempHitLocZ));

                        yield break;
                    }
                }
            }
            yield return null;
        }

    }

    // public void OnPlayerClickBoard(int tempHitLocX, int tempHitLocZ)

    IEnumerator OnPlayerClickBoard(int tempHitLocX, int tempHitLocZ)
    {
        if (state != BattleState.Player1Turn)
            yield return null;

        Debug.LogWarning("Player Turn");

        MineTile tempTile;

        // Look for tile state, if the tile is found at this x,z location that means theres a ship time where a kraken tile is
        this.gameObject.GetComponentInParent<Board>().newPlayer2WorldBoard.TryGetValue((tempHitLocX, tempHitLocZ), out tempTile);
        //Debug.Log("tempChildX: " + tempChildBoardX + "tempChildZ: " + tempChildBoardZ);

        Debug.LogWarning("PLayer clicked Tile: " + tempTile);

        if (tempTile != null)
        {
            //Debug.Log("tempTileOffsetX: " + tempTile.offsetTileLocationX + " tempTileOffsetZ: " + tempTile.offsetTileLocationZ);
            if (tempHitLocX == tempTile.offsetTileLocationX && tempHitLocZ == tempTile.offsetTileLocationZ)
            {
                //Debug.Log("The Player has hit a Kraken Tile!!! : Tile Name: " + tempTile.gameObject.name );

                //newQMDialogue.listSentences.Clear();
                //newQMDialogue.listSentences.Add("We hit one o' her tentacles!!! Keep up the pressure!");
                //DM.StartDialogue(newQMDialogue);
                yield return StartCoroutine(completeQMDialogue("We hit one o' her tentacles!!! Keep up the pressure!"));

                tempTile.isHit = true;

                
                // Set the kraken tile line not about to move. ie the kraken tentecle is harpooned
                tempTile.GetComponentInParent<MineLine>().tileLineAbleMove = false;

                // Reveal the hit kraken tile to the player
                // Play the blood splatter effect
                //ParticleSystem tempKrakenHurt = Instantiate(krakenHurt, new Vector3(tempHitLocX * 2, 1, tempHitLocZ * 2), Quaternion.Euler(-90, 0, 0));
                //Destroy(tempKrakenHurt);
                spawnAndDestroyParticle(krakenHurt, tempHitLocX, tempHitLocZ);

                tempTile.gameObject.transform.position = new Vector3(tempHitLocX * 2, 0, tempHitLocZ * 2);

                Animation anim = tempTile.gameObject.transform.GetChild(0).GetComponent<Animation>();
                if(anim != null)
                {
                    anim["KrakenHit"].wrapMode = WrapMode.Once;
                    anim.Play("KrakenHit");
                    anim.Play("KrakenIdleAction");
                }


                //if (krakenObject.GetComponent<Unit>().checkUnitTilesStatus())
                //{
                //    state = BattleState.Won;
                //    EndBattle();
                //}

                if (tempTile.gameObject.GetComponentInParent<MineLine>().checkLineTilesStatus())
                {
                    //Debug.Log("It says all the tiles are destroyed!!!");
                    tempTile.gameObject.GetComponentInParent<MineLine>().tileLineDestroyed = true;
                    tempTile.gameObject.GetComponentInParent<MineLine>().playTentacleDeathAnimation();
                }
                state = BattleState.KrakenTurn;
            }
        }
        else
        {
            Debug.Log("The Player missed...");

            // Since the player did not keep harpooning, free kraken tentacles from harpoon

            // Go through each tile lines to check which can't move since the player missed on his turn
            foreach (KeyValuePair<string, MineLine> kvp in krakenObject.GetComponent<Unit>().tileLines)
            {
                // Tile line is not able to move
                if (kvp.Value.tileLineAbleMove == false && kvp.Value.tileLineDestroyed != true)
                {
                    kvp.Value.unHookHarpoon();
                    kvp.Value.tileLineAbleMove = true;
                }

            }

            yield return StartCoroutine(completeQMDialogue("We missed the target! The Kraken freed herself from the harpoon!!! Keep lookin' fer The Kraken!"));

            state = BattleState.KrakenTurn;
        }
        state = BattleState.KrakenTurn;
        StartCoroutine(KrakenTurn(tempTile));

        yield return null;
    }


    IEnumerator KrakenTurn(MineTile tempTile)
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Kraken's Turn");

        yield return StartCoroutine(completeKrakenDialogue("AOARRRRR!!!!"));

        //krakenDialogue.listSentences.Clear();
        //krakenDialogue.listSentences.Add("AOARRRRR!!!!");
        //DM.StartDialogue(krakenDialogue);

        //Debug.LogWarning("Checking Kraken Tile status");
        // checking if the Kraken unit is destroyed
        if (krakenObject.GetComponent<Unit>().checkUnitTilesStatus())
        {
            state = BattleState.Won;
            EndBattle();
        }
        else
        {
            // List of all the kraken Tiles that are not harpooned
            List<MineLine> krakenTilesToMove = new List<MineLine>();

            // List of all the kraken tiles that are harpooned
            List<MineLine> krakenTilesNotMove = new List<MineLine>();

            // Go through each tile lines to check which can move and whcih can't and add them to a list of objects that can or can't move
            foreach (KeyValuePair<string, MineLine> kvp in krakenObject.GetComponent<Unit>().tileLines)
            {
                //Debug.Log("Key = {0}, Value = {1}" + kvp.Key + " " + kvp.Value);
                
                // Tile line is able to move
                if (kvp.Value.tileLineAbleMove == true)
                {
                    krakenTilesToMove.Add(kvp.Value);
                }

                // tile line is not able to move
                else if (kvp.Value.tileLineAbleMove == false)
                {
                    krakenTilesNotMove.Add(kvp.Value);
                }

            }

            // For all the kraken tiles that can still move, add those tiles and its neighbors back into the free nodes
            foreach (MineLine ml in krakenTilesToMove)
            {
                foreach (Transform child in ml.gameObject.transform)
                {
                    // Set the child head position and its neighbors to 0 in the free nodes dictionary
                    addAvailableNodes(child.GetComponent<MineTile>().offsetTileLocationX, child.GetComponent<MineTile>().offsetTileLocationZ);
                }

            }

            // Remove all the harpooned kraken tiles nodes and its neighbors from the free list (POTENTIAL FIX FOR THE EDGE CASES)
            foreach (MineLine ml in krakenTilesNotMove)
            {
                foreach (Transform child in ml.gameObject.transform)
                {
                    // Set the child head position and its neighbors to 1 in the free nodes dictionary
                    removeAvailableNodes(child.GetComponent<MineTile>().offsetTileLocationX, child.GetComponent<MineTile>().offsetTileLocationZ);
                }

            }


            foreach (MineLine ml in krakenTilesToMove)
            {
                //updateKrakenFreeNodes(ml.gameObject);
                selectRandomPlayer2Loc(ml.gameObject);
            }

            //Debug.LogWarning("Checking Player Ship status");
            // Player's ships have all been destroyed
            if (player1.GetComponent<Unit>().checkUnitTilesStatus())
            {
                //newQMDialogue.listSentences.Clear();
                //newQMDialogue.listSentences.Add("The Kraken has destroyed all our ships! It's O'er...");
                //DM.StartDialogue(newQMDialogue);

                yield return StartCoroutine(completeQMDialogue("The Kraken has destroyed all our ships! It's O'er..."));

                foreach (Transform child in shipInfoImage.transform)
                {
                    child.transform.GetComponent<TextMeshProUGUI>().SetText("Sunk");
                    child.transform.GetComponent<TextMeshProUGUI>().color = Color.red;
                }

                state = BattleState.Lost;
                EndBattle();
            }
            else
            {
                Debug.Log("Attack the Kraken!");
                state = BattleState.Player1Turn;
                player1Turn();
            }
        }
    }

    void removeAvailableNodes(int headTileXPos, int headTileYPos)
    {

        availableNodes.Remove((headTileXPos, headTileYPos));
        availableNodes.Remove((headTileXPos + 1, headTileYPos));
        availableNodes.Remove((headTileXPos - 1, headTileYPos));
        availableNodes.Remove((headTileXPos, headTileYPos + 1));
        availableNodes.Remove((headTileXPos, headTileYPos - 1));
        availableNodes.Remove((headTileXPos + 1, headTileYPos + 1));
        availableNodes.Remove((headTileXPos + 1, headTileYPos - 1));
        availableNodes.Remove((headTileXPos - 1, headTileYPos + 1));
        availableNodes.Remove((headTileXPos - 1, headTileYPos - 1));

    }

    void addAvailableNodes(int headTileXPos, int headTileYPos)
    {
        availableNodes[(headTileXPos, headTileYPos)] = 0;
        availableNodes[(headTileXPos + 1, headTileYPos)] = 0;
        availableNodes[(headTileXPos - 1, headTileYPos)] = 0;
        availableNodes[(headTileXPos, headTileYPos + 1)] = 0;
        availableNodes[(headTileXPos, headTileYPos - 1)] = 0;
        availableNodes[(headTileXPos + 1, headTileYPos + 1)] = 0;
        availableNodes[(headTileXPos + 1, headTileYPos - 1)] = 0;
        availableNodes[(headTileXPos - 1, headTileYPos + 1)] = 0;
        availableNodes[(headTileXPos - 1, headTileYPos - 1)] = 0;

        var keys = new List<(int, int)>(availableNodes.Keys);
        foreach ((int, int) key in keys)
        {
            if (key.Item1 <= 0 || key.Item1 >= 9 || key.Item2 <= 0 || key.Item2 >= 9)
            {
                //Debug.LogWarning("Removing Key: " + key);
                availableNodes.Remove(key);
            }

        }

    }

    void EndBattle()
    {
        if (state == BattleState.Won)
        {
            audioManager.Stop("The Kraken");
            audioManager.Play("ACWon");
            StartCoroutine(completeQMDialogue("We lived t' tell our tales! Off t' the next adventure!"));
            gameBeginText.text = "Victory!!!";
            StartCoroutine(WaitForGameBeginText());
        }
        else if (state == BattleState.Lost)
        {
            audioManager.Stop("The Kraken");
            audioManager.Play("Saren");
            audioManager.Play("KrakenTurn");
            StartCoroutine(completeKrakenDialogue("SKREEEEEEONGK!!!!"));
            gameBeginText.text = "Defeat!!!";
            StartCoroutine(WaitForGameBeginText());
        }
    }

    void resetFreeNodes()
    {
        // Second Try
        foreach (KeyValuePair<(int, int), int> kvp in this.gameObject.GetComponentInParent<Board>().player2WorldBoard)
        {
            //availableNodes.Add(kvp.Key);
            availableNodes.Add(kvp.Key, 0);
        }


        var keys = new List<(int, int)>(availableNodes.Keys);

        //// Change the border node locations to 1 so they will not get picked for tile placement
        int i = 0;
        foreach ((int, int) key in keys)
        {
            if (key == (i, 0))
                availableNodes.Remove(key);
            if (key == (i, 9))
                availableNodes.Remove(key);
            i++;
            if (i == 10)
                i = 0;
        }
        i = 1;
        int j = 1;
        foreach ((int, int) key in keys)
        {
            //Debug.Log("Key: " + key + " i: " + i);
            //Debug.Log("Key = {0}, Value = {1} " + key + " " + availableNodes[key]);
            if (key == (0, i))
            {
                availableNodes.Remove(key);
                i++;
            }
            if (key == (9, j))
            {
                availableNodes.Remove(key);
                j++;
            }
            if (i == 10)
                i = 1;
        }

        i = 0;
    }

    IEnumerator WaitForGameBeginText()
    {
        while (true)
        {
            if (!gameStateAnimator.GetBool("IsGameState"))
            {
                gameStateAnimator.SetBool("IsGameState", true);
                yield return new WaitForSeconds(3f);
                gameStateAnimator.SetBool("IsGameEnd", true);
                restartGameButton.gameObject.SetActive(true);
                yield break;
            }
            yield return null;
        }
    }

    public void restartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void spawnAndDestroyParticle(ParticleSystem particle, float tempX, float tempZ)
    {
        GameObject particleEffect = Instantiate(particle.gameObject, new Vector3(tempX * 2, 0, tempZ * 2), Quaternion.Euler(-90, 0, 0)) as GameObject;
        ParticleSystem parts = particleEffect.GetComponent<ParticleSystem>();
        float totalDuration = parts.main.duration + parts.startLifetime;
        Destroy(particleEffect, totalDuration);
    }
}

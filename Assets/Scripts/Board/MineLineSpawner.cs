using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MineLineSpawner : MonoBehaviour
{
    private Vector3 mousePosition;
    [SerializeField]
    public List<GameObject> player1MineLineTemplate;
    [SerializeField]
    public List<GameObject> player1MineLineObject;
    [SerializeField]
    private int maxCruiserLimit;
    [SerializeField]
    private GameObject player1Object;

    [SerializeField]
    public Dialogue beginingDialogue;

    [SerializeField]
    public DialogueManager DM;
    // global variable to keep track of what mine line to place from the list
    private int templatesListIndex = 0;

    private bool gameStart = false;

    // Game begin text animator
    public Animator gameStateAnimator;
    // Game Begin Text
    public TMP_Text gameBeginText;

    // Start is called before the first frame update
    void Start()
    {
        DM.StartDialogue(beginingDialogue);
        gameBeginText.text = "Place Ships!!";
        //Instantiate(player1Object, new Vector3(0, 0, 0), Quaternion.identity);
        StartCoroutine(customUpdate());
    }

    IEnumerator customUpdate()
    {
        while (true)
        {
            if (DM.startDialogue == false)
            {
                if (gameStart == false)
                {
                    yield return StartCoroutine(WaitForGameBeginText());
                    //gameStateAnimator.SetBool("IsGameState", true);
                    gameStart = true;
                }
                //Debug.Log(templatesListIndex + " " + player1MineLineTemplate.Count);
                if (templatesListIndex != player1MineLineTemplate.Count)
                {
                    gameStateAnimator.SetBool("IsGameState", false);
                    updateTransparentMineLine(player1MineLineTemplate[templatesListIndex]);
                    if (Input.GetButtonDown("Fire1"))
                    {
                        Debug.Log("Primary Mouse button Pressed");
                        if (checkMinePlaceAllow(player1MineLineTemplate[templatesListIndex]))
                        {
                            Debug.Log("Can Allow mine placement");
                            placeMines(player1MineLineTemplate[templatesListIndex], player1MineLineObject[templatesListIndex]);
                        }
                        else
                        {
                            Debug.Log("Cannot allow placement");

                        }

                    }
                }
                else
                {
                    //Debug.Log("Everything is placed: Battle Ready");
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    this.gameObject.GetComponent<MineLineSpawner>().enabled = false;
                    yield break;
                }
            }
            yield return null;
        }

    }


    private void updateTransparentMineLine(GameObject P1MineLineTempalate)
    {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("RotVer"))
        {
            Debug.Log("Rotate 90");
            if (P1MineLineTempalate.transform.rotation.eulerAngles.y == 0)
                P1MineLineTempalate.transform.Rotate(0, 90, 0);
            else
                P1MineLineTempalate.transform.Rotate(0, -90, 0);
        }

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Player1BoardNode"))
            {

                P1MineLineTempalate.transform.position = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y), Mathf.Round(hit.point.z));
                //Debug.Log(threeMineLineTemplate.transform.position + " : CHILD 0: " + threeMineLineTemplate.transform.GetChild(0).transform.position + 
                //    " : CHILD 1: " + threeMineLineTemplate.transform.GetChild(1).transform.position +
                //    " : CHILD 2: " + threeMineLineTemplate.transform.GetChild(2).transform.position);

            }
        }
    }

    bool checkMinePlaceAllow(GameObject P1MineLineTempalate)
    {
        bool tempCheckTiles = false;
        for (int i = 0; i < P1MineLineTempalate.transform.childCount; i++)
        {
            int tempTileState;
            int tempChildBoardX = (int)P1MineLineTempalate.transform.GetChild(i).transform.position.x / 2;
            int tempChildBoardZ = (int)P1MineLineTempalate.transform.GetChild(i).transform.position.z / 2;

            // Look for tile state
            this.gameObject.GetComponent<Board>().player1WorldBoard.TryGetValue((tempChildBoardX, tempChildBoardZ), out tempTileState);
            //this.gameObject.GetComponent<Board>().newPlayer1WorldBoard.TryGetValue((tempChildBoardX, tempChildBoardZ), out tempMineTile);

            //Debug.Log("Child (" + i + ") location: " + tempChildBoardX + ", " + 0 + ", " + tempChildBoardZ + " ; Location state: " + tempTileState);
            //Debug.Log("Child (" + i + ") location: " + tempChildBoardX + ", " + 0 + ", " + tempChildBoardZ + " ; Location state: " + tempMineTile.emptyOrOccupied);

            if (tempTileState == 0 && tempChildBoardX < 10 && tempChildBoardZ < 10 && tempChildBoardX > 0 && tempChildBoardZ > 0)
                tempCheckTiles = true;
            else
            {
                tempCheckTiles = false;
                return tempCheckTiles;
            }
        }

        return tempCheckTiles;

    }

    void placeMines(GameObject P1MineLineTemplate, GameObject P1MineLine)
    {
        GameObject tempMineLine = Instantiate(P1MineLine, new Vector3(P1MineLineTemplate.transform.position.x, P1MineLineTemplate.transform.position.y + 1, P1MineLineTemplate.transform.position.z), P1MineLineTemplate.transform.rotation);

        // Add the mine tiles to player list of mine tiles
        player1Object.GetComponent<Unit>().tileLines.Add(tempMineLine.GetComponent<MineLine>().mineLineName, tempMineLine.GetComponent<MineLine>());

        // update the board tiles where the mine tiles were placed
        for (int i = 0; i < tempMineLine.transform.childCount; i++)
        {
            // Offset world tile location to match the board data structure location for updating
            int tempChildBoardX = (int)tempMineLine.transform.GetChild(i).transform.position.x / 2;
            int tempChildBoardZ = (int)tempMineLine.transform.GetChild(i).transform.position.z / 2;

            this.gameObject.GetComponent<Board>().player1WorldBoard[(tempChildBoardX, tempChildBoardZ)] = 1;

            if (tempMineLine.transform.GetChild(i).tag != "ShipPart") { 
                tempMineLine.transform.GetChild(i).GetComponent<MineTile>().emptyOrOccupied = 1;
                tempMineLine.transform.GetChild(i).GetComponent<MineTile>().offsetTileLocationX = tempChildBoardX;
                tempMineLine.transform.GetChild(i).GetComponent<MineTile>().offsetTileLocationZ = tempChildBoardZ;

                this.gameObject.GetComponent<Board>().newPlayer1WorldBoard[(tempChildBoardX, tempChildBoardZ)] = tempMineLine.transform.GetChild(i).GetComponent<MineTile>();
            }

        }

        P1MineLineTemplate.SetActive(false);
        if (templatesListIndex < player1MineLineTemplate.Count)
            ++templatesListIndex;

    }

    IEnumerator WaitForGameBeginText()
    {
        while (true)
        {
            //gameStateAnimator.SetBool("IsGameState", true);
            if (!gameStateAnimator.GetBool("IsGameState"))
            {
                gameStateAnimator.SetBool("IsGameState", true);
                yield return new WaitForSeconds(3f);
                yield break;
            }
            yield return null;
        }
        //not here yield return null;
    }

}

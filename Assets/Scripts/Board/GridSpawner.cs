using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    public int xStart, yStart;
    public int colLength, rowLength;
    public float x_space, y_space;
    public GameObject Nodes, Player1Nodes2, player2Nodes;
    // Start is called before the first frame update

    void Start()
    {

        for (int i = 0; i < colLength * rowLength; i++)
        {
            Instantiate(Nodes, new Vector3(xStart + (x_space * (i % colLength)), 0, yStart + (y_space * (i / colLength))), Quaternion.identity);
            Instantiate(player2Nodes, new Vector3(xStart + (x_space * (i % colLength)), 42, yStart + (y_space * (i / colLength))), Quaternion.identity);
            this.gameObject.GetComponent<Board>().player1WorldBoard.Add((xStart + (i % colLength), yStart + (i / colLength)), 0);
            this.gameObject.GetComponent<Board>().player2WorldBoard.Add((xStart + (i % colLength), yStart + (i / colLength)), 0);


        }
    }

}

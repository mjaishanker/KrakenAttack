using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTile : MonoBehaviour
{
    [SerializeField]
    public bool isHit = false;
    [SerializeField]
    public int emptyOrOccupied;
    [SerializeField]
    public int offsetTileLocationX;
    [SerializeField]
    public int offsetTileLocationZ;
    [SerializeField]
    public Vector3 ogLocalPosition;

    [SerializeField]
    public Vector3 currentLocalPosition;

    public void setOGLocalPos()
    {
        ogLocalPosition = transform.localPosition;
    }

}

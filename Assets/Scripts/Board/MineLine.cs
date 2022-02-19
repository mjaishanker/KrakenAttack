using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLine : MonoBehaviour
{
    public string mineLineName;
    public int numberOfTiles;
    public int maxHp; // numberOfTiles
    public int currentHp;
    public bool tileLineDestroyed = false;
    public bool tileLineAbleMove = true; // Check if Kraven tile can move

    public bool checkLineTilesStatus()
    {
        int countTilesDestroyed = 0;
        foreach (Transform child in this.transform)
        {
            //Debug.LogWarning("Checking Tile Line: " + gameObject.name + " child: " + child.name + " is hit: " + child.GetComponent<MineTile>().isHit);
            if (child.GetComponent<MineTile>().isHit == true)
            {
                countTilesDestroyed++;
            }

            //    tileLineDestroyed = true;
            //else
            //{

            //}
            //    tileLineDestroyed = false;
        }

        if (countTilesDestroyed == this.transform.childCount)
            tileLineDestroyed = true;
        else
            tileLineDestroyed = false;
        return tileLineDestroyed;
    }

    public void unHookHarpoon()
    {
        foreach (Transform child in this.transform)
        {
            //Debug.LogWarning("Checking Tile Line: " + gameObject.name + " child: " + child.name + " is hit: " + child.GetComponent<MineTile>().isHit);
            if (child.GetComponent<MineTile>().isHit == true)
            {
                child.GetComponent<MineTile>().isHit = false;
                Animation anim = child.GetChild(0).GetComponent<Animation>();
                if(anim != null)
                    anim.Stop();
                child.localPosition = new Vector3(child.GetComponent<MineTile>().ogLocalPosition.x, child.GetComponent<MineTile>().ogLocalPosition.y, child.GetComponent<MineTile>().ogLocalPosition.z);
            }
        }
    }

    public void playTentacleDeathAnimation()
    {
        if(tileLineDestroyed == true)
        {
            foreach (Transform child in this.transform)
            {
                //Debug.LogWarning("Checking Tile Line: " + gameObject.name + " child: " + child.name + " is hit: " + child.GetComponent<MineTile>().isHit);
                if (child.GetComponent<MineTile>().isHit == true)
                {
                    Animation anim = child.gameObject.transform.GetChild(0).GetComponent<Animation>();
                    if(anim != null)
                    {
                        anim["KrakenTentacle_Death_Animation_Baked"].wrapMode = WrapMode.Once;
                        anim.Play("KrakenTentacle_Death_Animation_Baked");
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// makes sure (maybe) that objects get rendered in front of each other in a properly pseudo-3D way, since this game is all taking place in 2D...

public class DepthSorter : MonoBehaviour {

    SpriteRenderer rend;
    public float offset; // account for the fact that objects aren't measured from base

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {
        rend.sortingOrder = (int)((-(transform.position.y + GameMaster.GAMEPLAY_HEIGHT + offset) / GameMaster.GAMEPLAY_HEIGHT) * 100);
	}
}

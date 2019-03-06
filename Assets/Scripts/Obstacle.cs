using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    protected float size;
    protected Squirrel squirrel;
    protected bool isEnabled = true;
    protected SpriteRenderer rend;

    // Use this for initialization
    protected virtual void Start () {
        squirrel = GameObject.Find("Squirrel").GetComponent<Squirrel>();
        rend = GetComponent<SpriteRenderer>();
	}

    protected void Enable(bool yes)
    {
        if (yes)
        {
            isEnabled = true;
            rend.enabled = true;
        } else
        {
            isEnabled = false;
            rend.enabled = false;
        }
    }

    protected virtual void HitSquirrel()
    {
        squirrel.GetHit();
    }

    // Update is called once per frame
    protected virtual void Update () {
        if (isEnabled)
        {
            if (Vector3.SqrMagnitude(transform.position - squirrel.transform.position) < size)
            {
                HitSquirrel();
            }
        }
    }
}

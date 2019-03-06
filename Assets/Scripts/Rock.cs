using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Obstacle {

    GameObject tree;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        size = 20;
        tree = GameObject.Find("HomeTree");
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        if (Vector3.SqrMagnitude(transform.position - tree.transform.position) < size + 100)
        {
            Enable(false);
        } else
        {
            Enable(true);
        }
    }
}

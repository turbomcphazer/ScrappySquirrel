using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Rock {

    protected override void Start()
    {
        base.Start();
        size = 10;
    }

    protected override void HitSquirrel()
    {
        squirrel.StartTripping();
    }
}

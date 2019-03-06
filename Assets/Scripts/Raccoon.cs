using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raccoon : Obstacle {

    Vector3 vel;
    float speed;
    float maxSpeed;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        size = 15;
        speed = 300;
        maxSpeed = 70;
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.timeSinceLevelLoad * 50) * 5f);
        transform.position += vel * Time.deltaTime;
        vel += (squirrel.transform.position - transform.position).normalized * speed * Time.deltaTime;
        vel = vel.normalized * maxSpeed;
    }
}

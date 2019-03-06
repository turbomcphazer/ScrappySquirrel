using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : Obstacle {

    public float posTimer;
    Vector3 basePos;
    public GameObject blade;
    SpriteRenderer bladeRend;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        bladeRend = blade.GetComponent<SpriteRenderer>();
        size = 20;
        basePos = transform.position;
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
        posTimer += Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.timeSinceLevelLoad * 3) * 10f);
        blade.transform.Rotate(new Vector3(0, 0, 300 * Time.deltaTime));
        transform.position = basePos + Vector3.right * Mathf.Sin(posTimer) * (GameMaster.GAMEPLAY_WIDTH / 2.8f);
    }

    private void LateUpdate()
    {
        bladeRend.sortingOrder = rend.sortingOrder - 1;
    }
}

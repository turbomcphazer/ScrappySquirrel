using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meter : MonoBehaviour {

    const float MAX_HEIGHT = 70;
    public GameObject meter;
    public TextMesh txt;

	// Use this for initialization
	void Start () {
		
	}

    public void SetMeter(float set)
    {
        meter.transform.localScale = new Vector3(4, MAX_HEIGHT * set);
    }

    public void SetText(string set)
    {
        txt.text = set;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

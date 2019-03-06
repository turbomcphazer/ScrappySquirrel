using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sweat : MonoBehaviour {

    float timer;
    Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.localPosition;
        Reset();
	}

    public void Reset()
    {
        timer = 0.3f;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-90, 90));
        transform.localPosition = startPos;
        transform.Translate(Vector3.up * 4);
    }

    // Update is called once per frame
    void Update () {

        transform.Rotate(new Vector3(0, 0, Mathf.Sign(transform.rotation.z) * Time.deltaTime * 300));
        transform.Translate(Vector3.up * 20 * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

    public GameObject instructions;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else
        {

            if (Input.anyKeyDown)
            {
                if (!instructions.activeInHierarchy)
                {
                    if (!Input.GetKeyDown(KeyCode.I))
                    {
                        SceneManager.LoadScene(1);
                    }
                    else
                    {
                        instructions.SetActive(true);
                    }
                }
                else
                {
                    instructions.SetActive(false);
                }
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squirrel : MonoBehaviour {

    const int MAX_CARRY = 5;

    const int SOUND_OUCH = 0;
    const int SOUND_TRIP = 1;

    public Sweat[] sweat;

    float speed = 65;
    int curAcorns = 0;
    Acorn[] acornList;
    GameMaster gm;
    float invinceTime = 0;
    public SpriteRenderer rend;
    public SpriteRenderer bodyRend;
    public SpriteRenderer footRend;
    public SpriteRenderer tailRend;
    AudioSource[] sounds;
    Vector3 lastPos;
    float trippingTime = 0;

    // Use this for initialization
    void Start () {
        gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        sounds = GetComponents<AudioSource>();
        acornList = new Acorn[MAX_CARRY];
    }

    public void GetHit()
    {
        if (invinceTime <= 0)
        {
            sounds[SOUND_OUCH].Play();
            invinceTime = 1f;
            DropAcorns();
        }
    }

    public void ReportTreeContact(HomeTree tree)
    {
        if (curAcorns > 0)
        {
            int remainder = gm.PayRent(curAcorns);
            if (remainder > -1)
            {
                curAcorns = remainder;
                for (int i = remainder; i < MAX_CARRY; i++)
                {
                    if (acornList[i] != null)
                    {
                        acornList[i].Hide();
                    }
                    acornList[i] = null;
                }
                tree.Wobble();
            } else
            {
                // rent is already paid
            }
        }
    }

    void DropAcorns()
    {
        for (int i = 0; i < MAX_CARRY; i++)
        {
            if (acornList[i] != null)
            {
                acornList[i].OnHit();
            }
            acornList[i] = null;
        }
        curAcorns = 0;
    }

    public int GetAcorn(Acorn newAcorn)
    {
        if (curAcorns < MAX_CARRY)
        {
            acornList[curAcorns] = newAcorn;
            curAcorns++;
            return curAcorns;
        } else
        {
            return -1;
        }
    }

    void GetInput()
    {
        lastPos = transform.position;
        int x = 0;
        int y = 0;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            y = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            y = -1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            x = -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            x = 1;
        }

        if (trippingTime > 0)
        {
            x = -x;
            y = -y;
        }

        Vector3 newPos = transform.position;

        if (Mathf.Abs(x) + Mathf.Abs(y) > 1) // move slower on a diagonal
        {
            newPos += Vector3.up * Time.deltaTime * speed * y / 1.4142f;
            newPos += Vector3.right * Time.deltaTime * speed * x / 1.4142f;
        }
        else
        {
            newPos += Vector3.up * Time.deltaTime * speed * y;
            newPos += Vector3.right * Time.deltaTime * speed * x;
        }

        if (gm.state == GameMaster.ST_NORMAL)
        {
            transform.position = new Vector3(Mathf.Clamp(newPos.x, -GameMaster.GAMEPLAY_WIDTH / 2, GameMaster.GAMEPLAY_WIDTH / 2), Mathf.Clamp(newPos.y, -GameMaster.GAMEPLAY_HEIGHT / 2, GameMaster.GAMEPLAY_HEIGHT / 2));
        } else
        {
            Vector3 clampPos = new Vector3(Mathf.Clamp(newPos.x, -GameMaster.GAMEPLAY_WIDTH / 2, GameMaster.GAMEPLAY_WIDTH / 2), Mathf.Clamp(newPos.y, -GameMaster.GAMEPLAY_HEIGHT / 2, GameMaster.GAMEPLAY_HEIGHT / 2));
            if (clampPos != newPos) // went off screen
            {
                x = 0;
                y = 0;
                if (newPos.x < -GameMaster.GAMEPLAY_WIDTH / 2)
                {
                    x = -1;
                }
                if (newPos.x > GameMaster.GAMEPLAY_WIDTH / 2)
                {
                    x = 1;
                }
                if (newPos.y < -GameMaster.GAMEPLAY_HEIGHT / 2)
                {
                    y = -1;
                }
                if (newPos.y > GameMaster.GAMEPLAY_HEIGHT / 2)
                {
                    y = 1;
                }
                gm.SwitchScreens(x, y);
                transform.position -= new Vector3(GameMaster.GAMEPLAY_WIDTH * x * 0.95f, GameMaster.GAMEPLAY_HEIGHT * y * 0.95f);
            } else
            {
                transform.position = clampPos;
            }
        }
    }

    public void StartTripping()
    {
        if (trippingTime == 0)
        {
            trippingTime = 3;
            invinceTime = 1;
            sounds[SOUND_TRIP].Play();
        }
    }

    void SpawnSweat()
    {
        for (int i = 0; i < sweat.Length; i++)
        {
            if (!sweat[i].gameObject.activeInHierarchy)
            {
                sweat[i].gameObject.SetActive(true);
                sweat[i].Reset();
                break;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        GetInput();

        if (lastPos != transform.position) // we moved, so animate
        {
            bodyRend.transform.rotation = Quaternion.Euler(0, 0, -Mathf.Sin(Time.timeSinceLevelLoad * 40) * 10);
            footRend.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.timeSinceLevelLoad * 40) * 20);
            tailRend.transform.localPosition = new Vector3(Mathf.Sign(lastPos.x - transform.position.x) * 4, -1.5f);
            tailRend.transform.localScale = new Vector3(Mathf.Sign(lastPos.x - transform.position.x), 1);
            tailRend.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sign(lastPos.x - transform.position.x) * -90 + (Mathf.Cos(Time.timeSinceLevelLoad * 40) * 30));
        } else
        {
            bodyRend.transform.rotation = Quaternion.identity;
            footRend.transform.rotation = Quaternion.identity;
        }

        if (invinceTime > 0)
        {
            invinceTime -= Time.deltaTime;
            if (Time.frameCount % 2 == 0)
            {
                bodyRend.enabled = false;
            } else
            {
                bodyRend.enabled = true;
            }
        }
        else
        {
            bodyRend.enabled = true;
        }

        if (trippingTime > 0)
        {
            rend.color = new Color(Mathf.Sin(Time.timeSinceLevelLoad - (Mathf.PI / 2)), Mathf.Sin(Time.timeSinceLevelLoad), Mathf.Sin(Time.timeSinceLevelLoad + (Mathf.PI / 2)), 1f);
            trippingTime -= Time.deltaTime;
            if (trippingTime <= 0)
            {
                trippingTime = 0;
                rend.color = Color.white;
            }
        }

        if (gm.SquirrelSweating())
        {
            if (Time.frameCount % 10 == 0)
            {
                SpawnSweat();
            }
        }
	}

    private void LateUpdate()
    {
        footRend.sortingOrder = rend.sortingOrder + 1;
        footRend.color = rend.color;
        footRend.enabled = bodyRend.enabled;
        tailRend.sortingOrder = rend.sortingOrder - 1;
        tailRend.color = rend.color;
        tailRend.enabled = bodyRend.enabled;
        bodyRend.sortingOrder = rend.sortingOrder;
        bodyRend.color = rend.color;
    }
}

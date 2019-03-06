using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

    struct ScreenData {
        public float acornRate;
        public int startAcorns;
        public int baseRent;
        public int curRent;
        public bool treeRented;
        public bool lostTree;
        public Vector3 treePos;
        public bool[] obstacles;
    }

    public const float GAMEPLAY_WIDTH = 134;
    public const float GAMEPLAY_HEIGHT = 70;

    public const int ST_NORMAL = 0;
    public const int ST_FINDNEWTREE = 1;
    public const int ST_GAMEOVER = 2;

    const int WORLD_SIZE = 5; // 25 screens
    ScreenData[,] sData;

    const int SOUND_MUSIC = 0;
    const int SOUND_PAYRENT = 1;
    const int SOUND_RENTINCREASE = 2;
    const int SOUND_PRICEDOUT = 3;
    const int SOUND_GAMEOVER = 4;

    const int MAX_ACORNS = 20;
    float acornSpawnRate = 1;
    float acornSpawnTime = 1;

    float maxTime = 20;
    public float rentTime;

    bool firstRound = true;

    int rentRate = 5;
    int curRent;

    int totalPaid = 0;
    int totalMonths = 0;

    Meter timeMeter;
    Meter rentMeter;
    Meter newTreeMeter;

    List<Acorn> acornList;
    public GameObject acornPrefab;

    public GameObject pricedOutWarning;
    public GameObject pricedOutArrows;
    public GameObject rentIncreasedWarning;
    public GameObject gameOverScreen;
    public GameObject businessSquirrel;

    public GameObject[] obstacles;

    public GameObject bg;

    HomeTree tree;

    AudioSource[] sounds;

    public int state;

    int curScreenX = 0;
    int curScreenY = 0;

    float gameOverTimer = 5;

    private void Awake()
    {
        InstantiateAcorns();
    }

    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
        sounds = GetComponents<AudioSource>();
        tree = GameObject.Find("HomeTree").GetComponent<HomeTree>();
        timeMeter = GameObject.Find("TimeMeter").GetComponent<Meter>();
        rentMeter = GameObject.Find("RentMeter").GetComponent<Meter>();
        newTreeMeter = GameObject.Find("NewTreeMeter").GetComponent<Meter>();
        newTreeMeter.gameObject.SetActive(false);
        rentTime = maxTime;
        curRent = rentRate;
        UpdateRentMeter();
        state = ST_NORMAL;
        InitWorld();
        InitScreen(curScreenX, curScreenY);
	}

    void InitWorld()
    {
        curScreenX = WORLD_SIZE / 2;
        curScreenY = WORLD_SIZE / 2;
        sData = new ScreenData[WORLD_SIZE, WORLD_SIZE];
        for (int y = 0; y < WORLD_SIZE; y++)
        {
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                sData[x, y].acornRate = 1;
                sData[x, y].startAcorns = 10;
                sData[x, y].baseRent = Random.Range(3, 8);
                sData[x, y].curRent = sData[x,y].baseRent;
                sData[x, y].treeRented = false;
                sData[x, y].lostTree = false;
                sData[x, y].treePos = new Vector3(Random.Range(-(GAMEPLAY_WIDTH / 2.5f), GAMEPLAY_WIDTH / 2.5f), Random.Range(-(GAMEPLAY_HEIGHT / 2.5f), GAMEPLAY_HEIGHT / 2.5f)); // move this inward a little so you don't hit it immediately on entering a new screen
                sData[x, y].obstacles = new bool[4];
                int chessDistance = Mathf.Clamp(Mathf.Max(Mathf.Abs(curScreenX - x), Mathf.Abs(curScreenY - y)), 0, 2);

                for (int i = 0; i < sData[x, y].obstacles.Length; i++)
                {
                    sData[x, y].obstacles[i] = false;
                }

                for (int i = 0; i < chessDistance; i++) // spawn one obstacle per distance measure from first tree
                {
                    int newObs = Random.Range(0, obstacles.Length);
                    while (sData[x, y].obstacles[newObs] == true)
                    {
                        newObs = Random.Range(0, obstacles.Length);
                    }
                    sData[x, y].obstacles[newObs] = true;
                }
            }
        }
        sData[curScreenX, curScreenY].treeRented = true;
        sData[curScreenX, curScreenY].baseRent = 5;
        sData[curScreenX, curScreenY].curRent = 5;
        sData[curScreenX, curScreenY].treePos = new Vector3(GAMEPLAY_WIDTH / 2.5f, -GAMEPLAY_HEIGHT / 2.8f);
    }

    void InstantiateAcorns()
    {
        acornList = new List<Acorn>();
        for (int i = 0; i < MAX_ACORNS; i++)
        {
            GameObject temp = GameObject.Instantiate(acornPrefab);
            temp.transform.position = Vector3.zero;
            acornList.Add(temp.GetComponent<Acorn>());
            acornList[i].Hide();
        }
    }

    public bool SquirrelSweating()
    {
        return ((rentTime < 5) && (curRent > 0));
    }

    void SpawnAcorn()
    {
        for (int i = 0; i < acornList.Count; i++)
        {
            if (acornList[i].state == Acorn.ST_HIDDEN)
            {
                acornList[i].Spawn();
                break;
            }
        }
    }

    void UpdateRentMeter()
    {
        rentMeter.SetMeter((float)Mathf.Clamp(curRent, 0, rentRate) / (float)rentRate);
        rentMeter.SetText("RENT (" + Mathf.Clamp(curRent,0, rentRate).ToString() + "/" + rentRate.ToString() + ")");
    }

    void UpdateTimeMeter()
    {
        timeMeter.SetMeter(Mathf.Clamp(rentTime, 0, rentTime) / maxTime);
    }

    void UpdateNewTreeMeter()
    {
        newTreeMeter.SetMeter(Mathf.Clamp(rentTime, 0, rentTime) / maxTime);
    }

    void InitScreen(int x, int y)
    {
        rentRate = sData[x, y].baseRent;
        curRent = sData[x,y].curRent;
        tree.transform.position = sData[x, y].treePos;
        if (sData[x, y].lostTree)
        {
            tree.SetSprite(1);
            tree.SetPriceText(null);
            businessSquirrel.SetActive(true);
        } else
        {
            businessSquirrel.SetActive(false);
            tree.SetSprite(0);
            if (sData[x, y].treeRented) // only on first screen 
            {
                tree.SetPriceText(null);
            } else
            {
                tree.SetPriceText("TO RENT: (" + curRent.ToString() + "/" + rentRate.ToString() + ")");
            }
        }
        for (int i = 0; i < acornList.Count; i++)
        {
            if (acornList[i].state != Acorn.ST_COLLECTED)
            {
                acornList[i].Hide();
            }
        }
        for (int i = 0; i < sData[x,y].obstacles.Length; i++)
        {
            if (i < obstacles.Length) {
                obstacles[i].SetActive(sData[x, y].obstacles[i]);
            }
        }
        for (int i = 0; i < sData[x, y].startAcorns; i++)
        {
            SpawnAcorn();
        }
    }

    public void SwitchScreens(int x, int y)
    {
        curScreenX += x;
        curScreenY += y;

        if ((curScreenX + curScreenY) % 2 == 1)
        {
            bg.transform.localScale = new Vector3(9, -9);
        } else
        {
            bg.transform.localScale = new Vector3(9, 9);
        }
        
        // loop on edges, but careful of how modulus works on negative numbers
        if (curScreenX < 0)
        {
            curScreenX += WORLD_SIZE;
        }
        if (curScreenY < 0)
        {
            curScreenY += WORLD_SIZE;
        }
        curScreenX = (curScreenX % WORLD_SIZE); // loop on edges
        curScreenY = (curScreenY % WORLD_SIZE); // loop on edges
        InitScreen(curScreenX, curScreenY);
    }

    public int PayRent(int amount)
    {
        if ((curRent > 0)&&(!sData[curScreenX, curScreenY].lostTree))
        {
            sounds[SOUND_PAYRENT].Play();
            curRent -= amount;
            sData[curScreenX, curScreenY].curRent = curRent;
            totalPaid += amount;
            UpdateRentMeter();
            if (curRent > 0)
            {
                if (!sData[curScreenX, curScreenY].treeRented)
                {
                    tree.SetPriceText("TO RENT: (" + curRent.ToString() + "/" + rentRate.ToString() + ")");
                }
                return 0;
            }
            else
            {
                if (!sData[curScreenX, curScreenY].treeRented)
                {
                    int remainder = RentTree();
                    return -remainder;
                }
                return -curRent;
            }
        } else
        {
            return -1;
        }
    }

    int RentTree()
    {
        sData[curScreenX, curScreenY].treeRented = true;
        state = ST_NORMAL;
        sounds[SOUND_PRICEDOUT].Stop();
        pricedOutArrows.SetActive(false);
        pricedOutWarning.SetActive(false);
        TimeUp();
        rentRate = sData[curScreenX, curScreenY].baseRent;
        int remainder = curRent;
        curRent = rentRate;
        UpdateRentMeter();
        UpdateTimeMeter();
        rentMeter.gameObject.SetActive(true);
        timeMeter.gameObject.SetActive(true);
        newTreeMeter.gameObject.SetActive(false);
        tree.SetPriceText(null);
        firstRound = true;
        return remainder;
    }

    void GetPricedOut()
    {
        firstRound = true;
        sData[curScreenX, curScreenY].lostTree = true;
        sounds[SOUND_PRICEDOUT].Play();
        state = ST_FINDNEWTREE;
        for (int y = 0; y < WORLD_SIZE; y++) // every round, starting rent goes up everywhere
        {
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                sData[x, y].baseRent += 2;
                sData[x, y].curRent = sData[x, y].baseRent;
            }
        }
        rentTime = maxTime;
        rentMeter.gameObject.SetActive(false);
        timeMeter.gameObject.SetActive(false);
        UpdateNewTreeMeter();
        newTreeMeter.gameObject.SetActive(true);
        tree.SetSprite(1);
    }

    void MakeRent()
    {
        totalMonths++;
        firstRound = false;
        rentRate += 20;
        curRent = rentRate;
        UpdateRentMeter();
        rentTime = maxTime;
        UpdateTimeMeter();
        sounds[SOUND_RENTINCREASE].Play();
    }

    void GameOver()
    {
        state = ST_GAMEOVER;
        Time.timeScale = 0;
        gameOverTimer = 5;
        sounds[SOUND_MUSIC].Stop();
        sounds[SOUND_PRICEDOUT].Stop();
        sounds[SOUND_GAMEOVER].Play();
        gameOverScreen.SetActive(true);
        pricedOutWarning.gameObject.SetActive(false);
        pricedOutArrows.gameObject.SetActive(false);
        rentIncreasedWarning.gameObject.SetActive(false);
        tree.SetPriceText(null);
        GameObject.Find("Score").GetComponent<TextMesh>().text = totalMonths.ToString();
    }

    void TimeUp()
    {
        switch (state)
        {
            case ST_NORMAL:
            if (curRent > 0)
            {
                GetPricedOut();
            }
            else
            {
                MakeRent();
            }
            break;
            case ST_FINDNEWTREE:
            GameOver();
            break;
        }
        
    }

    public Vector3 CheckForRocks(Vector3 where)
    {
        if (obstacles[0].gameObject.activeInHierarchy) 
        {
            foreach (Transform child in obstacles[0].transform)
            {
                if (Vector3.SqrMagnitude(child.position - where) < 50)
                {
                    return (where - child.position).normalized * 20;
                }
            }

            return Vector3.zero;
        } else
        {
            return Vector3.zero;
        }
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        /*if (Input.GetKeyDown(KeyCode.S))
        {
            ScreenCapture.CaptureScreenshot("screen" + Time.frameCount + ".png");
        }*/

        /*if (Input.GetKey(KeyCode.X))
        {
            rentTime -= Time.deltaTime * 10f;
        }*/

        switch (state)
        {
            case ST_NORMAL:
            if (!firstRound)
            {
                if ((rentTime > maxTime - 1)&&((maxTime - rentTime) % 0.2f < 0.1f))
                {
                    rentIncreasedWarning.gameObject.SetActive(true);
                }
                else
                {
                    rentIncreasedWarning.gameObject.SetActive(false);
                }
            }
            if (curRent <= 0) // time drops faster if rent is already paid
            {
                rentTime -= Time.deltaTime * 2;
            }
            UpdateTimeMeter();
            break;
            case ST_FINDNEWTREE:
            UpdateNewTreeMeter();
            if ((maxTime - rentTime) % 0.5f < 0.25f)
            {
                if (rentTime > maxTime - 2)
                {
                    pricedOutWarning.gameObject.SetActive(true);
                }
                else { 
                    pricedOutWarning.gameObject.SetActive(false);
                }
                pricedOutArrows.gameObject.SetActive(true);
            } else
            {
                pricedOutWarning.gameObject.SetActive(false);
                pricedOutArrows.gameObject.SetActive(false);
            }
            break;
            case ST_GAMEOVER:
            gameOverTimer -= Time.unscaledDeltaTime;
            if (gameOverTimer <= 0)
            {
                if (Input.anyKey)
                {
                    Time.timeScale = 1;
                    SceneManager.LoadScene(0);
                }
            }
            break;
        }

        acornSpawnTime -= Time.deltaTime;
        if (curRent <= 0)
        {
            acornSpawnTime -= Time.deltaTime * 2; // time speeds up in rent-paid state, so spawn rate needs to speed up accordingly
        }
        if (acornSpawnTime <= 0)
        {
            acornSpawnTime += Mathf.Clamp((acornSpawnRate - (rentRate * 0.01f)), 0, acornSpawnRate); // spawn more often as rent gets higher
            SpawnAcorn();
        }

        if ((curRent > 0) && (curRent < 5) && (rentTime < 2.5f))
        {
            rentTime -= Time.deltaTime * 0.5f; // you might actually make it, so increase chance of photo finish
        }
        else
        {
            rentTime -= Time.deltaTime;
        }

        if (rentTime <= 0)
        {
            TimeUp();
        }
	}
}

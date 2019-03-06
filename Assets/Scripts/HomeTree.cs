using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTree : MonoBehaviour
{

    public TextMesh priceText;
    public SpriteRenderer rend;
    public Sprite[] sprites;

    float TREE_SIZE = 80;

    Squirrel squirrel;

    float wobbleTimer = 0;

    // Use this for initialization
    void Awake()
    {
        squirrel = GameObject.Find("Squirrel").GetComponent<Squirrel>();
    }

    public void SetPriceText(string set)
    {
        priceText.text = set;
    }

    public void SetSprite(int which)
    {
        rend.sprite = sprites[which];
    }

    public void Wobble()
    {
        wobbleTimer = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.SqrMagnitude(transform.position - squirrel.transform.position) < TREE_SIZE)
        {
            squirrel.ReportTreeContact(this);
        }
        if (wobbleTimer > 0)
        {
            wobbleTimer -= Time.deltaTime * 3;
            if (wobbleTimer < 0)
            {
                wobbleTimer = 0;
            }
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(wobbleTimer * 20) * (wobbleTimer * 10));
        }
        if (Time.frameCount % 8 < 4)
        {
            priceText.color = Color.yellow;
        } else
        {
            priceText.color = Color.red;
        }
    }
}

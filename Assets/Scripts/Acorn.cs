using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MonoBehaviour {

    const int SOUND_COLLECT = 0;

    public const int ST_HIDDEN = 0;
    public const int ST_GROUND = 1;
    public const int ST_COLLECTED = 2;
    public const int ST_LOST = 3;

    const float ACORN_SIZE = 20;
    Squirrel squirrel;

    public int state;

    float wobblePos;
    float wobbleRange;
    public SpriteRenderer rend;
    SpriteRenderer squirrelRend;
    AudioSource[] sounds;
    float scaleModifier = 0;
    Vector3 vel;
    Vector3 baseScale;

    GameMaster gm;

    public Sprite[] sprites;

    // Use this for initialization
    void Awake () {
        sounds = GetComponents<AudioSource>();
        squirrel = GameObject.Find("Squirrel").GetComponent<Squirrel>();
        squirrelRend = squirrel.GetComponent<SpriteRenderer>();
        gm = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        baseScale = transform.localScale;
        state = ST_HIDDEN;
    }

    public void Spawn()
    {
        scaleModifier = 0;
        transform.position = new Vector3(Camera.main.transform.position.x + Random.Range(-GameMaster.GAMEPLAY_WIDTH / 2, GameMaster.GAMEPLAY_WIDTH / 2), Camera.main.transform.position.y + Random.Range(-GameMaster.GAMEPLAY_HEIGHT / 2, GameMaster.GAMEPLAY_HEIGHT / 2));
        transform.position += gm.CheckForRocks(transform.position);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -GameMaster.GAMEPLAY_WIDTH / 2.2f, GameMaster.GAMEPLAY_WIDTH / 2.2f), Mathf.Clamp(transform.position.y, -GameMaster.GAMEPLAY_HEIGHT / 2.2f, GameMaster.GAMEPLAY_HEIGHT / 2.2f));
        transform.rotation = Quaternion.identity;
        transform.localScale = baseScale;
        rend.enabled = true;
        state = ST_GROUND;
    }

    public void Hide()
    {
        state = ST_HIDDEN;
        rend.sprite = sprites[0];
        transform.parent = null;
        rend.enabled = false;
        transform.position = Vector3.zero;
    }

    public void OnHit()
    {
        state = ST_LOST;
        transform.parent = null;
        vel = Quaternion.Euler(0,0,Random.Range(0,360)) * Vector3.up * 100;
        vel.y *= 0.5f;
        vel += Vector3.up * 100;
    }

    void GetCollected()
    {
        int stackPos = squirrel.GetAcorn(this);
        if (stackPos > -1)
        {
            sounds[SOUND_COLLECT].Play();
            state = ST_COLLECTED;
            rend.sprite = sprites[1];
            scaleModifier = 1;
            transform.parent = squirrel.transform;
            transform.localScale = baseScale;
            transform.rotation = Quaternion.identity;
            transform.localPosition = new Vector3(0, 2.5f + (3 * stackPos), -0.1f * stackPos);
            wobbleRange = 0.4f * stackPos;
        }
    }

    // Update is called once per frame
    void Update() {
        wobblePos += Time.deltaTime * 12;
        switch (state) {
            case ST_HIDDEN:
            rend.enabled = false;
            break;
            case ST_GROUND:
            scaleModifier = Mathf.Clamp(scaleModifier + Time.deltaTime * 4, 0, 1);
            transform.localScale = new Vector3(baseScale.x * scaleModifier + Mathf.Sin(scaleModifier * Mathf.PI), baseScale.y * scaleModifier + Mathf.Sin(scaleModifier * Mathf.PI));
            rend.enabled = true;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(wobblePos) * 10);
            if (Vector3.SqrMagnitude(transform.position - squirrel.transform.position) < ACORN_SIZE)
            {
                GetCollected();
            }
            break;
            case ST_COLLECTED:
            transform.localPosition = new Vector3(Mathf.Sin(wobblePos) * wobbleRange, transform.localPosition.y, transform.position.z);
            break;
            case ST_LOST:
            transform.position += vel * Time.deltaTime;
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * 800 * -Mathf.Sign(transform.position.x - squirrel.transform.position.x)));
            vel += Vector3.down * Time.deltaTime * 500;
            if ((Mathf.Abs(transform.position.x) > (GameMaster.GAMEPLAY_WIDTH / 1.8f))||
                    (Mathf.Abs(transform.position.y) > (GameMaster.GAMEPLAY_HEIGHT / 1.8f)))
            {
                Hide();
            }
            break;
        }
	}

    private void LateUpdate()
    {
        if (state == ST_COLLECTED)
        {
            rend.sortingOrder = squirrelRend.sortingOrder; // override depth sorter
        }
    }
}

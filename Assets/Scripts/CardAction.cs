using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAction : MonoBehaviour
{
    public Sprite[] faces;
    public Sprite back;
    public bool faceUp = false;
    public enum Suits {Spade, Heart, Diamond, Club};
    public Suits suit = Suits.Spade;
    public enum Types {Blank, One, Two, Three, Four, Five, Six, Seven, Eight, Ace};
    public Types type = Types.Blank;
    private SpriteRenderer spriteRenderer;
    public float animationTime = 2.0f;
    private bool isFlipping = false;
    private int fps = 30;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCard(int value, bool shown) {
        suit = (Suits)(value / 10);
        type = (Types)(value % 10);
        faceUp = shown;
        SetSprite();
    }

    public void SetSprite() {
        if (!spriteRenderer) return;
        if (faceUp) {
            spriteRenderer.sprite = faces[(int)suit * 10 + (int)type];
        } else {
            spriteRenderer.sprite = back;
        }
    }

    public void Flip() {
        faceUp = !faceUp;
        SetSprite();
    }

    private void OnMouseDown() {
        if (!isFlipping) {
            StartCoroutine("TopFlip");
        }
    }

    public IEnumerator SideFlip() {
        isFlipping = true;
        int frames = Mathf.FloorToInt(animationTime * fps);
        float inc_ry = 180f / frames;
        float ry = transform.rotation.y;
        float px = transform.position.x;
        float pz = transform.position.z;
        for(int i = 0; i < frames; i++) {
            ry += inc_ry;
            transform.eulerAngles = new Vector3(transform.rotation.x, ry, transform.rotation.z);
            transform.position = new Vector3(px + 0.5f * (1 - Mathf.Cos(ry * Mathf.Deg2Rad)),
                                             transform.position.y, 
                                             pz + 0.5f * Mathf.Sin(ry * Mathf.Deg2Rad));
            if(i == frames / 2) {
                Flip();
                ry = ry + 180;
            }
            yield return new WaitForSeconds(1 / fps);
        }
        transform.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.z);
        transform.position = new Vector3(px, transform.position.y, pz);
        isFlipping = false;
    }

    public IEnumerator TopFlip() {
        isFlipping = true;
        int frames = Mathf.FloorToInt(animationTime * fps);
        float inc_rx = -180f / frames;
        float rx = transform.rotation.x;
        float py = transform.position.y;
        float pz = transform.position.z;
        for (int i = 0; i < frames; i++) {
            rx += inc_rx;
            transform.eulerAngles = new Vector3(rx, transform.rotation.y, transform.rotation.z);
            transform.position = new Vector3(transform.position.x,
                                             py - 0.75f * (1 - Mathf.Cos(rx * Mathf.Deg2Rad)),
                                             pz + 0.75f * Mathf.Sin(rx * Mathf.Deg2Rad));
            if (i == frames / 2) {
                Flip();
                rx = rx + 180;
            }
            yield return new WaitForSeconds(1 / fps);
        }
        transform.eulerAngles = new Vector3(0, transform.rotation.y, transform.rotation.z);
        transform.position = new Vector3(transform.position.x, py, pz);
        isFlipping = false;
    }

    public IEnumerator MoveTo(Vector3 to, float time) {
        int frames = Mathf.FloorToInt(time * fps);
        Vector3 step = (to - transform.position) / (float)frames;
        for(int i = 0; i < frames; i++) {
            transform.Translate(step);
            yield return new WaitForSeconds(1 / fps);
        }
        transform.position = to;
    }

    public IEnumerator MoveAndFlip(Vector3 to, float time, string direction) {
        yield return StartCoroutine(MoveTo(to, time));
        yield return new WaitForSeconds(1 / fps);
        yield return StartCoroutine(direction);
    }
}

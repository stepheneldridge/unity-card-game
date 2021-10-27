using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<int> cards;
    public Sprite emptyDeck;
    public Sprite cardBack;
    public GameObject cardPrefab;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cards = new List<int>();
        for(int i = 0; i < 40; i++) {
            cards.Add(i);
        }
        Shuffle();
    }

    // Update is called once per frame
    void Update()
    {
        if(cards.Count == 0) {
            spriteRenderer.sprite = emptyDeck;
        } else {
            spriteRenderer.sprite = cardBack;
        }
    }

    public GameObject SpawnTopCard() {
        if (cards.Count == 0) return null;
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.001f);
        GameObject card = Instantiate(cardPrefab, spawnPosition, transform.rotation);
        CardAction action = card.GetComponent<CardAction>();
        action.SetCard(cards[0], false);
        cards.RemoveAt(0);
        return card;
    }

    public void Shuffle() {
        for(int i = 0; i < cards.Count; i++) {
            int index = Random.Range(0, cards.Count);
            int temp = cards[i];
            cards[i] = cards[index];
            cards[index] = temp;
        }
    }

    public IEnumerator AddCard(GameObject card, bool wait=false) {
        CardAction action = card.GetComponent<CardAction>();
        cards.Add((int)action.suit * 10 + (int)action.type);
        if (action.faceUp) {
            yield return StartCoroutine(action.SideFlip());
        } else if(wait) {
            yield return new WaitForSeconds(action.animationTime / 2);
        }
        yield return StartCoroutine(action.MoveTo(transform.position, 0.5f));
        Destroy(card);
    }
}

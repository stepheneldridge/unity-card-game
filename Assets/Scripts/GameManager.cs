using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform[] handPositions;
    public DeckManager deck;
    private GameObject[] hand;
    public GameObject[] buttons;
    public Button redraw;
    public GameObject gameOver;
    public Text gameOverScore;
    private int buttonsLeft;
    private bool canRedraw = true;
    private int totalScore;

    void Start() {
        totalScore = 0;
        buttonsLeft = buttons.Length;
        hand = new GameObject[handPositions.Length];
        foreach(GameObject i in buttons) {
            Button b = i.GetComponent<Button>();
            Text t = i.GetComponentsInChildren<Text>()[1];
            t.text = "";
            b.onClick.AddListener(() => ButtonPressed(i.name, i, t));
        }
        DrawHand();
    }

    private int[] Sort(GameObject[] data) {
        int[] types = new int[hand.Length];
        for (int i = 0; i < hand.Length; i++) {
            CardAction action = hand[i].GetComponent<CardAction>();
            types[i] = (int)action.type;
        }
        Array.Sort(types);
        return types;
    }

    private int GetCount(int value, int[] set) {
        int count = 0;
        foreach(int i in set) {
            if (i == value) count++;
        }
        return count;
    }

    private int GetN(int[] cards, int matches, int start = 4) {
        for (int i = start; i >= 0; i--) {
            if (cards[i] == 0) return -1;
            if (GetCount(cards[i], cards) >= matches) {
                return i;
            }
        }
        return -1;
    }

    private int GetMax(int[] cards) {
        int maxIndex = -1;
        int maxValue = -1;
        for(int i = 0; i < cards.Length; i++) {
            if(cards[i] >= maxValue) {
                maxValue = cards[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    private int GetScore(string type) {
        int score = 0;
        int[] types = Sort(hand);
        int wildCards = GetCount(0, types);
        int result;
        int matches = 0;
        switch (type) {
            case "Straight":
                result = GetN(types, 2);
                if(wildCards >= 3) {
                    score = 100 + types[4] * 5;
                }else if(wildCards == 2 && result < 2) {
                    if(types[4] - types[2] <= 4) {
                        score = 100 + types[4] * 5;
                    }else if (types[3] - (types[4] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[2] - (types[3] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    }
                }else if(wildCards == 1 && result < 1) {
                    if (types[4] - types[1] <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[3] - (types[4] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[2] - (types[3] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[1] - (types[2] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    }
                } else if(result < 0) {
                    if (types[4] - types[0] <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[3] - (types[4] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[2] - (types[3] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[1] - (types[2] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    } else if (types[0] - (types[1] - 9) <= 4) {
                        score = 100 + types[4] * 5;
                    }
                }
                break;
            case "FullHouse":
                result = GetN(types, 3);
                if(result > -1) {
                    if(result == 4 && types[0] == types[1]) {
                        score = 120 + types[result] * 3 + types[0] * 2;
                    }else if(result == 2 && types[3] == types[4]) {
                        score = 120 + types[result] * 3 + types[3] * 2;
                    }else if(wildCards == 1) {
                        score = 120 + types[result] * 3 + types[4] * 2;
                    }
                }else if(wildCards == 1) {
                    result = GetN(types, 2);
                    if (result > -1) {
                        score = 120 + types[result] * 3;
                        result = GetN(types, 2, result - 2);
                        if (result > -1) {
                            score += types[result] * 2;
                        } else {
                            score = 0;
                        }
                    }
                }else if(wildCards == 2) {
                    result = GetN(types, 2);
                    if(result > -1) {
                        if(types[4] == types[result]) {
                            score = 120 + types[result] * 3 + types[2] * 2;
                        } else {
                            score = 120 + types[result] * 2 + types[4] * 3;
                        }
                    }
                }else if(wildCards == 3) {
                    result = GetN(types, 2);
                    if(result > -1) {
                        score = 120 + types[result] * 3;
                    } else {
                        score = 120 + types[4] * 3 + types[3] * 2;
                    }
                }else if(wildCards >= 4) {
                    score = 120 + types[4] * 3;
                }
                break;
            case "FiveKind":
                if(wildCards == 1) {
                    result = GetN(types, 4);
                    if(result > -1) {
                        score = 200 + types[result] * 5;
                    }
                }else if(wildCards == 2) {
                    result = GetN(types, 3);
                    if (result > -1) {
                        score = 200 + types[result] * 5;
                    }
                } else if (wildCards == 3) {
                    result = GetN(types, 2);
                    if (result > -1) {
                        score = 200 + types[result] * 5;
                    }
                } else if (wildCards >= 4) {
                    score = 200 + types[4] * 5;
                }
                break;
            case "FourKind":
                result = GetN(types, 4);
                if(result > -1) {
                    score = 160 + types[result] * 4;
                }else if(wildCards >= 3) {
                    score = 160 + types[4] * 4;
                }else if(wildCards == 2) {
                    result = GetN(types, 2);
                    if(result > -1) {
                        score = 160 + types[result] * 4;
                    }
                }else if(wildCards == 1) {
                    result = GetN(types, 3);
                    if (result > -1) {
                        score = 160 + types[result] * 4;
                    }
                }
                break;
            case "ThreeKind":
                result = GetN(types, 3);
                if(wildCards >= 3) {
                    score = 80 + types[4] * 3;
                }else if(wildCards == 2) {
                    if(result > -1) {
                        score = 80 + types[result] * 3;
                    } else {
                        result = GetN(types, 2);
                        if(result > -1) {
                            score = 80 + types[result] * 3;
                        } else {
                            score = 80 + types[4] * 3;
                        }
                    }
                }else if(wildCards == 1) {
                    if(result > -1) {
                        score = 80 + types[result] * 3;
                    } else {
                        result = GetN(types, 2);
                        if (result > -1) {
                            score = 80 + types[result] * 3;
                        }
                    }
                } else {
                    if(result > -1) {
                        score = 80 + types[result] * 3;
                    }
                }
                break;
            case "TwoPair":
                if(wildCards == 4) {
                    score = 60 + types[4] * 2;
                }else if(wildCards >= 2) {
                    result = GetN(types, 2);
                    if(result > -1) {
                        score = 60 + types[result] * 2;
                    } else {
                        score = 60 + types[4] * 2 + types[3] * 2;
                    }
                }else if(wildCards == 1) {
                    result = GetN(types, 2);
                    if(result > -1) {
                        if(result == 4) {
                            score = 60 + types[result] * 2 + types[2] * 2;
                        } else {
                            score = 60 + types[result] * 2 + types[4] * 2;
                        }
                    }
                } else {
                    result = GetN(types, 2);
                    if(result > -1) {
                        score = 60 + types[result] * 2;
                        result = GetN(types, 2, result - 2);
                        if(result > -1) {
                            score += types[result] * 2;
                        } else {
                            score = 0;
                        }
                    }
                }
                break;
            case "Pair":
                result = GetN(types, 2);
                if(result > -1) {
                    score = 40 + types[result] * 2;
                }else if(wildCards >= 1) {
                    score = 40 + types[4] * 2;
                }
                break;
            case "Junk":
                foreach(GameObject i in hand) {
                    CardAction action = i.GetComponent<CardAction>();
                    if (action.type == CardAction.Types.Blank) {
                        score += 10;
                    }
                    score += (int)action.type;
                }
                break;
            case "Spades":
                foreach (GameObject i in hand) {
                    CardAction action = i.GetComponent<CardAction>();
                    if (action.suit == CardAction.Suits.Spade) {
                        matches++;
                        if(action.type == CardAction.Types.Blank) {
                            score += 10;
                        }
                        score += (int)action.type;
                    }
                }
                score *= matches;
                break;
            case "Diamonds":
                foreach (GameObject i in hand) {
                    CardAction action = i.GetComponent<CardAction>();
                    if (action.suit == CardAction.Suits.Diamond) {
                        matches++;
                        if (action.type == CardAction.Types.Blank) {
                            score += 10;
                        }
                        score += (int)action.type;
                    }
                }
                score *= matches;
                break;
            case "Hearts":
                foreach (GameObject i in hand) {
                    CardAction action = i.GetComponent<CardAction>();
                    if (action.suit == CardAction.Suits.Heart) {
                        matches++;
                        if (action.type == CardAction.Types.Blank) {
                            score += 10;
                        }
                        score += (int)action.type;
                    }
                }
                score *= matches;
                break;
            case "Clubs":
                foreach (GameObject i in hand) {
                    CardAction action = i.GetComponent<CardAction>();
                    if (action.suit == CardAction.Suits.Club) {
                        matches++;
                        if (action.type == CardAction.Types.Blank) {
                            score += 10;
                        }
                        score += (int)action.type;
                    }
                }
                score *= matches;
                break;
        }
        return score;
    }

    private void ButtonPressed(string name, GameObject buttonObj, Text score) {
        Button button = buttonObj.GetComponent<Button>();
        button.interactable = false;
        Text t = buttonObj.GetComponentsInChildren<Text>()[0];
        t.color = Color.grey;
        int handScore = GetScore(name);
        totalScore += handScore;
        score.text = handScore.ToString();
        buttonsLeft--;
        if(buttonsLeft > 0) {
            StartCoroutine("ReplaceHand");
        } else {
            GameOver();
        }
    }

    public void GameOver() {
        gameOver.SetActive(true);
        gameOverScore.text = totalScore.ToString();
    }

    public IEnumerator ReplaceHand() {
        for (int i = 0; i < hand.Length; i++) {
            CardAction action = hand[i].GetComponent<CardAction>();
            StartCoroutine(deck.AddCard(hand[i], true));
        }
        yield return new WaitForSeconds(1);
        DrawHand();
    }

    public void RedrawHand() {
        if (canRedraw) {
            for (int i = 0; i < hand.Length; i++) {
                CardAction action = hand[i].GetComponent<CardAction>();
                if (!action.faceUp) {
                    StartCoroutine(deck.AddCard(hand[i]));
                    DrawCard(i);
                }
            }
            canRedraw = false;
            redraw.interactable = false;
        }
    }

    public void DrawCard(int index) {
        GameObject card = deck.SpawnTopCard();
        if (card) {
            hand[index] = card;
            CardAction action = card.GetComponent<CardAction>();
            StartCoroutine(action.MoveAndFlip(handPositions[index].position, 0.5f, "SideFlip"));
        }
    }

    public void DrawHand() {
        for(int i = 0; i < handPositions.Length; i++) {
            if (hand[i] == null) {
                DrawCard(i);
            }
        }
        canRedraw = true;
        redraw.interactable = true;
    }

    public void Quit() {
        Application.Quit();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

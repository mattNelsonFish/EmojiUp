using UnityEngine;
using System.Collections;

public class Deck : MonoBehaviour {

    [SerializeField] GameObject[] mDeck;

    void Start() {

    }

    public Sprite RandomCard() {
        int deckSize = mDeck.Length;
        int randomCardNum = Random.Range(0, deckSize);

        return mDeck[randomCardNum].GetComponent<SpriteRenderer>().sprite;
    }
}

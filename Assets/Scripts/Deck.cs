using UnityEngine;
using System.Collections;

public class Deck : MonoBehaviour {

    [SerializeField] GameObject[] mDeck;
    private int lastSelection = -1;

    void Start() {

    }

    public Sprite RandomCard() {
        int randomCardNum = -1;
        int deckSize = mDeck.Length;
        if(deckSize > 1){
            do {
                randomCardNum = Random.Range(0, deckSize);
            } while (lastSelection == randomCardNum);
        }
        else if (deckSize == 1) {
            randomCardNum = 0;
        }
        else {
            randomCardNum = -1;
        }
        Sprite retSprite = null;
        try {
            retSprite = mDeck[randomCardNum].GetComponent<SpriteRenderer>().sprite;
        }
        catch {
            retSprite = null;
        }
        return retSprite;
    }
}

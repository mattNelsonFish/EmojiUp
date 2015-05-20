using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Rules : MonoBehaviour {

    static string unknownRules = "We don't know those rules! Feel free to create your own :D";

    static string partyRules = "This game is super fun!\nAfter a short timer, there will be a series of emoji displayed on the screen. See if your friends can get you to say the hidden phrase. They can hint, but they can't say the words!\nGood luck!";

    [SerializeField] Text fillArea;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static string ForMode(GameManager.GameType gameType){
        string ret;
        switch (gameType) {
            case GameManager.GameType.PARTY:
                ret = partyRules;
                break;
            default:
                ret = unknownRules;
                break;
        }

        return ret;
    }

    private void FillRulesField(string contents) {
        fillArea.text = contents;
    }

    private void OnEnable() {
        FillRulesField(ForMode(GameManager.Instance.CurrentGameType));
    }
}

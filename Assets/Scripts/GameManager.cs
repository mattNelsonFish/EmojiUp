using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // gametype can be used for differntiating decks, since this will always be party play
    public enum GameType { PARTY, UNDETERMINED }
    GameType meCurrGameType;
    public GameType CurrentGameType {
        get {
            return meCurrGameType;
        }
        private set {
            meCurrGameType = value;
        }
    }

    [SerializeField] CanvasController canvasController;
    [SerializeField] HeadsUpController headsUp;

    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null) {
                if (GameObject.Find("GameManager") == null) {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField] GameObject mMenuFront;
    [SerializeField] GameObject mMenuDeckSelect;

    void Awake() {
        if (_instance != null) {
            DestroyImmediate(this.gameObject);
        }
        else {
            _instance = this;
            if (canvasController == null) {
                GameObject cc = GameObject.Find("CanvasController");
                if (cc != null && cc.GetComponent<CanvasController>() != null) {
                    canvasController = cc.GetComponent<CanvasController>();
                }
            }
            if (headsUp == null) {
                GameObject hu = GameObject.Find("HeadsUpController");
                if (hu != null && hu.GetComponent<HeadsUpController>() != null) {
                    headsUp = hu.GetComponent<HeadsUpController>();
                }
            }
        }
    }

	// Use this for initialization
    void Start() {
        canvasController.ChangeCanvas("mainmenu");
        mMenuFront.SetActive(true);
        mMenuDeckSelect.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void chooseDeck() {
        mMenuFront.SetActive(false);
        mMenuDeckSelect.SetActive(true);
    }

    void backFromDeckSelect() {
        mMenuFront.SetActive(true);
        mMenuDeckSelect.SetActive(false);
    }

    void playGame() {
        Debug.Log("Play Game pressed");
        CurrentGameType = GameType.PARTY;
        canvasController.ChangeCanvas("game");

        headsUp.StartNewGame();
    }

    void returnToPrev() {
        canvasController.ShowPrevCanvas();
        if (headsUp.CurrentGameState != HeadsUpController.GameState.NONE) {
            headsUp.ChangeGameState(HeadsUpController.GameState.PREVIOUS);
        }
    }

    void showRules() {
        Rules.ForMode(CurrentGameType);
        canvasController.AddChangeCanvas("rules");
        if (headsUp.CurrentGameState == HeadsUpController.GameState.PLAYING || headsUp.CurrentGameState == HeadsUpController.GameState.PAUSED) {
            headsUp.ChangeGameState(HeadsUpController.GameState.RULES);
        }
    }

    void quitToMenu() {
        canvasController.ChangeCanvas("mainmenu");
        mMenuFront.SetActive(true);
        mMenuDeckSelect.SetActive(false);
    }
}

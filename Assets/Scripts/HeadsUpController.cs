using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeadsUpController : MonoBehaviour {

    [SerializeField] Image readyPanel;
    [SerializeField] Image timerPanel;
    [SerializeField] Text gameTimerText;
    DeviceOrientation mPrevOrientation;

    public enum GameState {
        PLAYING, READYING, INTRO_COUNTDOWN, ENDGAME, PAUSED, RULES, NONE, PREVIOUS
    }
    private GameState mePreviousState;
    private GameState meCurrentState;
    public GameState CurrentGameState{
        get {
            return meCurrentState;
        }
        private set {
            meCurrentState = value;
        }
    }

    private float mfGameDuration = 2f;
    public float GameDuration {
        get {
            return mfGameDuration;
        }
        set {
            if (meCurrentState == GameState.NONE || meCurrentState == GameState.READYING) {
                mfGameDuration = value;
            }
        }
    }
    private float mfGameTimer = 0;
    /// <summary>
    /// Set takes time in minutes if game isn't playing. Else it will take the given value. 
    /// This is to allow easier countdown application, but still allow easy intialization
    /// </summary>
    public float GameTimer {
        get {
            return mfGameTimer;
        }
        private set {
            if (meCurrentState != GameState.PLAYING ) {
                mfGameTimer = value * 60;
            }
            else {
                mfGameTimer = value;
            }
        }
    }

    private int miNumCorrect = 0;
    public int NumberCorrect{
        get {
            return miNumCorrect;
        }
        private set {
            miNumCorrect = value;
        }
    }

    private int miNumIncorrect = 0;
    public int NumberIncorrect {
        get {
            return miNumIncorrect;
        }
        private set {
            miNumIncorrect = value;
        }
    }

    void Awake() {
        meCurrentState = GameState.NONE; 
    }
	// Use this for initialization
	void Start () {
        mPrevOrientation = DeviceOrientation.Unknown;
	}
	
	// Update is called once per frame
	void Update () {
        // check accellerometer
        switch(CurrentGameState){
            case GameState.READYING:
                break;
            
            case GameState.INTRO_COUNTDOWN:
                break;
            
            case GameState.PAUSED:
                break;

            case GameState.RULES:
                break;
            
            case GameState.PLAYING:
                GameTimer -= Time.deltaTime;
                if (gameTimerText != null) {
                    gameTimerText.text = "Time Left: " + (int)(GameTimer/60) + ":" + (int)((GameTimer/60-(int)GameTimer/60)*60); 
                }
                #if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus)) {
                    correctAnswer();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) {
                    incorrectAnswer();
                }
                #endif

                if (Input.deviceOrientation == DeviceOrientation.FaceDown && mPrevOrientation != DeviceOrientation.FaceDown) {
                    correctAnswer();
                }
                else if (Input.deviceOrientation == DeviceOrientation.FaceUp && mPrevOrientation != DeviceOrientation.FaceUp) {
                    incorrectAnswer();
                }
                break;

            case GameState.ENDGAME:
                break;
        }
	}

    public void ChangeGameState(GameState newGS) {
        if(newGS!= GameState.PREVIOUS){
            mePreviousState = CurrentGameState;
            CurrentGameState = newGS;
        }
        switch (newGS) {
            case GameState.READYING:
                showReadyPanel();
                if (mePreviousState == GameState.NONE) {
                    ResetGame();
                }
                break;

            case GameState.INTRO_COUNTDOWN:
                showTimerPanel(3.5f, true, 0.25f);
                break;

            case GameState.PAUSED:
                break;

            case GameState.PLAYING:
                break;

            case GameState.PREVIOUS:
                ChangeGameState(mePreviousState);
                break;

            case GameState.ENDGAME:
                EndGame();
                break;
        }
    }



    public void StartNewGame() {
        // GameManager.Instance.CurrentGameType;
        ChangeGameState(GameState.READYING);
        ResetGame();
    }

    private void showReadyPanel() {
        readyPanel.gameObject.SetActive(true);
    }

    private void showIntroTimer() {
        ChangeGameState(GameState.INTRO_COUNTDOWN);
    }

    private void showTimerPanel(float duration, bool fadeout, float fadeDuration) {
        if (timerPanel != null && !timerPanelRunning) {
            StartCoroutine(runTimerPanel(duration, fadeout, fadeDuration));
        }
    }

    /// <summary>
    /// Assumes the timerPanel exists.
    /// </summary>
    /// <returns></returns>
    bool timerPanelRunning = false;
    IEnumerator runTimerPanel(float duration, bool fadeout, float fadeDuration) {
        timerPanelRunning = true;
        timerPanel.gameObject.SetActive(true);
        float totalTime = 0;
        int countNum = (int)duration;
        Text timerDisplay = null;
        if (timerPanel.transform.FindChild("TimerDisplay") != null) {
            timerDisplay = (timerPanel.transform.FindChild("TimerDisplay").GetComponent<Text>() as Text);
            timerDisplay.text = "" + countNum;
        }
        while (totalTime < duration) {
            totalTime += Time.deltaTime;
            if (timerDisplay != null && duration - totalTime < (float)countNum-0.5f && countNum > 0) {
                countNum--;
                timerDisplay.text = ""+countNum;
            }
            yield return Time.deltaTime;
        }
        
        if (fadeout) {
            totalTime = 0;
            float initialAlpha = timerPanel.color.a;
            float targetAlpha = 0;
            Color initialColor = timerPanel.color;
            Color newColor = initialColor;
            while (totalTime < fadeDuration) {
                totalTime += Time.deltaTime;
                float durationRatio = totalTime / fadeDuration;
                newColor.a = Mathf.Lerp(initialAlpha, targetAlpha, durationRatio);
                timerPanel.color = newColor;
                yield return Time.deltaTime;
            }
            timerPanel.gameObject.SetActive(false);
            timerPanel.color = initialColor;
        }
        else {
            timerPanel.gameObject.SetActive(false);
        }
        timerPanelRunning = false;
        ChangeGameState(GameState.PLAYING);
    }

    void startPlayingImmediate() {
        ChangeGameState(GameState.PLAYING);
    }

    void correctAnswer() {
        Debug.Log("Correct answer");
    }

    void incorrectAnswer() {
        Debug.Log("Incorrect answer");
    }

    public void ResetGame() {
        GameTimer = GameDuration;
        NumberCorrect = 0;
        NumberIncorrect = 0;
    }

    public void EndGame() {
        Debug.Log("You got " + NumberCorrect + " correct, and " + NumberIncorrect + " incorrect in " + GameDuration + " minutes");


    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeadsUpController : MonoBehaviour {

    [SerializeField] Image readyPanel;
    [SerializeField] Image timerPanel;
    [SerializeField] Image endgamePanel;
    [SerializeField] Text gameTimerText;
    [SerializeField] GameObject hudParent;
    DeviceOrientation mPrevOrientation;

    public enum Difficulty { EASY, HARD }
    Difficulty mCurrentDifficulty = Difficulty.EASY;
    public Difficulty CurrentDifficulty {
        get {
            return mCurrentDifficulty;
        }
        set {
            if (meCurrentState == GameState.NONE || meCurrentState == GameState.READYING) {
                mCurrentDifficulty = value;
            }
        }
    }

    private string mDeckName = "";
    public string UsingDeck {
        get {
            return mDeckName;
        }
        set {
            if (meCurrentState == GameState.NONE || meCurrentState == GameState.READYING) {
                mDeckName = value;
            }
        }
    }

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

    private float mfGameDuration = 0.5f;
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

    Deck mCurrDeck;
    public Deck CurrentDeck {
        get {
            return mCurrDeck;
        }
        set {
            if (meCurrentState == GameState.NONE || meCurrentState == GameState.READYING) {
                mCurrDeck = value;
            }
        }
    }

    bool mfEndSooner = false;
    public bool EndConfirmed {
        get {
            return mfEndSooner;
        }
        set {
            if (CurrentGameState == GameState.ENDGAME) {
                mfEndSooner = value;
            }
        }
    }

    const float END_DURATION_MIN = 2f;

    float mfAnswerTimer = 0;
    const float ANSWER_DELAY = 0.7f;

    [SerializeField] Image mLeftImage;
    [SerializeField] Image mMiddleImage;
    [SerializeField] Image mRightImage;
    [SerializeField] Image mLeftImageBack;
    [SerializeField] Image mMiddleImageBack;
    [SerializeField] Image mRightImageBack;
    [SerializeField] Image mAnswerBack;
    
    [SerializeField] Sprite mRightSprite;
    [SerializeField] Sprite mWrongSprite;

    [SerializeField] Text mEndGameText;

    void Awake() {
        meCurrentState = GameState.NONE;
        showEndgameStats(false);
        showHUD(false, true);
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
                CurrentDeck = (Resources.Load("Decks/" + UsingDeck) as GameObject).GetComponent<Deck>();
                showCards(false);
                showHUD(false);
                break;
            
            case GameState.INTRO_COUNTDOWN:
                if (introTimerPlaying && !timerPanelRunning) {
                    introTimerPlaying = false;
                    Sprite tempSprite = CurrentDeck.RandomCard();

                    if (tempSprite != null) {
                        mMiddleImage.sprite = tempSprite;
                    }
                    else {
                        GameManager.Instance.QuitToMenu();
                    }
                    showAnswerBack(false);
                    ChangeGameState(GameState.PLAYING);
                }
                break;
            
            case GameState.PAUSED:
                break;

            case GameState.RULES:
                break;
            
            case GameState.PLAYING:
                GameTimer -= Time.deltaTime;
                showHUD(true);
                if (GameTimer < 0) {
                    showCards(false);
                    ChangeGameState(GameState.ENDGAME);
                }
                else {
                    showCards(true);
                    if (gameTimerText != null) {
                        int minutes = (int)(GameTimer / 60);
                        int seconds = (int)((GameTimer / 60 - (int)GameTimer / 60) * 60);
                        
                        gameTimerText.text = "Time Left: " + minutes + ":" + (seconds < 10 ? "0"+seconds : ""+seconds );
                    }
                    if (mfAnswerTimer > 0) {
                        mfAnswerTimer -= Time.deltaTime;
                    }
                    else {
                        #if UNITY_EDITOR
                        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus)) {
                            answer(true);
                        }
                        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus)) {
                            answer(false);
                        }
                        #endif

                        //Debug.Log("Orientation: " + Input.deviceOrientation);
                        if (Input.deviceOrientation == DeviceOrientation.FaceDown && mPrevOrientation != DeviceOrientation.FaceDown) {
                            //Debug.Log("(FaceDown) Prev Orientation: " + mPrevOrientation);
                            mPrevOrientation = DeviceOrientation.FaceDown;
                            answer(true);
                        }
                        else if (Input.deviceOrientation == DeviceOrientation.FaceUp && mPrevOrientation != DeviceOrientation.FaceUp) {
                            //Debug.Log("(FaceUp) Prev Orientation: " + mPrevOrientation);
                            mPrevOrientation = DeviceOrientation.FaceUp;
                            answer(false);
                        }
                        else if (Input.deviceOrientation != DeviceOrientation.FaceUp && Input.deviceOrientation != DeviceOrientation.FaceDown) {
                            mPrevOrientation = DeviceOrientation.Unknown;
                        }
                    }
                }
                break;

            case GameState.ENDGAME:
                break;
        }
	}

    bool introTimerPlaying = false;
    public void ChangeGameState(GameState newGS) {
        if(newGS!= GameState.PREVIOUS){
            mePreviousState = CurrentGameState;
            CurrentGameState = newGS;
        }
        if (CurrentGameState != GameState.INTRO_COUNTDOWN) {
            introTimerPlaying = false;
        }
        switch (newGS) {
            case GameState.READYING:
                showReadyPanel();
                if (mePreviousState == GameState.NONE) {
                    ResetGame();
                }

                mPrevOrientation = Input.deviceOrientation;
                break;

            case GameState.INTRO_COUNTDOWN:
                    introTimerPlaying = true;
                    showTimerPanel(3.5f, true, 0.25f);
                    showHUD(false);

                    mPrevOrientation = Input.deviceOrientation;
                break;

            case GameState.PAUSED:
                break;

            case GameState.PLAYING:
                mPrevOrientation = Input.deviceOrientation;
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

        showHUD(true);
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
    }

    void startPlayingImmediate() {
        ChangeGameState(GameState.PLAYING);
    }

    void answer(bool correct) {
        if (correct) {
            Debug.Log("Correct answer");
            NumberCorrect++;
        }
        else {
            Debug.Log("Incorrect answer");
            NumberIncorrect++;
        }
        setAnswerBack(correct);

        StartCoroutine(RandomCardIn(0.5f));
    }

    IEnumerator RandomCardIn(float timeTillCard) {
        showAnswerBack(true);
        while (timeTillCard > 0) {
            timeTillCard -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            mfAnswerTimer = ANSWER_DELAY;
        }
        showAnswerBack(false);

        Sprite tempSprite = CurrentDeck.RandomCard();
        if (tempSprite != null) {
            mMiddleImage.sprite = tempSprite;
        }
        else {
            GameManager.Instance.QuitToMenu();
        }

        mfAnswerTimer = ANSWER_DELAY;
    }

    /// <summary>
    /// Prepares the back image to show the correct response.
    /// </summary>
    /// <param name="correct"></param>
    void setAnswerBack(bool? correct) {
        switch(correct){
            case true:
                //mMiddleImageBack.sprite = mLeftImageBack.sprite = mRightImageBack.sprite = 
                mAnswerBack.sprite = mRightSprite;
                break;
            case false:
                //mMiddleImageBack.sprite = mLeftImageBack.sprite = mRightImageBack.sprite =
                mAnswerBack.sprite = mWrongSprite;
                break;
            default:
                showAnswerBack(false);
                break;
        }
    }

    void showAnswerBack(bool show) {
       // mLeftImageBack.gameObject.SetActive(CurrentDifficulty == Difficulty.EASY ? false : show);
       // mMiddleImageBack.gameObject.SetActive(show);
       // mRightImageBack.gameObject.SetActive(CurrentDifficulty == Difficulty.EASY ? false : show);
        mAnswerBack.gameObject.SetActive(show);
    }

    void showCards(bool show) {
        mLeftImage.gameObject.SetActive(CurrentDifficulty == Difficulty.EASY ? false : show);
        mMiddleImage.gameObject.SetActive(show);
        mRightImage.gameObject.SetActive(CurrentDifficulty == Difficulty.EASY ? false : show);
    }

    public void ResetGame() {
        mfEndSooner = false;
        GameTimer = GameDuration;
        NumberCorrect = 0;
        NumberIncorrect = 0;
        UsingDeck = "";
        showCards(false);
        showEndgameStats(false);
        showHUD(true);
        mPrevOrientation = Input.deviceOrientation;
    }

    public void EndGame() {
        float timeTillExit = 0f;
        if(CurrentGameState != GameState.ENDGAME){
            timeTillExit = 3f;
        }
        StartCoroutine(EndGameIn(timeTillExit));
    }

    private IEnumerator EndGameIn(float timeTillExit) {
        showCards(false);
        showHUD(false);
        showEndgameStats(true);
        yield return new WaitForSeconds(END_DURATION_MIN);
        while (!EndConfirmed) {
            yield return new WaitForSeconds(0.25f);
        }
        GameManager.Instance.QuitToDeckSelection();
        showEndgameStats(false);
        showHUD(true);
    }

    void showEndgameStats(bool show) {
        if (show) {
            fillEndgameStats();
        }
        endgamePanel.gameObject.SetActive(show);
    }

    void fillEndgameStats() {
        mEndGameText.text = "Correct: " + NumberCorrect + "\nWrong: " + NumberIncorrect;
    }

    void showHUD(bool show) {
        hudParent.SetActive(show);
    }

    void showHUD(bool show, bool applyAll) {
        showHUD(true);
        if (applyAll) {
            showGameTimer(true);
            showQuitButton(true);
            showRulesButton(true);
        }
    }

    void showGameTimer(bool show) {
        if (hudParent.transform.FindChild("GameTimer")) {
            hudParent.transform.FindChild("GameTimer").gameObject.SetActive(show);
        }
    }

    void showQuitButton(bool show) {
        if (hudParent.transform.FindChild("Quit")) {
            hudParent.transform.FindChild("Quit").gameObject.SetActive(show);
        }
    }

    void showRulesButton(bool show) {
        if (hudParent.transform.FindChild("Rules")) {
            hudParent.transform.FindChild("Rules").gameObject.SetActive(show);
        }
    }
}

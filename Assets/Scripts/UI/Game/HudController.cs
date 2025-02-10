using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour
{
    private PlayerController playerScript;
    private JoystickController bombingJoystickScript;
    private LoopChecker LoopCheckerScript;

    private AudioSource menuInSound, menuOutSound, coinSound;

    private TMP_Text pointsCounter;

    [SerializeField] private GameObject pointsSumPrefab, pointsPrefab, epicComboLight;

    private Transform cameraTransform, pauseMenu, deadMenu, morePointsContainer, pointsContainer;

    private Camera cameraComponent;

    private int points, actualLevel;
    public int GiveActualLevel
    {
        set { actualLevel = value; }
    }

    [SerializeField] private float everyPointAddInterval;
    private float deadPanelOutProgress = -1, lastDeadPanelYPos, lastTimeScale;
    public float actualTimescale = 1;

    private bool isPaused, pretendsToBePaused, changingScene, isInOptions;
    public bool IsPaused
    {
        set { isPaused = value; }
        get { return isPaused; }
    }
    public bool SetChangingScene
    {
        set { changingScene = value; }
    }
    public bool SetIsInOptions
    {
        set { isInOptions = value; }
    }
    public bool PretendsToBePaused
    {
        get { return pretendsToBePaused; }
        set { pretendsToBePaused = value; }
    }

    private void Start()
    {
        cameraTransform = GameObject.Find("Camera").transform;
        pauseMenu = GameObject.Find("Canvas/pauseMenu").transform;
        deadMenu = GameObject.Find("Canvas/deadMenu/BothMenusContainer/deadMenu").transform;
        morePointsContainer = GameObject.Find("Canvas/MorePointsContainer").transform;
        pointsContainer = GameObject.Find("NotPhysicElementsContainer").transform;

        menuInSound = GameObject.Find("UIsounds/MenuInSound").GetComponent<AudioSource>();
        menuOutSound = GameObject.Find("UIsounds/MenuOutSound").GetComponent<AudioSource>();
        coinSound = GameObject.Find("UIsounds/CoinSound").GetComponent<AudioSource>();

        pointsCounter = GameObject.Find("Canvas/PointsCounter").GetComponent<TMP_Text>();

        pauseMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        bombingJoystickScript = GameObject.Find("Canvas/TouchControllers/Controllers/Bombing Joystick").GetComponent<JoystickController>();
        LoopCheckerScript = GameObject.Find("Player").GetComponent<LoopChecker>();

        cameraComponent = cameraTransform.GetComponentInChildren<Camera>();

        deadMenu.gameObject.SetActive(false);

        deadMenu.localPosition = new Vector3(0, -550, 0);

        pretendsToBePaused = false;

        //PlayerPrefs.SetInt("Highscore", 0);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause")) PauseManager();

        AdjustTimeScale();

        CameraPauseAdjust();
        DeadPanelAdjust();

        SendVariableInfo();
    }

    public void ChangePointsValue(int sumPoints, Vector3 pointsPos, int combo, int type) // Combo >= 1;   Type -->  0 = Default attack, 1 = Single Loop, 2 = Combo Loop
    {
        if (type != 1 && LoopCheckerScript.PointsScored()) // First, detect if player is looping to DOUBLE the score in that case.
        {
            sumPoints *= 2;
            type = 2;
        }

        points += sumPoints;
        pointsPos = new Vector3(pointsPos.x, pointsPos.y, -15);

        if (combo > 1) ShowComboText(combo, pointsPos);
        if (type != 0) ShowAdditionalTextOnScore(type, combo > 1, pointsPos);

        Instantiate(pointsPrefab, pointsPos, Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>().text = "+ " + sumPoints;
        if (type == 2) Instantiate(epicComboLight, pointsPos, Quaternion.identity, pointsContainer);

        StartCoroutine(SlowlyAddPoints(sumPoints));
    }

    void ShowComboText(int comboNum, Vector3 pointsPosition)
    {
        TMP_Text comboText = Instantiate(pointsPrefab, pointsPosition + new Vector3(0, 1.3f), Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>();
        comboText.fontSize /= 2;
        comboText.text = "COMBO x" + comboNum;
    }

    void ShowAdditionalTextOnScore(int caseNumber, bool comboTextAppears, Vector3 pointsPosition)
    {
        string textToShow;
        float textSizeMultiplier;

        switch (caseNumber)
        {
            case 1:
                textToShow = "Loop";
                textSizeMultiplier = 0.5f;
                break;
            case 2:
                textToShow = "LOOP! x2";
                textSizeMultiplier = 0.7f;
                break;
            default:
                Debug.LogWarning("Point addition case number " + caseNumber + ", (which you entered) does not exist, stupid monkey. These are the possible ones: 0 = Default attack, 1 = Single Loop, 2 = Combo Loop");
                // If you're wondering how you got here, go to where the "caseNumber" variable (coming from the "type" variable in the "ChangePointsValue" function)
                // value is originally entered and replace it with a valid one (the Log Warning explains which ones are valid).
                return;
        }

        TMP_Text additionalText = Instantiate(pointsPrefab, pointsPosition + new Vector3(0, comboTextAppears ? 2.5f : 1.2f), Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>();
        additionalText.text = textToShow;
        additionalText.fontSize *= textSizeMultiplier;
    }

    private IEnumerator SlowlyAddPoints(int pointsIsum)
    {
        GameObject pointsSumAnimationObj = Instantiate(pointsSumPrefab, morePointsContainer);
        pointsSumAnimationObj.transform.GetChild(1).GetComponent<TMP_Text>().text = "+" + pointsIsum;

        yield return new WaitForSeconds(0.53f); // --> Time it takes for the animation to reach the ending point

        for (int idx = 1; idx <= pointsIsum; idx++)
        {
            pointsCounter.text = (points - pointsIsum + idx).ToString(); // As "points" is not changed progressively, I have to substract the points THIS explosion sums
            // from the total points I have, this way it's like summing 0 points to the previous score. Then add "idx", which DOES change progressively, to have
            // A nice scale of +1s from the previous score to the actual one.

            coinSound.Stop();
            coinSound.pitch = Random.Range(0.9f, 1.05f);
            coinSound.Play();

            yield return new WaitForSeconds(everyPointAddInterval);
        }

        yield return new WaitForSeconds(1);

        Destroy(pointsSumAnimationObj);
    }

    public void PauseManager()
    {
        if (changingScene || isInOptions) return;

        if (pretendsToBePaused) Continue();
        else Pause();
    }

    public void Pause()
    {
        if (PlayerController.dead) return;
        
        pretendsToBePaused = true;

        Cursor.visible = true;

        menuInSound.Play();
    }

    public void Continue()
    {
        pretendsToBePaused = false;

        Cursor.visible = false;

        menuOutSound.Play();
    }

    void AdjustTimeScale()
    {
        if (pretendsToBePaused && actualTimescale > 0) actualTimescale -= Time.unscaledDeltaTime * (actualTimescale + 0.003f) * 10;
        if (!pretendsToBePaused && actualTimescale < 1) actualTimescale += Time.unscaledDeltaTime * (1 - actualTimescale + 0.003f) * 10;

        if (actualTimescale <= 0)
        {
            if (actualTimescale < 0) actualTimescale = 0;
            isPaused = true;
        }
        if (actualTimescale >= 1)
        {
            if (actualTimescale > 1) actualTimescale = 1;
            isPaused = false;
        }

        if (lastTimeScale != 1) Time.timeScale = actualTimescale;
        lastTimeScale = actualTimescale;
    }

    void CameraPauseAdjust()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(5.8f, 7, actualTimescale);
        cameraTransform.GetChild(0).localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-8, 0, actualTimescale));

        if (actualTimescale >= 1) pauseMenu.gameObject.SetActive(false);
        else
        {
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.localPosition = new Vector3(0, Mathf.Lerp(0, -550, actualTimescale), 0);
        }
    }

    void DeadPanelAdjust()
    {
        if (deadPanelOutProgress < 0)
        {
            deadMenu.gameObject.SetActive(false);
            return;
        }

        deadMenu.gameObject.SetActive(true);

        if (deadPanelOutProgress < 1) deadPanelOutProgress += Time.deltaTime * (1 - deadPanelOutProgress) * 8;
        else if (deadPanelOutProgress > 1) deadPanelOutProgress = 1;

        deadMenu.parent.parent.localPosition += new Vector3(0, Mathf.Lerp(-550, 0, deadPanelOutProgress) - lastDeadPanelYPos, 0);
        lastDeadPanelYPos = deadMenu.parent.parent.localPosition.y;
    }

    public void DeadPanelOut()
    {
        deadPanelOutProgress = 0;
        menuInSound.Play();
        if (!PlayerPrefs.HasKey("GamesPlayed")) PlayerPrefs.SetInt("GamesPlayed", 1);
        else
        {
            PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed") + 1);
            if (PlayerPrefs.GetInt("GamesPlayed") == 10) deadMenu.parent.GetComponent<QuizMenuAppear>().QuizMenuStartAnim();
        }

        lastDeadPanelYPos = deadMenu.parent.parent.localPosition.y;
        DeadPanelStats();
    }

    private void DeadPanelStats()
    {
        deadMenu.Find("Score").GetComponent<TMP_Text>().text = points.ToString();

        if (!PlayerPrefs.HasKey("Highscore")) PlayerPrefs.SetInt("Highscore", 0);
        if (!PlayerPrefs.HasKey("HighscoreLevel" + actualLevel)) PlayerPrefs.SetInt("HighscoreLevel" + actualLevel, 0);
        if (!PlayerPrefs.HasKey("TotalPointsLevel" + actualLevel)) PlayerPrefs.SetInt("TotalPointsLevel" + actualLevel, 0);
        if (!PlayerPrefs.HasKey("TotalGamesLevel" + actualLevel)) PlayerPrefs.SetInt("TotalGamesLevel" + actualLevel, 0);

        PlayerPrefs.SetInt("TotalPointsLevel" + actualLevel, PlayerPrefs.GetInt("TotalPointsLevel" + actualLevel) + points);
        PlayerPrefs.SetInt("TotalGamesLevel" + actualLevel, PlayerPrefs.GetInt("TotalGamesLevel" + actualLevel) + 1);

        if (points > PlayerPrefs.GetInt("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", points);
            deadMenu.Find("NewHighscore/Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
            StartCoroutine(MakeMenuShake());
        }
        else
        {
            deadMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
            deadMenu.Find("NewHighscore").gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("HighscoreLevel" + actualLevel) < points)
        {
            PlayerPrefs.SetInt("HighscoreLevel" + actualLevel, points);
        }
    }

    IEnumerator MakeMenuShake()
    {
        yield return new WaitForSeconds(0.64f);
        deadMenu.Find("NewHighscore").GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.06f);
        deadMenu.parent.parent.GetComponent<ShakeController>().Shake(1);
    }

    private void SendVariableInfo()
    {
        playerScript.SetIsPaused(pretendsToBePaused);
        bombingJoystickScript.SetIsPaused = pretendsToBePaused;

        InGameSound.timeSpeed = actualTimescale;
        MusicSound.timeSpeed = actualTimescale;
    }
}

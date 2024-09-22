using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour
{
    private PlayerController playerScript;
    private JoystickController bombingJoystickScript;

    private AudioSource menuInSound, menuOutSound;

    [SerializeField] private GameObject pointsSumPrefab;

    private Transform cameraTransform, pauseMenu, deadMenu, morePointsContainer;

    private Camera cameraComponent;

    private int points, actualLevel;
    public int GiveActualLevel
    {
        set { actualLevel = value; }
    }

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

        menuInSound = GameObject.Find("UIsounds/MenuInSound").GetComponent<AudioSource>();
        menuOutSound = GameObject.Find("UIsounds/MenuOutSound").GetComponent<AudioSource>();

        pauseMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        bombingJoystickScript = GameObject.Find("Canvas/TouchControllers/Controllers/Bombing Joystick").GetComponent<JoystickController>();

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

    public void ChangePointsValue(int sumPoints)
    {
        points += sumPoints;

        PointsSumAnimController newPointsSum = Instantiate(pointsSumPrefab, morePointsContainer).GetComponent<PointsSumAnimController>();
        newPointsSum.SetPoints = points;
        newPointsSum.SetPointsIsum = sumPoints;
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
        deadMenu.parent.parent.GetComponent<ShakeController>().Shake();
    }

    private void SendVariableInfo()
    {
        playerScript.SetIsPaused(pretendsToBePaused);
        bombingJoystickScript.SetIsPaused = pretendsToBePaused;

        InGameSound.timeSpeed = actualTimescale;
        MusicSound.timeSpeed = actualTimescale;
    }
}

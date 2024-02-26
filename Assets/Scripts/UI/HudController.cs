using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HudController : MonoBehaviour
{
    private PlayerController playerScript;
    private MapGenerator mapGeneratorScript;
    private PlayerSoundsController playerSoundsScript;
    private TargetController targetScript;

    private AudioSource menuInSound, menuOutSound;

    [SerializeField] private GameObject pointsSumPrefab;

    private Transform cameraTransform, pauseMenu, deadMenu, morePointsContainer;

    private Camera cameraComponent;

    private int points;

    private float deadPanelOutProgress = -1;
    public float actualTimescale = 1;

    private bool isPaused, pretendsToBePaused, changingScene;
    public bool SetIsPaused
    {
        set { isPaused = value; }
    }
    public bool SetChangingScene
    {
        set { changingScene = value; }
    }

    private void Start()
    {
        cameraTransform = GameObject.Find("Camera").transform;
        pauseMenu = GameObject.Find("Canvas/pauseMenu").transform;
        deadMenu = GameObject.Find("Canvas/deadMenu").transform;
        morePointsContainer = GameObject.Find("Canvas/MorePointsContainer").transform;

        menuInSound = GameObject.Find("UIsounds/MenuInSound").GetComponent<AudioSource>();
        menuOutSound = GameObject.Find("UIsounds/MenuOutSound").GetComponent<AudioSource>();

        pauseMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerSoundsScript = GameObject.Find("Player/Sounds").GetComponent<PlayerSoundsController>();
        mapGeneratorScript = GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>();
        targetScript = GameObject.Find("Target").GetComponent<TargetController>();

        cameraComponent = cameraTransform.GetComponentInChildren<Camera>();

        deadMenu.gameObject.SetActive(false);

        deadMenu.localPosition = new Vector3(0, -550, 0);

        pretendsToBePaused = false;

        //PlayerPrefs.SetInt("Highscore", 0);
    }

    private void Update()
    {
        if (Input.GetButtonUp("Pause") && !changingScene) PauseManager();

        AdjustTimeScale();
        //AdjustOptionPanelProgress();

        CameraPauseAdjust();
        //OptionPanelAdjust();
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

    private void PauseManager()
    {
        if (pretendsToBePaused) Continue();
        else Pause();
    }

    public void Pause()
    {
        if (PlayerController.dead) return;
        
        pretendsToBePaused = true;

        targetScript.SetIsPaused = true;
        Cursor.visible = true;

        menuInSound.Play();
    }

    public void Continue()
    {
        pretendsToBePaused = false;

        targetScript.SetIsPaused = false;
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

        Time.timeScale = actualTimescale;
    }

    /*void AdjustOptionPanelProgress()
    {
        if (isInOptions && optionsPanelProgress < 1) optionsPanelProgress += Time.unscaledDeltaTime * (1 - optionsPanelProgress + 0.003f) * 10;
        if (!isInOptions && optionsPanelProgress > 0) optionsPanelProgress -= Time.unscaledDeltaTime * (optionsPanelProgress + 0.003f) * 10;

        if (optionsPanelProgress < 0) optionsPanelProgress = 0;
        if (optionsPanelProgress > 1) optionsPanelProgress = 1;
    }*/

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

    /*void OptionPanelAdjust()
    {
        if (optionsPanelProgress <= 0)
        {
            if (canDisableOptionsMenu) optionsMenu.gameObject.SetActive(false);
            return;
        }
        else optionsMenu.gameObject.SetActive(true);

        optionsMenu.localPosition = new Vector2(0, Mathf.Lerp(-550, 0, optionsPanelProgress));

        if (isPaused && pretendsToBePaused) pauseMenu.localPosition = new Vector2(0, Mathf.Lerp(0, 550, optionsPanelProgress));
        if (deadPanelOutProgress >= 0) deadMenu.localPosition = new Vector2(0, Mathf.Lerp(0, 550, optionsPanelProgress));
    }*/

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

        deadMenu.localPosition = new Vector3(0, Mathf.Lerp(-550, 0, deadPanelOutProgress), 0);
    }

    public void DeadPanelOut()
    {
        deadPanelOutProgress = 0;
        menuInSound.Play();

        DeadPanelStats();
    }

    private void DeadPanelStats()
    {
        deadMenu.Find("Score").GetComponent<TMP_Text>().text = points.ToString();

        if (!PlayerPrefs.HasKey("Highscore")) PlayerPrefs.SetInt("Highscore", 0);
        if (points > PlayerPrefs.GetInt("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", points);
            deadMenu.Find("Highscore").SetParent(deadMenu.Find("NewHighscore"));
            deadMenu.Find("NewHighscore/Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
            StartCoroutine(MakeMenuShake());
        }
        else
        {
            deadMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
            deadMenu.Find("NewHighscore").gameObject.SetActive(false);
        }
    }

    IEnumerator MakeMenuShake()
    {
        yield return new WaitForSeconds(0.64f);
        deadMenu.Find("NewHighscore").GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.06f);
        deadMenu.GetComponent<ShakeController>().Shake();
    }

    private void SendVariableInfo()
    {
        playerScript.SetIsPaused = pretendsToBePaused;
        mapGeneratorScript.SetIsPaused = isPaused;
        playerSoundsScript.SetTimeSpeed = actualTimescale;
    }
}

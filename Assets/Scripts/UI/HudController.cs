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

    private TMP_Text pauseHighscore;
    
    [SerializeField] private GameObject pointsSumPrefab;

    private Transform canvasTransform, cameraTransform, pauseMenu, deadMenu, optionsMenu;

    private Camera cameraComponent;

    private int points;

    private float deadPanelOutProgress, optionsPanelProgress;
    public float actualTimescale = 1;

    private bool isPaused, isInOptions, pretendsToBePaused, canDisableOptionsMenu;
    public bool SetInOptions
    {
        set { isInOptions = value; }
    }
    public bool SetIsPaused
    {
        set { isPaused = value; }
    }
    public bool SetCanDisableOptionsMenu
    {
        set { canDisableOptionsMenu = value; }
    }

    private void Start()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
        cameraTransform = GameObject.Find("Camera").transform;
        pauseMenu = canvasTransform.Find("pauseMenu").transform;
        deadMenu = canvasTransform.Find("deadMenu").transform;
        optionsMenu = canvasTransform.Find("optionsMenu").transform;
        
        pauseMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        playerSoundsScript = GameObject.Find("Player/Sounds").GetComponent<PlayerSoundsController>();
        mapGeneratorScript = GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>();
        targetScript = GameObject.Find("Target").GetComponent<TargetController>();

        cameraComponent = cameraTransform.GetComponentInChildren<Camera>();

        deadMenu.gameObject.SetActive(false);

        deadMenu.localPosition = new Vector3(0, -550, 0);
        optionsMenu.localPosition = new Vector3(0, -550, 0);

        pretendsToBePaused = false;
        isInOptions = false;
        deadPanelOutProgress = -1;
    }
    private void Update()
    {
        if (Input.GetButtonUp("Pause")) PauseManager();

        AdjustTimeScale();
        AdjustOptionPanelProgress();

        CameraPauseAdjust();
        OptionPanelAdjust();
        DeadPanelAdjust();

        SendVariableInfo();
    }

    public void ChangePointsValue(int sumPoints)
    {
        points += sumPoints;

        PointsSumAnimController newPointsSum = Instantiate(pointsSumPrefab, canvasTransform).GetComponent<PointsSumAnimController>();
        newPointsSum.SetPoints = points;
        newPointsSum.SetPointsIsum = sumPoints;
    }

    private void PauseManager()
    {
        if (isInOptions)
        {
            isInOptions = false;
            return;
        }

        if (pretendsToBePaused) Continue();
        else Pause();
    }

    public void Pause()
    {
        if (PlayerController.dead) return;
        
        pretendsToBePaused = true;

        targetScript.SetIsPaused = true;
        Cursor.visible = true;
    }

    public void Continue()
    {
        pretendsToBePaused = false;

        targetScript.SetIsPaused = false;
        Cursor.visible = false;
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

    void AdjustOptionPanelProgress()
    {
        if (isInOptions && optionsPanelProgress < 1) optionsPanelProgress += Time.unscaledDeltaTime * (1 - optionsPanelProgress + 0.003f) * 10;
        if (!isInOptions && optionsPanelProgress > 0) optionsPanelProgress -= Time.unscaledDeltaTime * (optionsPanelProgress + 0.003f) * 10;

        if (optionsPanelProgress < 0) optionsPanelProgress = 0;
        if (optionsPanelProgress > 1) optionsPanelProgress = 1;
    }

    void CameraPauseAdjust()
    {
        cameraComponent.orthographicSize = Mathf.Lerp(5.8f, 7, actualTimescale);
        cameraTransform.GetChild(0).localEulerAngles = new Vector3(0, 0, Mathf.Lerp(-8, 0, actualTimescale));

        if (actualTimescale >= 1 || optionsPanelProgress >= 1) pauseMenu.gameObject.SetActive(false);
        else
        {
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.localPosition = new Vector3(0, Mathf.Lerp(0, -550, actualTimescale), 0);
        }
    }

    void OptionPanelAdjust()
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
    }

    void DeadPanelAdjust()
    {
        if (deadPanelOutProgress < 0 || optionsPanelProgress >= 1)
        {
            deadMenu.gameObject.SetActive(false);
            return;
        }

        deadMenu.gameObject.SetActive(true);

        if (deadPanelOutProgress < 1) deadPanelOutProgress += Time.deltaTime * (1 - deadPanelOutProgress) * 8;
        else if (deadPanelOutProgress > 1) deadPanelOutProgress = 1;

        if (optionsPanelProgress <= 0) deadMenu.localPosition = new Vector3(0, Mathf.Lerp(-550, 0, deadPanelOutProgress), 0);
    }

    public void DeadPanelOut()
    {
        deadPanelOutProgress = 0;
        Cursor.visible = true;

        DeadPanelStats();
    }

    private void DeadPanelStats()
    {
        deadMenu.Find("Score").GetComponent<TMP_Text>().text = points.ToString();

        if (!PlayerPrefs.HasKey("Highscore")) PlayerPrefs.SetInt("Highscore", 0);
        if (points > PlayerPrefs.GetInt("Highscore")) PlayerPrefs.SetInt("Highscore", points);
        else deadMenu.Find("NewHighscore").gameObject.SetActive(false);

        deadMenu.Find("Highscore").GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
    }

    private void SendVariableInfo()
    {
        playerScript.SetIsPaused = pretendsToBePaused;
        mapGeneratorScript.SetIsPaused = isPaused;
        playerSoundsScript.SetTimeSpeed = actualTimescale;
    }
}

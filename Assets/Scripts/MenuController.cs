using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    public static event OnKeyPress onKeyPressed;
    public delegate void OnKeyPress(KeyCode keyCode);

    public static event OnLevelSelect onLevelSelect;
    public delegate void OnLevelSelect(int levelIndex);

    public static event OnMusicToggle onMusicToggle;
    public delegate void OnMusicToggle();

    public static event OnSFXToggle onSFXToggle;
    public delegate void OnSFXToggle();

    public Canvas menu;

    public LevelsSO levels;

    public GameObject buttonPrefab;

    public Button btnContinue;

    bool menuEnabled;

    bool gameOver;


	// Use this for initialization
	void Start () {
        btnContinue.enabled = false;

        menuEnabled = true;
        gameOver = false;        
        LevelController.onGameOver += LevelController_onGameOver;
        LevelController.onPowerDown += LevelController_onPowerDown;
        LevelController.onPowerUp += LevelController_onPowerUp;
	}

    //TODO show some gui label to show what powerup/down is active

    void LevelController_onPowerUp(PowerUps powerUp)
    {
        switch (powerUp)
        {
            case PowerUps.DoublePaddleSize:
                break;
            case PowerUps.UndoReverse:   
                break;
            default:
                break;
        }
    }

    void LevelController_onPowerDown(PowerDowns powerDown)
    {
        switch (powerDown)
        {
            case PowerDowns.ReverseControls:
                break;
            case PowerDowns.HalfPaddleSize:
                break;
            default:
                break;
        }
    }

    void LevelController_onGameOver()
    {
        btnContinue.enabled = false;
        toogleGameState();
        gameOver = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toogleGameState();        
        }
	}

    void toogleGameState()
    {
        if (!gameOver)
        {
            onKeyPressed(KeyCode.Escape);
            menuEnabled = !menuEnabled;
            menu.gameObject.SetActive(menuEnabled);
        }
    }

    public void OnExitClick()
    {
        Application.Quit();
    }

    public void OnNewGameClick()
    {
        btnContinue.enabled = true;
        onLevelSelect(0);
        gameOver = false;
        toogleGameState();
    }

    public void OnContinueClick()
    {
        toogleGameState();
    }

    public void OnMusicToggleClick()
    {
        onMusicToggle();
    }

    public void OnSFXToggleClick()
    {
        onSFXToggle();
    }
}

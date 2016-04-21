using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// Author: Erik Jungnickel - http://backyard-dev.de
/// Handles loading of different levels.
/// </summary>
public class LevelController : MonoBehaviour
{
    public static event OnLevelCompleted onLevelCompleted;
    public delegate void OnLevelCompleted();

    public static event OnPowerUp onPowerUp;
    public delegate void OnPowerUp(PowerUps powerUp);

    public static event OnBlockHit onBlockHit;
    public delegate void OnBlockHit(Block block);

    public static event OnPowerDown onPowerDown;
    public delegate void OnPowerDown(PowerDowns powerDown);

    public static event OnGameOver onGameOver;
    public delegate void OnGameOver();

    public GameObject BlockPrefab;

    //defines the position offset for the blocks
    public Vector2 PositionOffset;

    private int score = 0;
    private int lives = 3;

    public Text scoreText;
    public Text livesText;

    public LevelsSO levels;

    public int levelIndex = 0;

    int numBlocks;

    // Use this for initialization
    void Start()
    {
        //get the paddle controller and register for the lostball event
        PaddleController.onLostBall += LevelController_lostBall;

        MenuController.onLevelSelect += LevelController_onLevelSelect;
    }

    void LevelController_onLevelSelect(int levelIndex)
    {
        Block[] blocks = FindObjectsOfType<Block>();
        foreach (Block b in blocks)
        {
            Destroy(b.gameObject);
        }
        
        onLevelCompleted();

        if (levelIndex < levels.levels.Count)
        {
            LoadLevel(levels.levels[levelIndex]);
        }
    }

    void LevelController_lostBall()
    {
        lives--;
        livesText.text = "Lives: " + lives;
        if (lives <= 0)
        {
            onGameOver();
        }
    }

    /// <summary>
    /// Creates the actual Block GameObjects defined by the levelStructure.
    /// </summary>
    /// <param name="level"></param>
    void LoadLevel(Level level)
    {
        Array powerUps = Enum.GetValues(typeof(PowerUps));
        Array powerDowns = Enum.GetValues(typeof(PowerDowns));

        for (int y = 0; y < level.height; y++)
        {
            for (int x = 0; x < level.width; x++)
            {
                int index = y * level.width + x;

                if (level.levelStructure[index] == 0)
                    continue;

                GameObject block = (GameObject)Instantiate(BlockPrefab, new Vector3(PositionOffset.x + x, PositionOffset.y - y, 0), Quaternion.identity);
                block.transform.parent = GameObject.Find("Blocks").transform;
                block.GetComponent<Block>().hp = level.levelStructure[index];

                //set powerup
                if (UnityEngine.Random.Range(0f, 1f) >= 0.8f) //20% chance for a powerup/powerdown
                {
                    if (UnityEngine.Random.Range(0f, 1f) >= 0.8f) //20% chance for a powerDown
                    {
                        PowerDowns powerDown = (PowerDowns)powerDowns.GetValue(UnityEngine.Random.Range(0, powerDowns.Length));
                        block.GetComponent<Block>().powerDown = powerDown;
                    }
                    else
                    {
                        PowerUps powerUp = (PowerUps)powerUps.GetValue(UnityEngine.Random.Range(0, powerUps.Length));
                        block.GetComponent<Block>().powerUp = powerUp;
                    }
                }
                else
                {
                    block.GetComponent<Block>().powerUp = null;
                    block.GetComponent<Block>().powerDown = null;
                }

                block.GetComponent<Block>().onBlockHit += LevelController_blockHit;

                //increase blockcount - used to determine if level is finished
                numBlocks++;
            }
        }
    }

    void LevelController_blockHit(Block block)
    {        
        onBlockHit(block);

        //block is destroyed
        if (block.currentHp <= 0)
        {
            if (block.powerUp.HasValue)
            {
                onPowerUp(block.powerUp.Value);
            }
            if (block.powerDown.HasValue)
            {
                onPowerDown(block.powerDown.Value);
            }

            score++;
            scoreText.text = "Score: " + score;
            numBlocks--;
            if (numBlocks <= 0)
            {
                //Level completed - load next
                onLevelCompleted();

                levelIndex++;
                if (levelIndex < levels.levels.Count)
                {
                    LoadLevel(levels.levels[levelIndex]);
                }
                else
                {
                    //no more levels - start from beginning
                    levelIndex = 0;
                    LoadLevel(levels.levels[levelIndex]);
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Author: Erik Jungnickel - http://backyard-dev.de
/// Handles the controls of the paddle.
/// </summary>
public class PaddleController : MonoBehaviour
{
    public static event OnLostBall onLostBall;
    public delegate void OnLostBall();

    public GameObject ball;
    Rigidbody ballBody;
    bool gameStarted = false;
    public int PaddleSpeed = 20;
    Rigidbody paddleBody;
    bool gamePaused;
    public float ballSpeed = 10;

    bool reversedControls;
    //paddlesize = 0 -> default size, 1 -> double size, -1 half size
    //we have to control the max double or half powerups/downs or the paddle will get too small or too big
    int paddleSize = 0;

    public GameObject LeftBoundary;
    public GameObject RightBoundary;

    private Vector2 mapBounds;
    private Vector2 paddleBounds;

    // Use this for initialization
    void Start()
    {
        paddleBounds = new Vector2();
        mapBounds = new Vector2();
        mapBounds.x = LeftBoundary.transform.position.x + LeftBoundary.GetComponent<Renderer>().bounds.extents.x;
        mapBounds.y = RightBoundary.transform.position.x - RightBoundary.GetComponent<Renderer>().bounds.extents.x;

        reversedControls = false;

        //start the game in paused mode
        gamePaused = true;
        Time.timeScale = 0;

        ballBody = ball.GetComponent<Rigidbody>();
        paddleBody = GetComponent<Rigidbody>();

        LevelController.onLevelCompleted += PaddleController_onLevelCompleted;
        LevelController.onPowerDown += PaddleController_onPowerDown;
        LevelController.onPowerUp += PaddleController_onPowerUp;

        MenuController.onKeyPressed += PaddleController_onKeyPressed;
    }

    void PaddleController_onPowerUp(PowerUps powerUp)
    {
        switch (powerUp)
        {
            case PowerUps.DoublePaddleSize:
                if (paddleSize < 1)
                {
                    paddleBody.transform.localScale = new Vector3(paddleBody.transform.localScale.x * 2, paddleBody.transform.localScale.y, paddleBody.transform.localScale.z);
                    paddleSize++;
                }
                break;
            case PowerUps.UndoReverse:
                if (reversedControls)
                    reversedControls = false;
                break;
            default:
                Debug.LogError("Unknown powerUp type: " + powerUp);
                break;
        }
    }

    void PaddleController_onPowerDown(PowerDowns powerDown)
    {
        switch (powerDown)
        {
            case PowerDowns.ReverseControls:
                reversedControls = !reversedControls;
                break;
            case PowerDowns.HalfPaddleSize:
                if (paddleSize > -1)
                {
                    paddleBody.transform.localScale = new Vector3(paddleBody.transform.localScale.x / 2, paddleBody.transform.localScale.y, paddleBody.transform.localScale.z);
                    paddleSize--;
                }
                break;
            default:
                Debug.LogError("Unknown powerdown type: " + powerDown);
                break;
        }
    }

    void PaddleController_onKeyPressed(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Escape)
        {
            gamePaused = !gamePaused;

            Time.timeScale = gamePaused ? 0 : 1;
        }
    }

    void PaddleController_onLevelCompleted()
    {
        ResetBall();
        ResetPaddle();
        ResetPowerUps();
    }

    void FixedUpdate()
    {
        if (!gamePaused)
        {
            //Start the ball
            if ((Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) && !gameStarted)
            {
                //ballBody.transform.parent = null;
                gameStarted = true;
                ballBody.AddForce(new Vector3(paddleBody.velocity.x / 2, ballSpeed, 0), ForceMode.Impulse);
            }

            //Paddle movement
            float move = Input.GetAxis("Horizontal");
            if (reversedControls)
                move *= -1;

            float paddleMoveX = paddleBody.position.x + (move * Time.fixedDeltaTime * PaddleSpeed);

            paddleBounds.x = transform.position.x - GetComponent<Renderer>().bounds.extents.x;
            paddleBounds.y = transform.position.x + GetComponent<Renderer>().bounds.extents.x;

            if (move < 0)
            {
                if (paddleBounds.x >= mapBounds.x)
                    paddleBody.MovePosition(new Vector3(paddleMoveX, paddleBody.position.y, 0));
            }
            else
            {
                if (paddleBounds.y <= mapBounds.y)
                    paddleBody.MovePosition(new Vector3(paddleMoveX, paddleBody.position.y, 0));
            }

            //if game has not started, move the ball with the paddle
            if (!gameStarted)
            {
                ballBody.MovePosition(new Vector3(paddleBody.position.x, -8.2f, 0));
            }

            //Since we add force to the ball depending on the velocity of the paddle, the ball may get too fast. So we clamp its velocity.
            ballBody.velocity = Vector3.ClampMagnitude(ballBody.velocity, ballSpeed);

            //check if the ball is below the paddle
            if (ballBody.position.y <= -9.6f)
            {
                //reset the ball position
                ResetBall();

                //fire event
                onLostBall();
            }

            //Ball can get trapped in a horizontal movement - we give it a slight push
            if (gameStarted && Mathf.Abs(ballBody.velocity.y) < 0.5f)
            {
                ballBody.AddForce(new Vector3(0, 1, 0));
            }

            //ball moves to slow - add some force
            if (gameStarted && Mathf.Abs(ballBody.velocity.magnitude) < ballSpeed)
            {
                if (ballBody.velocity.y >= 0)
                    ballBody.AddForce(new Vector3(0, ballSpeed, 0));
                else
                    ballBody.AddForce(new Vector3(0, -ballSpeed, 0));
            }
        }
    }

    /// <summary>
    /// Resets the balls position
    /// </summary>
    void ResetBall()
    {
        gameStarted = false;
        ballBody.velocity = Vector3.zero;
        //ball.transform.parent = transform;
        ball.transform.localPosition = new Vector3(0, 1, 0);
    }

    void ResetPaddle()
    {
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
    }

    void ResetPowerUps()
    {
        reversedControls = false;

        if (paddleSize == 1)
        {
            paddleBody.transform.localScale = new Vector3(paddleBody.transform.localScale.x / 2, paddleBody.transform.localScale.y, paddleBody.transform.localScale.z);
        }
        if (paddleSize == -1)
        {
            paddleBody.transform.localScale = new Vector3(paddleBody.transform.localScale.x * 2, paddleBody.transform.localScale.y, paddleBody.transform.localScale.z);
        }

        paddleSize = 0;
    }

    /// <summary>
    /// Add force in x direction depending of the paddle speed.
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Equals("Ball"))
        { //Is it the ball that is hitting us? ("us" = the paddle)
            ballBody.AddForce(new Vector3(-paddleBody.velocity.x / 2, ballSpeed, 0), ForceMode.Impulse); //dividing by factor 2 just because it would be too much force without.
        }
    }
}

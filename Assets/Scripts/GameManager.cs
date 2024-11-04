using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public CameraFollow cameraFollow;
    private int currentBirdIndex;
    public SlingShot slingshot;
    [HideInInspector]
    public static GameState CurrentGameState = GameState.Start;
    private List<GameObject> Bricks;
    private List<GameObject> Birds;
    private List<GameObject> Pigs;

    void Start()
    {
        InitializeGame();
        slingshot.BirdThrown -= Slingshot_BirdThrown;
        slingshot.BirdThrown += Slingshot_BirdThrown;
    }

    private void InitializeGame()
    {
        CurrentGameState = GameState.Start;
        slingshot.enabled = false;

        Bricks = new List<GameObject>(GameObject.FindGameObjectsWithTag("Brick"));
        Birds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird"));
        Pigs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Pig"));
    }

    private void Slingshot_BirdThrown(object sender, System.EventArgs e)
    {
        currentBirdIndex++;
        if (currentBirdIndex < Birds.Count)
        {
            cameraFollow.BirdToFollow = Birds[currentBirdIndex].transform;
        }
    }

    void Update()
    {
        switch (CurrentGameState)
        {
            case GameState.Start:
                if (Input.GetMouseButtonUp(0))
                {
                    AnimateBirdToSlingshot();
                }
                break;

            case GameState.BirdMovingToSlingshot:
                // No action needed in this state
                break;

            case GameState.Playing:
                if (slingshot.slingshotState == SlingshotState.BirdFlying &&
                    (BricksBirdsPigsStoppedMoving() || Time.time - slingshot.TimeSinceThrown > 5f))
                {
                    slingshot.enabled = false;
                    AnimateCameraToStartPosition();
                    CurrentGameState = GameState.BirdMovingToSlingshot;
                }
                break;

            case GameState.Won:
            case GameState.Lost:
                if (Input.GetMouseButtonUp(0))
                {
                    Application.LoadLevel(Application.loadedLevel);
                }
                break;

            default:
                break;
        }
    }
    private bool AllPigsDestroyed()
    {
        return Pigs.All(x => x == null);
    }
    private void AnimateCameraToStartPosition()
    {
        float distanceToStartPosition = Vector2.Distance(Camera.main.transform.position, cameraFollow.StartingPosition);
        float duration = distanceToStartPosition / 10f;
        if (duration == 0.0f) duration = 0.1f;

        Camera.main.transform.positionTo(duration, cameraFollow.StartingPosition)
            .setOnCompleteHandler((x) =>
            {
                cameraFollow.IsFollowing = false;

                if (AllPigsDestroyed())
                {
                    CurrentGameState = GameState.Won;
                }
                else if (currentBirdIndex == Birds.Count - 1)
                {
                    CurrentGameState = GameState.Lost;
                }
                else
                {
                    slingshot.slingshotState = SlingshotState.Idle;
                    currentBirdIndex++;
                    AnimateBirdToSlingshot();
                }
            });
    }
    void AnimateBirdToSlingshot()
    {
        CurrentGameState = GameState.BirdMovingToSlingshot;
        Birds[currentBirdIndex].transform.positionTo(
            Vector2.Distance(Birds[currentBirdIndex].transform.position / 10,
            slingshot.BirdWaitPosition.transform.position) / 10, // duration
            slingshot.BirdWaitPosition.transform.position) // final position
                .setOnCompleteHandler((animation) =>
                {
                    animation.complete();
                    animation.destroy(); // destroy the animation
                    CurrentGameState = GameState.Playing;
                    slingshot.enabled = true; // enable the slingshot
                    slingshot.BirdToThrow = Birds[currentBirdIndex]; // set the current bird for throwing
                });
    }
}
public enum GameState
{
    Start,
    BirdMovingToSlingshot,
    Playing,
    Won,
    Lost
}
using UnityEngine;
using System.Collections.Generic;
using System;

public class SlingShot : MonoBehaviour
{
    private Vector3 SlingshotMiddleVector;
    public SlingShotState slingshotState;

    public Transform LeftSlingshotOrigin, RightSlingshotOrigin;
    public LineRenderer SlingshotLineRenderer1;
    public LineRenderer SlingshotLineRenderer2;
    public LineRenderer TrajectoryLineRenderer;

    public GameObject BirdToThrow;
    public Transform BirdWaitPosition;

    public float ThrowSpeed;
    public float TimeSinceThrown;

    void Start()
    {
        SlingshotLineRenderer1.sortingLayerName = "Foreground";
        SlingshotLineRenderer2.sortingLayerName = "Foreground";
        TrajectoryLineRenderer.sortingLayerName = "Foreground";

        slingshotState = SlingShotState.Idle;
        SlingshotLineRenderer1.SetPosition(0, LeftSlingshotOrigin.position);
        SlingshotLineRenderer2.SetPosition(0, RightSlingshotOrigin.position);

        SlingshotMiddleVector = new Vector3(
            (LeftSlingshotOrigin.position.x + RightSlingshotOrigin.position.x) / 2,
            (LeftSlingshotOrigin.position.y + RightSlingshotOrigin.position.y) / 2,
            0
        );
    }
    void Update()
    {
        switch (slingshotState)
        {
            case SlingshotState.Idle:
                InitializeBird();
                DisplaySlingshotLineRenderers();

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 tapLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    if (BirdToThrow.GetComponent<CircleCollider2D>().OverlapPoint(tapLocation))
                    {
                        slingshotState = SlingshotState.UserPulling;
                    }
                }
                break;
            case SlingshotState.UserPulling:
                DisplaySlingshotLineRenderers();

                if (Input.GetMouseButton(0))
                {
                    Vector3 tapLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    tapLocation.z = 0;

                    if (Vector3.Distance(tapLocation, SlingshotMiddleVector) > 1.5f)
                    {
                        Vector3 maxPosition = (tapLocation - SlingshotMiddleVector).normalized * 1.5f + SlingshotMiddleVector;
                        BirdToThrow.transform.position = maxPosition;
                    }
                    else
                    {
                        BirdToThrow.transform.position = tapLocation;
                    }

                    float distanceToMiddle = Vector3.Distance(SlingshotMiddleVector, BirdToThrow.transform.position);
                    DisplayTrajectoryLineRenderer2(distanceToMiddle);
                }
                else
                {
                    SetTrajectoryLineRenderersActive(false);

                    TimeSinceThrown = Time.time;
                    float distanceToMiddle = Vector3.Distance(SlingshotMiddleVector, BirdToThrow.transform.position);

                    if (distanceToMiddle > 1)
                    {
                        SetSlingshotLineRenderersActive(false);
                        slingshotState = SlingshotState.BirdFlying;
                        ThrowBird(distanceToMiddle);
                    }
                    else // Not pulled long enough, reinitiate
                    {
                        float animationDuration = distanceToMiddle / 10; // Adjusted duration
                        BirdToThrow.transform.positionTo(animationDuration, BirdWaitPosition.transform.position)
                            .setOnCompleteHandler((x) =>
                            {
                                x.complete();
                                x.destroy();
                                InitializeBird();
                            });
                    }
                }
                break;
                // Additional cases...         
        }
    }
    private void ThrowBird(float distance)
    {
        Vector3 velocity = SlingshotMiddleVector - BirdToThrow.transform.position;
        BirdToThrow.GetComponent<Bird>().OnThrow();

        Rigidbody2D birdRigidbody = BirdToThrow.GetComponent<Rigidbody2D>();
        birdRigidbody.velocity = new Vector2(velocity.x, velocity.y) * ThrowSpeed * distance;

        BirdThrown?.Invoke(this, EventArgs.Empty);
    }
    void DisplaySlingshotLineRenderers()
    {
        Vector3 birdPosition = BirdToThrow.transform.position;
        SlingshotLineRenderer1.SetPosition(1, birdPosition);
        SlingshotLineRenderer2.SetPosition(1, birdPosition);
    }

    void SetSlingshotLineRenderersActive(bool active)
    {
        SlingshotLineRenderer1.enabled = active;
        SlingshotLineRenderer2.enabled = active;
    }

    void SetTrajectoryLineRenderersActive(bool active)
    {
        TrajectoryLineRenderer.enabled = active;
    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Constants
{
    public const float BirdColliderRadiusBig = 0.5f; // Adjust to your needs
    public const float BirdColliderRadiusNormal = 0.3f; // Adjust to your needs
    public const float MinVelocity = 0.1f; // Adjust to your needs
}

public enum BirdState
{
    BeforeThrown,
    Thrown
}

public class Bird : MonoBehaviour
{
    public BirdState State { get; private set; } // Define the State property
    void Start()
    {
        TrailRenderer trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;
        trailRenderer.sortingLayerName = "Foreground";

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.radius = Constants.BirdColliderRadiusBig;

        State = BirdState.BeforeThrown;
    }
    void FixedUpdate()
    {
        if (State == BirdState.Thrown &&
            GetComponent<Rigidbody2D>().velocity.sqrMagnitude <= Constants.MinVelocity)
        {
            StartCoroutine(DestroyAfter(2));
        }
    }
    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
    public void OnThrow()
    {
        GetComponent<AudioSource>().Play();

        GetComponent<TrailRenderer>().enabled = true;

        GetComponent<Rigidbody2D>().isKinematic = false;

        GetComponent<CircleCollider2D>().radius = Constants.BirdColliderRadiusNormal;

        State = BirdState.Thrown;
    }
}

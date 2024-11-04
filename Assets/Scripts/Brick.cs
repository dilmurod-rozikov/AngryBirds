using UnityEngine;

public class Brick : MonoBehaviour
{
    public float Health = 70f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collisionRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (collisionRigidbody == null)
        {
            return;
        }

        float damage = collisionRigidbody.velocity.magnitude * 10;

        if (damage >= 10)
        {
            GetComponent<AudioSource>().Play();
        }

        Health -= damage;

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
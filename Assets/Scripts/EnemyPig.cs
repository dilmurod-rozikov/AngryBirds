using UnityEngine;

public class Pig : MonoBehaviour
{
    public float Health = 150f;
    public Sprite SpriteShownWhenHurt;
    private float ChangeSpriteHealth;

    void Start()
    {
        ChangeSpriteHealth = Health - 30f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D collisionRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (collisionRigidbody != null)
        {
            if (collision.gameObject.CompareTag("Bird"))
            {
                GetComponent<AudioSource>().Play();
                Destroy(gameObject);
            }
            else
            {
                float damage = collisionRigidbody.velocity.magnitude * 10;
                Health -= damage;

                if (damage >= 10)
                {
                    GetComponent<AudioSource>().Play();
                }

                if (Health < ChangeSpriteHealth)
                {
                    GetComponent<SpriteRenderer>().sprite = SpriteShownWhenHurt;
                }

                if (Health <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
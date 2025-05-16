using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public enum Speeds { Slow = 0, Normal = 1, Fast = 2, Faster = 3, Fastest = 4 };
//public enum Gamemodes { Cube = 0, Ship = 1, Ball = 2, UFO = 3, Wave = 4, Spider = 5 };

public class Movement : MonoBehaviour
{
    public Speeds CurrentSpeed;
    // public Gamemodes CurrentGamemode;
    //                       0      1      2       3      4
    float[] SpeedValues = { 8.6f, 10.4f, 12.96f, 15.6f, 19.27f };

    public float GroundCheckRadius;
    public LayerMask GroundMask;
    public Transform Sprite;
    public GameObject particle, explosion;

    Rigidbody2D rb;

    public int Gravity = 1;
    public bool clickProcessed = false;

    public AudioClip ExplosionSound, WinSound;
    AudioSource source;
    bool gameOver = false;

    bool IsPointerOverUI()
    {
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
    if (Input.touchCount > 0)
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    else
        return false;
#else
    return EventSystem.current.IsPointerOverGameObject();
#endif
    }

    void Start()
    {
        gameOver = false;
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        source.volume = PlayerPrefs.GetFloat("VfxSlider", .5f);
    }

    void FixedUpdate()
    {
        if (gameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        rb.linearVelocity = new Vector2(SpeedValues[(int)CurrentSpeed], rb.linearVelocity.y);

        Cube();

        if (TouchingWall())
        {
            GameOver();
        }
    }


    public bool OnGround()
    {
        return Physics2D.OverlapBox(transform.position + Vector3.down * Gravity * 0.5f, Vector2.right * 1.2f + Vector2.up * GroundCheckRadius, 0, GroundMask);
    }

    bool TouchingWall()
    {
        return Physics2D.OverlapBox((Vector2)transform.position + (Vector2.right * 0.5f), Vector2.up * 0.5f + (Vector2.right * (GroundCheckRadius / 2)), 0, GroundMask);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Obstacle"))
        {
            GameOver();
        }
        else if (other.collider.CompareTag("Finish"))
        {

            print("level complete");
            source.PlayOneShot(WinSound);
            particle.SetActive(false);
            GameObject.FindObjectOfType<ScrollingScript>().speed = 0;
            UIScript.instance.showWinPanel();
            this.enabled = false;
        }
    }


    void GameOver()
    {
        print("game over");
        gameOver = true;
        source.PlayOneShot(ExplosionSound);
        explosion.SetActive(true);
        particle.SetActive(false);
        Sprite.gameObject.SetActive(false);
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        UIScript.instance.ShowGameOverPanel(); // Show the UI instead of restarting
    }

    void Restart()
    {
        UIScript.instance.Restart();
    }
    void Cube()
    {

        //createGamemode(rb, this, true, 19.5269f, 9.057f, true, false, 409.1f);


        if (!Input.GetMouseButton(0) || OnGround())
            clickProcessed = false;

        rb.gravityScale = 9.057f * Gravity;
        LimitYVelocity(19.5269f, rb);

        if (Input.GetMouseButton(0) && !IsPointerOverUI())
        {
            if (OnGround() && !clickProcessed)
            {
                clickProcessed = true;
                rb.linearVelocity = Vector2.up * 19.5269f * Gravity;
            }
        }
        if (OnGround())
        {
            Sprite.rotation = Quaternion.Euler(0, 0, Mathf.Round(Sprite.rotation.eulerAngles.z / 90) * 90);
            particle.SetActive(true);

        }
        else if (!OnGround())
        {
            Sprite.Rotate(Vector3.back, 409.1f * Time.deltaTime * Gravity);
            particle.SetActive(false);
        }



    }

    // void Ship()
    // {
    //     transform.rotation = Quaternion.Euler(0, 0, rb.velocity.y * 2);

    //     if (Input.GetMouseButton(0))
    //         rb.gravityScale = -4.314969f;
    //     else
    //         rb.gravityScale = 4.314969f;

    //     rb.gravityScale = rb.gravityScale * Gravity;
    // }

    // void Ball()
    // {
    //     createGamemode(rb, this, true, 0, 6.2f, false, true);
    // }

    // void UFO()
    // {
    //     createGamemode(rb, this, false, 10.841f, 4.1483f, false, false, 0, 10.841f);
    // }

    // void Wave()
    // {
    //     rb.gravityScale = 0;
    //     rb.velocity = new Vector2(0, SpeedValues[(int)CurrentSpeed] * (Input.GetMouseButton(0) ? 1 : -1) * Gravity);
    // }

    // void Spider()
    // {
    //     createGamemode(rb, this, true, 238.29f, 6.2f, false, true, 0, 238.29f);
    // }

    // public void ChangeThroughPortal(Gamemodes Gamemode, Speeds Speed, int gravity, int State)
    // {
    //     switch (State)
    //     {
    //         case 0:
    //             CurrentSpeed = Speed;
    //             break;
    //         case 1:
    //             CurrentGamemode = Gamemode;
    //             break;
    //         case 2:
    //             Gravity = gravity;
    //             rb.gravityScale = Mathf.Abs(rb.gravityScale) * gravity;
    //             break;
    //     }
    // }


    void LimitYVelocity(float limit, Rigidbody2D rb)
    {
        int gravityMultiplier = (int)(Mathf.Abs(rb.gravityScale) / rb.gravityScale);
        if (rb.linearVelocity.y * -gravityMultiplier > limit)
            rb.linearVelocity = Vector2.up * -limit * gravityMultiplier;
    }
    void createGamemode(Rigidbody2D rb, Movement host, bool onGroundRequired, float initalVelocity, float gravityScale, bool canHold = false, bool flipOnClick = false, float rotationMod = 0, float yVelocityLimit = Mathf.Infinity)
    {
        if (!Input.GetMouseButton(0) || canHold && host.OnGround())
            host.clickProcessed = false;

        rb.gravityScale = gravityScale * host.Gravity;
        LimitYVelocity(yVelocityLimit, rb);

        if (Input.GetMouseButton(0))
        {
            if (host.OnGround() && !host.clickProcessed || !onGroundRequired && !host.clickProcessed)
            {
                host.clickProcessed = true;
                rb.linearVelocity = Vector2.up * initalVelocity * host.Gravity;
                host.Gravity *= flipOnClick ? -1 : 1;
            }
        }
        if (host.OnGround() || !onGroundRequired)
        {

            host.Sprite.rotation = Quaternion.Euler(0, 0, Mathf.Round(-host.Sprite.rotation.eulerAngles.z / 90) * 90);
        }
        else
            host.Sprite.Rotate(Vector3.back, rotationMod * Time.deltaTime * host.Gravity);


    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.down * Gravity * 0.5f, Vector2.right * 1.2f + Vector2.up * GroundCheckRadius);
        Gizmos.DrawCube((Vector2)transform.position + (Vector2.right * 0.5f), Vector2.up * 0.5f + (Vector2.right * (GroundCheckRadius / 2)));
    }

}


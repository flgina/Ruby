using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Include the namespace required to use Unity UI and Input System
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    // health
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    int currentHealth;

    // invincible
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    // prefab
    public GameObject HealthIncreasePrefab;
    public GameObject HealthDecreasePrefab;

    // audio    
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioSource backgroundManager;
    AudioSource audioSource;

    // text
    public TextMeshProUGUI scoreText;
    private int score = 0;
	public GameObject winTextObject;
    public GameObject loseTextObject;
    bool gameOver;
    bool winGame;
    public static int level = 1;

    // move
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public float speed = 3.0f;

    // animate
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    // cogs
    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo;
    public TextMeshProUGUI ammoText;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        rigidbody2d = GetComponent<Rigidbody2D>();
        AmmoText();

        // Set the text property of the Win Text UI to an empty string, making the 'You Win' (game over message) blank
        scoreText.text = "Robots Fixed: " + score.ToString() + "/5";
        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
        gameOver = false;
        winGame = false;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            
            if (currentAmmo > 0)
            {
                ChangeAmmo(-1);
                AmmoText();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    // // if statement to teleport
                    if (score >= 5)
                    {
                        SceneManager.LoadScene("Scene 2");
                        level = 2;
                    }

                    else
                    {
                        character.DisplayDialog();
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }

            if (winGame == true)
            {
                SceneManager.LoadScene("Level 1");
                level = 1;
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {            
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            //this might be a good place to instantiate your "damage" particles
            GameObject HealthDecreaseObject = Instantiate(HealthDecreasePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            
            PlaySound(hitSound);
        }

        if (currentHealth == 1)
        {
            loseTextObject.SetActive(true);
            if (level == 1)
            {
                winTextObject.SetActive(false);
            }

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());

            gameOver = true;

            backgroundManager.Stop();
            SoundManager2Script.PlaySound("No Hope");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeAmmo(int amount)
    {
        currentAmmo = Mathf.Abs(currentAmmo + amount);
        Debug.Log("Ammo: " + currentAmmo);
    }

    public void AmmoText()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }
    
    void Launch()
    {
         if (currentAmmo > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);
            
            animator.SetTrigger("Launch");
            
            PlaySound(throwSound);
        }
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void StopSound(AudioClip clip)
    {
        audioSource.Stop();
    }

    public void FixedRobots(int amount)
    {
        score += amount;
        scoreText.text = "Fixed Robots: " + score.ToString() + "/5";

        Debug.Log("Fixed Robots: " + score);

        // Talk to Jambi to visit stage 2
        if (score == 5 && level == 1)
        {
            winTextObject.SetActive(true);
        }

        if (score == 5 && level == 2)
        {
            winTextObject.SetActive(true);

            winGame = true;

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;

            Destroy(gameObject.GetComponent<SpriteRenderer>());

            // BackgroundMusicManager is turned off
            backgroundManager.Stop();

            // Calls sound script and plays win sound
            SoundManagerScript.PlaySound("Victory");
        }
    }
}
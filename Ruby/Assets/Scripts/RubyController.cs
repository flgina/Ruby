using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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

    // cog
    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo;
    public TextMeshProUGUI cogsText;

    // move
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public float speed = 3.0f;

    // animator
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    // audio
    AudioSource audioSource;
    public AudioClip box; 
    public AudioClip kitty;   
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip backgroundSound;

    // cherry
    public TextMeshProUGUI cherryText;
    //public GameObject CherryPrefab;
    private int cherries;

    // prefab    
    public GameObject HealthIncreasePrefab;
    public GameObject HealthDecreasePrefab;

    // score/gameover/level
    private int score = 0;
    public TextMeshProUGUI scoreText;
	public TextMeshProUGUI GameOverTextObject;
    bool gameOver;
    bool winGame;
    public static int level = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        
        // health
        currentHealth = maxHealth;

        // cherry
        cherryText.text = "";
        if (level == 2)
        {
            cherries = 0;
            cherryText.text = "Cherries: " + cherries.ToString() + "/5";
        }

        audioSource = GetComponent<AudioSource>();

        rigidbody2d = GetComponent<Rigidbody2D>();
        cogsText.text = "Ammo: " + ammo.ToString();

        // Set the text property of the Win Text UI to an empty string, making the 'You Win' (game over message) blank
        scoreText.text = "Robots: " + score.ToString() + "/5";
        GameOverTextObject.text = "";
        gameOver = false;
        winGame = false;

        audioSource.clip = backgroundSound;
        audioSource.Play();
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
                cogsText.text = "Ammo: " + ammo.ToString();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                PlaySound(kitty);
            }
        }

        if (cherries == 5)
        {
            cherryText.text = "You have collected all the cherries!";
        }

        if (score == 5)
        {
            GameOverTextObject.text = "Talk to Jambi to visit\nstage two!";
        }

        if ((level == 2) && (score == 5))
        {
            GameOverTextObject.text = "You Win!\nGame Created by Gina Lofoco\nPress R to restart";
            
            gameOver = true;
        }
        
        if (currentHealth <= 0)
        {
            GameOverTextObject.text = "You Lost!\n Press R to restart";

            gameOver = true;

            speed = 0;
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
            audioSource.clip = backgroundSound;
            audioSource.Stop();

            audioSource.clip = loseSound;
            audioSource.Play();
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeAmmo(int amount)
    {
        // Ammo math code
        currentAmmo = Mathf.Abs(currentAmmo + amount);
        Debug.Log("Ammo: " + currentAmmo);
    }

    public void AmmoText()
    {
        cogsText.text = "Ammo: " + currentAmmo.ToString();
    }

    public void ChangeScore(int scoreAmount)
	{
        if (scoreAmount > 0)
        {
            score += scoreAmount;
            scoreText.text = "Robots: " + score.ToString() + "/5";
            Debug.Log("Robots: " + score);
        }

        if (scoreAmount == 5)
        {           
            audioSource.clip = backgroundSound;
            audioSource.Stop();

            audioSource.clip = winSound;
            audioSource.Play();
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Box")
        {
            PlaySound(box);
        }

        if (collision.gameObject.tag == "cherry")
        {
            collision.gameObject.SetActive(false);

            // Add one to the score variable 'count'
            cherries += 1;

            // Run the 'SetCountText()' function (see below)
            cherryText.text = "Cherries: " + cherries.ToString() + "/5";
        }

        if (collision.gameObject.tag == "potion")
        {
            collision.gameObject.SetActive(false);
            speed = 5.0f;
            timeInvincible = 2.0f;
        }

        if (collision.gameObject.tag == "cog")
        {
            collision.gameObject.SetActive(false);
            currentAmmo += 5;
            cogsText.text = "Ammo: " + currentAmmo.ToString();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
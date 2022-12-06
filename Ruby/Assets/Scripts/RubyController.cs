using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    
    public GameObject projectilePrefab;
    public GameObject collectEffectPrefab;
    public GameObject HealthIncreasePrefab;
    public GameObject HealthDecreasePrefab;
    
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip backgroundSound;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;

    private int score = 0;
    public TextMeshProUGUI scoreText;
	public TextMeshProUGUI GameOverTextObject;
    bool gameOver;
    bool winGame;

    public int cogs { get { return currentCogs; } }
    public int currentCogs;

    public int collected { get { return currentCollected; } }
    public int currentCollected;

    public int CogsValue = 4;
    public int maxCollected = 5;
    
    public TextMeshProUGUI cogsText;

    public static int level = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        currentCollected = maxCollected;
        cogsText.text = CogsValue.ToString();

        // Set the text property of the Win Text UI to an empty string, making the 'You Win' (game over message) blank
        scoreText.text = "Robots Fixed: " + score.ToString() + "/5";
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
            CogsValue = 0;
            cogsText.text = CogsValue.ToString();
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

    void SetCogsText()
    {
        cogsText.text = "Ammo: " + cogs.ToString();
        Debug.Log("Ammo: " + cogs);
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

    public void ChangeScore(int scoreAmount)
	{
        if (scoreAmount > 0)
        {
            score += scoreAmount;
            scoreText.text = "Robots Fixed: " + score.ToString() + "/5";
            Debug.Log("Robots Fixed: " + score);
        }

        if (scoreAmount == 5)
        {           
            audioSource.clip = backgroundSound;
            audioSource.Stop();

            audioSource.clip = winSound;
            audioSource.Play();
        }
	}

    public void ChangeCogs(int amount)
    {
        currentCogs = Mathf.Clamp(currentCogs + amount, 0, CogsValue);
        CogsValue += 4;
        cogsText.text = CogsValue.ToString();
        GameObject collectEffect = Instantiate(collectEffectPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        PlaySound(hitSound);
    }

    public void Changecollected(int amount)
    {
        currentCollected = Mathf.Clamp(currentCollected + amount, 0, maxCollected);
        //CollectBar.instance.SetValue(currentCollected / (float)maxCollected);
    }
    
    void Launch()
    {
        if (CogsValue > 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Launch(lookDirection, 300);

            animator.SetTrigger("Launch");
            PlaySound(throwSound);
            CogsValue -= 1;
            cogsText.text = CogsValue.ToString();
        }
    } 

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
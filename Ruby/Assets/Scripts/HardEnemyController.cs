using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical;
    public float changeTime = 3.0f;

    public ParticleSystem smokeEffect;
    
    Rigidbody2D newrigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;
    
    Animator animator;

    // this line of code creates a variable called "rubyController" to store information about the RubyController script!
    private RubyController rubyController; 
    
    // Start is called before the first frame update
    void Start()
    {
        // this line of code finds the RubyController script by looking for a "RubyController" tag on Ruby
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController"); 

        if (rubyControllerObject != null)

        {

            // and this line of code finds the rubyController and then stores it in a variable
            rubyController = rubyControllerObject.GetComponent<RubyController>(); 

            print ("Found the RubyConroller Script!");

        }

        if (rubyController == null)

        {

            print ("Cannot find GameController Script!");

        }

        newrigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        Vector2 position = newrigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        
        newrigidbody2D.MovePosition(position);
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController >();

        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }
    
    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        newrigidbody2D.simulated = false;
        //optional if you added the fixed animation
        animator.SetTrigger("Fixed");
        
        smokeEffect.Stop();

        if (broken == false)
            {
                rubyController.ChangeScore(1);
            }
    }
}
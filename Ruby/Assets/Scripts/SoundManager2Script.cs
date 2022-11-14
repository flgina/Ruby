using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager2Script : MonoBehaviour
{
    public static AudioClip loseSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        loseSound = Resources.Load<AudioClip>("No Hope");

        audioSrc = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch(clip)
        {
            case "No Hope":
                audioSrc.PlayOneShot(loseSound);
                break;
        }
    }
}

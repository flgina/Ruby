using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip winSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        winSound = Resources.Load<AudioClip>("Victory");

        audioSrc = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public static void PlaySound(string clip)
    {
        switch(clip)
        {
            case "Victory":
                audioSrc.PlayOneShot(winSound);
                break;
        }
    }*/
}


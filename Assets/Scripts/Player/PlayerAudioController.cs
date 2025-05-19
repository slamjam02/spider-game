using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected AudioClip jumpClip, walkClip;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = walkClip;
        audioSource.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
        // if (playerMovement.isWalking && !(playerMovement.Airborne()))
        {
            audioSource.UnPause();
        }
        else
        {
            audioSource.Pause();
        }
    }
}

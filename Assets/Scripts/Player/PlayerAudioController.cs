using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip jumpClip;

    private AudioSource walkAudioSource;
    private AudioSource sfxAudioSource;
    private bool hasPlayedJumpSound = false;

    void Start()
    {
        // Create the walking AudioSource
        walkAudioSource = gameObject.AddComponent<AudioSource>();
        walkAudioSource.clip = walkClip;
        walkAudioSource.loop = true;
        walkAudioSource.playOnAwake = false;

        // Create the SFX AudioSource for things like jump
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
    }

    void Update()
    {
        // Walking loop
        if (playerMovement.isWalking)
        {
            if (!walkAudioSource.isPlaying)
                walkAudioSource.Play();
        }
        else
        {
            if (walkAudioSource.isPlaying)
                walkAudioSource.Stop();
        }

        // Jump SFX
        if (playerMovement.hasJumped && !hasPlayedJumpSound)
        {
            sfxAudioSource.PlayOneShot(jumpClip);
            hasPlayedJumpSound = true;
            Debug.Log("Jump clip played");
        }
        else if (!playerMovement.hasJumped)
        {
            hasPlayedJumpSound = false;
        }
    }
}

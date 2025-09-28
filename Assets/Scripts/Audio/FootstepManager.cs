using System;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] footstepClips; // Assign your 3 footstep clips here

    [Header("Player Input")]
    [SerializeField] private PlayerInput playerInput; // Assign your PlayerInput instance here

    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.5f; // Time between steps

    private float stepTimer = 0f;

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.OnMoveInputEvent += HandleMoveInput;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.OnMoveInputEvent -= HandleMoveInput;
        }
    }

    private Vector2 movementInput;

    private void HandleMoveInput(Vector2 input)
    {
        movementInput = input;
    }

    private void Update()
    {
        if (movementInput.magnitude > 0.1f) // Player is moving
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = stepInterval; // reset timer when player stops
        }
    }

    private void PlayFootstep()
    {
        if (footstepClips.Length == 0 || SoundFXManager.instance == null)
            return;

        // Pick a random clip
        AudioClip clip = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];

        // Play it at the player's position
        SoundFXManager.instance.PlaySoundFXClip(clip, playerInput.transform, 1f);
    }
}


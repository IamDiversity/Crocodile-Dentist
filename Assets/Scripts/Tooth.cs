using System;
using System.Collections;
using UnityEngine;

public class Tooth : MonoBehaviour
{
    [SerializeField]
    private bool _isActivated = false;

    private BoxCollider _boxCollider;
    public Vector3 StartPosition { get; private set; } // Starting position of the tooth (initial position)
    public Vector3 EndPosition { get; private set; }   // Ending position of the tooth (when it's pressed and going down)
    private float _toothTransitionDuration = 0.25f;
    private static Tooth _trappedTooth; // The trapped tooth
    public static Tooth TrappedTooth {
        get => _trappedTooth;
        set {
            if (_trappedTooth != value) {
                _trappedTooth = value;
            }
        }
    }

    private Coroutine moveCoroutine;
    
    void Awake()
    {
        _boxCollider = GetComponentInChildren<BoxCollider>();
        StartPosition = gameObject.transform.position;
        EndPosition = StartPosition + Vector3.down * 0.3f;
    }

    public void OnPressed()
    {
        // IF : this tooth isn't already pressed / activated
        if (!_isActivated) {
            // Set it to "activated"
            _isActivated = true;
            // IF : it's player's turn
            if (GameManager.instance.PlayerTurn == EN_Players.PLAYER_01) {
                // Set the "clicked" variable to true to indicate that the player has pressed a tooth
                GameManager.instance.clicked = true;
            }
            // IF : a coroutine already exist, stop it first before launching a new one for the tooth going down animation (to avoid glitchy replaying animation)
            if (moveCoroutine != null) {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine =  StartCoroutine(MoveTo());
        }
    }
    
    
    public void ResetTooth()
    {
        // Reset "isActivated" field to be able to select it using the "SelectionManager"
        _isActivated = false;
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }
    }
    
    private IEnumerator MoveTo()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < _toothTransitionDuration) {
            // Calculate the fraction of time that has passed
            float t = elapsedTime / _toothTransitionDuration;
            // Interpolate between the start and end positions
            gameObject.transform.position = Vector3.Lerp(StartPosition, EndPosition, t);
            // Increment elapsed time
            elapsedTime += Time.deltaTime;
            // Wait for the next frame
            yield return null;
        }
        // Ensure the cube reaches the exact end position
        gameObject.transform.position = EndPosition;
        GameManager.instance.FinishTurn(this);
    }

    public bool IsActivated()
    {
        return _isActivated;
    }
    
    
}

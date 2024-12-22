using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    private GameManager _gameManager;
    private readonly float _toothAnimWaitingTime = 0.175f;
    
    private void Start()
    {
        _gameManager = GameManager.instance;
    }

    private void OnEnable()
    {
        // Subscribe to "OnTurnChanged" event
        GameManager.OnTurnChanged += Play;
    }

    private void OnDisable()
    {
        // Unsubscribe from "OnTurnChanged" event
        GameManager.OnTurnChanged -= Play;
    }

    public void Play(EN_Players playerTurn)
    {
        StartCoroutine(AiPlay(playerTurn));
    }
    
    private IEnumerator AiPlay(EN_Players playerTurn)
    {
        // IF : it's AI's turn
        if (playerTurn == EN_Players.PLAYER_02) {
            yield return new WaitForSeconds(1);
            // Select a valid tooth index from the ones left (not pressed)
            int randomIndex = Random.Range(0, GameManager.instance._validTeeth.Count);
            StartCoroutine(SelectedToothVFX(randomIndex));
            
            yield return null;
        }
    }

    private IEnumerator SelectedToothVFX(int index)
    {
        // Get a reference to that randomly selected tooth
        Tooth selectedTooth = GameManager.instance._validTeeth[index];
        // Play a blink animation on the selected tooth by the AI (VFX for the player to know which tooth has been selected by AI)
        if (selectedTooth.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)) {
            SelectionManager selectionManager = GameManager.instance._selectionManager;
            selectionManager.SetMaterial(selectedTooth, selectionManager._aiHighlightMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._defaultMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._aiHighlightMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._defaultMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._aiHighlightMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._defaultMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._aiHighlightMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
            selectionManager.SetMaterial(selectedTooth, selectionManager._defaultMaterial);
            yield return new WaitForSeconds(_toothAnimWaitingTime);
        }
        // Press that tooth
        GameManager.instance._validTeeth[index].OnPressed();
        yield return null;
    }
}

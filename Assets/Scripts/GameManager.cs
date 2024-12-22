using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public SelectionManager _selectionManager;
    
    [FormerlySerializedAs("uiManager")]
    [SerializeField]
    private UIManager _uiManager;
    
    [FormerlySerializedAs("ai")]
    [SerializeField]
    private AI _ai;

    [SerializeField]
    private Animator _animation;

    [FormerlySerializedAs("teethParent")]
    [SerializeField]
    private GameObject _teethParent;

    public static GameManager instance { get; private set; }
    private Tooth[] _teeth;
    public List<Tooth> _validTeeth;
    private EN_Players _playerTurn;
    public EN_Players PlayerTurn {
        get => _playerTurn;
        set 
        {
            if (_playerTurn != value) {
                _playerTurn = value;
            }    
        }
    }
    private EN_GameState _gameState;
    public EN_GameState GameState {
        get => _gameState;
        set 
        {
            if (_gameState != value) {
                _gameState = value;
                OnGameStateChanged(_gameState);
            }   
        }
    }
    private EN_Players _winner;
    public bool clicked;
    private System.Random _random;

    // EVENTS
    public static event Action<EN_Players> OnTurnChanged;
    public static event Action OnStartPreRound;
    public static event Action OnStartRound;
    public static event Action<EN_Players> OnEndRound;
    
    // METHODS
    private void Awake()
    {
        #region Singleton
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else if (instance != this) {
            throw new Exception("Can't have more than 1 GameManager object in the hierarchy!");
        }
        #endregion
         _random = new System.Random(); 
    }

    private void Start()
    {
        // Get all the <Tooth> scripts once from the "_teeth" parent object
        _teeth = _teethParent.GetComponentsInChildren<Tooth>();
        GameState = EN_GameState.PRE_ROUND;
    }

    // Handle the GameState
    private void OnGameStateChanged(EN_GameState gameState)
    {
        switch (gameState) 
        {
            case EN_GameState.PRE_ROUND:
                StartPreRound();
                OnStartPreRound?.Invoke();
                break;
            
            case EN_GameState.ROUND:
                StartRound();
                OnStartRound?.Invoke();
                break;
            
            case EN_GameState.END_ROUND:
                StartCoroutine(EndRound());
                break;
        }
    }

    public void SetTrappedTooth()
    {
        Tooth.TrappedTooth = _teeth[_random.Next(_teeth.Length)];
    }
    
    public void ResetTrappedTooth()
    {
        Tooth.TrappedTooth = null;
    }

    public void ResetAllTeeth()
    {
        foreach (Tooth tooth in _teeth) {
            tooth.ResetTooth();
        }
    }
    
    public void FinishTurn(Tooth pressedTooth)
    {
        if (IsToothTrapped(pressedTooth)) 
        {
            GameState = EN_GameState.END_ROUND;
            return;
        }

        // Remove the pressed tooth from the "_validTeeth" list for the AI to not pick an already pressed tooth
        _validTeeth.Remove(pressedTooth);
        
        if (PlayerTurn == EN_Players.PLAYER_01) {
            // Player's turn
            PlayerTurn = EN_Players.PLAYER_02; // Change to AI's turn
        } 
        else if (PlayerTurn == EN_Players.PLAYER_02) {
            // AI's turn
            PlayerTurn = EN_Players.PLAYER_01; // Change to player's turn
            clicked = false;
        }
        // Call the "OnTurnChanged" event for the AI to receive it and start to play
        OnTurnChanged?.Invoke(PlayerTurn);
    }

    private bool IsToothTrapped(Tooth pressedTooth)
    {
        return pressedTooth == Tooth.TrappedTooth;
    }

    public Tooth GetRandomValidTooth()
    {
        //   IF : the amount of valid teeth equals 0 in the "_validTeeth" list, return null
        // ELSE : Choose a random tooth in the "_validTeeth" list and return it
        if (_validTeeth.Count == 0) {
            return null;
        }
        
        Tooth validTooth = _validTeeth[_random.Next(_validTeeth.Count)];
        return validTooth;
    }

    private void StartPreRound()
    {
        // TODO: Add all the logic the check if everything is met before starting the Round
        GameState = EN_GameState.ROUND;
    }

    private void StartRound()
    {
        _winner = EN_Players.NONE;
        PlayerTurn = EN_Players.PLAYER_01;
        // Clear the list of valid teeth for the AI to use
        _validTeeth.Clear();
        _validTeeth = _teeth?.ToList();
        
        ResetTrappedTooth();
        ResetAllTeeth();
        SetTrappedTooth();
    }

    private IEnumerator EndRound()
    {
        // Set the winner of the game in "_winner" field
        _winner = (PlayerTurn == EN_Players.PLAYER_01) ? EN_Players.PLAYER_02 : EN_Players.PLAYER_01;
        // Play the mouth closing animation of the crocodile
        _animation.SetBool("IsMouthClosed", true);
        // Wait 5 sec (for the UI stuff to appear)
        yield return new WaitForSeconds(1.0f);
        OnEndRound?.Invoke(_winner);
        Invoke(nameof(LoadScene), 3.0f);
        
    }

    private void LoadScene()
    {
        // Load the scene again to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}

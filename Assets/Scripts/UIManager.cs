using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [Header("GUI")] [SerializeField]
    private GameObject _hud;
    
    [FormerlySerializedAs("textEnd")]
    [SerializeField]
    private TextMeshProUGUI _textEnd;

    public static UIManager instance { get; private set; }

    private void Awake()
    {
        #region Singleton
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            } else if (instance != this) {
                throw new Exception("Can't have more than 1 UIManager object in the hierarchy!");
            }
        #endregion
    }

    private void OnEnable()
    {
        GameManager.OnEndRound += OnEndRound;
    }

    private void OnDisable()
    {
        GameManager.OnEndRound -= OnEndRound;
    }

    public void OnEndRound(EN_Players winner)
    {
        // Show the HUD and the text saying if we won or lost
        _hud.SetActive(true);
        _textEnd.text = winner == EN_Players.PLAYER_01 ? "You win !" : "You loose !";
        
    }
}

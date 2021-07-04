using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState {BATTLE, OVERWORLD, TITLE, CUTSCENE }
public enum BattleState {WAIT, ACTIVE}
public class EventManager : MonoBehaviour
{
    [SerializeField]
    internal GameManager GM;
    [SerializeField]
    internal GameState _GameState;
    [SerializeField]
    internal BattleState _BattleState;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public GameState ChangeInGameState(GameState GS)
    {
        switch (GS)
        {
            case GameState.TITLE:
                _GameState = GameState.TITLE;

                break;
            case GameState.OVERWORLD:
                _GameState = GameState.OVERWORLD;

                break;
            case GameState.BATTLE:
                _GameState = GameState.BATTLE;
                SceneManager.LoadScene("TEST - Battle");
                break;
            case GameState.CUTSCENE:
                _GameState = GameState.CUTSCENE;

                break;
        }
        return GS;
    }
}

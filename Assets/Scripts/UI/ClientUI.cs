using com.tictactoe.client;
using com.tictactoe.common;
using UnityEngine;

namespace com.testnet.ui
{
    public class ClientUI : MonoBehaviour
    {
        [SerializeField] TicTacToeUI _ticTacToeUi;
        [SerializeField] LobbyUI _lobbyUi;

        private void Start()
        {
            _ticTacToeUi.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            TicTacToeHandleUpdateGameStateSystem.OnHandleGameState += OnTicTacToeStateHandle;
        }

        private void OnDisable()
        {
            TicTacToeHandleUpdateGameStateSystem.OnHandleGameState -= OnTicTacToeStateHandle;
        }

        private void OnTicTacToeStateHandle(TicTacToeUpdateGameStateRpc state)
        {
            _ticTacToeUi.UpdateGameState(state);
            _ticTacToeUi.gameObject.SetActive(true);
            
        }
    }
}
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
    }
}
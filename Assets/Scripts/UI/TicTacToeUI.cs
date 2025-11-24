using com.tictactoe.common;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.testnet.ui
{
    public class TicTacToeUI : MonoBehaviour
    {
        public Color[] _playerColors;
        [SerializeField] private RawImage _turnInfoBackground;
        [SerializeField] private RawImage [] _cellStateImages;
        [SerializeField] private TMPro.TextMeshProUGUI _turnInfoText;
        [SerializeField] private Button _exitButton;

        public void UpdateGameState(TicTacToeUpdateGameStateRpc gameState)
        {
            bool isPlayerTurn = gameState.Turn % 2 == gameState.PlayerOrder;
            if (TicTacToeUtils.GameIsEnded(gameState.GameResultFlags))
            {
                _turnInfoBackground.color = new Color(0, 0, 0);
                if(TicTacToeUtils.IsDraw(gameState.GameResultFlags))
                {
                    _turnInfoText.text = "Is Draw";
                } 
                else
                {
                    bool playerIsWin = TicTacToeUtils.PlayerIsWin(gameState.GameResultFlags, gameState.PlayerOrder);
                    _turnInfoText.text = playerIsWin ? "You win" : "Opponent win";
                }
            } 
            else
            {
                
                _turnInfoText.text = isPlayerTurn ? "Your turn" : "Wait opponent turn";
                _turnInfoBackground.color = _playerColors[gameState.Turn % 2];
            }
            UpdateCellsState(gameState.CellsPlayer1, gameState.CellsPlayer2);
        }

        public void UpdateCellsState(ushort player1, ushort player2)
        {
            for (int i = 0; i < _cellStateImages.Length; i++)
            {
                _cellStateImages[i].color = GetCellColor(i, player1, player2);
            }
        }

        private void Start()
        {
            UpdateCellsState(0, 0);
            for(int i=0; i<_cellStateImages.Length; i++)
            {
                InitializeClickListener(_cellStateImages[i], i);
            }
        }

        private void InitializeClickListener(RawImage image, int index)
        {
            EventTrigger eventTrigger = image.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = image.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnCellClicked(index); });
            eventTrigger.triggers.Add(entry);
        }

        private Color GetCellColor(int index, ushort player1, ushort player2)
        {
            if (((player1 >> index) & 1) == 1)
            {
                return _playerColors[0];
            }
            else if (((player2 >> index) & 1) == 1)
            {
                return _playerColors[1];
            }
            return Color.clear;
        }

        private void OnCellClicked(int index)
        {
            foreach(var world in World.All)
            {
                if(world.IsClient())
                {
                    var rpc = world.EntityManager.CreateEntity(
                        ComponentType.ReadOnly<TicTacToeTryMakeMoveRpc>(),
                        ComponentType.ReadOnly<SendRpcCommandRequest>());
                    world.EntityManager.SetComponentData(rpc, new TicTacToeTryMakeMoveRpc { CellIndex = index });
                }
            }
        }
    }
}
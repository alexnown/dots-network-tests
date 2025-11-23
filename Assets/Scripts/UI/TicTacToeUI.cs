using com.testnet.common;
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
        [SerializeField] private RawImage [] _cellStateImages;
        [SerializeField] private TMPro.TextMeshProUGUI _turnInfoText;
        [SerializeField] private Button _exitButton;

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

        public void UpdateCellsState(ushort player1, ushort player2)
        {
            for(int i=0; i<_cellStateImages.Length; i++)
            {
                _cellStateImages[i].color = GetCellColor(i, player1, player2);
            }
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
            Debug.Log("cell click " + index);
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
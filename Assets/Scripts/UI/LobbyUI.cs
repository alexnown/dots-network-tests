using com.testnet.common;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.UI;

namespace com.testnet.ui
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI _playersInLobbyText;
        [SerializeField] TMPro.TextMeshProUGUI _ticTacToeInfoText;
        [SerializeField] TMPro.TextMeshProUGUI _realtimeBattleRoomInfoText;
        [SerializeField] Button _startTicTacToeBtn;
        [SerializeField] Button _startRealtimeBtn;

        private void Start()
        {
            _playersInLobbyText.text = "Waiting connection to lobby";
            _ticTacToeInfoText.text = string.Empty;
            _realtimeBattleRoomInfoText.text = string.Empty;
        }

        private void Update()
        {
            int clientsWorld = 0;
            int componentsCount = 0;
            //todo: add update delay
            foreach(var world in World.All)
            {
                if(!world.IsClient())
                {
                    continue;
                }
                clientsWorld++;
                var statisticsQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LobbyStatisticsData>().Build(world.EntityManager);
                componentsCount += statisticsQuery.CalculateEntityCount();
                using(var dataArray = statisticsQuery.ToComponentDataArray<LobbyStatisticsData>(Allocator.Temp)) 
                {
                    if (dataArray.Length > 0)
                    {
                        _playersInLobbyText.text = $"Connections in lobby: {dataArray[0].ConnectionsCount}";
                    }
                }
            }
            _ticTacToeInfoText.text = $"{clientsWorld}/{World.All.Count},  components={componentsCount}";
        }
    }
}
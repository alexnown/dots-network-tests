using Unity.Entities;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.NetCode;
using Unity.Networking.Transport;
using com.testnet.client;

public class GameUI : MonoBehaviour
{
    private const string DEFAULT_SERVER_IP = "127.0.0.1";
    private const ushort DEFAULT_SERVER_PORT = 7979;

    [SerializeField] private TMP_InputField _ipField;
    [SerializeField] private TMP_InputField _portField;
    [SerializeField] private Button _startServerButton;
    [SerializeField] private Button _startClientButton;
    void Start()
    {
        _startServerButton.onClick.AddListener(OnStartServerClicked);
        _startClientButton.onClick.AddListener(OnStartClientClicked);
    }

    private void RemoveButtonListeners()
    {
        _startServerButton.onClick.RemoveAllListeners();
        _startClientButton.onClick.RemoveAllListeners();
    }

    private void OnStartClientClicked()
    {
        RemoveButtonListeners();
        DestroyLocalWorld();
        InitializeClientWorld();
    }

    private void InitializeClientWorld()
    {
        ushort port = ValidateAndGetPort();
        NetworkEndpoint endpoint;
        if (!NetworkEndpoint.TryParse(_ipField.text, port, out endpoint))
        {
            _ipField.text = DEFAULT_SERVER_IP;
            endpoint = NetworkEndpoint.Parse(DEFAULT_SERVER_IP, port);
        }
        var clientWorld = ClientServerBootstrap.CreateClientWorld("Client World");
        using var networkDeliverQuery = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        networkDeliverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, endpoint);
        World.DefaultGameObjectInjectionWorld = clientWorld;
        SceneManager.LoadScene(1);
    }

    private ushort ValidateAndGetPort()
    {
        ushort port;
        if (!ushort.TryParse(_portField.text, out port))
        {
            _portField.text = DEFAULT_SERVER_PORT.ToString();
            return DEFAULT_SERVER_PORT;
        }
        return port;
    }

    private void OnStartServerClicked()
    {
        RemoveButtonListeners();
        DestroyLocalWorld();
        var serverWorld = ClientServerBootstrap.CreateServerWorld("Server World");
        ushort port = ValidateAndGetPort();
        var servEndpoint = NetworkEndpoint.AnyIpv4.WithPort(port);
        using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(servEndpoint);
        //todo: remove client init
        InitializeClientWorld();
        World.DefaultGameObjectInjectionWorld = serverWorld;
        SceneManager.LoadScene(1);
    }

    private void DestroyLocalWorld()
    {
        foreach(var world in World.All)
        {
            if(world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }
    }
}

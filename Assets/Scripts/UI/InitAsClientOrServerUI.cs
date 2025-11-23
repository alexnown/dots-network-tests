using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Networking.Transport;
using com.testnet.common;

namespace com.testnet.ui
{
    public class InitAsClientOrServerUI : MonoBehaviour
    {
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
            if(ValidateIpAndPort())
            {
                RemoveButtonListeners();
                SceneManager.LoadScene("ClientStartScene");
            }
        }

        private bool ValidateIpAndPort()
        {
            if(ValidatePort(out ushort port))
            {
                NetworkEndpoint endpoint;
                if (!NetworkEndpoint.TryParse(_ipField.text, port, out endpoint))
                {
                    _ipField.text = NetworkConstants.DEFAULT_SERVER_LOBBY_IP;
                    _portField.text = NetworkConstants.DEFAULT_SERVER_LOBBY_PORT.ToString();
                    return false;
                }
                NetworkConstants.SERVER_LOBBY_IP = _ipField.text;
                NetworkConstants.SERVER_LOBBY_PORT = port;
                return true;
            }
            return false;
        }

        private bool ValidatePort(out ushort port)
        {
            if (!ushort.TryParse(_portField.text, out port))
            {
                _portField.text = NetworkConstants.DEFAULT_SERVER_LOBBY_PORT.ToString();
                port = NetworkConstants.SERVER_LOBBY_PORT;
                return false;
            }
            return true;
        }

        private void OnStartServerClicked()
        {
            if(ValidateIpAndPort())
            {
                RemoveButtonListeners();
                SceneManager.LoadScene("ClientStartScene", LoadSceneMode.Single);
                SceneManager.LoadSceneAsync("ServerStartScene", LoadSceneMode.Additive);
            }
        }
    }
}
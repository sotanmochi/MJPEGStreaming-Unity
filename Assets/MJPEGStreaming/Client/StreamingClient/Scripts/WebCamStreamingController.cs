using UnityEngine;
using UnityEngine.UI;

namespace MJPEGStreaming.Client
{
    public class WebCamStreamingController : MonoBehaviour
    {
        [SerializeField] WebCamStreamingService _webCamStreamingService;
        [SerializeField] GameObject _textureSteamingClientObject;
        ITextureStreamingClient _textureSteamingClient;

        [SerializeField] bool _useSkybox;
        [SerializeField] int _intervalTimeMillisec = 100;

        [SerializeField] RawImage _rawImage;
        [SerializeField] Dropdown _nodeSelectionDropdown;

        [SerializeField] Text _webcamTextureSize;
        [SerializeField] Text _clientIdText;
        [SerializeField] InputField _serverAddress;
        [SerializeField] InputField _serverPort;
        [SerializeField] Button _connect;
        [SerializeField] Button _disconnect;
        [SerializeField] Button _startStreaming;
        [SerializeField] Button _stopStreaming;

        WebCamTexture _webCamTexture;
        Material _skyboxMaterial;

        void Start()
        {
            _textureSteamingClient = _textureSteamingClientObject.GetComponent<ITextureStreamingClient>();

            _skyboxMaterial = RenderSettings.skybox;

            SetupDropdown();

            _nodeSelectionDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            _startStreaming.onClick.AddListener(OnClickStartStreaming);
            _stopStreaming.onClick.AddListener(OnClickStopStreaming);
            _connect.onClick.AddListener(OnClickConnect);
            _disconnect.onClick.AddListener(OnClickDisconnect);
        }

        void Update()
        {
            _clientIdText.text = "Client ID : " + _textureSteamingClient.ClientId;
        }

        void SetupDropdown()
        {
            Dropdown dropdown = _nodeSelectionDropdown;
            dropdown.ClearOptions();
            dropdown.RefreshShownValue();
            dropdown.options.Add(new Dropdown.OptionData { text = "Select camera" });

            foreach(var device in WebCamTexture.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData { text = device.name });
            }

            dropdown.RefreshShownValue();
        }

        void OnDropdownValueChanged(int selectedValue)
        {
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();

                if (_rawImage != null)
                {
                    _rawImage.texture = null;
                }
                if (_useSkybox)
                {
                    _skyboxMaterial.mainTexture = null;
                }
            }

            if (selectedValue < 1)
            {
                Debug.Log("WebCam is null.");
                return;
            }

            WebCamDevice selectedDevice = WebCamTexture.devices[selectedValue - 1];
            _webCamTexture = new WebCamTexture(selectedDevice.name);
            _webCamTexture.Play();

            if (_rawImage != null)
            {
                _rawImage.texture = _webCamTexture;
            }
            if (_useSkybox)
            {
                _skyboxMaterial.mainTexture = _webCamTexture;
            }

            _webcamTextureSize.text = "Texture size : " + _webCamTexture.width + "x" + _webCamTexture.height;
            Debug.Log("WebCamTexture: " + _webCamTexture.width + "x" + _webCamTexture.height);

            _webCamStreamingService.Initialize(_webCamTexture, _textureSteamingClient, _intervalTimeMillisec);
        }

        void OnClickConnect()
        {
            _textureSteamingClient.StartClient(_serverAddress.text,int.Parse(_serverPort.text));
        }

        void OnClickDisconnect()
        {
            _textureSteamingClient.StopClient();
        }

        void OnClickStartStreaming()
        {
            _webCamStreamingService.StartStreaming();
        }

        void OnClickStopStreaming()
        {
            _webCamStreamingService.StopStreaming();
        }
    }
}

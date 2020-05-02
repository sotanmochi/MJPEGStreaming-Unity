using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace MJPEGStreaming.Client
{
    public class WebCamStreamingService : MonoBehaviour
    {
        ITextureStreamingClient _textureStreamingCLient;
        WebCamTexture _webCamTexture;
        Texture2D _texture2D;
        float _intervalTimeMillisec;
        bool _initialized = false;
        IDisposable _disposable;
    
        int _frameCount;

        public void Initialize(WebCamTexture webCamTexture, ITextureStreamingClient textureStreamingClient, float intervalTimeMillisec = 100)
        {
            _webCamTexture = webCamTexture;
            _textureStreamingCLient = textureStreamingClient;
            _intervalTimeMillisec = intervalTimeMillisec;
            _texture2D = new Texture2D(_webCamTexture.width, _webCamTexture.height);
            _initialized = true;
        }

        public void StartStreaming()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            if (_initialized)
            {
                Debug.Log("***** StartStream *****");
                Debug.Log(" Interval time: " + _intervalTimeMillisec + "[ms]");
                StopStreaming();
                _disposable = this.UpdateAsObservable()
                                .ThrottleFirst(TimeSpan.FromMilliseconds(_intervalTimeMillisec))
                                .Subscribe(_ => 
                                {
                                    _texture2D.SetPixels32(_webCamTexture.GetPixels32());

                                    // sw.Start();

                                    byte[] textureData = ImageConversion.EncodeToJPG(_texture2D);
                                    _textureStreamingCLient.BroadcastRawTextureData(textureData, _texture2D.width, _texture2D.height, ++_frameCount);

                                    // sw.Stop();
                                    // Debug.Log("FrameCount: " + _frameCount + ", Processing time: " + sw.ElapsedMilliseconds + "[ms]");
                                    // sw.Reset();
                                });
            }
            else
            {
                Debug.LogError("WebCamStreamer has not been initialized.");
            }
        }

        public void StopStreaming()
        {
            if (_disposable != null)
            {
                _disposable.Dispose();
                Debug.Log("***** StopStream *****");
            }
        }
    }
}

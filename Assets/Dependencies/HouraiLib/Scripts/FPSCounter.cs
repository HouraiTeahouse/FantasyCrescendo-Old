using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace HouraiTeahouse {

    [RequireComponent(typeof(Text))]
    public sealed class FPSCounter : MonoBehaviour {

        [SerializeField]
        GameObject _text;

        [SerializeField]
        NetworkManager _networkManager;

        float deltaTime;
        float fps;
        string outputText;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            if (_text == null)
                _text = gameObject;
            StartCoroutine(UpdateDisplay());
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            if (_networkManager == null)
                _networkManager = NetworkManager.singleton;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
        }

        IEnumerator UpdateDisplay() {
            ITextAcceptor textAcceptor = null;
            while (true) {
                yield return new WaitForSeconds(0.5f);
                var text = $"{fps:0.}FPS";
                if (_networkManager != null && _networkManager.client != null)
                    text += $"/{_networkManager.client.GetRTT():0.} RTT";
                if (textAcceptor == null) {
                    textAcceptor = _text.SetUIText(text);
                    if (textAcceptor == null) {
                        Destroy(this);
                        yield break;
                    }
                } else {
                    textAcceptor.SetText(text);
                }
            }
        }

    }

}

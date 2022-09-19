using UnityEngine;
using UnityEngine.UI;

namespace Flux.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenURLButton : MonoBehaviour
    {
        private Button _button = null!;

        [SerializeField]
        private string _url = "https://google.com";
        
        private void OnEnable()
        {
            if (_button == null)
                _button = GetComponent<Button>();
            
            _button.onClick.AddListener(OpenURL);
        }

        private void OpenURL()
        {
            Application.OpenURL(_url);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OpenURL);
        }
    }
}

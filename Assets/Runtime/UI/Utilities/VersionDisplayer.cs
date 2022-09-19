using UnityEngine;
using TMPro;

namespace Flux.UI.Utilities
{
    [ExecuteAlways, RequireComponent(typeof(TMP_Text))]
    public class VersionDisplayer : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TMP_Text>().text = $"Version - {Application.version}";
        }
    }
}
using Elysium.Components;
using TMPro;
using UnityEngine;

namespace Elysium.TextMeshPro
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TMP_TextInputFieldValueBinding : ComponentPropertyBinding
    {
        private TMP_InputField _inputField = null!;

        private void Awake() => _inputField = GetComponent<TMP_InputField>();

        protected override void Start()
        {
            base.Start();
            _inputField.onValueChanged.AddListener(InputFieldValueChanged);
        }
        private void OnDestroy() => _inputField.onValueChanged.RemoveListener(InputFieldValueChanged);
        
        private void InputFieldValueChanged(string value) => SetValue(value);
        
        public override void OnValueChanged(object? value) => _inputField.text = value?.ToString() ?? string.Empty;
    }
}
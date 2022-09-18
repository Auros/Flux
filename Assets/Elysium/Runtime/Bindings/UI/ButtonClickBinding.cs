using Elysium.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Elysium.Bindings.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickBinding : ComponentPropertyBinding
    {
        private Button _button = null!;

        private void Awake() => _button = GetComponent<Button>();

        protected override void Start()
        {
            base.Start();
            _button.onClick.AddListener(ButtonClicked);
        }

        private void OnDestroy() => _button.onClick.RemoveListener(ButtonClicked);

        private void ButtonClicked() => SendCommand();

        protected internal override void SetInteraction(bool value)
        {
            _button.interactable = value;
        }

        public override void OnValueChanged(object? value)
        {
            // Do nothing. Commands don't need to set any value.
        }
    }
}
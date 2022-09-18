using Elysium.Components;

namespace Flux.UI.Bindings
{
    public class GameObjectActivityBinding : ComponentPropertyBinding
    {
        public override void OnValueChanged(object? value)
        {
            if (value is not bool valueAsBool)
                return;

            gameObject.SetActive(valueAsBool);
        }
    }
}
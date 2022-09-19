using Elysium.Components;
using UnityEngine.EventSystems;

namespace Flux.UI.Bindings
{
    public class SubmitCommandSender : ComponentPropertyBinding, ISubmitHandler
    {
        public override void OnValueChanged(object? value)
        {
            // Do nothing, we are only sending commands.
        }

        public void OnSubmit(BaseEventData eventData)
        {
            SendCommand();
        }
    }
}
using UnityEngine;

namespace Elysium.Templates
{
    public class ObjectInstantiator : MonoBehaviour
    {
        public virtual GameObject Instantiate(GameObject template, Transform parent)
        {
            return Object.Instantiate(template, parent);
        }
    }
}
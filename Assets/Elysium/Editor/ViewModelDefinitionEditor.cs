using UnityEditor;
using UnityEngine;

namespace Elysium.Editor
{
    [CustomEditor(typeof(ViewModelDefinition))]
    public class ViewModelDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var definition = (target as ViewModelDefinition)!;
            
            var unityObjectViewModel = EditorGUILayout.ObjectField("View Model", definition.ViewModelObject, typeof(Object), true);
            if (definition.ViewModel is not null && definition.ViewModel is not Object)
                return;
            
            definition.ViewModel = unityObjectViewModel;
        }
    }
}
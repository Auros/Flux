using UnityEditor;

namespace Elysium.Editor
{
    [CustomEditor(typeof(ViewModelDefinition))]
    public class ViewModelDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var definition = (target as ViewModelDefinition)!;
            
            var unityObjectViewModel = EditorGUILayout.ObjectField("View Model", definition.ViewModelObject, typeof(UnityEngine.Object), true);
            definition.ViewModel = unityObjectViewModel;
        }
    }
}
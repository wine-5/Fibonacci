#if UNITY_EDITOR
using Fibonacci.Utilities.ScriptableVariables;
using UnityEditor;
using UnityEngine;

namespace Fibonacci.Editor.ScriptableVariables
{
    [CustomPropertyDrawer(typeof(FloatReference))]
    public sealed class FloatReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);

            var useConstantProp = property.FindPropertyRelative("useConstant");
            var constantProp = property.FindPropertyRelative("constantValue");
            var variableProp = property.FindPropertyRelative("variable");

            const float toggleWidth = 18f;
            var toggleRect = new Rect(position.x, position.y, toggleWidth, position.height);
            var fieldRect = new Rect(position.x + toggleWidth + 4f, position.y, position.width - toggleWidth - 4f, position.height);

            useConstantProp.boolValue = EditorGUI.Toggle(toggleRect, useConstantProp.boolValue);

            if (useConstantProp.boolValue)
            {
                EditorGUI.PropertyField(fieldRect, constantProp, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, variableProp, GUIContent.none);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif

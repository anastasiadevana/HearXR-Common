using HearXR.Common;
using UnityEditor;
using UnityEngine;

namespace HearXR.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ButtonAttribute buttonSettings = (ButtonAttribute) attribute;
            return DisplayButton(ref buttonSettings) ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing : 0;
        }
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ButtonAttribute buttonSettings = (ButtonAttribute) attribute;
        
            if (!DisplayButton(ref buttonSettings)) return;
        
            string buttonLabel = (!string.IsNullOrEmpty(buttonSettings.label)) ? buttonSettings.label : label.text;

            if (property.serializedObject.targetObject is MonoBehaviour mb)
            {
                if (GUI.Button(position, buttonLabel))
                {
                    mb.SendMessage(buttonSettings.methodName, buttonSettings.methodParameter);
                }
            }
        }

        /// <summary>
        /// Should the button be displayed?
        /// </summary>
        /// <param name="buttonSettings">Button attribute for this field.</param>
        /// <returns>True if button should be displayed, false otherwise.</returns>
        private bool DisplayButton(ref ButtonAttribute buttonSettings)
        {
            return (buttonSettings.displayIn == ButtonAttribute.DisplayIn.PlaymodeAndEditor) ||
                   (buttonSettings.displayIn == ButtonAttribute.DisplayIn.Editor && !Application.isPlaying) ||
                   (buttonSettings.displayIn == ButtonAttribute.DisplayIn.Playmode && Application.isPlaying);
        }
    }
}

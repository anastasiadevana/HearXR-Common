//------------------------------------------------------------------------------
// In-editor test button
//------------------------------------------------------------------------------
//
// MIT License
//
// Copyright (c) 2022 Anastasia Devana
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//------------------------------------------------------------------------------
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

using UnityEditor;
using UnityEngine;

namespace HearXR.Common
{
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        private const float MIN_MAX_TEXTBOX_WIDTH = 50.0f;
        private const float SPACING = 5.0f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxRangeAttribute attributeSettings = (MinMaxRangeAttribute) attribute; 
            SerializedPropertyType propertyType = property.propertyType;
            Rect drawerRect = EditorGUI.PrefixLabel(position, label);

            if (propertyType == SerializedPropertyType.Vector2)
            {
                EditorGUI.BeginChangeCheck(); 
                
                Vector2 propertyValue = property.vector2Value;
                float minVal = propertyValue.x;
                float maxVal = propertyValue.y; 
                
                Rect minValRect = new Rect(drawerRect.position.x, 
                    drawerRect.position.y, 
                    MIN_MAX_TEXTBOX_WIDTH, 
                    drawerRect.height);
                
                minVal = EditorGUI.FloatField(minValRect, float.Parse(minVal.ToString("F3")));
                
                Rect sliderRect = new Rect(minValRect.position.x + minValRect.width + SPACING,
                    drawerRect.position.y, 
                    drawerRect.width - (MIN_MAX_TEXTBOX_WIDTH + SPACING) * 2, 
                    drawerRect.height);
                
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, 
                    attributeSettings.min,attributeSettings.max);

                Rect maxValRect = new Rect(sliderRect.position.x + sliderRect.width + SPACING,
                    drawerRect.position.y,
                    MIN_MAX_TEXTBOX_WIDTH,
                    drawerRect.height);

                maxVal = EditorGUI.FloatField(maxValRect, float.Parse(maxVal.ToString("F3")));
                
                // Check clamping.
                if (attributeSettings.useLimits)
                {
                    if (minVal > attributeSettings.minUpperLimit)
                    {
                        minVal = attributeSettings.minUpperLimit;
                    }

                    if (maxVal < attributeSettings.maxLowerLimits)
                    {
                        maxVal = attributeSettings.maxLowerLimits;
                    }
                }
                
                if (minVal < attributeSettings.min)
                {
                    minVal = attributeSettings.min;
                }

                if (maxVal > attributeSettings.max)
                {
                    maxVal = attributeSettings.max;
                }

                if (minVal > maxVal)
                {
                    minVal = maxVal;
                }

                propertyValue = new Vector2(minVal, maxVal);
                
                if (EditorGUI.EndChangeCheck())
                {
                    property.vector2Value = propertyValue;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(property: property, includeChildren: true);
            }
        }
    }
}

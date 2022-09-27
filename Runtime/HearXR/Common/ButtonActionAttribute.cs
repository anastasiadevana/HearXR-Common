using UnityEngine;

namespace HearXR.Common
{
    public class ButtonAttribute : PropertyAttribute
    {
        public enum DisplayIn
        {
            Playmode,
            Editor,
            PlaymodeAndEditor
        }
    
        public readonly string label;
        public readonly string methodName;
        public readonly object methodParameter;
        public readonly DisplayIn displayIn;

        public ButtonAttribute(string methodName, object methodParameter = null, DisplayIn displayIn = DisplayIn.Playmode, string label = "")
        {
            this.methodName = methodName;
            this.methodParameter = methodParameter;
            this.displayIn = displayIn;
            this.label = label;
        }
    }   
}

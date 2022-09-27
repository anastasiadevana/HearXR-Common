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

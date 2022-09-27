//------------------------------------------------------------------------------
// Custom editor min/max slider
//------------------------------------------------------------------------------
//
// Copyright (c) 2022 Anastasia Devana
//
// May be used for reference purposes only. Contact author for any intended
// duplication or intended use beyond exploratory read, compilation, testing.
// No license or rights transfer implied from the open publication. This
// software is made available strictly on an "as is" basis without warranty of
// any kind, express or implied.
//------------------------------------------------------------------------------
using UnityEngine;

namespace HearXR.Common
{
    /// <summary>
    /// Creates a custom min/max slider in editor, with optional upper and lower limits on min and max respectively.
    /// </summary>
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;
        public readonly float minUpperLimit;
        public readonly float maxLowerLimits;
        public readonly bool useLimits;

        public MinMaxRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
            useLimits = false;
        }

        public MinMaxRangeAttribute(float min, float max, float minUpperLimit, float maxLowerLimits)
        {
            this.min = min;
            this.max = max;
            this.minUpperLimit = minUpperLimit;
            this.maxLowerLimits = maxLowerLimits;
            useLimits = true;
        }
    }
} 
    


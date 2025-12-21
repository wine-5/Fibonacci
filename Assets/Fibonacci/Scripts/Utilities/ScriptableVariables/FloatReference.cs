using System;
using UnityEngine;

namespace Fibonacci.Utilities.ScriptableVariables
{
    [Serializable]
    public struct FloatReference
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private float constantValue;
        [SerializeField] private FloatVariable variable;

        public FloatReference(float constantValue)
        {
            useConstant = true;
            this.constantValue = constantValue;
            variable = null;
        }

        public float Value
        {
            get
            {
                if (useConstant) return constantValue;
                return variable != null ? variable.Value : constantValue;
            }
        }

        public bool UseConstant => useConstant;
        public float ConstantValue => constantValue;
        public FloatVariable Variable => variable;

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}

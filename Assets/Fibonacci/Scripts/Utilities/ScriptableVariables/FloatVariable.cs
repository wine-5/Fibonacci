using UnityEngine;

namespace Fibonacci.Utilities.ScriptableVariables
{
    [CreateAssetMenu(menuName = "Fibonacci/Variables/Float", fileName = "FloatVariable")]
    public sealed class FloatVariable : ScriptableObject
    {
        [SerializeField] private float value;

        public float Value
        {
            get => value;
            set => this.value = value;
        }

        public static implicit operator float(FloatVariable variable)
        {
            return variable != null ? variable.Value : 0f;
        }
    }
}

using UnityEngine;
using Fibonacci.InGame.BorderLine.Effects;
using Fibonacci.Utilities.ScriptableVariables;

namespace Fibonacci.InGame.BorderLine.UI
{
    [CreateAssetMenu(menuName = "Fibonacci/BorderLine/Effect Definition", fileName = "BorderLineEffectDefinition")]
    public sealed class BorderLineEffectDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite icon;

        [Header("Effect")]
        [SerializeField] private BorderLineEffect effect;
        [SerializeField] private FloatReference floatValue;

        public string Id => id;
        public Sprite Icon => icon;

        public BorderLineEffect Effect => effect;
        public FloatReference FloatValue => floatValue;
    }
}
using System;

namespace Jexreffy.FractionFarms {
    [Serializable]
    public struct SequenceStep {
        public bool IsProblem;
        public bool HasText;
        public string LanguageKey;
        public bool HasAnimation;
        public string AnimationKey;
    }
}
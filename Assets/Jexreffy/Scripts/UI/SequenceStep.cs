using System;

namespace Jexreffy.FractionFarms {
    [Serializable]
    public struct SequenceStep {
        public bool IsProblem;
        public int PointsAvailable;
        public int IncorrectPenalty;
        public string LanguageKey;
        public bool HasAnimation;
        public string AnimationKey;
    }
}
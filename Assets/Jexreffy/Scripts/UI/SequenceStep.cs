using System;

namespace Jexreffy.FractionFarms {
    [Serializable]
    public struct SequenceStep {
        public bool IsProblem;
        public bool EnableTiles;
        public int PointsAvailable;
        public int IncorrectPenalty;
        public string DenominatorKey;
        public string NumeratorKey;
        public string LanguageKey;
        public bool HasAnimation;
        public string AnimationKey;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;
using TMPro;

namespace Jexreffy.FractionFarms {
    public class EndingScreen : MonoBehaviour {

        public TextMeshProUGUI ScoreLabel;
        public TextMeshProUGUI ScoreValue;

        public Animator FaderAnimator;

        private const string SCORE = "score";
        private const string FADE_IN_TRIGGER = "FadeIn";

        void Awake() {
            ScoreLabel.text = PlatformController.Instance.GetText(SCORE);
            ScoreValue.text = PlatformController.Instance.Score.ToString();

            FaderAnimator.SetTrigger(FADE_IN_TRIGGER);
        }
    }
}
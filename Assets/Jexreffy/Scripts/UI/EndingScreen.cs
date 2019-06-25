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
        
        private static readonly int FadeIn = Animator.StringToHash("FadeIn");

        private const string SCORE = "score";

        private void Awake() {
            ScoreLabel.text = PlatformController.Instance.GetText(SCORE);
            ScoreValue.text = PlatformController.Instance.Score.ToString();

            FaderAnimator.SetTrigger(FadeIn);
            
            PlatformController.Instance.CompleteGame();
        }
    }
}
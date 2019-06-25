using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;

namespace Jexreffy.FractionFarms {
    public class TitleButtons : MonoBehaviour {
        public Animator SceneAnimator;
        public Animator FaderAnimator;

        private bool _gameReady;

        private static readonly WaitForSeconds SceneDelay = new WaitForSeconds(0.6f);
        private static readonly int FadeIn = Animator.StringToHash("FadeIn");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");
        private static readonly int GameStarted = Animator.StringToHash("GameStarted");

        void Start() {
            FaderAnimator.SetTrigger(FadeIn);
        }

        private void Update() {
            if (_gameReady || !PlatformController.Instance.DataLoaded) return;
            
            _gameReady = true;
            SceneAnimator.SetTrigger(GameStarted);
        }

        public void OnTitleAdvance() {
            FaderAnimator.SetTrigger(FadeOut);
            StartCoroutine(DelaySceneChange());
        }

        private static IEnumerator DelaySceneChange() {
            yield return SceneDelay;
            PlatformController.AdvanceScene();
        }
    }
}


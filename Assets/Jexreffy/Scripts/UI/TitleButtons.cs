using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;

namespace Jexreffy.FractionFarms {
    public class TitleButtons : MonoBehaviour {
        public Animator FaderAnimator;

        private static readonly WaitForSeconds SCENE_DELAY = new WaitForSeconds(0.6f);

        private const string FADE_IN_TRIGGER = "FadeIn";
        private const string FADE_OUT_TRIGGER = "FadeOut";

        void Start() {
            FaderAnimator.SetTrigger(FADE_IN_TRIGGER);
        }

        public void OnTitleAdvance() {
            FaderAnimator.SetTrigger(FADE_OUT_TRIGGER);
            StartCoroutine(DelaySceneChange());
        }

        public IEnumerator DelaySceneChange() {
            yield return SCENE_DELAY;
            PlatformController.Instance.AdvanceScene();
        }
    }
}


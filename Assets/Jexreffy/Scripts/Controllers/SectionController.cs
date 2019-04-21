using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;
using TMPro;

namespace Jexreffy.FractionFarms {
    public class SectionController : MonoBehaviour {

        public GameObject InstructionContainer;
        public TextMeshProUGUI Instructions;
        public Button SkipButton;
        public Button SubmitButton;

        public GameObject ProblemContainer;
        public TextMeshProUGUI ProblemXNumerator;
        public TextMeshProUGUI ProblemXDenominator;
        public TextMeshProUGUI ProblemYNumerator;
        public TextMeshProUGUI ProblemYDenominator;

        public TextMeshProUGUI AnswerNumerator;
        public TextMeshProUGUI AnswerDivider;
        public TextMeshProUGUI AnswerDenominator;

        public List<SequenceStep> SequenceSteps = new List<SequenceStep>();

        public List<int> ProblemXNumerators = new List<int>();
        public List<int> ProblemXDenominators = new List<int>();
        public List<int> ProblemYNumerators = new List<int>();
        public List<int> ProblemYDenominators = new List<int>();

        protected int _currentStep = -1;
        protected int _currentQuestion;

        protected int _currentNumerator;
        protected int _currentDenominator;

        void Start() {
            AdvanceStep();
        }

        protected SequenceStep CurrentStep { get { return SequenceSteps[_currentStep]; } }

        private void AdvanceStep() {
            _currentStep++;

            if (_currentStep >= SequenceSteps.Count) {
                //TODO Advance to the next scene
            } else if (CurrentStep.IsProblem) {
                InstructionContainer.SetActive(false);
                ProblemContainer.SetActive(true);
                SkipButton.gameObject.SetActive(false);
                SubmitButton.gameObject.SetActive(true);
                AnswerNumerator.gameObject.SetActive(true);
                AnswerDivider.gameObject.SetActive(true);
                AnswerDenominator.gameObject.SetActive(true);

                OnQuestionStep();

                ProblemXNumerator.text = ProblemXNumerators[_currentQuestion].ToString();
                ProblemXDenominator.text = ProblemXDenominators[_currentQuestion].ToString();
                ProblemYNumerator.text = ProblemYNumerators[_currentQuestion].ToString();
                ProblemYDenominator.text = ProblemYDenominators[_currentQuestion].ToString();
            } else {
                InstructionContainer.SetActive(true);
                ProblemContainer.SetActive(false);
                SkipButton.gameObject.SetActive(true);
                SubmitButton.gameObject.SetActive(false);
                AnswerNumerator.gameObject.SetActive(false);
                AnswerDivider.gameObject.SetActive(false);
                AnswerDenominator.gameObject.SetActive(false);

                OnInstructionStep();

                Instructions.text = PlatformController.Instance.GetTextAndSpeak(CurrentStep.LanguageKey);
            }
        }

        public virtual void OnInstructionStep() { }
        public virtual void OnQuestionStep() { }

        public void OnSubmitAnswer() {
            if (_currentNumerator == ProblemXNumerators[_currentQuestion] * ProblemYNumerators[_currentQuestion] &&
                _currentDenominator == ProblemXDenominators[_currentQuestion] * ProblemYDenominators[_currentQuestion]) {
                _currentQuestion++;
                OnCorrectAnswer();
                AdvanceStep();
            }
        }

        public virtual void OnCorrectAnswer() { }

        public void OnSkipInstructions() {
            AdvanceStep();
        }
    }
}

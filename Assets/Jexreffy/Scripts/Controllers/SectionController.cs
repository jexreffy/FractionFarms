using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;
using TMPro;

namespace Jexreffy.FractionFarms {
    public class SectionController : MonoBehaviour {

        public int XSize;
        public int YSize;

        public Animator SequenceAnimator;
        public Animator FaderAnimator;
        public GameObject InstructionContainer;
        public TextMeshProUGUI Instructions;
        public Button SkipButton;
        public Button SubmitButton;

        public RectTransform ScoreTransform;
        public TextMeshProUGUI ScoreLabel;
        public TextMeshProUGUI ScoreValue;

        public GameObject ProblemContainer;
        public TextMeshProUGUI ProblemXWhole;
        public TextMeshProUGUI ProblemXNumerator;
        public TextMeshProUGUI ProblemXDenominator;
        public TextMeshProUGUI ProblemYWhole;
        public TextMeshProUGUI ProblemYNumerator;
        public TextMeshProUGUI ProblemYDenominator;

        public TextMeshProUGUI AnswerWhole;
        public TextMeshProUGUI AnswerNumerator;
        public TextMeshProUGUI AnswerDivider;
        public TextMeshProUGUI AnswerDenominator;

        public List<SequenceStep> SequenceSteps = new List<SequenceStep>();

        public List<int> ProblemXWholes = new List<int>();
        public List<int> ProblemXNumerators = new List<int>();
        public List<int> ProblemXDenominators = new List<int>();
        public List<int> ProblemYWholes = new List<int>();
        public List<int> ProblemYNumerators = new List<int>();
        public List<int> ProblemYDenominators = new List<int>();
        public List<int> AnswerWholes = new List<int>();
        public List<int> AnswerNumerators = new List<int>();
        public List<int> AnswerDenominators = new List<int>();

        protected int _currentStep = -1;
        protected int _currentQuestion;
        protected int _currentScore;

        protected int _currentXWhole;
        protected int _currentXNumerator;
        protected int _currentXDenominator = 1;

        protected int _currentYWhole;
        protected int _currentYNumerator;
        protected int _currentYDenominator = 1;
        
        protected int _currentWhole;
        protected int _currentNumerator;
        protected int _currentDenominator = 1;

        protected bool _isError;
        protected string _errorText;

        private static readonly WaitForSeconds SceneDelay = new WaitForSeconds(0.6f);
        private static readonly int Default = Animator.StringToHash("Default");
        private static readonly int FadeIn = Animator.StringToHash("FadeIn");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");
        
        private static readonly string DEFAULT_ANIMATION = "Default";

        private const string SCORE = "score";

        private void Start() {
            ScoreLabel.text = PlatformController.Instance.GetText(SCORE);
            ScoreValue.text = PlatformController.Instance.Score.ToString();

            AdvanceStep();
            FaderAnimator.SetTrigger(FadeIn);
        }

        protected SequenceStep CurrentStep { get { return SequenceSteps[_currentStep]; } }
        public bool EnableTiles { get { return CurrentStep.EnableTiles; } }

        private void AdvanceStep() {
            _currentStep++;

            if (_currentStep >= SequenceSteps.Count) {
                FaderAnimator.SetTrigger(FadeOut);
                StartCoroutine(DelaySceneChange());
            } else if (CurrentStep.IsProblem) {
                ShowProblem();
            } else {
                ShowInstruction();
            }
        }

        private void ShowProblem() {
            InstructionContainer.SetActive(false);
            ProblemContainer.SetActive(true);
            SkipButton.gameObject.SetActive(false);
            SubmitButton.gameObject.SetActive(true);

            if (AnswerWhole != null) {
                AnswerWhole.gameObject.SetActive(CurrentStep.EnableTiles);
                AnswerWhole.text = "0";
            }
            AnswerNumerator.gameObject.SetActive(CurrentStep.EnableTiles);
            AnswerNumerator.text = "0";
            AnswerDivider.gameObject.SetActive(CurrentStep.EnableTiles);
            AnswerDenominator.gameObject.SetActive(CurrentStep.EnableTiles);
            AnswerDenominator.text = "1";

            _currentScore = CurrentStep.PointsAvailable;

            OnQuestionStep();

            if (ProblemXWhole != null && ProblemXWholes[_currentQuestion] > 0) {
                ProblemXWhole.gameObject.SetActive(true);
                ProblemXWhole.text = ProblemXWholes[_currentQuestion].ToString();
            } else if (ProblemXWhole != null) {
                ProblemXWhole.gameObject.SetActive(false);
            }
            ProblemXNumerator.text = ProblemXNumerators[_currentQuestion].ToString();
            ProblemXDenominator.text = ProblemXDenominators[_currentQuestion].ToString();

            if (ProblemYWhole != null && ProblemYWholes[_currentQuestion] > 0) {
                ProblemYWhole.gameObject.SetActive(true);
                ProblemYWhole.text = ProblemYWholes[_currentQuestion].ToString();
            } else if (ProblemYWhole != null) {
                ProblemYWhole.gameObject.SetActive(false);
            }
            ProblemYNumerator.text = ProblemYNumerators[_currentQuestion].ToString();
            ProblemYDenominator.text = ProblemYDenominators[_currentQuestion].ToString();

            if (SequenceAnimator != null) SequenceAnimator.SetTrigger(Default);
        }

        private void ShowInstruction() {
            InstructionContainer.SetActive(true);
            ProblemContainer.SetActive(false);
            SkipButton.gameObject.SetActive(true);
            SubmitButton.gameObject.SetActive(false);

            if (AnswerWhole != null) AnswerWhole.gameObject.SetActive(false);
            AnswerNumerator.gameObject.SetActive(false);
            AnswerDivider.gameObject.SetActive(false);
            AnswerDenominator.gameObject.SetActive(false);

            OnInstructionStep();

            Instructions.text = PlatformController.Instance.GetTextAndSpeak(_isError ? _errorText : CurrentStep.LanguageKey);
            
            if (SequenceAnimator != null) SequenceAnimator.SetTrigger(CurrentStep.HasAnimation ? CurrentStep.AnimationKey : DEFAULT_ANIMATION);
        }

        public void OnSubmitAnswer() {
            if (_currentWhole == AnswerWholes[_currentQuestion] &&
                _currentNumerator == AnswerNumerators[_currentQuestion] &&
                _currentDenominator == AnswerDenominators[_currentQuestion]) {
                _currentWhole = 0;
                _currentNumerator = 0;
                _currentDenominator = 1;

                _currentXWhole = 0;
                _currentXNumerator = 0;
                _currentXDenominator = 1;

                _currentYWhole = 0;
                _currentYNumerator = 0;
                _currentYDenominator = 1;
                PlatformController.Instance.UpdateProgress(_currentScore);
                ScoreValue.text = PlatformController.Instance.Score.ToString();
                _currentQuestion++;
                AdvanceStep();
            } else {
                _isError = true;
                _currentScore = Mathf.Max(_currentScore - CurrentStep.IncorrectPenalty, 0);

                if (_currentXDenominator != ProblemXDenominators[_currentQuestion] ||
                    _currentYDenominator != ProblemYDenominators[_currentQuestion]) {
                    _errorText = CurrentStep.DenominatorKey;
                } else if (_currentXWhole != ProblemXWholes[_currentQuestion] ||
                           _currentYWhole != ProblemYWholes[_currentQuestion] ||
                           _currentXNumerator != ProblemXNumerators[_currentQuestion] ||
                           _currentYNumerator != ProblemYNumerators[_currentQuestion]) {
                    _errorText = CurrentStep.NumeratorKey;
                } else {
                    _errorText = CurrentStep.LanguageKey;
                }
                ShowInstruction();
            }

            OnAnswerSubmitted();
        }

        public void OnSkipInstructions() {
            if (_isError) {
                _isError = false;
                ShowProblem();
            } else {
                AdvanceStep();
            }
        }

        public static IEnumerator DelaySceneChange() {
            yield return SceneDelay;
            PlatformController.AdvanceScene();
        }

        public virtual void OnInstructionStep() { }
        public virtual void OnQuestionStep() { }
        public virtual void DisableTiles() { }
        public virtual void UpdateDenominator(int tileIndex, bool yAxis) { }
        public virtual void UpdateNumerator() { }
        public virtual void UpdateHighlighting(int tileIndex, bool yAxis) { }
        public virtual void OnAnswerSubmitted() { }
    }
}

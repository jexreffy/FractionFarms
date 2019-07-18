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
        public Button HintButton;

        public RectTransform ScoreTransform;
        public TextMeshProUGUI ScoreLabel;
        public TextMeshProUGUI ScoreValue;

        public GameObject ProblemContainer;
        public TextMeshProUGUI ProblemLabel;
        public TextMeshProUGUI ProblemXWhole;
        public TextMeshProUGUI ProblemXNumerator;
        public TextMeshProUGUI ProblemXDenominator;
        public TextMeshProUGUI ProblemYWhole;
        public TextMeshProUGUI ProblemYNumerator;
        public TextMeshProUGUI ProblemYDenominator;

        public GameObject AnswerContainer;
        public TextMeshProUGUI AnswerLabel;
        public TextMeshProUGUI AnswerXWhole;
        public TextMeshProUGUI AnswerXNumerator;
        public TextMeshProUGUI AnswerXDenominator;
        public TextMeshProUGUI AnswerYWhole;
        public TextMeshProUGUI AnswerYNumerator;
        public TextMeshProUGUI AnswerYDenominator;
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
        public List<int> AnswerTiles = new List<int>();
        public List<int> AnswerWholes = new List<int>();
        public List<int> AnswerNumerators = new List<int>();
        public List<int> AnswerDenominators = new List<int>();

        protected int _currentStep = -1;
        protected int _currentQuestion;
        protected int _currentScore;
        protected int _currentTile = -1;

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

        protected bool _showHint;

        private static readonly WaitForSeconds HintDelay = new WaitForSeconds(5f);
        private static readonly WaitForSeconds SceneDelay = new WaitForSeconds(0.6f);
        private static readonly int Default = Animator.StringToHash("Default");
        private static readonly int FadeIn = Animator.StringToHash("FadeIn");
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");

        private const string DEFAULT_ANIMATION = "Default";
        private const string HINT_SUBMIT = "hint_submit";

        private const string SCORE = "score";
        private const string QUESTION = "question";
        private const string ANSWER = "answer";

        private void Start() {
            ScoreLabel.text = PlatformController.Instance.GetText(SCORE);
            ScoreValue.text = PlatformController.Instance.Score.ToString();

            ProblemLabel.text = PlatformController.Instance.GetText(QUESTION);
            AnswerLabel.text = PlatformController.Instance.GetText(ANSWER);

            AdvanceStep();
            FaderAnimator.SetTrigger(FadeIn);
        }

        protected SequenceStep CurrentStep { get { return SequenceSteps[_currentStep]; } }
        public bool EnableTiles => CurrentStep.EnableTiles;

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

        private void ShowProblem(bool hint = false) {
            InstructionContainer.SetActive(false);
            ProblemContainer.SetActive(true);
            AnswerContainer.SetActive(true);
            SkipButton.gameObject.SetActive(false);
            SubmitButton.gameObject.SetActive(true);
            HintButton.gameObject.SetActive(true);
            
            if (AnswerWhole != null) AnswerWhole.gameObject.SetActive(CurrentStep.EnableTiles);
            AnswerNumerator.gameObject.SetActive(CurrentStep.EnableTiles);
            AnswerDivider.gameObject.SetActive(CurrentStep.EnableTiles);
            AnswerDenominator.gameObject.SetActive(CurrentStep.EnableTiles);

            if (!hint) {
                UpdateAnswerText();

                if (!_isError) _currentScore = CurrentStep.PointsAvailable;

                OnQuestionStep();
            }

            if (ProblemXWhole != null) ProblemXWhole.text = ProblemXWholes[_currentQuestion].ToString();
            ProblemXNumerator.text = ProblemXNumerators[_currentQuestion].ToString();
            ProblemXDenominator.text = ProblemXDenominators[_currentQuestion].ToString();

            if (ProblemYWhole != null) ProblemYWhole.text = ProblemYWholes[_currentQuestion].ToString();
            ProblemYNumerator.text = ProblemYNumerators[_currentQuestion].ToString();
            ProblemYDenominator.text = ProblemYDenominators[_currentQuestion].ToString();

            if (SequenceAnimator != null) SequenceAnimator.SetTrigger(Default);
        }

        private void ShowInstruction(bool hint = false) {
            InstructionContainer.SetActive(true);
            ProblemContainer.SetActive(false);
            AnswerContainer.SetActive(false);
            SkipButton.gameObject.SetActive(true);
            SubmitButton.gameObject.SetActive(false);
            HintButton.gameObject.SetActive(false);

            if (AnswerWhole != null) AnswerWhole.gameObject.SetActive(false);
            AnswerNumerator.gameObject.SetActive(false);
            AnswerDivider.gameObject.SetActive(false);
            AnswerDenominator.gameObject.SetActive(false);

            if (!hint) OnInstructionStep();

            Instructions.text = PlatformController.Instance.GetTextAndSpeak(_isError ? _errorText : CurrentStep.LanguageKey);
            
            if (SequenceAnimator != null) SequenceAnimator.SetTrigger(CurrentStep.HasAnimation ? CurrentStep.AnimationKey : DEFAULT_ANIMATION);
        }

        protected void UpdateAnswerText() {
            if (AnswerXWhole != null) AnswerXWhole.text = _currentXWhole.ToString();
            AnswerXNumerator.text = _currentXNumerator.ToString();
            AnswerXDenominator.text = _currentXDenominator.ToString();
            
            if (AnswerYWhole != null) AnswerYWhole.text = _currentYWhole.ToString();
            AnswerYNumerator.text   = _currentYNumerator.ToString();
            AnswerYDenominator.text = _currentYDenominator.ToString();
            
            if (AnswerWhole != null) AnswerWhole.text = _currentWhole.ToString();
            AnswerNumerator.text   = _currentNumerator.ToString();
            AnswerDenominator.text = _currentDenominator.ToString();
        }

        public void OnSubmitAnswer() {
            if (IsProblemCorrect) {
                PlatformController.Instance.UpdateProgress(_currentScore);
                ScoreValue.text = PlatformController.Instance.Score.ToString();
                _currentQuestion++;
                AdvanceStep();
            } else {
                _currentScore = Mathf.Max(_currentScore - CurrentStep.IncorrectPenalty, 0);

                EvaluateProblemProgress();
                ShowInstruction();
            }
            
            _currentTile = -1;
                
            _currentWhole       = 0;
            _currentNumerator   = 0;
            _currentDenominator = 1;

            _currentXWhole       = 0;
            _currentXNumerator   = 0;
            _currentXDenominator = 1;

            _currentYWhole       = 0;
            _currentYNumerator   = 0;
            _currentYDenominator = 1;

            OnAnswerSubmitted();
        }

        public void OnSkipInstructions() {
            if (!_isError && CurrentStep.IsProblem) {
                if (_showHint) return;
                
                StartCoroutine(ShowHint());
            } else if (_isError) {
                ShowProblem();
                _isError = false;
            } else {
                AdvanceStep();
            }
        }

        private bool IsProblemCorrect =>
            _currentTile         == AnswerTiles[_currentQuestion]          &&
            _currentWhole        == AnswerWholes[_currentQuestion]         &&
            _currentNumerator    == AnswerNumerators[_currentQuestion]     &&
            _currentXDenominator == ProblemXDenominators[_currentQuestion] &&
            _currentYDenominator == ProblemYDenominators[_currentQuestion] &&
            _currentDenominator  == AnswerDenominators[_currentQuestion];

        private void EvaluateProblemProgress(bool hint = false) {
            _isError = true;
            
            if (_currentDenominator != AnswerDenominators[_currentQuestion]) {
                if (_currentXDenominator != ProblemXDenominators[_currentQuestion] &&
                    _currentYDenominator != ProblemYDenominators[_currentQuestion]) {
                    _errorText = CurrentStep.DenominatorKey;
                } else if (_currentXDenominator != ProblemXDenominators[_currentQuestion]) {
                    _errorText = $"{CurrentStep.DenominatorKey}_x";
                } else {
                    _errorText = $"{CurrentStep.DenominatorKey}_y";
                }
            } else if (_currentTile != AnswerTiles[_currentQuestion]) {
                _errorText = CurrentStep.TileKey;
            } else if (_currentXDenominator != ProblemXDenominators[_currentQuestion] ||
                       _currentYDenominator != ProblemYDenominators[_currentQuestion]) {
                _errorText = CurrentStep.ReverseKey;
            } else if (CurrentStep.EnableTiles &&
                       (_currentXWhole     != ProblemXWholes[_currentQuestion]     ||
                        _currentYWhole     != ProblemYWholes[_currentQuestion]     ||
                        _currentXNumerator != ProblemXNumerators[_currentQuestion] ||
                        _currentYNumerator != ProblemYNumerators[_currentQuestion])) {

                if ((_currentXWhole != ProblemXWholes[_currentQuestion] || _currentXNumerator != ProblemXNumerators[_currentQuestion]) &&
                    (_currentYWhole != ProblemYWholes[_currentQuestion] || _currentYNumerator != ProblemYNumerators[_currentQuestion])) {
                    _errorText = CurrentStep.NumeratorKey;
                } else if (_currentXWhole != ProblemXWholes[_currentQuestion] || _currentXNumerator != ProblemXNumerators[_currentQuestion]) {
                    _errorText = $"{CurrentStep.NumeratorKey}_x";
                } else {
                    _errorText = $"{CurrentStep.NumeratorKey}_y";
                }
            } else if (!IsProblemCorrect) {
                _errorText = CurrentStep.LanguageKey;
            } else {
                _errorText = HINT_SUBMIT;
            }
        }

        public IEnumerator ShowHint() {
            _showHint = true;
            EvaluateProblemProgress(true);
            ShowInstruction(true);
            HintButton.gameObject.SetActive(false);
            
            yield return HintDelay;
            
            _showHint = false;
            _isError = false;
            ShowProblem(true);
            HintButton.gameObject.SetActive(true);
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

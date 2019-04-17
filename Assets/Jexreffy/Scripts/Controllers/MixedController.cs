using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jexreffy.FractionFarms {
    public class MixedController : MonoBehaviour {

        public List<UnitTile> Tiles = new List<UnitTile>();

        public GameObject InstructionContainer;
        public TextMeshProUGUI Instructions;

        public GameObject ProblemContainer;
        public TextMeshProUGUI ProblemXNumerator;
        public TextMeshProUGUI ProblemXDenominator;
        public TextMeshProUGUI ProblemYNumerator;
        public TextMeshProUGUI ProblemYDenominator;

        public TextMeshProUGUI AnswerNumerator;
        public TextMeshProUGUI AnswerDenominator;

        public List<int> ProblemXNumerators = new List<int>();
        public List<int> ProblemXDenominators = new List<int>();
        public List<int> ProblemYNumerators = new List<int>();
        public List<int> ProblemYDenominators = new List<int>();

        private int _currentQuestion;

        private int _currentNumerator;
        private int _currentDenominator;
        
        void Awake() {
            SetInstructionText(true);
        }
        
        void Update() {
            if (_currentDenominator != Tiles[0].Denominator) {
                _currentDenominator = Tiles[0].Denominator;
                AnswerDenominator.text = _currentDenominator.ToString();
            }

            int selectedCount = Tiles[0].GetSelected(1);
            if (_currentNumerator != selectedCount) {
                _currentNumerator = selectedCount;
                AnswerNumerator.text = _currentNumerator.ToString();
            }
        }

        private void SetInstructionText(bool isProblem) {
            if (isProblem) {
                InstructionContainer.SetActive(false);
                ProblemContainer.SetActive(true);

                ProblemXNumerator.text = ProblemXNumerators[_currentQuestion].ToString();
                ProblemXDenominator.text = ProblemXDenominators[_currentQuestion].ToString();
                ProblemYNumerator.text = ProblemYNumerators[_currentQuestion].ToString();
                ProblemYDenominator.text = ProblemYDenominators[_currentQuestion].ToString();
            } else {
                InstructionContainer.SetActive(true);
                ProblemContainer.SetActive(false);
            }
        }

        public void OnSubmitAnswer() {
            if (_currentNumerator == ProblemXNumerators[_currentQuestion] * ProblemYNumerators[_currentQuestion] &&
                _currentDenominator == ProblemXDenominators[_currentQuestion] * ProblemYDenominators[_currentQuestion]) {
                _currentQuestion++;
                Tiles[0].ResetTile();
                SetInstructionText(true);
            }
        }

        public void OnSkipInstructions() {

        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jexreffy.FractionFarms {
    public class BasicController : MonoBehaviour {

        public TextMeshProUGUI Instructions;
        public UnitTile Tile;

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

        }
        
        void Update() {
            if (_currentDenominator != Tile.Denominator) {
                _currentDenominator = Tile.Denominator;
                AnswerDenominator.text = _currentDenominator.ToString();
            }

            int selectedCount = Tile.GetSelected(1);
            if (_currentNumerator != selectedCount) {
                _currentNumerator = selectedCount;
                AnswerNumerator.text = _currentNumerator.ToString();
            }
        }

        private void SetQuestionText() {

        }

        public void OnSubmitAnswer() {
            if (_currentNumerator == ProblemXNumerators[_currentQuestion] * ProblemYNumerators[_currentQuestion] &&
                _currentDenominator == ProblemXDenominators[_currentQuestion] * ProblemYDenominators[_currentQuestion]) {
                _currentQuestion++;
                Tile.ResetTile();
                Debug.Log(Time.time + ": Question Correct");
            }
        }

        public void OnSkipInstructions() {

        }
    }
}
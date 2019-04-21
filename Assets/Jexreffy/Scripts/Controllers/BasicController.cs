using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;
using TMPro;

namespace Jexreffy.FractionFarms {
    public sealed class BasicController : SectionController {

        public UnitTile Tile;

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

        public override void OnInstructionStep() {
            Tile.DisableTile();
        }

        public override void OnQuestionStep() {
            Tile.EnableTile();
        }

        public override void OnCorrectAnswer() {
            Tile.ResetTile();
        }
    }
}
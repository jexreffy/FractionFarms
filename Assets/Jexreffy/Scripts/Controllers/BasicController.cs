using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Jexreffy.LoL;
using TMPro;

namespace Jexreffy.FractionFarms {
    public sealed class BasicController : SectionController {

        public UnitTile Tile;

        private void Awake() {
            Tile.Parent = this;
        }

        public override void OnInstructionStep() {
            Tile.DisableTile();
        }

        public override void OnQuestionStep() {
            Tile.EnableTile();
        }

        public override void DisableTiles() {
            Tile.DisableTile();
        }

        public override void UpdateDenominator(int tileIndex, bool yAxis) {
            if (_currentDenominator == Tile.CurrentDenominator) return;

            _currentTile = 0;
            _currentDenominator = Tile.CurrentDenominator;
            if (yAxis) {
                _currentYDenominator = Tile.CurrentYDenominator;
            } else {
                _currentXDenominator = Tile.CurrentXDenominator;
            }
            
            UpdateAnswerText();
        }

        public override void UpdateNumerator() {
            var selectedCount = Tile.GetSelected(1);
            if (_currentNumerator == selectedCount) return;
            
            _currentNumerator    = selectedCount;
            
            UpdateAnswerText();
        }

        public override void UpdateHighlighting(int tileIndex, bool yAxis) {
            if (yAxis) {
                _currentYNumerator = Tile.CurrentYNumerator;
            } else {
                _currentXNumerator = Tile.CurrentXNumerator;
            }
            
            UpdateAnswerText();
        }

        public override void OnAnswerSubmitted() {
            Tile.ResetTile();
        }
    }
}
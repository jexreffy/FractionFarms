using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jexreffy.FractionFarms {
    public sealed class ImproperController : SectionController {

        public List<UnitTile> Tiles = new List<UnitTile>();

        private bool _isEnabling;

        private void Awake() {
            for (var i = 0; i < Tiles.Count; i++) {
                Tiles[i].Parent = this;
                Tiles[i].Index = i;
            }
        }

        public override void OnInstructionStep() {
            DisableTiles();
        }

        public override void OnQuestionStep() {
            _isEnabling = true;
            for (var i = 0; i < Tiles.Count; i++) {
                Tiles[i].EnableTile();
            }
            _isEnabling = false;
        }

        public override void DisableTiles() {
            if (_isEnabling) return;

            for (var i = 0; i < Tiles.Count; i++) {
                Tiles[i].DisableTile();
            }
        }

        public override void UpdateDenominator(int tileIndex, bool yAxis) {
            if (_currentDenominator == Tiles[tileIndex].CurrentDenominator) return;

            _currentTile = 0;
            _currentDenominator = Tiles[tileIndex].CurrentDenominator;
            if (yAxis) {
                _currentYDenominator = Tiles[tileIndex].CurrentYDenominator;
            } else {
                _currentXDenominator = Tiles[tileIndex].CurrentXDenominator;
            }
            AnswerDenominator.text = _currentDenominator.ToString();
        }

        public override void UpdateNumerator() {
            _currentNumerator = 0;
            for (var i = 0; i < Tiles.Count; i++) {
                _currentNumerator += Tiles[i].GetSelected(1);
            }
            AnswerNumerator.text = _currentNumerator.ToString();
        }

        public override void UpdateHighlighting(int tileIndex, bool yAxis) {
            if ((!yAxis || (_currentYWhole     == tileIndex / XSize &&
                            _currentYNumerator == Tiles[tileIndex].CurrentYNumerator)) &&
                (yAxis || (_currentXWhole     == tileIndex % XSize &&
                           _currentXNumerator == Tiles[tileIndex].CurrentXNumerator))) return;
            
            if (yAxis) {
                _currentYWhole     = tileIndex / XSize;
                _currentYNumerator = Tiles[tileIndex].CurrentYNumerator;
            } else {
                _currentXWhole     = tileIndex % XSize;
                _currentXNumerator = Tiles[tileIndex].CurrentXNumerator;
            }

            for (var i = 0; i < Tiles.Count; i++) {
                if (i == tileIndex) continue;

                if (yAxis && i / XSize != tileIndex / XSize) {
                    Tiles[i].OnHighlightY(i / XSize < tileIndex / XSize ? _currentYDenominator : 0);
                } else if (!yAxis && i % XSize != tileIndex % XSize) {
                    Tiles[i].OnHighlightX(i % XSize < tileIndex % XSize ? _currentXDenominator : 0);
                }
            }
        }

        public override void OnAnswerSubmitted() {
            for (var i = 0; i < Tiles.Count; i++) {
                Tiles[i].ResetTile();
            }
        }
    }
}
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

        void Awake() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].Parent = this;
            }
        }

        public override void OnInstructionStep() {
            DisableTiles();
        }

        public override void OnQuestionStep() {
            _isEnabling = true;
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].EnableTile();
            }
            _isEnabling = false;
        }

        public override void DisableTiles() {
            if (_isEnabling) return;

            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].DisableTile();
            }
        }

        public override void UpdateDenominator(int tileIndex, bool yAxis) {
            if (_currentDenominator != Tiles[tileIndex].Denominator) {
                _currentDenominator = Tiles[tileIndex].Denominator;
                AnswerDenominator.text = _currentDenominator.ToString();
            }
        }

        public override void UpdateNumerator(int tileIndex) {
            _currentNumerator = 0;
            for (int i = 0; i < Tiles.Count; i++) {
                _currentNumerator += Tiles[i].GetSelected(1);
            }
            AnswerNumerator.text = _currentNumerator.ToString();
        }

        public override void OnCorrectAnswer() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].ResetTile();
            }
        }
    }
}
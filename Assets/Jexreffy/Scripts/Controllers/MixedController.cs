using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jexreffy.FractionFarms {
    public sealed class MixedController : SectionController {

        public int XSize;
        public int YSize;
        public List<UnitTile> Tiles = new List<UnitTile>();

        private int _xDenominator = 1;
        private int _yDenominator = 1;
        
        void Awake() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].Parent = this;
                Tiles[i].Index = i;
            }
        }

        public override void OnInstructionStep() {
            DisableTiles();
        }

        public override void OnQuestionStep() {
            //Tile.EnableTile();
        }

        public override void DisableTiles() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].DisableTile();
            }
        }

        public override void UpdateDenominator(int tileIndex, bool yAxis) {
            if (Tiles[tileIndex].IsTileEnabled) {
                if (yAxis) {
                    _yDenominator = Tiles[tileIndex].CurrentYDenominator;
                } else {
                    _xDenominator = Tiles[tileIndex].CurrentXDenominator;
                }
                
                _currentDenominator = _xDenominator * _yDenominator;
                AnswerDenominator.text = _currentDenominator.ToString();
                UpdateNumerator();

                for (int i = 0; i < Tiles.Count; i++) {
                    if (i == tileIndex) continue;

                    if ((yAxis && i / XSize != tileIndex / XSize) || (!yAxis && i % XSize != tileIndex % XSize)) {
                        Tiles[i].ResetTile(true, !yAxis, yAxis);
                    } else if ((yAxis && i / XSize == tileIndex / XSize && Tiles[i].CurrentXDenominator > 1 && Tiles[i].CurrentXDenominator != _xDenominator) ||
                               (!yAxis && i % XSize == tileIndex % XSize && Tiles[i].CurrentYDenominator > 1 && Tiles[i].CurrentYDenominator != _yDenominator)) {

                        if (yAxis) {
                            _xDenominator = 1;
                        } else {
                            _yDenominator = 1;
                        }
                        Tiles[i].ResetTile(true, yAxis, !yAxis);
                        UpdateNumerator();
                    }
                }
            }
        }

        public override void UpdateNumerator() {
            int totalNumerator = 0;
            for (int i = 0; i < Tiles.Count; i++) {
                totalNumerator += Tiles[i].GetSelected(1) * (_currentDenominator / Tiles[i].CurrentDenominator);
            }

            _currentWhole = totalNumerator / _currentDenominator;
            AnswerWhole.text = _currentWhole.ToString();
            _currentNumerator = totalNumerator % _currentDenominator;
            AnswerNumerator.text = _currentNumerator.ToString();
        }

        public override void OnAnswerSubmitted() {
            _xDenominator = 1;
            _yDenominator = 1;
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].ResetTile();
            }
        }
    }
}
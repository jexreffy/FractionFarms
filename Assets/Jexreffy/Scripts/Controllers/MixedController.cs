using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jexreffy.FractionFarms {
    public sealed class MixedController : SectionController {

        public List<UnitTile> Tiles = new List<UnitTile>();
        
        void Awake() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].Parent = this;
                Tiles[i].Index = i;
            }
        }

        public override void OnInstructionStep() {
            DisableTiles();
        }

        public override void DisableTiles() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].DisableTile();
            }
        }

        public override void UpdateDenominator(int tileIndex, bool yAxis) {
            if (Tiles[tileIndex].IsTileEnabled) {
                if (yAxis) {
                    _currentYDenominator = Tiles[tileIndex].CurrentYDenominator;
                } else {
                    _currentXDenominator = Tiles[tileIndex].CurrentXDenominator;
                }
                
                _currentDenominator = _currentXDenominator * _currentYDenominator;
                AnswerDenominator.text = _currentDenominator.ToString();
                UpdateNumerator();

                for (int i = 0; i < Tiles.Count; i++) {
                    if (i == tileIndex) continue;

                    if ((yAxis && i / XSize != tileIndex / XSize) || (!yAxis && i % XSize != tileIndex % XSize)) {
                        Tiles[i].ResetTile(true, !yAxis, yAxis);
                    } else if ((yAxis && i / XSize == tileIndex / XSize && Tiles[i].CurrentXDenominator > 1 && Tiles[i].CurrentXDenominator != _currentXDenominator) ||
                               (!yAxis && i % XSize == tileIndex % XSize && Tiles[i].CurrentYDenominator > 1 && Tiles[i].CurrentYDenominator != _currentYDenominator)) {

                        if (yAxis) {
                            _currentXDenominator = 1;
                        } else {
                            _currentYDenominator = 1;
                        }
                        Tiles[i].ResetTile(true, yAxis, !yAxis);
                        UpdateNumerator();
                    }

                    
                    if (yAxis && i / XSize != tileIndex / XSize) {
                        _currentYWhole = 0;
                        _currentYNumerator = 0;
                        Tiles[i].OnHighlightY(0);
                    } else if (!yAxis && i % XSize != tileIndex % XSize) {
                        _currentXWhole = 0;
                        _currentXNumerator = 0;
                        Tiles[i].OnHighlightX(0);
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

        public override void UpdateHighlighting(int tileIndex, bool yAxis) {
            if (Tiles[tileIndex].IsTileEnabled) {
                if (yAxis) {
                    _currentYWhole = tileIndex / XSize;
                    _currentYNumerator = Tiles[tileIndex].CurrentYNumerator;
                } else {
                    _currentXWhole = tileIndex % XSize;
                    _currentXNumerator = Tiles[tileIndex].CurrentXNumerator;
                }

                for (int i = 0; i < Tiles.Count; i++) {
                    if (i == tileIndex) continue;

                    if (yAxis && i / XSize != tileIndex / XSize) {
                        Tiles[i].OnHighlightY(i / XSize < tileIndex / XSize ? 1 : 0);
                    } else if (!yAxis && i % XSize != tileIndex % XSize) {
                        Tiles[i].OnHighlightX(i % XSize < tileIndex % XSize ? 1 : 0);
                    }
                }
            }
        }

        public override void OnAnswerSubmitted() {
            for (int i = 0; i < Tiles.Count; i++) {
                Tiles[i].ResetTile();
            }
        }
    }
}
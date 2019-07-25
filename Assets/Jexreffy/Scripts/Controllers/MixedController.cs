using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace Jexreffy.FractionFarms {
    public sealed class MixedController : SectionController {

        public List<UnitTile> Tiles = new List<UnitTile>();

        private void Awake() {
            for (var i = 0; i < Tiles.Count; i++) {
                Tiles[i].Parent = this;
                Tiles[i].Index = i;
            }
        }

        public override void OnInstructionStep() {
            DisableTiles();
        }

        public override void DisableTiles() {
            for (var i = 0; i < Tiles.Count; i++) {
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

                for (var i = 0; i < Tiles.Count; i++) {
                    if (i == tileIndex) continue;

                    if ((yAxis && i / XSize != tileIndex / XSize) || (!yAxis && i % XSize != tileIndex % XSize)) {
                        Tiles[i].ResetTile(true, !yAxis, !yAxis, yAxis, yAxis);
                    } else if ((yAxis && i / XSize == tileIndex / XSize && Tiles[i].CurrentXDenominator > 1  && Tiles[i].CurrentXDenominator != _currentXDenominator) ||
                               (!yAxis && i % XSize == tileIndex % XSize && Tiles[i].CurrentYDenominator > 1 && Tiles[i].CurrentYDenominator != _currentYDenominator)) {
                    
                        if (yAxis) {
                            _currentXDenominator = 1;
                        } else {
                            _currentYDenominator = 1;
                        }
                        Tiles[i].ResetTile(true, yAxis, yAxis, !yAxis, !yAxis);
                    }

                    
                    if (yAxis && i / XSize != tileIndex / XSize) {
                        _currentYWhole     = 0;
                        _currentYNumerator = 0;
                        Tiles[i].OnHighlightY(0);
                    } else if (!yAxis && i % XSize != tileIndex % XSize) {
                        _currentXWhole     = 0;
                        _currentXNumerator = 0;
                        Tiles[i].OnHighlightX(0);
                    }
                }
                
                _currentWhole     = 0;
                _currentNumerator = 0;
                UpdateNumerator();
            }

            _currentTile = -1;
            for (var i = 0; i < Tiles.Count; i++) {
                if (Tiles[i].CurrentXDenominator <= 1 || Tiles[i].CurrentYDenominator <= 1) continue;
                
                _currentTile = i;
                if (!CurrentStep.EnableTiles) {
                    _currentXWhole = _currentTile % XSize;
                    _currentYWhole = _currentTile / XSize;
                }
                break;
            }
            
            UpdateAnswerText();
        }

        public override void UpdateNumerator() {
            var totalNumerator = Tiles.Sum(t => t.GetSelected(1) * (_currentDenominator / t.CurrentDenominator));

            _currentWhole = totalNumerator / _currentDenominator;
            _currentNumerator = totalNumerator % _currentDenominator;
            
            UpdateAnswerText();
        }

        public override void UpdateHighlighting(int tileIndex, bool yAxis) {
            if (!Tiles[tileIndex].IsTileEnabled) return;

            _currentWhole     = 0;
            _currentNumerator = 0;
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
                    Tiles[i].OnHighlightY(i / XSize < tileIndex / XSize ? 1 : 0);
                } else if (!yAxis && i % XSize != tileIndex % XSize) {
                    Tiles[i].OnHighlightX(i % XSize < tileIndex % XSize ? 1 : 0);
                }
            }
            
            UpdateAnswerText();
        }

        public override void OnAnswerSubmitted() {
            for (var i = 0; i < Tiles.Count; i++) {
                Tiles[i].ResetTile(false,
                                   !_preserveXNumerator,
                                   !_preserveXDenominator,
                                   !_preserveYNumerator,
                                   !_preserveYDenominator);
            }
        }
    }
}
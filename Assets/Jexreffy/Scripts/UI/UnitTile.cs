using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace Jexreffy.FractionFarms {
    public class UnitTile : MonoBehaviour {

        public int MaxDenominator = 5;
        public bool PopulateHighlightX;
        public bool PopulateHighlightY;

        public Button Enabler;

        public TextMeshProUGUI XNumerator;
        public TextMeshProUGUI XDenominator;
        public Button XBackButton;
        public Button XForwardButton;

        public TextMeshProUGUI YNumerator;
        public TextMeshProUGUI YDivider;
        public TextMeshProUGUI YDenominator;
        public Button YBackButton;
        public Button YForwardButton;

        public GameObject XHighlightContainer;
        public GameObject YHighlightContainer;

        public GameObject TemplateTile;
        public GameObject TemplateXHighlight;
        public GameObject TemplateYHighlight;

        public List<Color> DefaultColors = new List<Color>();
        public List<ColorBlock> ButtonColors = new List<ColorBlock>();
        public Color HighlightColor = Color.yellow;

        [HideInInspector]
        public SectionController Parent;
        [HideInInspector]
        public int Index;

        private bool _enabled;

        private int _currentX = 1;
        private int _currentY = 1;

        private int _currentXHighlight = 0;
        private int _currentYHighlight = 0;

        private List<Button> _buttonPool;
        private List<GameObject> _buttonObjectPool;
        private List<RectTransform> _buttonTransformPool;

        private List<Image> _highlightPool;
        private List<GameObject> _highlightObjectPool;
        private List<RectTransform> _highlightTransformPool;

        private List<GameObject> _xHighlightObjectPool;
        private List<RectTransform> _xHighlightTransformPool;
        private List<GameObject> _yHighlightObjectPool;
        private List<RectTransform> _yHighlightTransformPool;

        private List<int> _selected;

        private Vector2 TILE_SPACER = new Vector2(4, 4);
        private Vector2 HIGHLIGHT_SPACER = new Vector2(-1, -1);

        void Awake() {
            int numTiles = MaxDenominator * MaxDenominator;

            _buttonPool = new List<Button>(numTiles);
            _buttonObjectPool = new List<GameObject>(numTiles);
            _buttonTransformPool = new List<RectTransform>(numTiles);

            _highlightPool = new List<Image>(numTiles);
            _highlightObjectPool = new List<GameObject>(numTiles);
            _highlightTransformPool = new List<RectTransform>(numTiles);

            _xHighlightObjectPool = new List<GameObject>(MaxDenominator);
            _xHighlightTransformPool = new List<RectTransform>(MaxDenominator);
            _yHighlightObjectPool = new List<GameObject>(MaxDenominator);
            _yHighlightTransformPool = new List<RectTransform>(MaxDenominator);

            _selected = new List<int>(numTiles);
            
            for (int i = 0; i < numTiles; i++) {
                _highlightObjectPool.Add(Instantiate(TemplateTile));
                _highlightPool.Add(_highlightObjectPool[i].GetComponent<Image>());
                _highlightTransformPool.Add(_highlightObjectPool[i].GetComponent<RectTransform>());
                _highlightTransformPool[i].SetParent(TemplateTile.transform.parent, false);
                _highlightObjectPool[i].GetComponent<Button>().enabled = false;
                _highlightObjectPool[i].name = "h" + i.ToString();
                _highlightObjectPool[i].SetActive(false);

                _buttonObjectPool.Add(Instantiate(TemplateTile));
                _buttonPool.Add(_buttonObjectPool[i].GetComponent<Button>());
                _buttonTransformPool.Add(_buttonObjectPool[i].GetComponent<RectTransform>());
                _buttonTransformPool[i].SetParent(TemplateTile.transform.parent, false);
                _buttonObjectPool[i].name = i.ToString();
                _buttonObjectPool[i].SetActive(false);
                _buttonPool[i].onClick.AddListener(OnTileClick);
                _selected.Add(0);
            }

            if (PopulateHighlightX || PopulateHighlightY) {
                for (int i = 0; i < MaxDenominator; i++) {
                    if (PopulateHighlightX) {
                        _xHighlightObjectPool.Add(Instantiate(TemplateXHighlight));
                        _xHighlightTransformPool.Add(_xHighlightObjectPool[i].GetComponent<RectTransform>());
                        _xHighlightTransformPool[i].SetParent(TemplateXHighlight.transform.parent, false);
                        _xHighlightObjectPool[i].name = (i + 1).ToString();
                        _xHighlightObjectPool[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                    }

                    if (PopulateHighlightY) {
                        _yHighlightObjectPool.Add(Instantiate(TemplateYHighlight));
                        _yHighlightTransformPool.Add(_yHighlightObjectPool[i].GetComponent<RectTransform>());
                        _yHighlightTransformPool[i].SetParent(TemplateYHighlight.transform.parent, false);
                        _yHighlightObjectPool[i].name = (i + 1).ToString();
                        _yHighlightObjectPool[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                    }
                }
            }

            InitializeTile();
        }

        private void InitializeTile() {
            OnTileEnabled();
            UpdateTile();
        }

        public bool IsTileEnabled { get { return _enabled; } }

        public void EnableTile() {
            Parent.DisableTiles();
            if (!_enabled) {
                _enabled = true;
                OnTileEnabled();
            }
        }

        public void DisableTile() {
            if (_enabled) {
                _enabled = false;
                OnTileEnabled();
            }
        }

        private void OnTileEnabled(bool overrideControls = false) {
            Enabler.enabled = !_enabled;
            bool tilesEnabled = _enabled && Parent.EnableTiles;

            if (!overrideControls) {
                XNumerator.gameObject.SetActive(_enabled);
                XDenominator.gameObject.SetActive(_enabled);
                XBackButton.gameObject.SetActive(_enabled);
                XForwardButton.gameObject.SetActive(_enabled);

                YNumerator.gameObject.SetActive(_enabled);
                YDivider.gameObject.SetActive(_enabled);
                YDenominator.gameObject.SetActive(_enabled);
                YBackButton.gameObject.SetActive(_enabled);
                YForwardButton.gameObject.SetActive(_enabled);

                XHighlightContainer.SetActive(tilesEnabled);
                YHighlightContainer.SetActive(tilesEnabled);
            }

            for (int i = 0; i < _buttonPool.Count; i++) {
                _buttonPool[i].image.raycastTarget = tilesEnabled;
                _buttonPool[i].interactable = tilesEnabled;
            }
        }

        public void ResetTile(bool overrideControls = false, bool resetX = true, bool resetY = true) {
            if (resetX) {
                _currentX = 1;
                _currentXHighlight = 0;
            }

            if (resetY) {
                _currentY = 1;
                _currentYHighlight = 0;
            }

            _selected[0] = 0;
            _buttonPool[0].image.color = DefaultColors[0];
            _buttonPool[0].colors = ButtonColors[0];
            _highlightPool[0].color = DefaultColors[0];

            _enabled = false;
            OnTileEnabled(overrideControls);

            UpdateTile();
        }

        private void UpdateTile() {
            XNumerator.text = _currentXHighlight.ToString() + "/";
            XDenominator.text = _currentX.ToString();
            XBackButton.interactable = _currentX > 1;
            XForwardButton.interactable = _currentX < MaxDenominator;
            YNumerator.text = _currentYHighlight.ToString();
            YDenominator.text = _currentY.ToString();
            YBackButton.interactable = _currentY > 1;
            YForwardButton.interactable = _currentY < MaxDenominator;

            for (int i = 0; i < _buttonObjectPool.Count; i++) {
                bool isActive = i % MaxDenominator < _currentX && i / MaxDenominator < _currentY;
                bool isHighlighted = i % MaxDenominator < _currentXHighlight || i / MaxDenominator < _currentYHighlight;
                _buttonObjectPool[i].SetActive(isActive);
                _highlightObjectPool[i].SetActive(isActive);
                if (isActive) {
                    _buttonTransformPool[i].anchorMin = new Vector2(i % MaxDenominator / (float)_currentX, i / MaxDenominator / (float)_currentY);
                    _buttonTransformPool[i].anchorMax = new Vector2(_buttonTransformPool[i].anchorMin.x + (1 % MaxDenominator / (float)_currentX), (i + MaxDenominator) / MaxDenominator / (float)_currentY);
                    _buttonTransformPool[i].offsetMin = TILE_SPACER;
                    _buttonTransformPool[i].offsetMax = -TILE_SPACER;

                    _highlightTransformPool[i].anchorMin = new Vector2(i % MaxDenominator / (float)_currentX, i / MaxDenominator / (float)_currentY);
                    _highlightTransformPool[i].anchorMax = new Vector2(_buttonTransformPool[i].anchorMin.x + (1 % MaxDenominator / (float)_currentX), (i + MaxDenominator) / MaxDenominator / (float)_currentY);
                    _highlightTransformPool[i].offsetMin = HIGHLIGHT_SPACER;
                    _highlightTransformPool[i].offsetMax = HIGHLIGHT_SPACER;
                    _highlightPool[i].color = isHighlighted ? HighlightColor : DefaultColors[0];
                } else {
                    _selected[i] = 0;
                    _buttonPool[i].image.color = DefaultColors[0];
                    _buttonPool[i].colors = ButtonColors[0];
                    _highlightPool[i].color = DefaultColors[0];
                }
            }

            if (PopulateHighlightX || PopulateHighlightY) {
                for (int i = 0; i < MaxDenominator; i++) {
                    bool isActive = i < _currentX;
                    if (PopulateHighlightX) {
                        _xHighlightObjectPool[i].SetActive(isActive);
                        if (isActive) {
                            _xHighlightTransformPool[i].anchorMin = new Vector2(i / (float)_currentX, 0);
                            _xHighlightTransformPool[i].anchorMax = new Vector2(_xHighlightTransformPool[i].anchorMin.x + 1 / (float)_currentX, 1);
                        }
                    }

                    if (PopulateHighlightY) {
                        isActive = i < _currentY;
                        _yHighlightObjectPool[i].SetActive(isActive);
                        if (isActive) {
                            _yHighlightTransformPool[i].anchorMin = new Vector2(i / (float)_currentY, 0);
                            _yHighlightTransformPool[i].anchorMax = new Vector2(_yHighlightTransformPool[i].anchorMin.x + 1 / (float)_currentY, 1);
                        }
                    }
                }
            }
        }

        public int CurrentXNumerator { get { return _currentXHighlight; } }
        public int CurrentYNumerator { get { return _currentYHighlight; } }
        public int CurrentXDenominator { get { return _currentX; } }
        public int CurrentYDenominator { get { return _currentY; } }
        public int CurrentDenominator { get { return _currentX * _currentY; } }

        public int GetSelected(int index) {
            if (index > 0 && index < DefaultColors.Count) {
                int retVal = 0;
                for (int i = 0; i < _selected.Count; i++) {
                    if (_selected[i] == index) retVal++;
                }
                return retVal;
            }

            return 0;
        }

        public void OnBackX() {
            if (_currentX > 1) {
                _currentX--;
                _currentXHighlight = 0;

                UpdateTile();
                Parent.UpdateDenominator(Index, false);
            }
        }

        public void OnForwardX() {
            if (_currentX < MaxDenominator) {
                _currentX++;
                _currentXHighlight = 0;

                UpdateTile();
                Parent.UpdateDenominator(Index, false);
            }
        }

        public void OnBackY() {
            if (_currentY > 1) {
                _currentY--;

                _currentYHighlight = _currentY;
                _currentYHighlight = 0;

                UpdateTile();
                Parent.UpdateDenominator(Index, true);
            }
        }

        public void OnForwardY() {
            if (_currentY < MaxDenominator) {
                _currentY++;
                _currentYHighlight = 0;

                UpdateTile();
                Parent.UpdateDenominator(Index, true);
            }
        }

        public void OnTileClick() {
            int index = int.Parse(EventSystem.current.currentSelectedGameObject.name);

            if (++_selected[index] >= DefaultColors.Count) _selected[index] = 0;

            _buttonPool[index].image.color = DefaultColors[_selected[index]];
            _buttonPool[index].colors = ButtonColors[_selected[index]];

            Parent.UpdateNumerator();
        }

        public void OnHighlightX() {
            OnHighlightX(int.Parse(EventSystem.current.currentSelectedGameObject.name));

            Parent.UpdateHighlighting(Index, false);
        }

        public void OnHighlightX(int x) {
            _currentXHighlight = x;
            UpdateTile();
        }

        public void OnHighlightY() {
            OnHighlightY(int.Parse(EventSystem.current.currentSelectedGameObject.name));

            Parent.UpdateHighlighting(Index, true);
        }

        public void OnHighlightY(int y) {
            _currentYHighlight = y;
            UpdateTile();
        }
    }
}
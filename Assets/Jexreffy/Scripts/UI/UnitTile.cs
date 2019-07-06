using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public Color XHighlightColor = Color.yellow;
        public Color YHighlightColor = Color.blue;
        public Color XYHighlightColor = Color.green;
        
        [HideInInspector]
        public SectionController Parent;
        [HideInInspector]
        public int Index;

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

        private readonly Vector2 _tileSpacer = new Vector2(4, 4);
        private readonly Vector2 _highlightSpacer = new Vector2(-1, -1);

        private void Awake() {
            var numTiles = MaxDenominator * MaxDenominator;

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
            
            var parent = TemplateTile.transform.parent;
            
            for (var i = 0; i < numTiles; i++) {
                _highlightObjectPool.Add(Instantiate(TemplateTile));
                _highlightPool.Add(_highlightObjectPool[i].GetComponent<Image>());
                _highlightTransformPool.Add(_highlightObjectPool[i].GetComponent<RectTransform>());
                _highlightTransformPool[i].SetParent(parent, false);
                _highlightObjectPool[i].GetComponent<Button>().enabled = false;
                _highlightObjectPool[i].name = $"h{i}";
                _highlightObjectPool[i].SetActive(false);

                _buttonObjectPool.Add(Instantiate(TemplateTile));
                _buttonPool.Add(_buttonObjectPool[i].GetComponent<Button>());
                _buttonTransformPool.Add(_buttonObjectPool[i].GetComponent<RectTransform>());
                _buttonTransformPool[i].SetParent(parent, false);
                _buttonObjectPool[i].name = i.ToString();
                _buttonObjectPool[i].SetActive(false);
                _buttonPool[i].onClick.AddListener(OnTileClick);
                _selected.Add(0);
            }

            if (PopulateHighlightX || PopulateHighlightY) {
                var xParent = TemplateXHighlight.transform.parent;
                var yParent = TemplateYHighlight.transform.parent;
                
                for (var i = 0; i < MaxDenominator; i++) {
                    if (PopulateHighlightX) {
                        _xHighlightObjectPool.Add(Instantiate(TemplateXHighlight));
                        _xHighlightTransformPool.Add(_xHighlightObjectPool[i].GetComponent<RectTransform>());
                        _xHighlightTransformPool[i].SetParent(xParent, false);
                        _xHighlightObjectPool[i].name = (i + 1).ToString();
                        _xHighlightObjectPool[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                    }

                    if (PopulateHighlightY) {
                        _yHighlightObjectPool.Add(Instantiate(TemplateYHighlight));
                        _yHighlightTransformPool.Add(_yHighlightObjectPool[i].GetComponent<RectTransform>());
                        _yHighlightTransformPool[i].SetParent(yParent, false);
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

        public bool IsTileEnabled { get; private set; }

        public void EnableTile() {
            Parent.DisableTiles();
            if (IsTileEnabled) return;
            
            IsTileEnabled = true;
            OnTileEnabled();
        }

        public void DisableTile() {
            if (!IsTileEnabled) return;
            
            IsTileEnabled = false;
            OnTileEnabled();
        }

        private void OnTileEnabled(bool overrideControls = false) {
            Enabler.enabled = !IsTileEnabled;
            var tilesEnabled = IsTileEnabled && Parent.EnableTiles;

            if (!overrideControls) {
                XNumerator.gameObject.SetActive(IsTileEnabled);
                XDenominator.gameObject.SetActive(IsTileEnabled);
                XBackButton.gameObject.SetActive(IsTileEnabled);
                XForwardButton.gameObject.SetActive(IsTileEnabled);

                YNumerator.gameObject.SetActive(IsTileEnabled);
                YDivider.gameObject.SetActive(IsTileEnabled);
                YDenominator.gameObject.SetActive(IsTileEnabled);
                YBackButton.gameObject.SetActive(IsTileEnabled);
                YForwardButton.gameObject.SetActive(IsTileEnabled);

                XHighlightContainer.SetActive(tilesEnabled);
                YHighlightContainer.SetActive(tilesEnabled);
            }

            for (var i = 0; i < _buttonPool.Count; i++) {
                _buttonPool[i].image.raycastTarget = tilesEnabled;
                _buttonPool[i].interactable = tilesEnabled;
            }
        }

        public void ResetTile(bool overrideControls = false, bool resetX = true, bool resetY = true) {
            if (resetX) {
                CurrentXDenominator = 1;
                CurrentXNumerator = 0;
            }

            if (resetY) {
                CurrentYDenominator = 1;
                CurrentYNumerator = 0;
            }

            _selected[0] = 0;
            _buttonPool[0].image.color = DefaultColors[0];
            _buttonPool[0].colors = ButtonColors[0];
            _highlightPool[0].color = DefaultColors[0];

            IsTileEnabled = false;
            OnTileEnabled(overrideControls);

            UpdateTile();
        }

        private void UpdateTile() {
            XNumerator.text = $"{CurrentXNumerator}/";
            XDenominator.text = CurrentXDenominator.ToString();
            XBackButton.interactable = CurrentXDenominator > 1;
            XForwardButton.interactable = CurrentXDenominator < MaxDenominator;
            YNumerator.text = CurrentYNumerator.ToString();
            YDenominator.text = CurrentYDenominator.ToString();
            YBackButton.interactable = CurrentYDenominator > 1;
            YForwardButton.interactable = CurrentYDenominator < MaxDenominator;

            for (var i = 0; i < _buttonObjectPool.Count; i++) {
                var isActive = i % MaxDenominator < CurrentXDenominator && i / MaxDenominator < CurrentYDenominator;
                var isXHighlighted = i % MaxDenominator < CurrentXNumerator;
                var isYHighlighted = i / MaxDenominator < CurrentYNumerator;
                _buttonObjectPool[i].SetActive(isActive);
                _highlightObjectPool[i].SetActive(isActive);
                if (isActive) {
                    _buttonTransformPool[i].anchorMin = new Vector2(i % MaxDenominator / (float)CurrentXDenominator, i / MaxDenominator / (float)CurrentYDenominator);
                    _buttonTransformPool[i].anchorMax = new Vector2(_buttonTransformPool[i].anchorMin.x + (1 % MaxDenominator / (float)CurrentXDenominator), (i + MaxDenominator) / MaxDenominator / (float)CurrentYDenominator);
                    _buttonTransformPool[i].offsetMin = _tileSpacer;
                    _buttonTransformPool[i].offsetMax = -_tileSpacer;

                    _highlightTransformPool[i].anchorMin = new Vector2(i % MaxDenominator / (float)CurrentXDenominator, i / MaxDenominator / (float)CurrentYDenominator);
                    _highlightTransformPool[i].anchorMax = new Vector2(_buttonTransformPool[i].anchorMin.x + (1 % MaxDenominator / (float)CurrentXDenominator), (i + MaxDenominator) / MaxDenominator / (float)CurrentYDenominator);
                    _highlightTransformPool[i].offsetMin = _highlightSpacer;
                    _highlightTransformPool[i].offsetMax = _highlightSpacer;
                    if (isXHighlighted && isYHighlighted) {
                        _highlightPool[i].color = XYHighlightColor;
                    } else if (isXHighlighted) {
                        _highlightPool[i].color = XHighlightColor;
                    } else if (isYHighlighted) {
                        _highlightPool[i].color = YHighlightColor;
                    } else {
                        _highlightPool[i].color = DefaultColors[0];
                    }
                } else {
                    _selected[i] = 0;
                    _buttonPool[i].image.color = DefaultColors[0];
                    _buttonPool[i].colors = ButtonColors[0];
                    _highlightPool[i].color = DefaultColors[0];
                }
            }

            if (!PopulateHighlightX && !PopulateHighlightY) return;
            
            for (var i = 0; i < MaxDenominator; i++) {
                var isActive = i < CurrentXDenominator;
                if (PopulateHighlightX) {
                    _xHighlightObjectPool[i].SetActive(isActive);
                    if (isActive) {
                        _xHighlightTransformPool[i].anchorMin = new Vector2(i / (float) CurrentXDenominator, 0);
                        _xHighlightTransformPool[i].anchorMax =
                            new Vector2(_xHighlightTransformPool[i].anchorMin.x + 1 / (float) CurrentXDenominator, 1);
                    }
                }

                if (PopulateHighlightY) {
                    isActive = i < CurrentYDenominator;
                    _yHighlightObjectPool[i].SetActive(isActive);
                    if (isActive) {
                        _yHighlightTransformPool[i].anchorMin = new Vector2(i / (float) CurrentYDenominator, 0);
                        _yHighlightTransformPool[i].anchorMax =
                            new Vector2(_yHighlightTransformPool[i].anchorMin.x + 1 / (float) CurrentYDenominator, 1);
                    }
                }
            }
        }

        public int CurrentXNumerator { get; private set; }
        public int CurrentYNumerator { get; private set; }
        public int CurrentXDenominator { get; private set; } = 1;
        public int CurrentYDenominator { get; private set; } = 1;
        public int CurrentDenominator => CurrentXDenominator * CurrentYDenominator;

        public int GetSelected(int index) {
            if (index <= 0 || index >= DefaultColors.Count) return 0;

            return _selected.Count(t => t == index);

        }

        public void OnBackX() {
            if (CurrentXDenominator <= 1) return;
            
            CurrentXDenominator--;
            CurrentXNumerator = 0;

            UpdateTile();
            Parent.UpdateDenominator(Index, false);
        }

        public void OnForwardX() {
            if (CurrentXDenominator >= MaxDenominator) return;
            
            CurrentXDenominator++;
            CurrentXNumerator = 0;

            UpdateTile();
            Parent.UpdateDenominator(Index, false);
        }

        public void OnBackY() {
            if (CurrentYDenominator <= 1) return;
            
            CurrentYDenominator--;

            CurrentYNumerator = CurrentYDenominator;
            CurrentYNumerator = 0;

            UpdateTile();
            Parent.UpdateDenominator(Index, true);
        }

        public void OnForwardY() {
            if (CurrentYDenominator >= MaxDenominator) return;
            
            CurrentYDenominator++;
            CurrentYNumerator = 0;

            UpdateTile();
            Parent.UpdateDenominator(Index, true);
        }

        public void OnTileClick() {
            var index = int.Parse(EventSystem.current.currentSelectedGameObject.name);

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
            CurrentXNumerator = x;
            UpdateTile();
        }

        public void OnHighlightY() {
            OnHighlightY(int.Parse(EventSystem.current.currentSelectedGameObject.name));

            Parent.UpdateHighlighting(Index, true);
        }

        public void OnHighlightY(int y) {
            CurrentYNumerator = y;
            UpdateTile();
        }
    }
}
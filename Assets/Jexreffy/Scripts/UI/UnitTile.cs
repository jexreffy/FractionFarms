using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UnitTile : MonoBehaviour {

    public TextMeshProUGUI XDenominator;
    public Button XBackButton;
    public Button XForwardButton;

    public TextMeshProUGUI YDenominator;
    public Button YBackButton;
    public Button YForwardButton;
    
    public GameObject TemplateTile;

    public List<Color> DefaultColors = new List<Color>();
    public List<ColorBlock> ButtonColors = new List<ColorBlock>();

    private int _currentX = 1;
    private int _currentY = 1;

    private List<Button> _buttonPool;
    private List<GameObject> _objectPool;
    private List<RectTransform> _transformPool;
    private List<int> _selected;

    private Vector2 SPACER = new Vector2(3, 3);

    private const int MAX_DENOMINATOR = 5;

    void Awake() {
        int numTiles = MAX_DENOMINATOR * MAX_DENOMINATOR;

        _buttonPool = new List<Button>(numTiles);
        _objectPool = new List<GameObject>(numTiles);
        _transformPool = new List<RectTransform>(numTiles);
        _selected = new List<int>(numTiles);

        for (int i = 0; i < numTiles; i++) {
            _objectPool.Add(Instantiate(TemplateTile));
            _buttonPool.Add(_objectPool[i].GetComponent<Button>());
            RectTransform rect = _objectPool[i].GetComponent<RectTransform>();
            _transformPool.Add(rect);
            rect.SetParent(TemplateTile.transform.parent, false);
            _objectPool[i].name = i.ToString();
            _objectPool[i].SetActive(false);
            _buttonPool[i].onClick.AddListener(OnTileClick);
            _selected.Add(0);
        }

        InitializeTile();
    }

    private void InitializeTile() {
        UpdateTile();
    }

    public void ResetTile() {
        _currentX = 1;
        _currentY = 1;

        _selected[0] = 0;
        _buttonPool[0].image.color = DefaultColors[0];
        _buttonPool[0].colors = ButtonColors[0];

        UpdateTile();
    }

    private void UpdateTile() {
        XDenominator.text = _currentX + "";
        XBackButton.interactable = _currentX > 1;
        XForwardButton.interactable = _currentX < MAX_DENOMINATOR;
        YDenominator.text = _currentY + "";
        YBackButton.interactable = _currentY > 1;
        YForwardButton.interactable = _currentY < MAX_DENOMINATOR;

        int numTiles = _currentX * _currentY;

        for (int i = 0; i < _objectPool.Count; i++) {
            _objectPool[i].SetActive(i % MAX_DENOMINATOR < _currentX && i / MAX_DENOMINATOR < _currentY);
            if (_objectPool[i].activeSelf) {
                _transformPool[i].anchorMin = new Vector2(i % MAX_DENOMINATOR / (float)_currentX, i / MAX_DENOMINATOR / (float)_currentY);
                _transformPool[i].anchorMax = new Vector2(_transformPool[i].anchorMin.x + (1 % MAX_DENOMINATOR / (float)_currentX), (i + MAX_DENOMINATOR) / MAX_DENOMINATOR / (float)_currentY);
                _transformPool[i].offsetMin = SPACER;
                _transformPool[i].offsetMax = -SPACER;
            } else {
                _selected[i] = 0;
                _buttonPool[i].image.color = DefaultColors[0];
                _buttonPool[i].colors = ButtonColors[0];
            }
        }
    }

    public int Denominator { get { return _currentX * _currentY; } }

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
            UpdateTile();
        }
    }

    public void OnForwardX() {
        if (_currentX < MAX_DENOMINATOR) {
            _currentX++;
            UpdateTile();
        }
    }

    public void OnBackY() {
        if (_currentY > 1) {
            _currentY--;
            UpdateTile();
        }
    }

    public void OnForwardY() {
        if (_currentY < MAX_DENOMINATOR) {
            _currentY++;
            UpdateTile();
        }
    }

    public void OnTileClick() {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (++_selected[index] >= DefaultColors.Count) _selected[index] = 0;

        _buttonPool[index].image.color = DefaultColors[_selected[index]];
        _buttonPool[index].colors = ButtonColors[_selected[index]];
    }
}

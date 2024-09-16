using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Controllers
{
    public class UIController : MonoBehaviour
    {
        private Button _initializeBtn;
        private Button _spawnBtn;
        private Button _playBtn;
        private Button _pauseBtn;
        private Button _forwardBtn;
        private Button _backwardBtn;
        private Button _saveBtn;
        private Button _loadBtn;

        private TextMeshProUGUI _rowsText;
        private Slider _rowsSlider;

        private TextMeshProUGUI _columnsText;
        private Slider _columnsSlider;

        private TextMeshProUGUI _spawnPercentageText;
        private Slider _spawnPercentageSlider;

        private TextMeshProUGUI _secondsPerGenerationText;
        private Slider _secondsPerGenerationSlider;

        private TextMeshProUGUI _repetitionCountText;

        private GameController _gameController;
        private GameController.GameState _state;

        private void Awake()
        {
            _playBtn = GameObject.Find("playBtn").GetComponent<Button>();
            _pauseBtn = GameObject.Find("pauseBtn").GetComponent<Button>();
            _forwardBtn = GameObject.Find("forwardBtn").GetComponent<Button>();
            _backwardBtn = GameObject.Find("backwardBtn").GetComponent<Button>();
            _initializeBtn = GameObject.Find("initializeBtn").GetComponent<Button>();
            _spawnBtn = GameObject.Find("spawnCellsBtn").GetComponent<Button>();
            _saveBtn = GameObject.Find("saveBtn").GetComponent<Button>();
            _loadBtn = GameObject.Find("loadBtn").GetComponent<Button>();

            _rowsText = GameObject.Find("rowsText").GetComponent<TextMeshProUGUI>();
            _rowsSlider = GameObject.Find("rowsSlider").GetComponent<Slider>();

            _columnsText = GameObject.Find("columnsText").GetComponent<TextMeshProUGUI>();
            _columnsSlider = GameObject.Find("columnsSlider").GetComponent<Slider>();

            _spawnPercentageText = GameObject.Find("spawnPercentageText").GetComponent<TextMeshProUGUI>();
            _spawnPercentageSlider = GameObject.Find("spawnPercentageSlider").GetComponent<Slider>();

            _secondsPerGenerationText = GameObject.Find("secondsPerGenerationText").GetComponent<TextMeshProUGUI>();
            _secondsPerGenerationSlider = GameObject.Find("secondsPerGenerationSlider").GetComponent<Slider>();

            _repetitionCountText = GameObject.Find("repetitionCountText").GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _gameController = GameController.Instance;

            _initializeBtn.onClick.AddListener(() =>
                _gameController.Initialize((int)_columnsSlider.value, (int)_rowsSlider.value));

            _spawnBtn.onClick.AddListener(() => _gameController.SpawnCells((int) _spawnPercentageSlider.value));
            _playBtn.onClick.AddListener(() => _gameController.Play(_secondsPerGenerationSlider.value));
            _pauseBtn.onClick.AddListener(() => _gameController.Pause());
            _forwardBtn.onClick.AddListener(() => _gameController.GotoNextGeneration());
            _backwardBtn.onClick.AddListener(() => _gameController.GotoPreviousGeneration());
            _saveBtn.onClick.AddListener(SaveUtil.Save);
            _loadBtn.onClick.AddListener(SaveUtil.Load);

            _columnsSlider.onValueChanged.AddListener(UpdateColumnsText);
            _rowsSlider.onValueChanged.AddListener(UpdateRowsText);
            _spawnPercentageSlider.onValueChanged.AddListener(UpdateSpawnText);
            _secondsPerGenerationSlider.onValueChanged.AddListener(UpdateSecondsPerGenerationText);

            UpdateRowsText(_rowsSlider.value);
            UpdateColumnsText(_columnsSlider.value);
            UpdateSpawnText(_spawnPercentageSlider.value);
            UpdateSecondsPerGenerationText(_secondsPerGenerationSlider.value);

            UpdateButtons(_state);
        }

        private void Update()
        {
            if (_state != GameController.Instance.State)
            {
                _state = GameController.Instance.State;
                UpdateButtons(_state);
            }

            _backwardBtn.interactable =
                _state == GameController.GameState.Paused && GameController.Instance.HasPreviousGeneration();

            var generationRepetitionCount = GameController.Instance.GetGenerationRepetitionCount();
            if (generationRepetitionCount > 0)
            {
                _repetitionCountText.enabled = true;
                _repetitionCountText.text = $"Repeating every {generationRepetitionCount} generations";
            }
            else
            {
                _repetitionCountText.enabled = false;
            }
        }

        private void UpdateButtons(GameController.GameState state)
        {
            switch (state)
            {
                case GameController.GameState.Uninitialized:
                    _initializeBtn.interactable = true;
                    _spawnBtn.interactable = false;
                    _playBtn.interactable = false;
                    _pauseBtn.interactable = false;
                    _backwardBtn.interactable = false;
                    _forwardBtn.interactable = false;
                    _saveBtn.interactable = false;
                    _loadBtn.interactable = SaveUtil.HasSavedState();

                    _rowsSlider.interactable = true;
                    _columnsSlider.interactable = true;
                    _spawnPercentageSlider.interactable = false;
                    _secondsPerGenerationSlider.interactable = false;
                    break;
                case GameController.GameState.Paused:
                    _initializeBtn.interactable = true;
                    _spawnBtn.interactable = true;
                    _playBtn.interactable = true;
                    _pauseBtn.interactable = false;
                    _backwardBtn.interactable = false;
                    _forwardBtn.interactable = true;
                    _saveBtn.interactable = true;
                    _loadBtn.interactable = true;

                    _rowsSlider.interactable = true;
                    _columnsSlider.interactable = true;
                    _spawnPercentageSlider.interactable = true;
                    _secondsPerGenerationSlider.interactable = true;
                    break;
                case GameController.GameState.Playing:
                    _initializeBtn.interactable = false;
                    _spawnBtn.interactable = false;
                    _playBtn.interactable = false;
                    _pauseBtn.interactable = true;
                    _backwardBtn.interactable = false;
                    _forwardBtn.interactable = false;
                    _saveBtn.interactable = false;
                    _loadBtn.interactable = false;

                    _rowsSlider.interactable = false;
                    _columnsSlider.interactable = false;
                    _spawnPercentageSlider.interactable = false;
                    _secondsPerGenerationSlider.interactable = false;
                    break;
            }
        }

        private void UpdateColumnsText(float value)
        {
            _columnsText.text = $"Columns: {(int)value}";
        }

        private void UpdateRowsText(float value)
        {
            _rowsText.text = $"Rows: {(int)value}";
        }

        private void UpdateSpawnText(float value)
        {
            _spawnPercentageText.text = $"Spawn Chance: {(int)value}%";
        }

        private void UpdateSecondsPerGenerationText(float value)
        {
            _secondsPerGenerationText.text = $"Seconds per Generation: {value:0.00}";
        }
    }
}
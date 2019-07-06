using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LoLSDK;
using SimpleJSON;

namespace Jexreffy.LoL {
    public class PlatformController : MonoBehaviour {

        private int _currentProgress;

        private static PlatformController _instance;
        public static PlatformController Instance => _instance ? _instance : new GameObject("PlatformController").AddComponent<PlatformController>();

        private const string PROJECT_ID = "com.jexreffy.fractionfarms";
        private const int MAXIMUM_PROGRESS = 15;

#if UNITY_EDITOR
        private const string LANG_FILE = "language.json";
        private const string QUESTION_FILE = "questions.json";
        private const string START_FILE = "startGame.json";
#endif

        private void Awake() {
            if (_instance != null) {
                Destroy(this);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);

#if UNITY_EDITOR
            LOLSDK.Init(new MockWebGL(), PROJECT_ID);
#elif UNITY_WEBGL
            LOLSDK.Init(new WebGL(), PROJECT_ID);
#endif

#if UNITY_EDITOR
            LoadMockData();
#endif

            LOLSDK.Instance.StartGameReceived += HandleStartGame;
            LOLSDK.Instance.GameStateChanged += HandleGameStateChange;
            LOLSDK.Instance.QuestionsReceived += HandleQuestions;
            LOLSDK.Instance.LanguageDefsReceived += HandleLanguageDefs;

            LOLSDK.Instance.GameIsReady();
        }

        private void Start() {
            if (SceneManager.GetActiveScene().buildIndex == 0) {
                AdvanceScene();
            }
        }

        public int Score { get; private set; }

        public static void AdvanceScene() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }

        private JSONNode StartData { get; set; }
        private JSONNode LanguageData { get; set; }
        private MultipleChoiceQuestionList QuestionData { get; set; }
        public bool DataLoaded { get; private set; }

        private void HandleStartGame(string json) {
            StartData = JSON.Parse(json);
        }

        private void HandleGameStateChange(GameState gameState) { }

        private void HandleQuestions(MultipleChoiceQuestionList questionList) {
            QuestionData = questionList;
        }

        private void HandleLanguageDefs(string json) {
            LanguageData = JSON.Parse(json);
            DataLoaded = true;
        }

        public string GetText(string key) {
            return LanguageData[key].Value;
        }

        public string GetTextAndSpeak(string key) {
            SpeakText(key);
            return LanguageData[key].Value;
        }

        public void CompleteGame() {
            LOLSDK.Instance.CompleteGame();
        }

        public void SpeakAlternative(int alternativeID) {
            LOLSDK.Instance.SpeakAlternative(alternativeID);
        }

        public void SpeakQuestion(int questionID) {
            LOLSDK.Instance.SpeakQuestion(questionID);
        }

        public void SpeakQuestionAndAlternatives(int questionID) {
            LOLSDK.Instance.SpeakQuestionAndAlternatives(questionID);
        }

        public void SpeakText(string key) {
            LOLSDK.Instance.SpeakText(key);
        }

        public void SubmitAnswer(MultipleChoiceAnswer answer) {
            LOLSDK.Instance.SubmitAnswer(answer);
        }

        public void UpdateProgress(int score) {
            Score += score;
            LOLSDK.Instance.SubmitProgress(Score, ++_currentProgress, MAXIMUM_PROGRESS);
        }


#if UNITY_EDITOR
        private void LoadMockData() {
            var startDataPath = Path.Combine(Application.streamingAssetsPath, START_FILE);
            var langCode = "en";

            Debug.Log(Time.time + ": Start File Exists? " + File.Exists(startDataPath).ToString());

            if (File.Exists(startDataPath)) {
                var startData = File.ReadAllText(startDataPath);
                var startGamePayload = JSON.Parse(startData);
                langCode = startGamePayload["languageCode"];
                HandleStartGame(startData);
            }

            var langPath = Path.Combine(Application.streamingAssetsPath, LANG_FILE);

            Debug.Log(Time.time + ": Lang File Exists? " + File.Exists(langPath).ToString());

            if (File.Exists(langPath)) {
                var langDataAsJson = File.ReadAllText(langPath);
                var langDefs = JSON.Parse(langDataAsJson);
                HandleLanguageDefs(langDefs[langCode].ToString());
            }

            var questionsPath = Path.Combine(Application.streamingAssetsPath, QUESTION_FILE);

            Debug.Log(Time.time + ": Question File Exists? " + File.Exists(langPath).ToString());

            if (File.Exists(questionsPath)) {
                var questionsData = File.ReadAllText(questionsPath);
                var questionPayload = MultipleChoiceQuestionList.CreateFromJSON(questionsData);
                HandleQuestions(questionPayload);
            }
        }
#endif
    }
}
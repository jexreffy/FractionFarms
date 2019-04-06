using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LoLSDK;
using SimpleJSON;

namespace Jexreffy.LoL {
    public class PlatformController : MonoBehaviour {

        private static PlatformController _instance;
        public static PlatformController Instance { get { return _instance ?? new GameObject("PlatformController").AddComponent<PlatformController>(); } }

        private const string PROJECT_ID = "com.jexreffy.fractionfarms";

#if UNITY_EDITOR
        private const string LANG_FILE = "language.json";
        private const string QUESTION_FILE = "questions.json";
        private const string START_FILE = "startGame.json";
#endif

        void Awake() {
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

            LOLSDK.Instance.GameIsReady();
        }

        public JSONNode StartData { get; private set; }
        public JSONNode LanguageData { get; private set; }
        public MultipleChoiceQuestionList QuestionData { get; private set; }

        void HandleStartGame(string json) {
            StartData = JSON.Parse(json);
        }
        
        void HandleGameStateChange(GameState gameState) {
            Debug.Log("HandleGameStateChange");
        }
        
        void HandleQuestions(MultipleChoiceQuestionList questionList) {
            Debug.Log("HandleQuestions");
            QuestionData = questionList;
        }
        
        void HandleLanguageDefs(string json) {
            LanguageData = JSON.Parse(json);
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

        public void SubmitProgress(int score, int current, int maximum = -1) {
            LOLSDK.Instance.SubmitProgress(score, current, maximum);
        }

#if UNITY_EDITOR
        private void LoadMockData() {
            string startDataPath = Path.Combine(Application.streamingAssetsPath, START_FILE);
            string langCode = "en";

            Debug.Log(Time.time + ": Start File Exists? " + File.Exists(startDataPath).ToString());

            if (File.Exists(startDataPath)) {
                string startData = File.ReadAllText(startDataPath);
                JSONNode startGamePayload = JSON.Parse(startData);
                langCode = startGamePayload["languageCode"];
                HandleStartGame(startData);
            }

            string langPath = Path.Combine(Application.streamingAssetsPath, LANG_FILE);

            Debug.Log(Time.time + ": Lang File Exists? " + File.Exists(langPath).ToString());

            if (File.Exists(langPath)) {
                string langDataAsJson = File.ReadAllText(langPath);
                JSONNode langDefs = JSON.Parse(langDataAsJson);
                HandleLanguageDefs(langDefs[langCode].ToString());
            }

            string questionsPath = Path.Combine(Application.streamingAssetsPath, QUESTION_FILE);

            Debug.Log(Time.time + ": Question File Exists? " + File.Exists(langPath).ToString());

            if (File.Exists(questionsPath)) {
                string questionsData = File.ReadAllText(questionsPath);
                MultipleChoiceQuestionList questionPayload = MultipleChoiceQuestionList.CreateFromJSON(questionsData);
                HandleQuestions(questionPayload);
            }
#endif
        }
    }
}
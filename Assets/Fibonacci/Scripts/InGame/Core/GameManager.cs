using Fibonacci.Event;
using Fibonacci.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace Fibonacci.InGame.Core
{
    public enum GamePhase
    {
        Drawing,
        Playing,
        Paused
    }

    /// <summary>
    /// ゲーム全体の進行状態、フェーズ遷移、およびメニュー操作を統括するマネージャー。
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        public GamePhase CurrentPhase { get; private set; } = GamePhase.Drawing;

        private GamePhase previousPhase;

        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuRoot;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();
            AbilityManager.Instance.ResetAbilities();
            SetupButtons();
        }

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            ResetPhase();
            Time.timeScale = 1f;
            if (pauseMenuRoot != null)
            {
                pauseMenuRoot.SetActive(false);
            }
        }

        /// <summary>
        /// メニュー画面の表示・非表示を切り替える。
        /// </summary>
        public void TogglePause()
        {
            if (pauseMenuRoot == null) return;

            if (CurrentPhase == GamePhase.Paused)
            {
                ResumeGame();
                return;
            }

            PauseGame();
        }

        /// <summary>
        /// タイトル画面へ遷移する。
        /// </summary>
        public void BackToTitle()
        {
            Time.timeScale = 1f;
            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(SceneName.Title);
            }
        }

        /// <summary>
        /// アプリケーションを終了する。
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// ゲーム（アクションフェーズ）を開始する。
        /// </summary>
        public void StartGame()
        {
            SetPhase(GamePhase.Playing);
        }

        /// <summary>
        /// ゲームの状態を初期状態（お絵かきフェーズ）にリセットする。
        /// </summary>
        public void ResetPhase()
        {
            SetPhase(GamePhase.Drawing);
        }

        /// <summary>
        /// インスペクターで設定された各ボタンにリスナーを登録する。
        /// </summary>
        private void SetupButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveAllListeners();
                resumeButton.onClick.AddListener(TogglePause);
            }

            if (titleButton != null)
            {
                titleButton.onClick.RemoveAllListeners();
                titleButton.onClick.AddListener(BackToTitle);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(QuitGame);
            }
        }

        private void PauseGame()
        {
            if (pauseMenuRoot == null) return;

            previousPhase = CurrentPhase;
            SetPhase(GamePhase.Paused);

            pauseMenuRoot.SetActive(true);
            Time.timeScale = 0f;
        }

        private void ResumeGame()
        {
            if (pauseMenuRoot == null) return;

            SetPhase(previousPhase);

            pauseMenuRoot.SetActive(false);
            Time.timeScale = 1f;
        }

        private void SetPhase(GamePhase newPhase)
        {
            CurrentPhase = newPhase;
            GameEvents.TriggerPhaseChanged(newPhase);
        }
    }
}
using Fibonacci.Event;
using Fibonacci.Scene;
using UnityEngine;
using UnityEngine.UI;

namespace Fibonacci.InGame.Core
{
    /// <summary>
    /// ゲームの進行状態（フェーズ）。
    /// </summary>
    public enum GamePhase
    {
        Drawing,
        Playing,
        Paused
    }

    /// <summary>
    /// ゲーム全体の進行状態、フェーズ遷移、およびメニュー操作を統括するマネージャー。
    /// 各シーンのメニューボタンを自動検索して機能を割り当てる。
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        protected override bool UseDontDestroyOnLoad => false;

        public GamePhase CurrentPhase { get; private set; } = GamePhase.Drawing;

        private GamePhase previousPhase;
        private GameObject pauseMenuRoot;

        private const string PAUSE_MENU_NAME = "PauseMenuRoot";
        private const string RESUME_BUTTON_NAME = "ResumeButton";
        private const string TITLE_BUTTON_NAME = "TitleButton";
        private const string QUIT_BUTTON_NAME = "QuitButton";

        protected override void Awake()
        {
            base.Awake();

            if (AbilityManager.Instance != null)
            {
                AbilityManager.Instance.ResetAbilities();
            }
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
            FindPauseMenuAndSetupButtons();
            ResetPhase();
            Time.timeScale = 1f;
        }

        /// <summary>
        /// メニュー画面の表示/非表示を切り替える。
        /// </summary>
        public void TogglePause()
        {
            if (CurrentPhase == GamePhase.Paused)
            {
                ResumeGame();
                return;
            }

            PauseGame();
        }

        /// <summary>
        /// 現在のステージを最初からやり直す。
        /// </summary>
        public void RestartStage()
        {
            Time.timeScale = 1f;
            SceneController.Instance.LoadScene(SceneController.Instance.CurrentStage);
        }

        /// <summary>
        /// タイトル画面へ戻る。
        /// </summary>
        public void BackToTitle()
        {
            Time.timeScale = 1f;
            SceneController.Instance.LoadScene(SceneName.Title);
        }

        /// <summary>
        /// アプリケーションを完全に終了する。
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void StartGame()
        {
            SetPhase(GamePhase.Playing);
        }

        public void ResetPhase()
        {
            SetPhase(GamePhase.Drawing);
        }

        /// <summary>
        /// メニューRootを探し、その子要素にあるボタンに機能を自動登録する。
        /// </summary>
        private void FindPauseMenuAndSetupButtons()
        {
            pauseMenuRoot = GameObject.Find(PAUSE_MENU_NAME);

            if (pauseMenuRoot == null) return;

            SetupButtonListener(RESUME_BUTTON_NAME, TogglePause);
            SetupButtonListener(TITLE_BUTTON_NAME, BackToTitle);
            SetupButtonListener(QUIT_BUTTON_NAME, QuitGame);

            pauseMenuRoot.SetActive(false);
        }

        /// <summary>
        /// メニューRoot配下から名前を指定してボタンを検索し、クリックイベントを登録する。
        /// 階層が深くても（ButtonContainerの中など）見つけることが可能。
        /// </summary>
        private void SetupButtonListener(string buttonName, UnityEngine.Events.UnityAction action)
        {
            Button btn = null;
            foreach (var b in pauseMenuRoot.GetComponentsInChildren<Button>(true))
            {
                if (b.name == buttonName)
                {
                    btn = b;
                    break;
                }
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
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
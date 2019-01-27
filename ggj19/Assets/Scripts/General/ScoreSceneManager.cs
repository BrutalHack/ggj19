using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrutalHack.ggj19.General
{
    public class ScoreSceneManager : MonoBehaviour
    {
        public TextMeshProUGUI textMeshPro;
        
        private void Start()
        {
            int score = ScoreController.Instance.Score;
            String message = "You made " + score + " friends!";
            textMeshPro.text = message;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                Debug.Log("Application Quit");
                Application.Quit();
            }
            else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                OpenStartScene();
            }
        }

        public void OpenStartScene()
        {
            SceneManager.LoadScene((int) Scenes.StartScreen, LoadSceneMode.Single);
        }
    }
}
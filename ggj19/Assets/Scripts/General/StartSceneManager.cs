using UnityEngine;
using UnityEngine.SceneManagement;

namespace BrutalHack.ggj19.General
{
    public class StartSceneManager : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                Debug.Log("Application Quit");
                Application.Quit();
            }
            else if (Input.anyKeyDown)
            {
                OpenGameScene();
            }
        }

        public void OpenGameScene()
        {
            SceneManager.LoadScene((int) Scenes.GameScreen, LoadSceneMode.Single);
        }
    }
}
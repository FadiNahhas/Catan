using UnityEngine;

namespace UI.Main_Menu
{
    public class SteamLoadingScreen : MonoBehaviour
    {
        [SerializeField] private GameObject loadingWheel;
        [SerializeField] private GameObject failText;
        [SerializeField] private GameObject quitButton;
        public void OnFailedToConnect()
        {
            loadingWheel.SetActive(false);
            failText.SetActive(true);
            quitButton.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
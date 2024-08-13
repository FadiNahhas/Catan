using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [TabGroup("References"), SerializeField, Required] private RectTransform mainMenu;
        [TabGroup("References"), SerializeField, Required] private RectTransform settingsMenu;
        [TabGroup("References"), SerializeField, Required] private RectTransform gameBrowser;
        
        [TabGroup("Settings"), SerializeField] private float animationDuration = 0.5f;
        [TabGroup("Settings"), SerializeField] private Ease animationEase;
        public void OpenSettings()
        {
            mainMenu.DOAnchorPosX(1200f, animationDuration).SetEase(animationEase);
            settingsMenu.DOAnchorPosX(0f, animationDuration).SetEase(animationEase);
        }

        public void CloseSettings()
        {
            mainMenu.DOAnchorPosX(0f, animationDuration).SetEase(animationEase);
            settingsMenu.DOAnchorPosX(-1200f, animationDuration).SetEase(animationEase);
        }

        public void OpenGameBrowser()
        {
            mainMenu.DOAnchorPosX(-1200f, animationDuration).SetEase(animationEase);
            gameBrowser.DOAnchorPosX(0f, animationDuration).SetEase(animationEase);   
        }
        
        public void CloseGameBrowser()
        {
            mainMenu.DOAnchorPosX(0f, animationDuration).SetEase(animationEase);
            gameBrowser.DOAnchorPosX(1700f, animationDuration).SetEase(animationEase);
        }
        
        public void QuitGame() => Application.Quit();
    }
}

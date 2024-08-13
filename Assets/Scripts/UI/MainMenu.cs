using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        private const float TargetXPos = 0f;
        
        [TabGroup("References"), SerializeField, Required] private RectTransform mainMenu;
        [TabGroup("References"), SerializeField, Required] private RectTransform settingsMenu;
        [TabGroup("References"), SerializeField, Required] private RectTransform gameBrowser;
        
        [TabGroup("Settings"), SerializeField] private float animationDuration = 0.5f;
        [TabGroup("Settings"), SerializeField] private Ease animationEase;
        
        #region Initial Positions

        private float _settingsMenuInitialXPos;
        private float _gameBrowserInitialXPos;
        
        #endregion

        private void OnEnable()
        { 
            _settingsMenuInitialXPos = settingsMenu.anchoredPosition.x;
            _gameBrowserInitialXPos = gameBrowser.anchoredPosition.x;
        }

        public void OpenSettings()
        {
            mainMenu.DOAnchorPosX(-_settingsMenuInitialXPos, animationDuration).SetEase(animationEase);
            settingsMenu.DOAnchorPosX(TargetXPos, animationDuration).SetEase(animationEase);
        }

        public void CloseSettings()
        {
            mainMenu.DOAnchorPosX(TargetXPos, animationDuration).SetEase(animationEase);
            settingsMenu.DOAnchorPosX(_settingsMenuInitialXPos, animationDuration).SetEase(animationEase);
        }

        public void OpenGameBrowser()
        {
            mainMenu.DOAnchorPosX(_settingsMenuInitialXPos, animationDuration).SetEase(animationEase);
            gameBrowser.DOAnchorPosX(TargetXPos, animationDuration).SetEase(animationEase);   
        }
        
        public void CloseGameBrowser()
        {
            mainMenu.DOAnchorPosX(TargetXPos, animationDuration).SetEase(animationEase);
            gameBrowser.DOAnchorPosX(_gameBrowserInitialXPos, animationDuration).SetEase(animationEase);
        }
        
        public void QuitGame() => Application.Quit();
    }
}

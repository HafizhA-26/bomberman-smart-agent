using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BombermanRL
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class LoadingHandler : MonoBehaviour
    {
        [SerializeField] private Image _bombIcon;
        [SerializeField] private Image _loadingThrobber;

        private RectTransform _loadingTransform;
        private CanvasGroup _loadingCanvasGroup;
        private Image _panelImage;
        private Tween _spinLoadingTween;
        private Tween _bombIconTween;
        private Vector3 _bombIconScale;
        private Color _basePanelColor;
        private float _minLoadingTime = 0;
        private bool _ableToClose = false; // True if has reached min loading time
        private bool _readyToClose = false; // True if HideLoading has been called even when min loading time has not been reached

        private void Awake()
        {
            _panelImage = GetComponent<Image>();
            _loadingCanvasGroup = GetComponent<CanvasGroup>();
            _loadingTransform = _loadingThrobber.GetComponent<RectTransform>();
            _loadingCanvasGroup.alpha = 0f;

            _basePanelColor = _panelImage.color;
            _bombIconScale = _bombIcon.transform.localScale;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if(gameObject.activeInHierarchy)
            {
                if(_minLoadingTime > 0)
                {
                    _minLoadingTime -= Time.deltaTime;
                }
                else if(!_ableToClose)
                {
                    _ableToClose = true;
                    if(_readyToClose) HideLoadingPanel();
                }
            }
        }

        public void ShowLoadingPanel(float minLoadingTime = 0, bool opaquePanel = false)
        {
            // Return if loading panel already showed up
            if (gameObject.activeInHierarchy) return;

            // Setup transparent or opaque loading
            if(opaquePanel)
            {
                _loadingThrobber.color = new Color(1f, 1f, 1f, 0.3f);
                _panelImage.color = new Color(_basePanelColor.r, _basePanelColor.g, _basePanelColor.b, 1f);
                _bombIcon.transform.localScale = _bombIconScale;
                _bombIconTween = _bombIcon.transform.DOPunchScale(new Vector3(0.2f, 0.2f), 1.5f, 1, 0).SetLoops(-1, LoopType.Restart);
                _bombIcon.gameObject.SetActive(true);
            }
            else
            {
                _loadingThrobber.color = new Color(1f, 1f, 1f, 1f);
                _panelImage.color = new Color(_basePanelColor.r, _basePanelColor.g, _basePanelColor.b, 0.5f);
                _bombIcon.gameObject.SetActive(false);
                _bombIconTween?.Kill();
            }

            _minLoadingTime = minLoadingTime;
            _ableToClose = _minLoadingTime <= 0;
            _readyToClose = false;

            // Start throbber looping animation
            if(_spinLoadingTween == null)
            {
                _spinLoadingTween = _loadingTransform.DOLocalRotate(Vector3.back * 360, 1.5f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
                _spinLoadingTween.Play();
            }

            gameObject.SetActive(true);
            _loadingCanvasGroup.DOFade(1f, 0.2f);
        }

        public void HideLoadingPanel()
        {
            _readyToClose = true;
            // Return if loading panel already hide
            if (!gameObject.activeInHierarchy || !_ableToClose) return;

            _loadingCanvasGroup.DOFade(0f, 0.2f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                _spinLoadingTween.Kill();
                _spinLoadingTween = null;
            });
        }
    }

}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiFade : MonoBehaviour
{
    [SerializeField] private Graphic _backgroundGraphicToFade;
    [SerializeField] private float _fadeInDurationSec = 0.75f;
    [SerializeField] private float _fadeOutDurationSec = 1f;

    public event Action OnFadeInEnded;
    public event Action OnFadeOutEnded;

    public bool IsFading { get { return _fadeInCoroutine != null || _fadeOutCoroutine != null; } }

    private const float _ZERO = 0f;
    private const float _ONE = 1f;

    private bool _isFadedIn;
    private bool _isFadedOut = true;
    private Coroutine _fadeInCoroutine;
    private Coroutine _fadeOutCoroutine;

#if UNITY_EDITOR
    [SerializeField] private KeyCode _debugToFadeIn = KeyCode.I;
    [SerializeField] private KeyCode _debugToFadeOut = KeyCode.O;

    private void Update()
    {
        if (Input.GetKeyDown(_debugToFadeIn))
        {
            TryToFadeIn();
        }

        if (Input.GetKeyDown(_debugToFadeOut))
        {
            TryToFadeOut();
        }
    }
#endif

    private void Start()
    {
        ResetFade();
    }

    private void ResetFade()
    {
        _backgroundGraphicToFade.gameObject.SetActive(false);
        _backgroundGraphicToFade.CrossFadeAlpha(_ZERO, _ZERO, false);
    }

    public void TryToFadeIn()
    {
        if (!IsFading && _isFadedOut)
        {
            _fadeInCoroutine = StartCoroutine(FadeInCoroutine());
        }
    }

    public void TryToFadeOut()
    {
        if (!IsFading && _isFadedIn)
        {
            _fadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        _backgroundGraphicToFade.gameObject.SetActive(true);
        _backgroundGraphicToFade.CrossFadeAlpha(_ONE, _fadeInDurationSec, false);
        yield return new WaitForSeconds(_fadeInDurationSec);
        _fadeInCoroutine = null;
        _isFadedIn = true;
        _isFadedOut = false;

        OnFadeInEnded?.Invoke();
    }

    private IEnumerator FadeOutCoroutine()
    {
        _backgroundGraphicToFade.CrossFadeAlpha(_ZERO, _fadeOutDurationSec, false);
        yield return new WaitForSeconds(_fadeOutDurationSec);
        _fadeOutCoroutine = null;
        ResetFade();
        _isFadedOut = true;
        _isFadedIn = false;

        OnFadeOutEnded?.Invoke();
    }
}
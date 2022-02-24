using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiGraphicCrossFader : MonoBehaviour
{
    [SerializeField] private Graphic _graphicToCrossFade;
    [SerializeField] private float _halfBlinkDuration = 0.5f;
    [SerializeField] private float _minAlphaTransparency = 0.3f;
    [SerializeField] private float _fadeInDuration = 0.25f;
    [SerializeField] private float _fadeOutDuration = 0.25f;

    private const float _ZERO = 0f;
    private const float _ONE = 1f;

    private Coroutine _blinkCoroutine;
    private Coroutine _turnOffCoroutine;

#if UNITY_EDITOR
    [SerializeField] private KeyCode _startBlinkKey = KeyCode.Q;
    [SerializeField] private KeyCode _stopBlinkKey = KeyCode.W;

    private void Update()
    {
        if (Input.GetKeyDown(_startBlinkKey))
        {
            SetBlinkState(true);
        }

        if (Input.GetKeyDown(_stopBlinkKey))
        {
            SetBlinkState(false);
        }
    }
#endif

    private void SetBlinkState(bool state)
    {
        if (state)
        {
            if (_blinkCoroutine == null && _turnOffCoroutine == null)
            {
                _blinkCoroutine = StartCoroutine(InfiniteBlink());
            }
        }
        else
        {
            if (_blinkCoroutine != null && _turnOffCoroutine == null)
            {
                _turnOffCoroutine = StartCoroutine(StopBlinkingAndFadeOut());
            }
        }
    }

    private IEnumerator InfiniteBlink()
    {
        _graphicToCrossFade.CrossFadeAlpha(_ZERO, _ZERO, false);
        _graphicToCrossFade.gameObject.SetActive(true);
        _graphicToCrossFade.CrossFadeAlpha(_minAlphaTransparency, _fadeInDuration, false);
        yield return new WaitForSeconds(_fadeInDuration);

        while (true)
        {
            _graphicToCrossFade.CrossFadeAlpha(_ONE, _halfBlinkDuration, false);
            yield return new WaitForSeconds(_halfBlinkDuration);

            _graphicToCrossFade.CrossFadeAlpha(_minAlphaTransparency, _halfBlinkDuration, false);
            yield return new WaitForSeconds(_halfBlinkDuration);
        }
    }

    private IEnumerator StopBlinkingAndFadeOut()
    {
        StopCoroutine(_blinkCoroutine);
        _blinkCoroutine = null;

        _graphicToCrossFade.CrossFadeAlpha(_ZERO, _fadeOutDuration, false);
        yield return new WaitForSeconds(_fadeOutDuration);

        StopAllCoroutines();
        _turnOffCoroutine = null;
        _graphicToCrossFade.gameObject.SetActive(false);
    }
}
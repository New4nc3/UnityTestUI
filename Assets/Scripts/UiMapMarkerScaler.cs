using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiMapMarkerScaler : MonoBehaviour
{
    public bool IsRunning { get { return _animationCoroutine != null; } }

    [SerializeField] private Image _imageToScale;
    [SerializeField] private float _middleScale = 0.6f;
    [SerializeField] private float _endScale = 1.1f;
    [SerializeField] private float _firstAnimationDurationSec = 2.25f;
    [SerializeField] private float _secondAnimationDurationSec = 1.75f;
    [SerializeField] private bool _looped = true;
    [SerializeField] private float _delayBetweenAnimationsSec = 0.75f;

    private Vector3 _initialScale = new Vector3(0, 0, 1);
    private Coroutine _animationCoroutine;

    private void OnEnable()
    {
        StartAnimation();
    }

    public void RequestStop(bool immediate = false)
    {
        if (immediate)
        {
            StopAnimation();
        }
        else
        {
            if (IsRunning)
            {
                _looped = false;
            }
        }
    }

    public void StartAnimation()
    {
        StopAnimation();

        _animationCoroutine = StartCoroutine(ScalingCoroutine());
    }

    private void ResetState()
    {
        print("Reset");
        _imageToScale.transform.localScale = _initialScale;
        SetAlpha(1f);
    }

    private void StopAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
    }

    private IEnumerator ScalingCoroutine()
    {
        do
        {
            ResetState();

            yield return StartCoroutine(FastLinearScaling());
            yield return StartCoroutine(SlowLinearScalingWithAlpha());

            if (_looped)
            {
                yield return new WaitForSeconds(_delayBetweenAnimationsSec);
            }

        } while (_looped);

        _animationCoroutine = null;
    }

    private IEnumerator FastLinearScaling()
    {
        float elapsed = 0;
        float frameScaleValue;

        while (elapsed < _firstAnimationDurationSec)
        {
            frameScaleValue = Mathf.Lerp(0, _middleScale, elapsed / _firstAnimationDurationSec);
            _imageToScale.transform.localScale = new Vector3(frameScaleValue, frameScaleValue, 1);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _imageToScale.transform.localScale = new Vector3(_middleScale, _middleScale, 1);
        yield return null;
    }

    private IEnumerator SlowLinearScalingWithAlpha()
    {
        float elapsed = 0;
        float frameValue;
        float frameScaleValue;
        float frameAlphaValue;

        while (elapsed < _secondAnimationDurationSec)
        {
            frameValue = elapsed / _secondAnimationDurationSec;
            frameScaleValue = Mathf.Lerp(_middleScale, _endScale, frameValue);
            frameAlphaValue = Mathf.Lerp(1, 0, frameValue);
            _imageToScale.transform.localScale = new Vector3(frameScaleValue, frameScaleValue, 1);
            SetAlpha(frameAlphaValue);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _imageToScale.transform.localScale = new Vector3(_endScale, _endScale, 1);
        SetAlpha(0f);
        yield return null;
    }

    private void SetAlpha(float newValue)
    {
        Color color = _imageToScale.color;
        color.a = newValue;
        _imageToScale.color = color;
    }
}
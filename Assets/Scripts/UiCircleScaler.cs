using System.Collections;
using UnityEngine;

public class UiCircleScaler : MonoBehaviour
{
    [SerializeField] private Transform _transformToScale;
    [SerializeField] private float _startScale;
    [SerializeField] private float _endScale;
    [SerializeField] private float _zoomInDuration;
    [SerializeField] private float _zoomOutDuration;
    [SerializeField] private float _delayBeforeStartDuration;
    [SerializeField] private float _delayBeforeZoomOutDuration;
    [SerializeField] private float _delayBeforeZoomInDuration;

    private Coroutine _scalerCoroutine;

    private void OnEnable()
    {
        StartScaleAnimation();
    }

    private void OnDisable()
    {
        TryToStopCoroutine();
    }

    private void TryToStopCoroutine()
    {
        if (_scalerCoroutine != null)
        {
            StopCoroutine(_scalerCoroutine);
            _scalerCoroutine = null;
        }

        ResetUI();
    }

    private void ResetUI()
    {
        SetScale(_startScale);
    }

    private void SetScale(float newScale)
    {
        _transformToScale.localScale = new Vector3(newScale, newScale, 1);
    }

    public void StartScaleAnimation()
    {
        TryToStopCoroutine();
        _scalerCoroutine = StartCoroutine(ScaleCoroutine());
    }

    private IEnumerator ScaleCoroutine()
    {
        float elapsed;

        if (_delayBeforeStartDuration != 0f)
        {
            yield return new WaitForSeconds(_delayBeforeStartDuration);
        }

        while (true)
        {
            elapsed = 0f;

            while (elapsed < _zoomInDuration)
            {
                SetScale(Mathf.Lerp(_startScale, _endScale, elapsed / _zoomInDuration));

                yield return null;

                elapsed += Time.deltaTime;
            }

            SetScale(_endScale);

            yield return new WaitForSeconds(_delayBeforeZoomOutDuration);

            elapsed = 0f;

            while (elapsed < _zoomOutDuration)
            {
                SetScale(Mathf.Lerp(_endScale, _startScale, elapsed / _zoomOutDuration));

                yield return null;

                elapsed += Time.deltaTime;
            }

            SetScale(_startScale);

            yield return new WaitForSeconds(_delayBeforeZoomInDuration);
        }
    }
}
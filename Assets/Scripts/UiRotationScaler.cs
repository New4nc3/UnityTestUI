using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiRotationScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _minStartAngleDegree = 5;
    [SerializeField] private float _maxStartAngleDegree = 20;
    [SerializeField] private float _startScale = 1;
    [SerializeField] private float _hoveredScale = 1.4f;
    [SerializeField] private float _zoomInDurationSec = 0.5f;
    [SerializeField] private float _zoomOutDurationSec = 0.35f;
    [SerializeField] private bool _randomizeRotationAfterSuccessZoomIn = true;

    private Vector3 _startRotationEuler, _startLocalScale;
    private Coroutine _pointerEnterHandlerCoroutine, _pointerExitHandlerCoroutine;

    private void Start()
    {
        _startLocalScale = new Vector3(_startScale, _startScale, 1);

        RandomizeStartRotation();
        ResetToInitialState();
    }

    private void RandomizeStartRotation()
    {
        _startRotationEuler = GetRandomEulerRotation(canValuesBeNegative: true);
    }

    private Vector3 GetRandomEulerRotation(bool canValuesBeNegative = true)
    {
        float x = GetRandomAngleInRange();
        float y = GetRandomAngleInRange();
        float z = GetRandomAngleInRange();

        return new Vector3(x, y, z);
    }

    private float GetRandomAngleInRange(bool canBeNegative = true)
    {
        float randomAngle = Random.Range(_minStartAngleDegree, _maxStartAngleDegree);

        if (canBeNegative)
        {
            int shouldBeNegative = Random.Range(0, 2);

            if (shouldBeNegative == 1)
            {
                randomAngle *= -1;
            }
        }

        return randomAngle;
    }

    private void ResetToInitialState()
    {
        transform.eulerAngles = _startRotationEuler;
        transform.localScale = _startLocalScale;
    }

    private IEnumerator AnimateZoomIn()
    {
        float frameScale = transform.localScale.x;
        float inversedScale = Mathf.InverseLerp(_hoveredScale, _startScale, frameScale);
        float elapsedTime = _zoomInDurationSec - _zoomInDurationSec * inversedScale;
        float normalizedTime;
        Vector3 frameEulers;

        while (elapsedTime < _zoomInDurationSec)
        {
            normalizedTime = elapsedTime / _zoomInDurationSec;
            frameEulers = Vector3.Lerp(_startRotationEuler, Vector3.zero, normalizedTime);
            frameScale = Mathf.Lerp(_startScale, _hoveredScale, normalizedTime);
            elapsedTime += Time.deltaTime;

            transform.eulerAngles = frameEulers;
            transform.localScale = new Vector3(frameScale, frameScale, 1);

            yield return null;
        }

        transform.eulerAngles = Vector3.zero;
        transform.localScale = new Vector3(_hoveredScale, _hoveredScale, 1);

        if (_randomizeRotationAfterSuccessZoomIn)
        {
            RandomizeStartRotation();
        }

        _pointerEnterHandlerCoroutine = null;
    }

    private IEnumerator AnimateZoomOut()
    {
        float frameScale = transform.localScale.x;
        float inversedScale = Mathf.InverseLerp(_startScale, _hoveredScale, frameScale);
        float elapsedTime = _zoomOutDurationSec - _zoomOutDurationSec * inversedScale;
        float normalizedTime;
        Vector3 frameEulers;

        while (elapsedTime < _zoomOutDurationSec)
        {
            normalizedTime = elapsedTime / _zoomOutDurationSec;
            frameEulers = Vector3.Lerp(Vector3.zero, _startRotationEuler, normalizedTime);
            frameScale = Mathf.Lerp(_hoveredScale, _startScale, normalizedTime);
            elapsedTime += Time.deltaTime;

            transform.eulerAngles = frameEulers;
            transform.localScale = new Vector3(frameScale, frameScale, 1);

            yield return null;
        }

        ResetToInitialState();
        _pointerExitHandlerCoroutine = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_pointerExitHandlerCoroutine != null)
        {
            StopCoroutine(_pointerExitHandlerCoroutine);
            _pointerExitHandlerCoroutine = null;
        }

        _pointerEnterHandlerCoroutine = StartCoroutine(AnimateZoomIn());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_pointerEnterHandlerCoroutine != null)
        {
            StopCoroutine(_pointerEnterHandlerCoroutine);
            _pointerEnterHandlerCoroutine = null;
        }

        _pointerExitHandlerCoroutine = StartCoroutine(AnimateZoomOut());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float _duration;
    [SerializeField] private float _resizeMultiplier;
    [SerializeField] private AnimationCurve _animationCurve;

    private Vector3 _initialSize;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _initialSize = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayAnimation(transform, _duration, _animationCurve);
    }

    public void PlayAnimation(Transform transform, float duration, AnimationCurve animationCurve)
    {
        if (_animationCoroutine != null)
            StopCoroutine(_animationCoroutine);

        _animationCoroutine = StartCoroutine(WoopAnimation(transform, duration, animationCurve));
    }

    private IEnumerator WoopAnimation(Transform transform, float duration, AnimationCurve animationCurve)
    {
        float elapsedTime = 0;

        while (elapsedTime < _duration)
        {
            transform.localScale = _initialSize + Vector3.one* animationCurve.Evaluate(elapsedTime / duration) * _resizeMultiplier;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}

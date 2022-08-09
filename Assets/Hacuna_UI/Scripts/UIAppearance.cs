using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIAppearance : MonoBehaviour
{
    [SerializeField] private AppearSide _appearSide;
    [SerializeField] private RectTransform _transform;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _animationTime;

    private float _startOffset;
    private RectTransform _canvasRect;
    private Vector2 _screenCenter;
    private Vector2 _modifier;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        SetModifier();
        gameObject.SetActive(true);
        _startOffset = GetStartOffset(_appearSide);

        var moveFunction = GetMoveFunction(_appearSide);

        StartCoroutine(Animating(_transform.transform, _appearSide, moveFunction));
    }

    private void SetModifier()
    {
        var canvasScaler = GetCanvasScaler();

        float widthMultiplier = canvasScaler.matchWidthOrHeight;
        _screenCenter = _canvasRect.rect.size / 2f;
        _modifier = new Vector2(_screenCenter.x / (Screen.width * widthMultiplier), _screenCenter.y / (Screen.height * (1 - widthMultiplier)));
    }

    private CanvasScaler GetCanvasScaler()
    {
        _canvasRect = transform.root.GetComponentInParent<RectTransform>();

        if (_canvasRect == null)
            throw new NullReferenceException("No canvas in root");

        var canvasScaler = _canvasRect.GetComponent<CanvasScaler>();

        if (canvasScaler == null)
            throw new NullReferenceException("No canvas scaler in root");

        return canvasScaler;
    }

    private Vector2 VerticalMove(Vector2 nextPosition, Vector2 startPos, float elapsedTime)
    {
        nextPosition.x = _transform.localPosition.x;
        nextPosition.y = startPos.y + _animationCurve.Evaluate(elapsedTime / _animationTime) * _startOffset;

        return nextPosition;
    }

    private Vector2 HorizontalMove(Vector2 nextPosition, Vector2 startPos, float elapsedTime)
    {
        nextPosition.x = startPos.x + _animationCurve.Evaluate(elapsedTime / _animationTime) * _startOffset; 
        nextPosition.y = _transform.localPosition.y;

        return nextPosition;
    }

    private float GetStartOffset(AppearSide appearSide)
    {
        float sign = GetStartOffsetSign(appearSide);
        float offset = sign * GetStartOffsetValue(appearSide);

        return offset;
    }

    private float GetStartOffsetSign(AppearSide appearSide)
    {
        return appearSide == AppearSide.Down || appearSide == AppearSide.Left ? 1 : -1;
    }

    private Func<Vector2, Vector2, float, Vector2> GetMoveFunction(AppearSide appearSide)
    {
        if (IsVerticalMove(appearSide))
            return VerticalMove;

        return HorizontalMove;
    }

    private bool IsVerticalMove(AppearSide appearSide)
    {
        return appearSide == AppearSide.Up || appearSide == AppearSide.Down;
    }

    private float GetStartOffsetValue(AppearSide appearSide)
    {
        switch (appearSide)
        {
            case AppearSide.Up:
                return (Screen.height - _transform.position.y + _transform.rect.height) * _modifier.y;
            case AppearSide.Down:
                return (_transform.position.y + _transform.rect.height) * _modifier.y;
            case AppearSide.Left:
                return (_transform.position.x + _transform.rect.width) * _modifier.x;
            case AppearSide.Right:
                return (Screen.width - _transform.position.x + _transform.rect.width) * _modifier.x;
            default:
                return 0;
        }
    }

    private IEnumerator Animating(Transform transform, AppearSide appearSide, Func<Vector2, Vector2, float, Vector2> GetNextPosition)
    {
        Vector2 startPos = GetStartPosition(transform, appearSide);

        transform.localPosition = startPos;

        float elapsedTime = 0;

        while (elapsedTime < _animationTime)
        {
            transform.localPosition = GetNextPosition(transform.localPosition, startPos, elapsedTime);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    private Vector2 GetStartPosition(Transform transform, AppearSide appearSide)
    {
        if (IsVerticalMove(appearSide))
            return new Vector2(transform.localPosition.x, transform.localPosition.y - _startOffset);

        return new Vector2(transform.localPosition.x - _startOffset, transform.localPosition.y);
    }
}

public enum AppearSide
{
    Up,
    Down,
    Left,
    Right
}

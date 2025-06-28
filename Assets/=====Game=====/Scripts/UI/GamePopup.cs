using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePopup<T> : MonoBehaviour where T : GamePopup<T>
{
    public static GamePopup<T> Instance { get; private set; }
    [SerializeField] Type_Move_Popup typeMove;
    [SerializeField] float duration = 1f;

    private float x = 1500, y = 2500;
    bool isShow = false;
    protected virtual void Awake()
    {

    }
    protected virtual void OnAnimateComplete()
    {

    }

    protected virtual void AnimateOpen(Action onComplete = null)
    {
        switch (typeMove)
        {
            case Type_Move_Popup.SlideFromTop:
                transform.localPosition = new Vector3(0, 2000, 0);
                transform.DOLocalMoveY(0, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.SlideFromBottom:
                transform.localPosition = Vector3.down * y;
                transform.DOLocalMoveY(0, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });

                break;
            case Type_Move_Popup.SlideFromLeft:
                transform.localPosition = Vector3.left * x;
                transform.DOLocalMoveX(0, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.SlideFromRight:
                transform.localPosition = Vector3.right * x;
                transform.DOLocalMoveX(0, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.FadeIn:
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.FadeOut:
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 1;
                canvasGroup.DOFade(0, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.ScaleUp:
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.ScaleDown:
                transform.localScale = Vector3.one;
                transform.DOScale(Vector3.zero, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.Rotate:
                transform.localRotation = Quaternion.Euler(0, 0, 180);
                transform.DORotate(Vector3.zero, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
        }
    }
    protected virtual void AnimateClose(Action onComplete = null)
    {
        switch (typeMove)
        {
            case Type_Move_Popup.SlideFromTop:
                transform.DOLocalMoveY(2000, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.SlideFromBottom:
                transform.DOLocalMoveY(-y, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.SlideFromLeft:
                transform.DOLocalMoveX(-x, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.SlideFromRight:
                transform.DOLocalMoveX(x, duration).
                    OnComplete(() =>
                    {
                        transform.localPosition = Vector3.zero;
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.FadeIn:
                CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.DOFade(0, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.FadeOut:
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.DOFade(1, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.ScaleUp:
                transform.DOScale(Vector3.zero, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.ScaleDown:
                transform.DOScale(Vector3.one, duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;
            case Type_Move_Popup.Rotate:
                transform.DORotate(new Vector3(0, 0, 180), duration).
                    OnComplete(() =>
                    {
                        OnAnimateComplete();
                        onComplete?.Invoke();
                    });
                break;


        }
    }
}

public enum Type_Move_Popup {
    SlideFromTop,
    SlideFromBottom,
    SlideFromLeft,
    SlideFromRight,
    FadeIn,
    FadeOut,
    ScaleUp,
    ScaleDown,
    Rotate
}

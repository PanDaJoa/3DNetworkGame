using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))] //무조건 써야 되는것 (이 스크립트에서의 필요 컴포넌트)
public class UI_DamageEffect : MonoBehaviour
{
    public static UI_DamageEffect Instance { get; private set; }
    public AnimationCurve ShowCurve;
    private CanvasGroup _canvasGroup;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    public void Show(float duration)
    {
        _canvasGroup.alpha = 1f;
        StartCoroutine(Show_Coroutine(duration));
    }

    private IEnumerator Show_Coroutine(float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;

            _canvasGroup.alpha = ShowCurve.Evaluate(elapsedTime/ duration);

            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }
}

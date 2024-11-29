using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColorEffect : MonoBehaviour
{
    public Color DamageColor = Color.red;
    public float DamageTimeSec = 1f;

    private SpriteRenderer _spriteRend;
    private Color _defaultColor;

    private void Start()
    {
        _spriteRend = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRend.color;
    }

    public void StartEffect()
    {
        StopCoroutine(nameof(StartEffectCoroutine));
        StartCoroutine(nameof(StartEffectCoroutine));
    }

    private IEnumerator StartEffectCoroutine()
    {
        float time = 0;
        float step = 1f / DamageTimeSec;

        while (time < DamageTimeSec)
        {
            time += Time.deltaTime;
            _spriteRend.color = Color.Lerp(DamageColor, _defaultColor, step * time);

            yield return null;
        }
    }
}

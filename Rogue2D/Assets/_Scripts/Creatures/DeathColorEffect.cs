using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathColorEffect : MonoBehaviour
{
    public Color DeathColor = new Color(255, 255, 255, 0);
    public float DeathTimeSec = 1f;

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
        float step = 1f / DeathTimeSec;

        while (time < DeathTimeSec)
        {
            time += Time.deltaTime;
            _spriteRend.color = Color.Lerp(_defaultColor, DeathColor, step * time);

            yield return null;
        }
    }
}

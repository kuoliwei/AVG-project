using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Image backgroundImage;
    public BackgroundDatabase database;

    public float fadeDuration = 0.5f;

    private Coroutine currentFade;

    public void ChangeBackground(string key)
    {
        //Debug.Log("ChangeBackground");
        Sprite target = database.GetBackgroundByKey(key);
        if (target == null) return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeTo(target));
    }

    private System.Collections.IEnumerator FadeTo(Sprite target)
    {
        // 先淡出
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(1f - (t / fadeDuration));
            yield return null;
        }

        backgroundImage.sprite = target;

        // 再淡入
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(t / fadeDuration);
            yield return null;
        }

        SetAlpha(1f);
    }

    private void SetAlpha(float alpha)
    {
        if (backgroundImage != null)
        {
            var color = backgroundImage.color;
            color.a = alpha;
            backgroundImage.color = color;
        }
    }
}

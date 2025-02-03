using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // Asigna la UI Image que cubre la pantalla
    [SerializeField] private float fadeDuration = 0.5f; // Duración del fade

    // Inicia el fundido a negro
    public void FadeToBlack()
    {
        StartCoroutine(FadeCoroutine(1f)); // 1 significa negro completo
    }

    // Inicia el fundido desde negro
    //public void FadeFromBlack()
    //{
    //    StartCoroutine(FadeCoroutine(0f)); // 0 significa sin negro
    //}

    private IEnumerator FadeCoroutine(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}

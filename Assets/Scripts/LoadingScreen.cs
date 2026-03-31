using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI References")]
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public CanvasGroup fadeCanvasGroup;

    [Header("Settings")]
    public float fadeDuration = 0.5f;

    [Range(0f, 0.9f)]
    public float minFakeProgress = 0.5f;

    private void Start()
    {
        if (progressBar != null) progressBar.value = 0f;
        if (progressText != null) progressText.text = "0%";
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        yield return StartCoroutine(Fade(0f, 1f));

        AsyncOperation operation = SceneManager.LoadSceneAsync("Jogo"); // ← hardcoded, só carrega o jogo
        operation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            targetProgress = Mathf.Max(targetProgress, minFakeProgress);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, Time.deltaTime * 0.8f);
            UpdateUI(displayedProgress);

            if (operation.progress >= 0.9f)
            {
                UpdateUI(1f);
                yield return new WaitForSeconds(0.2f);
                yield return StartCoroutine(Fade(1f, 0f));
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void UpdateUI(float progress)
    {
        if (progressBar != null)
            progressBar.value = progress;
        if (progressText != null)
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
    }

    private IEnumerator Fade(float from, float to)
    {
        if (fadeCanvasGroup == null) yield break;

        float elapsed = 0f;
        fadeCanvasGroup.alpha = from;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }
}
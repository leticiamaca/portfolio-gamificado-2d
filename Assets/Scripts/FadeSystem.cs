using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeSystem : MonoBehaviour
{
    public static FadeSystem Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Ativa a image antes do fade
        fadeImage.gameObject.SetActive(true);

        // Fade para preto
        yield return StartCoroutine(Fade(0f, 1f));

        SceneManager.LoadScene(sceneName);

        // Espera a cena carregar
        yield return null;

        // Fade saindo do preto
        yield return StartCoroutine(Fade(1f, 0f));

        // Desativa a image após o fade terminar
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = to;
        fadeImage.color = color;
    }
}

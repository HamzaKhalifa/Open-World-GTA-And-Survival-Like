using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenUI : MonoBehaviour
{
    [SerializeField] private Image _image = null;

    private void Awake()
    {
        SetBlackScreenOpacity(0);
    }

    private void SetBlackScreenOpacity(float opacity)
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, opacity);
    }

    public void SetBlackScreen(float fadeInAndOutDelay, float timeSpentOnBlackScreen, System.Action toDoDuringBlackScreen)
    {
        StartCoroutine(BlackScreenCoroutine(fadeInAndOutDelay, timeSpentOnBlackScreen, toDoDuringBlackScreen));
    }

    public IEnumerator BlackScreenCoroutine(float fadeInAndOutDelay, float timeSpentOnBlackScreen, System.Action toDoDuringBlackScreen)
    {
        _image.gameObject.SetActive(true);

        // Making the screen black
        float time = 0f;
        while (time < fadeInAndOutDelay)
        {
            time += Time.deltaTime;
            float normalizedTime = time / fadeInAndOutDelay;

            float nextOpacity = Mathf.Lerp(0, 1, normalizedTime);
            SetBlackScreenOpacity(nextOpacity);

            yield return null;
        }

        SetBlackScreenOpacity(1);

        toDoDuringBlackScreen();

        yield return new WaitForSeconds(timeSpentOnBlackScreen);

        time = 0f;
        while (time < fadeInAndOutDelay)
        {
            time += Time.deltaTime;
            float normalizedTime = time / fadeInAndOutDelay;

            float nextOpacity = Mathf.Lerp(1, 0, normalizedTime);

            SetBlackScreenOpacity(nextOpacity);

            yield return null;
        }

        SetBlackScreenOpacity(0);

        _image.gameObject.SetActive(false);

        yield return null;
    }
}

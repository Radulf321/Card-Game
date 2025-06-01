using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable
public class FadeHandler : MonoBehaviour
{
    public static FadeHandler? Instance { get; private set; }

    private string fadeInTrigger = "FadeInTrigger";
    private string fadeOutTrigger = "FadeOutTrigger";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        Animator fadeAnimator = transform.GetComponent<Animator>();
        // Fade Out
        fadeAnimator.SetTrigger(fadeOutTrigger);
        yield return WaitForAnimationFinish(fadeAnimator, fadeOutTrigger);

        // Load Scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Fade In
        fadeAnimator.SetTrigger(fadeInTrigger);
        yield return WaitForAnimationFinish(fadeAnimator, fadeInTrigger);
    }

    // Eine Hilfsmethode, um auf das Ende einer Animation zu warten
    private IEnumerator WaitForAnimationFinish(Animator animator, string triggerName)
    {
        yield return null;

        string targetAnimatorStateName;
        if (triggerName == fadeOutTrigger)
        {
            targetAnimatorStateName = "FadeOut";
        }
        else if (triggerName == fadeInTrigger)
        {
            targetAnimatorStateName = "FadeIn";
        }
        else
        {
            throw new System.ArgumentException("Unknown Trigger Name: " + triggerName);
        }

        float timer = 0f;
        float timeout = 30f;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(targetAnimatorStateName) && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (timer >= timeout)
        {
            throw new System.TimeoutException($"FadeHandler: Timeout while waiting for state '{targetAnimatorStateName}' for trigger '{triggerName}'.");
        }

        // Wait for the animation to finish
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && animator.GetCurrentAnimatorStateInfo(0).IsName(targetAnimatorStateName))
        {
            yield return null;
        }
    }
}
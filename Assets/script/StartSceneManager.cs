using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StartSceneManager : MonoBehaviour
{
    public enum StepType
    {
        StillImage,
        FadeMoveImage,
        DoubleFadeMoveImages
    }

    [System.Serializable]
    public class SequenceStep
    {
        public StepType stepType;

        [Header("Image 1")]
        public RawImage targetImage;
        public Texture texture;
        public Vector3 startPos;
        public Vector3 endPos;

        [Header("Image 2 (only for DoubleFadeMoveImages)")]
        public RawImage targetImage2;
        public Texture texture2;
        public Vector3 startPos2;
        public Vector3 endPos2;

        [Header("Animation")]
        public float duration = 1.5f;

    }

    [Header("UI")]
    public GameObject pressAText;

    [Header("Sequence")]
    public List<SequenceStep> steps = new List<SequenceStep>();

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Cleanup")]
    public int keepRecentSteps = 5;

    [Header("Title")]
    public GameObject titleObject;

    private int currentStepIndex = -1;
    private bool inputLocked = false;

    private void Start()
    {
        if (pressAText != null)
            pressAText.SetActive(true);

        HideAllStepImages();
    }

    private void Update()
    {
        if (Gamepad.current == null) return;

        // Press X to skip intro and load game scene
        if (Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        if (inputLocked) return;

        // Press A to continue
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            PlayNextStep();
        }
    }

    private void PlayNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex == 0 && titleObject != null)
            titleObject.SetActive(false);

        if (currentStepIndex >= steps.Count)
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        if (pressAText != null)
            pressAText.SetActive(false);

        int oldStepIndex = currentStepIndex - keepRecentSteps;
        if (oldStepIndex >= 0)
        {
            DestroyStepVisuals(oldStepIndex);
        }

        SequenceStep step = steps[currentStepIndex];

        switch (step.stepType)
        {
            case StepType.StillImage:
                ShowStillImage(step);
                break;

            case StepType.FadeMoveImage:
                StartCoroutine(PlayFadeMove(step));
                break;

            case StepType.DoubleFadeMoveImages:
                StartCoroutine(PlayDoubleFadeMove(step));
                break;
        }
    }

    private void ShowStillImage(SequenceStep step)
    {
        if (step.targetImage == null) return;

        step.targetImage.gameObject.SetActive(true);
        step.targetImage.texture = step.texture;

        Color c = step.targetImage.color;
        c.a = 1f;
        step.targetImage.color = c;
    }

    private IEnumerator PlayFadeMove(SequenceStep step)
    {
        if (step.targetImage == null) yield break;

        inputLocked = true;

        RawImage img = step.targetImage;
        img.gameObject.SetActive(true);
        img.texture = step.texture;
        img.rectTransform.localPosition = step.startPos;

        Color c = img.color;
        c.a = 0f;
        img.color = c;

        float time = 0f;

        while (time < step.duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / step.duration);

            img.rectTransform.localPosition = Vector3.Lerp(step.startPos, step.endPos, t);

            c.a = Mathf.Lerp(0f, 1f, t);
            img.color = c;

            yield return null;
        }

        img.rectTransform.localPosition = step.endPos;
        c.a = 1f;
        img.color = c;

        inputLocked = false;
    }

    private IEnumerator PlayDoubleFadeMove(SequenceStep step)
    {
        if (step.targetImage == null || step.targetImage2 == null) yield break;

        inputLocked = true;

        RawImage img1 = step.targetImage;
        RawImage img2 = step.targetImage2;

        img1.gameObject.SetActive(true);
        img2.gameObject.SetActive(true);

        img1.texture = step.texture;
        img2.texture = step.texture2;

        img1.rectTransform.localPosition = step.startPos;
        img2.rectTransform.localPosition = step.startPos2;

        Color c1 = img1.color;
        Color c2 = img2.color;
        c1.a = 0f;
        c2.a = 0f;
        img1.color = c1;
        img2.color = c2;

        float time = 0f;

        while (time < step.duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / step.duration);

            img1.rectTransform.localPosition = Vector3.Lerp(step.startPos, step.endPos, t);
            img2.rectTransform.localPosition = Vector3.Lerp(step.startPos2, step.endPos2, t);

            c1.a = Mathf.Lerp(0f, 1f, t);
            c2.a = Mathf.Lerp(0f, 1f, t);
            img1.color = c1;
            img2.color = c2;

            yield return null;
        }

        img1.rectTransform.localPosition = step.endPos;
        img2.rectTransform.localPosition = step.endPos2;

        c1.a = 1f;
        c2.a = 1f;
        img1.color = c1;
        img2.color = c2;

        inputLocked = false;
    }

    private void HideAllStepImages()
    {
        foreach (SequenceStep step in steps)
        {
            if (step.targetImage != null)
                step.targetImage.gameObject.SetActive(false);

            if (step.targetImage2 != null)
                step.targetImage2.gameObject.SetActive(false);
        }
    }

    private void DestroyStepVisuals(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= steps.Count) return;

        SequenceStep oldStep = steps[stepIndex];

        if (oldStep.targetImage != null)
        {
            Destroy(oldStep.targetImage.gameObject);
            oldStep.targetImage = null;
        }

        if (oldStep.targetImage2 != null)
        {
            if (oldStep.targetImage2 != oldStep.targetImage)
                Destroy(oldStep.targetImage2.gameObject);

            oldStep.targetImage2 = null;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class StarDisplayUI : MonoBehaviour
{
    public Image[] brownStars;
    public Image[] yellowStars;
    private int currentDisplayedStars = 0;

    private void Awake()
    {
        if (brownStars == null) brownStars = new Image[0];
        if (yellowStars == null) yellowStars = new Image[0];
        ResetStarLayers();
    }

    private void ResetStarLayers()
    {
        for (int i = 0; i < brownStars.Length; i++)
        {
            if (brownStars[i] == null) continue;
            brownStars[i].gameObject.SetActive(true);
            brownStars[i].enabled = true;
            brownStars[i].canvasRenderer.SetAlpha(1f);
        }

        for (int i = 0; i < yellowStars.Length; i++)
        {
            if (yellowStars[i] == null) continue;
            yellowStars[i].gameObject.SetActive(false);
            yellowStars[i].enabled = false;
            yellowStars[i].canvasRenderer.SetAlpha(0f);
        }

        currentDisplayedStars = 0;
    }

    public void UpdateStarDisplay(int earnedStars)
    {
        earnedStars = Mathf.Clamp(earnedStars, 0, yellowStars.Length);
        Debug.Log($"StarDisplayUI.UpdateStarDisplay called: earnedStars={earnedStars}");
        AlignYellowStarsToBrownStars();

        for (int i = 0; i < yellowStars.Length; i++)
        {
            if (yellowStars[i] == null)
                continue;

            bool shouldShow = i < earnedStars;
            yellowStars[i].gameObject.SetActive(shouldShow);
            yellowStars[i].enabled = shouldShow;
            yellowStars[i].canvasRenderer.SetAlpha(shouldShow ? 1f : 0f);
            yellowStars[i].color = Color.white;

            if (shouldShow)
            {
                yellowStars[i].transform.SetAsLastSibling();
            }
        }

        currentDisplayedStars = earnedStars;
    }

    private void AlignYellowStarsToBrownStars()
    {
        int count = Mathf.Min(brownStars.Length, yellowStars.Length);

        for (int i = 0; i < count; i++)
        {
            if (brownStars[i] == null || yellowStars[i] == null)
                continue;

            RectTransform brownRect = brownStars[i].rectTransform;
            RectTransform yellowRect = yellowStars[i].rectTransform;

            yellowRect.SetParent(brownRect.parent, false);
            yellowRect.anchorMin = brownRect.anchorMin;
            yellowRect.anchorMax = brownRect.anchorMax;
            yellowRect.pivot = brownRect.pivot;
            yellowRect.anchoredPosition = brownRect.anchoredPosition;
            yellowRect.sizeDelta = brownRect.sizeDelta;
            yellowRect.localScale = brownRect.localScale;
            yellowRect.localRotation = brownRect.localRotation;
            yellowRect.SetSiblingIndex(Mathf.Min(brownRect.GetSiblingIndex() + 1, yellowRect.parent.childCount - 1));
        }
    }

    public void ResetStarDisplay()
    {
        if (yellowStars == null) return;

        for (int i = 0; i < yellowStars.Length; i++)
        {
            if (yellowStars[i] == null) continue;
            yellowStars[i].gameObject.SetActive(false);
            yellowStars[i].enabled = false;
            yellowStars[i].canvasRenderer.SetAlpha(0f);
        }

        currentDisplayedStars = 0;
    }
}
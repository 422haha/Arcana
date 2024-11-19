using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardCombinedEffect : MonoBehaviour
{
    [System.Serializable]
    public class OutlineSettings
    {
        public Color outlineColorInner = new Color(1f, 1f, 1f, 0.9f);
        public Color outlineColorOuter = new Color(1f, 1f, 1f, 0f);
        [Range(0.1f, 2f)] public float animationSpeed = 1f;
        public float outlineThickness = 4f;
        public float innerThickness = 1f;
        [Range(0f, 1f)] public float minBrightness = 0.3f;
        [Range(0f, 1f)] public float maxBrightness = 1f;
        [Range(1f, 3f)] public float glowPower = 1.5f;
    }

    [System.Serializable]
    public class WaveSettings
    {
        public Color waveColor = new Color(0.3f, 0.7f, 1f, 0.5f);
        [Range(0.1f, 2f)] public float animationSpeed = 1f;
        public float waveThickness = 10f;
        public float waveSpacing = 5f;
    }

    public OutlineSettings outlineSettings = new OutlineSettings();
    public WaveSettings waveSettings = new WaveSettings();

    private Image outlineImage;
    private Image innerGlowImage;
    private Image[] waveEffectImages;
    private RectTransform cardRect;

    private const int WAVE_COUNT = 3;

    private void Awake()
    {
        cardRect = GetComponent<RectTransform>();
        SetupOutlineEffect();
        SetupWaveEffect();
    }

    private void SetupOutlineEffect()
    {
        outlineImage = CreateEffectObject("CardOutlineGlow").GetComponent<Image>();
        outlineImage.sprite = CreateOutlineSprite(true);
        outlineImage.color = outlineSettings.outlineColorOuter;
        FitRectTransform(outlineImage.rectTransform, Vector2.zero);

        innerGlowImage = CreateEffectObject("CardOutlineInner").GetComponent<Image>();
        innerGlowImage.sprite = CreateOutlineSprite(false);
        innerGlowImage.color = outlineSettings.outlineColorInner;
        FitRectTransform(innerGlowImage.rectTransform, Vector2.zero);
    }

    private void SetupWaveEffect()
    {
        waveEffectImages = new Image[WAVE_COUNT];

        for (int i = 0; i < WAVE_COUNT; i++)
        {
            Image waveImage = CreateEffectObject($"WaveEffect_{i}").GetComponent<Image>();
            waveImage.sprite = CreateWaveSprite();
            waveImage.color = waveSettings.waveColor;
            FitRectTransform(waveImage.rectTransform, Vector2.zero);
            waveEffectImages[i] = waveImage;
        }

        PlayAllAnimations();
    }

    private GameObject CreateEffectObject(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform, false);
        Image image = obj.AddComponent<Image>();
        image.material = new Material(Shader.Find("UI/Default"));
        return obj;
    }

    private void FitRectTransform(RectTransform rect, Vector2 padding)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = -padding * 2f;
        rect.anchoredPosition = Vector2.zero;
    }

    private Sprite CreateOutlineSprite(bool isOuter)
    {
        int width = 256;
        int height = (int)(width * (1050f / 750f));
        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distanceToEdge = GetDistanceToEdge(x, y, width, height);
                float alpha = 0f;

                if (isOuter)
                {
                    if (distanceToEdge <= outlineSettings.outlineThickness)
                    {
                        float normalizedDistance = distanceToEdge / outlineSettings.outlineThickness;
                        alpha = 1f - Mathf.Pow(normalizedDistance, outlineSettings.glowPower);
                    }
                }
                else
                {
                    if (distanceToEdge <= outlineSettings.innerThickness)
                    {
                        alpha = 1f - (distanceToEdge / outlineSettings.innerThickness);
                        alpha = Mathf.Pow(alpha, 0.5f);
                    }
                }

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    private Sprite CreateWaveSprite()
    {
        int width = 256;
        int height = (int)(width * (1050f / 750f));
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        float maxDistance = waveSettings.waveThickness * WAVE_COUNT + waveSettings.waveSpacing * (WAVE_COUNT - 1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distanceToEdge = GetDistanceToEdge(x, y, width, height);
                float alpha = 0f;

                if (distanceToEdge >= 0 && distanceToEdge <= maxDistance)
                {
                    float cumulativeAlpha = 0f;
                    for (int i = 0; i < WAVE_COUNT; i++)
                    {
                        float waveStart = i * (waveSettings.waveThickness + waveSettings.waveSpacing);
                        float waveEnd = waveStart + waveSettings.waveThickness;

                        if (distanceToEdge >= waveStart && distanceToEdge <= waveEnd)
                        {
                            float t = (distanceToEdge - waveStart) / waveSettings.waveThickness;
                            float waveAlpha = 1f - t;
                            waveAlpha = Mathf.Pow(waveAlpha, 2f);
                            cumulativeAlpha += waveAlpha;
                        }
                    }
                    alpha = Mathf.Clamp01(cumulativeAlpha);
                }

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    private float GetDistanceToEdge(int x, int y, int width, int height)
    {
        float left = x;
        float right = width - x - 1;
        float top = height - y - 1;
        float bottom = y;
        return Mathf.Min(left, right, top, bottom);
    }

    private void PlayAllAnimations()
    {
        // 아웃라인 애니메이션
        Sequence outerSequence = DOTween.Sequence();
        outerSequence.Append(
            DOTween.To(() => outlineSettings.minBrightness,
                (value) => {
                    Color newColor = outlineSettings.outlineColorOuter;
                    newColor.a = value * 0.5f;
                    outlineImage.color = newColor;
                },
                outlineSettings.maxBrightness,
                1f * outlineSettings.animationSpeed)
            .SetEase(Ease.InOutSine)
        );
        outerSequence.Append(
            DOTween.To(() => outlineSettings.maxBrightness,
                (value) => {
                    Color newColor = outlineSettings.outlineColorOuter;
                    newColor.a = value * 0.5f;
                    outlineImage.color = newColor;
                },
                outlineSettings.minBrightness,
                1f * outlineSettings.animationSpeed)
            .SetEase(Ease.InOutSine)
        );
        outerSequence.SetLoops(-1, LoopType.Restart);

        Sequence innerSequence = DOTween.Sequence();
        innerSequence.Append(
            DOTween.To(() => outlineSettings.minBrightness,
                (value) => {
                    Color newColor = outlineSettings.outlineColorInner;
                    newColor.a = value;
                    innerGlowImage.color = newColor;
                },
                outlineSettings.maxBrightness,
                1f * outlineSettings.animationSpeed)
            .SetEase(Ease.InOutSine)
        );
        innerSequence.Append(
            DOTween.To(() => outlineSettings.maxBrightness,
                (value) => {
                    Color newColor = outlineSettings.outlineColorInner;
                    newColor.a = value;
                    innerGlowImage.color = newColor;
                },
                outlineSettings.minBrightness,
                1f * outlineSettings.animationSpeed)
            .SetEase(Ease.InOutSine)
        );
        innerSequence.SetLoops(-1, LoopType.Restart);

        // 파동 애니메이션
        float totalDuration = 1.5f / waveSettings.animationSpeed;

        for (int i = 0; i < WAVE_COUNT; i++)
        {
            Image waveImage = waveEffectImages[i];
            waveImage.color = new Color(waveSettings.waveColor.r, waveSettings.waveColor.g, waveSettings.waveColor.b, 0f);

            Sequence waveSequence = DOTween.Sequence();
            waveSequence.AppendInterval(i * (totalDuration / WAVE_COUNT));
            waveSequence.AppendCallback(() => {
                waveImage.transform.localScale = Vector3.one;
                waveImage.color = new Color(waveSettings.waveColor.r, waveSettings.waveColor.g, waveSettings.waveColor.b, 1f);
            });
            waveSequence.Append(
                waveImage.DOFade(0f, totalDuration)
                         .SetEase(Ease.OutQuad)
            );
            waveSequence.Join(
                waveImage.transform.DOScale(1.3f, totalDuration)
                         .SetEase(Ease.Linear)
            );
            waveSequence.SetLoops(-1, LoopType.Restart);
        }
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
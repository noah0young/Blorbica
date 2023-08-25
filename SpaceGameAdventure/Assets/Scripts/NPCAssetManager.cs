using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCAssetManager : MonoBehaviour
{
    public static NPCAssetManager instance { get; private set; }

    [Header("SFX")]
    [SerializeField] private AudioClip defaultSFX;
    [SerializeField] private AudioClip[] sfx;
    [SerializeField] private string[] sfxNames;

    [Header("Fonts")]
    [SerializeField] private TMP_FontAsset defaultFont;
    [SerializeField] private TMP_FontAsset[] fonts;
    [SerializeField] private string[] fontNames;

    [Header("Color")]
    [SerializeField] private Color[] colors;
    [SerializeField] private string[] colorNames;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip GetSound(string id)
    {
        Debug.Log("id = " + id);
        if (id == null)
        {
            return defaultSFX;
        }
        for (int i = 0; i < sfx.Length; i++)
        {
            if (id == sfxNames[i])
            {
                Debug.Log("Found something");
                return sfx[i];
            }
        }
        Debug.Log("Found nothing");
        return null;
    }

    public TMP_FontAsset GetFont(string id)
    {
        if (id == null)
        {
            return defaultFont;
        }
        for (int i = 0; i < fonts.Length; i++)
        {
            if (id == fontNames[i])
            {
                return fonts[i];
            }
        }
        return null;
    }

    public Color GetColor(string id)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            if (id == colorNames[i])
            {
                return colors[i];
            }
        }
        return Color.black;
    }
}

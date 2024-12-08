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

    [Header("Evidence")]
    [SerializeField] private BasicThought[] evidence;
    [SerializeField] private string[] evidenceNames;

    [Header("Sprites")]
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private string[] spriteNames;

    private void Awake()
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
        if (id == null)
        {
            return defaultSFX;
        }
        for (int i = 0; i < sfx.Length; i++)
        {
            if (id == sfxNames[i])
            {
                return sfx[i];
            }
        }
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

    public Thought GetEvidence(string id)
    {
        for (int i = 0; i < evidence.Length; i++)
        {
            if (id == evidenceNames[i])
            {
                return evidence[i];
            }
        }
        throw new System.Exception("No Evidence Exists for that ID");
    }

    public Sprite GetSprite(string id)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            if (id == spriteNames[i])
            {
                return sprites[i];
            }
        }
        throw new System.Exception("No Sprite Exists for that ID, id = " + id);
    }
}

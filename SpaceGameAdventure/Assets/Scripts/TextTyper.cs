using UnityEngine;
using TMPro;

public abstract class TextTyper : MonoBehaviour
{
    public abstract void TypeText(TMP_Text textBox, string text);

    public abstract bool NotTyping();

    public abstract void ForceTypeAll();

    public abstract void SetSound(AudioClip sfx);

    public abstract void SetFont(TMP_FontAsset font);

    public abstract void SetFontSize(float size);

    public abstract void SetTextColor(Color color);
}

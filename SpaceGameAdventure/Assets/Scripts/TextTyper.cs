using UnityEngine;
using TMPro;

public abstract class TextTyper : MonoBehaviour
{
    public abstract void TypeText(TMP_Text textBox, string text);

    public abstract bool NotTyping();

    public abstract void ForceTypeAll();
}

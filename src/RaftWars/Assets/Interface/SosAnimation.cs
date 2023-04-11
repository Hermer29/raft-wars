using TMPro;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class SosAnimation : MonoBehaviour
{
    private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>(); 
        StartCoroutine(Show());
    }

    private IEnumerator Show()
    {
        while(true)
        {
            _text.text = "s";
            yield return new WaitForSeconds(.3f);
            _text.text = "so";
            yield return new WaitForSeconds(.3f);
            _text.text = "sos";
            yield return new WaitForSeconds(.3f);
        }
    }
}
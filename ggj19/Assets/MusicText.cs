using System.Collections;
using System.Collections.Generic;
using BrutalHack.ggj19.Music;
using MusicXml.Domain;
using TMPro;
using UnityEngine;

public class MusicText : MonoBehaviour
{
    private TMP_Text textMeshPro;
    public MusicController musicController;

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        textMeshPro.text = "";
        musicController.OnSnare += OnSnare;
        musicController.OnBass += OnBass;
    }

    private void OnSnare(TimedNote note)
    {
        textMeshPro.text = "Snare";
        StartCoroutine(ClearTextAfterDelay());
    }
    
    private void OnBass(TimedNote note)
    {
        textMeshPro.text = "Bass";
        StartCoroutine(ClearTextAfterDelay());
    }

    private IEnumerator ClearTextAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        textMeshPro.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

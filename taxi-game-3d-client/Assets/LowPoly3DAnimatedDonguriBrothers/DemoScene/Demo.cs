using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public AnimationClip[] animatorClip;
    Button button;
    int index;
    public Animator[] animator;

    void Start()
    { 
        button = GameObject.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(Push);
    }

    void Push() {
        index++;
        if (index >= animatorClip.Length) {
            index = 0;
        }
        string animationName = animatorClip[index].ToString().Substring(0,animatorClip[index].ToString().IndexOf(" "));
        foreach(Animator m in animator) {
            m.CrossFadeInFixedTime(animationName, 0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

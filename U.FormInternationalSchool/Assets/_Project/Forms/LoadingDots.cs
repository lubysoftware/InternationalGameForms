using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using Image = UnityEngine.UI.Image;

public class LoadingDots : MonoBehaviour
{

    //the total time of the animation
    public float repeatTime = 1;

    //the time for a dot to bounce up and come back down
    public float bounceTime = 0.25f;

    //how far does each dot move
    public float bounceHeight = 10;

    public List<GameObject> dots;

    void Start()
    {
        if (repeatTime < dots.Count * bounceTime)
        {
            repeatTime = dots.Count * bounceTime;
        }

        InvokeRepeating("Animate", 0, repeatTime);
    }

    void Animate()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            int dotIndex = i;

            dots[dotIndex].transform
                .DOMoveY(dots[dotIndex].transform.position.y + bounceHeight, bounceTime / 2)
                .SetDelay(dotIndex * bounceTime / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    dots[dotIndex].transform
                        .DOMoveY(dots[dotIndex].transform.position.y - bounceHeight, bounceTime / 2)
                        .SetEase(Ease.InQuad);
                });
        }
    }

    public void SetColors(Color mainColor, Color secondColor)
    {
        for(int i=0; i < dots.Count; i++)
        {
            dots[i].GetComponent<Image>().color = i % 2 == 0 ? mainColor : secondColor;
        }
    }
}

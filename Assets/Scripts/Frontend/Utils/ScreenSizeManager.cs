using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeManager : MonoBehaviour
{
    public RectTransform commentAndPreviousWin;
    public float widthSize,offset;
    void Start()
    {
        if (Screen.width == 800 && Screen.height == 600)
        {
            Debug.Log("small screen detected");
            float x = commentAndPreviousWin.rect.x;
            float y = commentAndPreviousWin.rect.y;
            float height = commentAndPreviousWin.rect.height;
            //commentAndPreviousWin.rect.Set(x,y , widthSize, height);
            commentAndPreviousWin.sizeDelta = new Vector2(widthSize, height);
            commentAndPreviousWin.anchoredPosition = new Vector2(-widthSize/2, y+offset);
            //commentAndPreviousWin.anchoredPosition=
        }
    }

    
}

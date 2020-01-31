using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChangeAnimation : MonoBehaviour
{
    public bool isUI = false;
    public SpriteRenderer targetSprite;
    public Image targetImage;
    public Sprite[] sprites;

    public void ChangeSprite(int index)
    {
        if(!isUI)   targetSprite.sprite = sprites[index];
        else        targetImage.sprite = sprites[index];
    }
}

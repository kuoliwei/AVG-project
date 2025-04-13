using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TestDOTween : MonoBehaviour
{
    public Image testImage;

    void Start()
    {
        // 測試將圖片淡入
        testImage.color = new Color(1, 1, 1, 0);
        testImage.DOFade(1f, 1f);
    }
}

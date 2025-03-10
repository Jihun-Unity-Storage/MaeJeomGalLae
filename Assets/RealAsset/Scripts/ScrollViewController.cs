using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    public ScrollRect scrollView;
    public Image targetImage;

    void Start()
    {
        scrollView.onValueChanged.AddListener(OnScroll);
    }

    void OnScroll(Vector2 value)
    {
        // 스크롤 뷰가 위로 스크롤될 때 이미지 투명도를 조정합니다.
        float alpha = scrollView.verticalNormalizedPosition;
        targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, alpha);
    }
    private void Update()
    {
        //Debug.Log(scrollView.verticalNormalizedPosition);
    }
}

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
        // ��ũ�� �䰡 ���� ��ũ�ѵ� �� �̹��� ������ �����մϴ�.
        float alpha = scrollView.verticalNormalizedPosition;
        targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, alpha);
    }
    private void Update()
    {
        //Debug.Log(scrollView.verticalNormalizedPosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Community_off : MonoBehaviour
{
    private Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    public void CommunityOff()
    {
        _animator.SetTrigger("off");
        StartCoroutine(DeactivateAfterDelay(0.3f));
    }
    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}

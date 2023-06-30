using System.Collections;
using UnityEngine;

public class SpearTrap : MonoBehaviour
{
    private Animator _spearTrapAnim;

    private void Awake()
    {
        _spearTrapAnim = GetComponent<Animator>();
        StartCoroutine(OpenCloseTrap());
    }

    private IEnumerator OpenCloseTrap()
    {
        while(true)
        {
            _spearTrapAnim.SetTrigger("open");
            yield return new WaitForSeconds(2);
            _spearTrapAnim.SetTrigger("close");
            yield return new WaitForSeconds(2);
        }
    }
}
using System.Collections;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(DanceLoop());
    }

    IEnumerator DanceLoop()
    {
        while (true)
        {

            yield return new WaitForSeconds(Random.Range(8f,16f));
            animator.SetTrigger(Random.Range(1, 17) + "");

        }
    }

}

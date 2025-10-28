using System.Collections;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    Animator animator;
    public int seed = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (seed != 0)
        {
            Random.InitState(seed);
        }

        animator = GetComponent<Animator>();
        StartCoroutine(DanceLoop());
    }

    IEnumerator DanceLoop()
    {
        while (true)
        {

            yield return new WaitForSeconds(Random.Range(5f,12f));
            animator.SetTrigger(Random.Range(0, 7) + "");

        }
    }

}

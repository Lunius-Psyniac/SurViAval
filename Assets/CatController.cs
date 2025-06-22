using System.Collections;
using UnityEngine;

public class CatController : MonoBehaviour
{
    private Animator animator;

    // You can change these values in the Inspector to tweak the cat's behavior
    public float minIdleTime = 5.0f;
    public float maxIdleTime = 12.0f;

    // -1 = none, 0 = sit, 1 = stretch
    private int lastAnimation = -1; 

    void Start()
    {
        animator = GetComponent<Animator>();
        // Start the automatic animation loop
        StartCoroutine(CatAnimationRoutine());
    }

    private IEnumerator CatAnimationRoutine()
    {
        // This loop will run forever
        while (true)
        {
            // Wait for a random amount of time in the Idle state
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);

            int choice = Random.Range(0, 2);
            // If the new choice is the same as the last animation, flip it
            if(choice == lastAnimation)
            {
                choice = 1 - choice; // This will flip 0 to 1 and 1 to 0
            }

            if (choice == 0)
            {
                animator.SetTrigger("PlaySit");
                Debug.Log("Cat is sitting.");
            }
            else
            {
                animator.SetTrigger("PlayStretch");
                Debug.Log("Cat is stretching.");
            }
            
            lastAnimation = choice; // Remember what we just played

            // Wait a single frame to allow the animator to start the transition out of Idle
            yield return null;

            // Wait until the animator has finished the entire sequence and returned to Idle,
            // AND is no longer in a transition. This prevents starting the next timer too early.
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !animator.IsInTransition(0));
            Debug.Log("Cat returned to Idle.");
        }
    }
} 
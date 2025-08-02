using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class NPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator up(int n)
    {
        float start = transform.position.y;
        animator.SetInteger("Direction", 1);
        animator.SetBool("Walking", true);
        while (transform.position.y < start+n)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.up;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Walking", false);
    }
    
    public IEnumerator down(int n)
    {
        float start = transform.position.y;
        animator.SetInteger("Direction", 3);
        animator.SetBool("Walking", true);
        while (transform.position.y > start - n)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.down;
            yield return new WaitForEndOfFrame();

        }
        animator.SetBool("Walking", false);
    }
    public IEnumerator right(int n)
    {
        float start = transform.position.x;
        animator.SetInteger("Direction", 2);
        animator.SetBool("Walking", true);
        while (transform.position.x < start + n)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.right;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Walking", false);
    }
    public IEnumerator left(int n)
    {
        float start = transform.position.x;
        animator.SetInteger("Direction", 4);
        animator.SetBool("Walking", true);
        while (transform.position.x > start - n)
        {
            transform.position += moveSpeed * Time.deltaTime * Vector3.left;
            yield return new WaitForEndOfFrame();
        }
        animator.SetBool("Walking", false);
    }
}

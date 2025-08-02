using System.Collections;
using UnityEngine;

public class testNPC : MonoBehaviour
{
    private NPC me;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(path());
    }

    IEnumerator path ()
    {
        me = gameObject.GetComponent<NPC>();
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1);
            yield return me.up(3);
            yield return new WaitForSeconds(1);
            yield return me.right(3);
            yield return new WaitForSeconds(1);
            yield return me.down(3);
            yield return new WaitForSeconds(1);
            yield return me.left(3);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    
}

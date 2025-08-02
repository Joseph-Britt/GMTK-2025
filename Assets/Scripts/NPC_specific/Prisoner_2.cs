using System.Collections;
using UnityEngine;

public class Prisoner_2 : MonoBehaviour
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
        yield return me.left(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    
}

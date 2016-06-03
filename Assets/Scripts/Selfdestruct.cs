using UnityEngine;
using System.Collections;

public class Selfdestruct : MonoBehaviour
{
    public float lifetime;
	void Start ()
    {
        StartCoroutine(Destroy());
	}
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifetime);
        GameObject.Destroy(gameObject);
    }
}
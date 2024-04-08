using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointOnOff : MonoBehaviour
{
    public void CloseCP()
    {
        StartCoroutine(CPOnOff());
    }

    IEnumerator CPOnOff()
    {
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider>().enabled = true;
    }
}

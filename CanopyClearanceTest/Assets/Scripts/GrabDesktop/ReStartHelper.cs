using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ILazy
{
    void LazyMethod(bool activated);
}

public class ReStartHelper : MonoBehaviour
{
    public void Lazy(ILazy lazyObject, float time)
    {
        StartCoroutine(LazyInit(lazyObject, time));
    }

    internal IEnumerator LazyInit(ILazy lazyObject, float time)
    {
        lazyObject.LazyMethod(false);
        yield return new WaitForSeconds(time);
        lazyObject.LazyMethod(true);
    }

}

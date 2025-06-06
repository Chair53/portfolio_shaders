using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveOverTime : MonoBehaviour
{
    [SerializeField] Material mat;
    float dissolveAmt = 0f;
    float mult = 1f;
    void Update()
    {
        mat.SetFloat("_DissolveValue", dissolveAmt);
        dissolveAmt += Time.deltaTime * mult;
        if (dissolveAmt >= 1f || dissolveAmt <= 0f)
            mult *= -1f;
        dissolveAmt = Mathf.Clamp01(dissolveAmt);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mudkip : MonoBehaviour
{
    [SerializeField] float low;
    [SerializeField] float high;
    [SerializeField] float spd = 1;

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y += spd * Time.deltaTime;
        if (pos.y >= high || pos.y <= low)
            spd *= -1f;
        transform.position = pos;
    }
}

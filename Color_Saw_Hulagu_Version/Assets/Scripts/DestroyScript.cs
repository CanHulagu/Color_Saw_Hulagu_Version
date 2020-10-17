using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 2f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    private Stack<GameObject> knightStack = new Stack<GameObject>();

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Knight"))
        {
            knightStack.Push(other.gameObject);
            other.transform.SetParent(transform);
            other.transform.position = transform.position + new Vector3(0, knightStack.Count);
        }
    }
}

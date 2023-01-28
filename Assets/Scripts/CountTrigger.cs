using UnityEngine;

public class CountTrigger : MonoBehaviour
{
    private int counter = 0;

    private void OnTriggerEnter(Collider other)
    {
        ++counter;
        print($"{counter}. This was counted: {other.name} at {other.transform.position}");
    }
}

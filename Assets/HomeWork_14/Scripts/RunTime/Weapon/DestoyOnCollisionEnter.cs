using UnityEngine;

public class DestoyOnCollisionEnter : MonoBehaviour
{
    [SerializeField] private float _delay;

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, _delay);
    }
}

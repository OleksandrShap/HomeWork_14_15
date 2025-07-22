using UnityEngine;

public class DestroyWithDelay : MonoBehaviour
{
    [SerializeField] private float _delay;

    private void Awake()
    {
        Destroy(gameObject, _delay);
    }
}

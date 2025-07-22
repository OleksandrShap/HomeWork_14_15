using UnityEngine;

public class UnparentOnColilisionEnter : MonoBehaviour
{
    [SerializeField] private Transform _target;
    
    private void OnCollisionEnter(Collision collision)
    {
        //if (collision == null) return;

        _target.parent = null;
        Destroy(_target.gameObject, 1.5f);
    }
}

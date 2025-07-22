using UnityEngine;

public class SpawnDecal : MonoBehaviour
{
    [SerializeField] private GameObject _prefabDecal;
    [SerializeField] private bool _invertNormal;

    private void OnCollisionEnter(Collision collision)
    {
        var contactPoint = collision.contacts[0];
        var rotation = Quaternion.LookRotation(_invertNormal ? -contactPoint.normal: contactPoint.normal);
        var instance = Instantiate(_prefabDecal, contactPoint.point, rotation);
    }
}

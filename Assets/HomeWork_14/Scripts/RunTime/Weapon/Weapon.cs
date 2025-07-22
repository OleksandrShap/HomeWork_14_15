using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Rigidbody _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private Transform _weaponPosition;
    

    public void Shoot()
    {
        var screenCenterPoint = new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f);
        var ray = Camera.main.ScreenPointToRay (screenCenterPoint);
                       
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            var bulletDirection = (hit.point - _bulletSpawnPoint.position).normalized;
            var bulletRotation = Quaternion.LookRotation(bulletDirection);
            var bullletInstance = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, bulletRotation);
            bullletInstance.AddForce(bulletDirection * _bulletSpeed, ForceMode.Impulse);
        }
                
    }
}

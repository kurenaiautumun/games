using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public Button button;
    public float bulletSpeed = 10f;

    void Update()
    {
        // Existing logic for spacebar shooting
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Shoot"))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Your shooting logic here
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
    }

    public void ShootFromButton()
    {
        // Call the existing shoot method when the button is clicked
        Shoot();
    }

    void Start()
    {
        // Add an onClick listener to the button to call ShootFromButton
        button.onClick.AddListener(ShootFromButton);
    }
}
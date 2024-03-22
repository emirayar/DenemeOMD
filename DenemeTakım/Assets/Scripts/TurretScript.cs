using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public float Range;
    public Transform Target;
    bool Detected = false;

    Vector2 Direction;
    public GameObject Namlu;
    public GameObject Bullet;
    public float FireRate;
    public float NextTimeToShoot = 0;
    public Transform ShootPoint;
    public float Force;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetpos = Target.position;
        Direction = targetpos-(Vector2)transform.position;
        RaycastHit2D rayinfo = Physics2D.Raycast(transform.position, Direction, Range);

        if (rayinfo)
        {   
            if (rayinfo.collider.gameObject.tag == "Player")
            {
                if (Detected == false)
                {
                    Detected = true;
                }
            }

        
            else 
           {
                if (Detected == true)
                {
                    Detected = false;

                }
           }
            
        }
        if (Detected)
        {
            Namlu.transform.up = Direction;
            if(Time.time >NextTimeToShoot)
            {
                NextTimeToShoot =Time.time+1/FireRate;
                Shoot();
            }
        }
    }
    void Shoot()
    {
       GameObject BulletIns = Instantiate(Bullet, ShootPoint.position, Quaternion.identity);
        BulletIns.GetComponent<Rigidbody2D>().AddForce(Direction * Force);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
    
}

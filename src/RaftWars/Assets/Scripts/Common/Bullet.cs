using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 target;
    private Vector3 dir;

    private float timer = 0;
    public float speed = 10;

    public GameObject explosion;

    public void SetPrefs(Vector3 target)
    {
        this.target = target;
        this.target += new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        dir = this.target - transform.position;
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        if (timer > Random.Range(0.2f, 0.6f))
        {
            GameObject _exp = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(_exp, 2);
            Destroy(gameObject);
        }
        else
            timer += Time.deltaTime;
    }


}

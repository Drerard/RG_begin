using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBody : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //IDamageable target = collision.collider.GetComponentInParent<IDamageable>();
        //if (target!=null)
        //{
        //    target.RecieveDamage(5);
        //    Debug.Log("ﬂ ¿“¿ Œ¬¿À »√–Œ ¿");
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints; //kýrýlma seviyesi 
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    public void Start()
    {
        if (hitPoints <= 0)
        {

            Destroy(this.gameObject);

        }
    }
    public void TakeDamge(int Damage)
    {
        hitPoints-=Damage;
        MakeRighter();
    }
    void MakeRighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * .5f;
        sprite.color = new Color(color.r,color.g,color.b,newAlpha);
    }
}

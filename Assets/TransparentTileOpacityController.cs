using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentTileOpacityController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private MeshRenderer tileRenderer;
    [SerializeField] private float distantToPlayer;
    [SerializeField] private float alpha;
    [SerializeField] private float maxOpacity = 0.5f;
    [SerializeField] private float distantToHide = 0.5f;
    [SerializeField] private float distantToShow = 4.5f;
    
    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        tileRenderer = GetComponent<MeshRenderer>();
        alpha = 0f;
    }

    private void Update()
    {
        distantToPlayer = Vector3.Distance(transform.localPosition, player.transform.GetChild(1).transform.localPosition);
        
        if (distantToPlayer < distantToShow)
        {
            gameObject.layer = 0;
            alpha = Mathf.InverseLerp(distantToShow, distantToHide, distantToPlayer) * maxOpacity;
        }
        if (distantToPlayer > distantToShow)
        {
            gameObject.layer = 3;
            alpha = 0f;
        }
        
        tileRenderer.material.color = new Color(tileRenderer.material.color.r, tileRenderer.material.color.g, tileRenderer.material.color.b, alpha);
    }
}

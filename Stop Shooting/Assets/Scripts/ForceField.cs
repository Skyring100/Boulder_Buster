using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private bool isCountDown;
    private Renderer render;
    private float matTransparency;
    [SerializeField] private float disappearAmount = 0.25f;
    private GameObject player;
    private Color matColor;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isCountDown)
        {
            matColor = render.material.color; 
            matTransparency -= disappearAmount;
            render.material.color =  new Color(matColor.r,matColor.g,matColor.b, matTransparency);
            if (matTransparency <= 0)
            {
                isCountDown = false;
                player.GetComponent<PlayerController>().TimeRunOut();
                gameObject.SetActive(false);
            }
        }
    }
    public void Begin()
    {
        render = gameObject.GetComponent<Renderer>();
        matTransparency = 1;
        matColor = render.material.color;
        render.material.color = new Color(matColor.r, matColor.g, matColor.b, matTransparency);
        isCountDown = false;
        player = GameObject.Find("Player");
    }
    public void TimeRunningOut()
    {
        isCountDown = true;
    }
}

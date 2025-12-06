using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public PlayerClass currentClass;
    public float speed = 50;
    public Rigidbody2D rb;

    void Start()
    {
        if(currentClass == null)
    {
            Debug.LogError("Player has no class assigned!");
            return;
        }
        Debug.Log("Player class = " + currentClass.className);
    }
    private void Update()
    {
        

    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
        currentClass.PrimaryAbility(gameObject);
        FindAnyObjectByType<HotbarUI>()?.HighlightButton(1);
    }
    if (Input.GetKeyDown(KeyCode.Alpha2))
    {
        currentClass.SecondaryAbility(gameObject);
        FindAnyObjectByType<HotbarUI>()?.HighlightButton(2);
    }
    if (Input.GetKeyDown(KeyCode.Alpha3))
    {
        currentClass.UltimateAbility(gameObject);
        FindAnyObjectByType<HotbarUI>()?.HighlightButton(3);
    }


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;
    }
}

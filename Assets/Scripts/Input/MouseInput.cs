using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    public static event Action<Vector2> OnRightClick;
    public static event Action<IDamagable> OnRightClickTarget;

    void Update()
    {
        OnMouseRightClick();
    }

    void OnMouseRightClick()
    {
       if (Input.GetMouseButtonDown(1)) // right mouse
        {
            if (Camera.main == null) return;

            Vector3 mouseScreenPos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            // check for a 2D collider at the click point
            Collider2D col = Physics2D.OverlapPoint((Vector2)mouseWorldPos);
            if (col != null)
            {
                var dam = col.GetComponent<IDamagable>();
                if (dam != null)
                {
                    OnRightClickTarget?.Invoke(dam);
                    return;
                }
            }

            OnRightClick?.Invoke((Vector2)mouseWorldPos);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reaching : MonoBehaviour
{
    public Vector3 mouseTarget;

    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        reachTarget();
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseTarget = Camera.main.ScreenToWorldPoint(mousePos);
        reachTarget();*/
    }

    private void reachTarget()
    {
        int size = lineRenderer.positionCount;

        if (size > 1)
        {
            Vector3 target = mouseTarget;
            Debug.Log(target.x);
            Debug.Log(target.y);
            Debug.Log(target.z);
            Vector3 root = lineRenderer.GetPosition(0);
            Vector3 oldPos = lineRenderer.GetPosition(size - 1);
            lineRenderer.SetPosition(size - 1, target);

            for (int i = size - 2; i >= 0; i--)
            {
                float distance = Vector3.Distance(lineRenderer.GetPosition(i), oldPos);
                //Debug.Log(distance);
                oldPos = lineRenderer.GetPosition(i);
                Vector3 newPos = target + distance * Vector3.Normalize(lineRenderer.GetPosition(i) - target);
                
                lineRenderer.SetPosition(i, newPos);
                target = lineRenderer.GetPosition(i);
                //Debug.Log(Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i+1)));
            }

            oldPos = lineRenderer.GetPosition(0);
            lineRenderer.SetPosition(0, root);
            for (int i = 1; i < size; i++)
            {
                float distance = Vector3.Distance(lineRenderer.GetPosition(i), oldPos);
                //Debug.Log(distance);
                oldPos = lineRenderer.GetPosition(i);
                Vector3 newPos = root + distance * Vector3.Normalize(lineRenderer.GetPosition(i) - root);

                lineRenderer.SetPosition(i, newPos);
                root = lineRenderer.GetPosition(i);
                //Debug.Log(Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i+1)));
            }
        }
    }
}

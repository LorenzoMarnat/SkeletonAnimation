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
    }

    // Update is called once per frame
    void Update()
    {
        reachTarget();
    }

    private void reachTarget()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseTarget = new Vector3(mousePosition.x, mousePosition.y, 0);

        int size = lineRenderer.positionCount;

        if (size > 1)
        {
            Vector3 target = mouseTarget;
            Vector3 root = lineRenderer.GetPosition(0);

            Vector3 oldPos = lineRenderer.GetPosition(size - 1);
            lineRenderer.SetPosition(size - 1, target);

            for (int i = size - 2; i >= 0; i--)
            {
                float distance = Vector3.Distance(lineRenderer.GetPosition(i), oldPos);

                oldPos = lineRenderer.GetPosition(i);

                Vector3 newPos = target + distance * Vector3.Normalize(oldPos - target);
                
                lineRenderer.SetPosition(i, newPos);

                target = lineRenderer.GetPosition(i);
            }

            oldPos = lineRenderer.GetPosition(0);
            lineRenderer.SetPosition(0, root);

            for (int i = 1; i < size; i++)
            {
                float distance = Vector3.Distance(lineRenderer.GetPosition(i), oldPos);

                oldPos = lineRenderer.GetPosition(i);

                Vector3 newPos = root + distance * Vector3.Normalize(oldPos - root);

                lineRenderer.SetPosition(i, newPos);

                root = lineRenderer.GetPosition(i);
            }
        }
    }
}

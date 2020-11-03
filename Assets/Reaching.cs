using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reaching : MonoBehaviour
{
    public Vector3 mouseTarget;
    public float constraintAngle;
    private LineRenderer lineRenderer;

    private float[] distBtwVertices;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupDistances();
    }

    // Update is called once per frame
    void Update()
    {
        reachTarget();
    }
    void SetupDistances()
    {
        distBtwVertices = new float[lineRenderer.positionCount - 1];
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            float d = Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i+1));
            distBtwVertices[i] = d;
        }
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

                float crtDstBtwPoints = Vector3.Distance(lineRenderer.GetPosition(i + 1), lineRenderer.GetPosition(i));
                float dstBtwPoints = distBtwVertices[i];
                float dstToMove = crtDstBtwPoints - dstBtwPoints;

                Vector3 dirToMove = (lineRenderer.GetPosition(i + 1) - lineRenderer.GetPosition(i)).normalized;
                Vector3 newPosition = lineRenderer.GetPosition(i) + dirToMove * dstToMove;

                if (i < size - 2)
                {
                    newPosition = GetNewPositionConstrained(lineRenderer.GetPosition(i+1), lineRenderer.GetPosition(i + 2), newPosition, distBtwVertices[i]);
                }

                lineRenderer.SetPosition(i, newPosition);

                //target = lineRenderer.GetPosition(i);
            }

            oldPos = lineRenderer.GetPosition(0);
            lineRenderer.SetPosition(0, root);

            for (int i = 1; i < size; i++)
            {
                float crtDstBtwPoints = Vector3.Distance(lineRenderer.GetPosition(i - 1), lineRenderer.GetPosition(i));
                float dstBtwPoints = distBtwVertices[i - 1];
                float dstToMove = crtDstBtwPoints - dstBtwPoints;

                Vector3 dirToMove = (lineRenderer.GetPosition(i - 1) - lineRenderer.GetPosition(i)).normalized;
                Vector3 newPosition = lineRenderer.GetPosition(i) + dirToMove * dstToMove;

                if (i > 1)
                {
                    newPosition = GetNewPositionConstrained(lineRenderer.GetPosition(i - 1), lineRenderer.GetPosition(i - 2), newPosition, distBtwVertices[i - 1]);
                }
                lineRenderer.SetPosition(i, newPosition);
                //root = lineRenderer.GetPosition(i);
            }
        }
    }

    Vector3 ClampPositionTo(Vector3 verticePositionM1, Vector3 alignedPosition, Vector3 newPosition, float r)
    {
        Vector3 alignedPositionZero = alignedPosition - verticePositionM1;
        float epsilon = Mathf.Deg2Rad * constraintAngle;
        float phi = Mathf.Atan2(alignedPositionZero.y, alignedPositionZero.x);

        float xp = verticePositionM1.x + r * Mathf.Cos(phi - epsilon);
        float yp = verticePositionM1.y + r * Mathf.Sin(phi - epsilon);
        Vector3 vp = new Vector3(xp, yp, 0);

        float xs = verticePositionM1.x + r * Mathf.Cos(phi + epsilon);
        float ys = verticePositionM1.y + r * Mathf.Sin(phi + epsilon);
        Vector3 vs = new Vector3(xs, ys, 0);


        if (Vector3.Distance(newPosition, vp) < Vector3.Distance(newPosition, vs))
        {
            return vp;
        }
        else
        {
            return vs;
        }
    }

    Vector3 GetNewPositionConstrained(Vector3 verticePositionM1, Vector3 verticePositionM2, Vector3 newPosition, float r)
    {
        Vector3 prevDir = (verticePositionM1 - verticePositionM2).normalized;
        Vector3 crtDir = (newPosition - verticePositionM1).normalized;
        Vector3 alignedPosition = verticePositionM1 + prevDir * r;
        float cosTheta = Vector3.Dot(prevDir, crtDir);
        float cosThetaMax = Mathf.Cos(constraintAngle * Mathf.Deg2Rad);

        if (cosTheta < cosThetaMax)
        {
            return ClampPositionTo(verticePositionM1, alignedPosition, newPosition, r);
        }
        else
        {
            return newPosition;
        }
    }
}

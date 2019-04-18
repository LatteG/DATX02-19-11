using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSightCalculator
{
    private HashSet<Triangle> triangles;

    public LineOfSightCalculator()
    {
        triangles = new HashSet<Triangle>();
    }

    public void DebugDrawTriangles(float y)
    {
        foreach (Triangle t in triangles)
        {
            t.DebugDrawTriangle(y);
        }
    }

    public bool PointIsInLOS(Vector3 point)
    {
        Vector2 p = new Vector2(point.x, point.y);

        foreach (Triangle t in triangles)
        {
            if (t.PointIsInTriangle(p))
            {
                return true;
            }
        }

        return false;
    }

    public void CalculateLOS(Vector3 originPos, float range, HashSet<GameObject> obstacles)
    {
        Vector2 pos = new Vector2(originPos.x, originPos.z);
        triangles.Clear();
        
        /*triangles.Add(new Triangle(pos, new Vector2(pos.x + range, pos.y), new Vector2(pos.x + range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x + range, pos.y), new Vector2(pos.x + range, pos.y - range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x - range, pos.y), new Vector2(pos.x - range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x - range, pos.y), new Vector2(pos.x - range, pos.y - range)));

        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y + range), new Vector2(pos.x + range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y + range), new Vector2(pos.x - range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y - range), new Vector2(pos.x + range, pos.y - range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y - range), new Vector2(pos.x - range, pos.y - range)));*/

        Vector2 scale2D;
        Vector3 lossyScale;

        foreach (GameObject obs in obstacles)
        {
            lossyScale = obs.transform.lossyScale;
            scale2D = new Vector2(lossyScale.x, lossyScale.z);

            triangles.UnionWith(GetTrianglesBetweenPointAndObstacle(pos, obs, scale2D));
        }

        // TODO: Merge triangles
    }

    private HashSet<Triangle> GetTrianglesBetweenPointAndObstacle(Vector2 pos, GameObject obstacle, Vector2 scale)
    {
        HashSet<Triangle> retSet = new HashSet<Triangle>();

        BoxCollider box = obstacle.transform.parent.gameObject.GetComponent<BoxCollider>();

        // Debug.Log("Box relative center: (" + box.center.x * scale.x + ", " + box.center.z * scale.y + ")");
        // Debug.Log("Box relative size:   (" + box.size.x * scale.x + ", " + box.size.z * scale.y + ")");

        Vector2 offset = new Vector2(obstacle.transform.parent.position.x, obstacle.transform.parent.position.z);

        Vector2[] corners = new Vector2[4];

        corners[0] = new Vector2(box.center.x - box.size.x / 2, box.center.z - box.size.z / 2);
        corners[1] = new Vector2(box.center.x + box.size.x / 2, box.center.z - box.size.z / 2);
        corners[2] = new Vector2(box.center.x - box.size.x / 2, box.center.z + box.size.z / 2);
        corners[3] = new Vector2(box.center.x + box.size.x / 2, box.center.z + box.size.z / 2);

        for (int i = 0; i < 4; i++)
        {
            corners[i].x *= scale.x;
            corners[i].y *= scale.y;
            corners[i] += offset;
            // Debug.Log("Corner " + i + ": (" + corners[i].x + ", " + corners[i].y + ")");
        }

        Vector2Int inds = new Vector2Int(-1, -1);
        float angle = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int k = 0; k < 4; k++)
            {
                if (i == k)
                {
                    continue;
                }

                float temp = Vector2.Angle(corners[i] - pos, corners[k] - pos);

                if (temp > angle)
                {
                    angle = temp;
                    inds.x = i;
                    inds.y = k;
                }
            }
        }

        Triangle t = new Triangle(pos, corners[inds.x], corners[inds.y]);

        bool hasTwoTriangles = false;
        for (int i = 0; i < 4; i++)
        {
            if (i == inds.x || i == inds.y)
            {
                continue;
            }

            if (t.PointIsInTriangle(corners[i]))
            {
                retSet.Add(new Triangle(pos, corners[inds.x], corners[i]));
                retSet.Add(new Triangle(pos, corners[inds.y], corners[i]));

                hasTwoTriangles = true;
            }
        }

        if (!hasTwoTriangles)
        {
            retSet.Add(t);
        }

        return retSet;
    }

    private class Triangle
    {
        readonly Vector2 a;
        readonly Vector2 b;
        readonly Vector2 c;

        private readonly float area;

        private readonly float SENSITIVITY = 0.000001f;
        private static float Area(Vector2 a, Vector2 b, Vector2 c)
        {
            return Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2);
        }

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = new Vector2(a.x, a.z);
            this.b = new Vector2(b.x, b.z);
            this.c = new Vector2(c.x, c.z);
            
            area = Area(this.a, this.b, this.c);
        }
        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            area = Area(a, b, c);
        }

        public bool PointIsInTriangle(Vector3 point)
        {
            return PointIsInTriangle(new Vector2(point.x, point.z));
        }
        public bool PointIsInTriangle(Vector2 point)
        {
            float areaPAB = Area(point, a, b);
            float areaPAC = Area(point, a, c);
            float areaPBC = Area(point, b, c);

            // If you make triangles where one side is a side of the original triangle and one
            // point is the input point and the three new triangles have the same area as the original triangle,
            // then the input point is inside the triangle.
            return Mathf.Abs(area - (areaPAB + areaPAC + areaPBC)) <= SENSITIVITY;
        }

        public void DebugDrawTriangle(float y)
        {
            Debug.DrawLine(new Vector3(a.x, y, a.y), new Vector3(b.x, y, b.y), Color.green);
            Debug.DrawLine(new Vector3(a.x, y, a.y), new Vector3(c.x, y, c.y), Color.green);
            Debug.DrawLine(new Vector3(c.x, y, c.y), new Vector3(b.x, y, b.y), Color.green);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InvertIf

public class LineOfSightCalculator
{
    private HashSet<Triangle> triangles;

    public LineOfSightCalculator()
    {
        triangles = new HashSet<Triangle>();
    }

    public void DebugDrawTriangles(float y, float duration)
    {
        foreach (Triangle t in triangles)
        {
            t.DebugDrawTriangle(y, duration);
        }
    }

    public bool PointIsInLos(Vector3 point)
    {
        Vector2 p = new Vector2(point.x, point.z);

        foreach (Triangle t in triangles)
        {
            if (t.PointIsInTriangle(p))
            {
                return true;
            }
        }

        return false;
    }

    public void CalculateLos(Vector3 originPos, HashSet<GameObject> obstacles, float range)
    {
        Vector2 pos = new Vector2(originPos.x, originPos.z);
        triangles.Clear();

        triangles.Add(new Triangle(pos, new Vector2(pos.x + range, pos.y), new Vector2(pos.x + range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x + range, pos.y), new Vector2(pos.x + range, pos.y - range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x - range, pos.y), new Vector2(pos.x - range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x - range, pos.y), new Vector2(pos.x - range, pos.y - range)));

        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y + range), new Vector2(pos.x + range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y + range), new Vector2(pos.x - range, pos.y + range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y - range), new Vector2(pos.x + range, pos.y - range)));
        triangles.Add(new Triangle(pos, new Vector2(pos.x, pos.y - range), new Vector2(pos.x - range, pos.y - range)));

        foreach (GameObject obs in obstacles)
        {
            Vector3 lossyScale = obs.transform.lossyScale;
            Vector2 scale2D = new Vector2(lossyScale.x, lossyScale.z);

            triangles.UnionWith(GetTrianglesBetweenPointAndObstacle(pos, obs, scale2D));
        }

//        Debug.Log("There are " + triangles.Count + " triangles before merging.");
//        PrintTriangles();

        Triangle[] temp = triangles.ToArray();
        triangles.Clear();
        triangles.UnionWith(MergeTriangles(temp));

//        Debug.Log("There are " + triangles.Count + " triangles after merging.");
//        PrintTriangles();
    }

    private IEnumerable<Triangle> GetTrianglesBetweenPointAndObstacle(Vector2 pos, GameObject obstacle, Vector2 scale)
    {
        HashSet<Triangle> retSet = new HashSet<Triangle>();

        Transform parentTransform = obstacle.transform.parent;
        BoxCollider box = parentTransform.gameObject.GetComponent<BoxCollider>();

        // Debug.Log("Box relative center: (" + box.center.x * scale.x + ", " + box.center.z * scale.y + ")");
        // Debug.Log("Box relative size:   (" + box.size.x * scale.x + ", " + box.size.z * scale.y + ")");

        Vector3 parentOffset = parentTransform.position;
        Vector2 offset = new Vector2(parentOffset.x, parentOffset.z);

        Vector2[] corners = new Vector2[4];

        corners[0] = new Vector2(box.center.x - box.size.x / 2, box.center.z - box.size.z / 2);
        corners[1] = new Vector2(box.center.x + box.size.x / 2, box.center.z - box.size.z / 2);
        corners[2] = new Vector2(box.center.x - box.size.x / 2, box.center.z + box.size.z / 2);
        corners[3] = new Vector2(box.center.x + box.size.x / 2, box.center.z + box.size.z / 2);

        // Scale and offset corners properly.
        for (int i = 0; i < 4; i++)
        {
            corners[i].x *= scale.x;
            corners[i].y *= scale.y;
            corners[i] += offset;
            // Debug.Log("Corner " + i + ": (" + corners[i].x + ", " + corners[i].y + ")");
        }

        Vector2Int inds = new Vector2Int(-1, -1);
        float angle = 0;

        // Finds the corners that have the highest angle when taking the origin into consideration.
        for (int i = 0; i < 4; i++)
        {
            for (int k = 0; k < 4; k++)
            {
                if (i == k)
                {
                    continue;
                }

                float temp = Vector2.Angle(corners[i] - pos, corners[k] - pos);

                if (!(temp > angle))
                {
                    continue;
                }

                angle = temp;
                inds.x = i;
                inds.y = k;
            }
        }

        Triangle t = new Triangle(pos, corners[inds.x], corners[inds.y]);

        bool hasTwoTriangles = false;
        for (int i = 0; i < 4; i++)
        {
            if (i == inds.x || i == inds.y || !t.PointIsInTriangle(corners[i]))
            {
                continue;
            }

            retSet.Add(new Triangle(pos, corners[inds.x], corners[i]));

            retSet.Add(new Triangle(pos, corners[inds.y], corners[i]));

            hasTwoTriangles = true;
        }

        if (!hasTwoTriangles)
        {
            retSet.Add(t);
        }

        return retSet;
    }

    private IEnumerable<Triangle> MergeTriangles(Triangle[] triangleArr)
    {
        List<Triangle> ret = new List<Triangle>();

        foreach (Triangle t in triangleArr)
        {
            if (!t.IsValid)
            {
                continue;
            }

            MergeTriangleIntoList(t, ret);
        }

        return ret;
    }

    private void MergeTriangleIntoList(Triangle triangle, List<Triangle> triangleList)
    {
        List<Triangle> newTrianglesToMerge = new List<Triangle>();

        bool keepTriangle = true;

        for (int i = 0; i < triangleList.Count; i++)
        {
            // Handles partial overlapping.
            if (IsPartiallyOverlapping(triangleList[i], triangle))
            {
                Triangle temp = SolvePartialOverlap(triangleList[i], triangle);

                // Replace the entry in the list if the new triangle is valid, otherwise clear that entry.
                if (temp.IsValid)
                {
                    triangleList[i] = temp;
                }
                else
                {
                    triangleList.RemoveAt(i--);
                }

                continue;
            }

            if (IsPartiallyOverlapping(triangle, triangleList[i]))
            {
                triangle = SolvePartialOverlap(triangle, triangleList[i]);

                // Continue the merging if the new triangle is valid, otherwise the triangle is not merged into the list.
                if (triangle.IsValid)
                {
                    continue;
                }

                keepTriangle = false;
                break;
            }


            // Handles partial overlapping with division of the overlapped triangle.
            if (IsPartiallyOverlappingAndDividing(triangleList[i], triangle))
            {
                Triangle[] temp = SolvePartialOverlapAndDivide(triangleList[i], triangle);

                // Remove the triangle at index i
                triangleList.RemoveAt(i--);
                
                // Add the return triangles if valid, increment i to skip comparing
                // them with the input triangle if they are added.
                foreach (Triangle t in temp)
                {
                    if (t.IsValid)
                    {
                        triangleList.Insert(++i, t);
                    }
                }

                continue;
            }

            if (IsPartiallyOverlappingAndDividing(triangle, triangleList[i]))
            {
                Triangle[] temp = SolvePartialOverlapAndDivide(triangle, triangleList[i]);

                // Set the input triangle to be the first triangle if it is valid.
                if (temp[0].IsValid)
                {
                    triangle = temp[0];
                    
                    // Add the second triangle to a list of new triangles to be merged once the current input
                    // has been added to the list, if it is valid.
                    if (temp[1].IsValid)
                    {
                        newTrianglesToMerge.Add(temp[1]);
                    }

                    continue;
                }

                // Set the input triangle to be the second triangle if it is valid and the first is not. 
                if (temp[1].IsValid)
                {
                    triangle = temp[1];
                    continue;
                }

                // Do not add either of the triangles if neither is valid.
                keepTriangle = false;
                break;
            }


            // Handles covering.
            if (IsCovering(triangleList[i], triangle))
            {
                // Remove the triangle from the list if it is covered by the input triangle.
                triangleList.RemoveAt(i--);
                continue;
            }

            if (IsCovering(triangle, triangleList[i]))
            {
                // Do not add the input triangle if it is being covered.
                keepTriangle = false;
                break;
            }
        }

        // Add the triangle unless the keepTriangle-flag has been set to false.
        if (keepTriangle)
        {
            triangleList.Add(triangle);
        }

        // Merge in any new triangles made from the input triangle in a partial cover with divide.
        foreach (Triangle t in newTrianglesToMerge)
        {
            MergeTriangleIntoList(t, triangleList);
        }
    }

    private bool IsCovering(Triangle covered, Triangle covering)
    {
        bool coverB = covering.PointIsInOrBehindTriangle(covered.B) && !covering.PointIsInTriangle(covered.B);
        bool coverC = covering.PointIsInOrBehindTriangle(covered.C) && !covering.PointIsInTriangle(covered.C);

        return coverB && coverC;
    }

    private bool IsPartiallyOverlapping(Triangle overlapped, Triangle overlapping)
    {
        if (overlapped.IsCorner(overlapping.B) || overlapped.IsCorner(overlapping.C))
        {
            return false;
        }

        bool overlapB = overlapped.PointIsInTriangle(overlapping.B);
        bool overlapC = overlapped.PointIsInTriangle(overlapping.C);

        return overlapB ^ overlapC;
    }

    private bool IsPartiallyOverlappingAndDividing(Triangle overlapped, Triangle overlapping)
    {
        if (overlapped.IsCorner(overlapping.B) || overlapped.IsCorner(overlapping.C))
        {
            return false;
        }

        bool overlapB = overlapped.PointIsInTriangle(overlapping.B);
        bool overlapC = overlapped.PointIsInTriangle(overlapping.C);

        return overlapB && overlapC;
    }

    private Triangle SolvePartialOverlap(Triangle overlapped, Triangle overlapping)
    {
        // Get the point from the overlapping triangle that is in the overlapped triangle.
        Vector2 r = overlapped.PointIsInTriangle(overlapping.B) ? overlapping.B : overlapping.C;

        // Find where the "hidden" point on the overlapped triangle should move.
        Vector2 intersectPos = Intersect(overlapped.B, overlapped.C, overlapping.A, r);

        // Find which point of the overlapped triangle is not covered by the overlapping triangle.
        Vector2 freePos = overlapping.PointIsInOrBehindTriangle(overlapped.C) ? overlapped.B : overlapped.C;

        // Return a new triangle that only contains the "visible" parts of the overlapped triangle.
        return new Triangle(overlapped.A, intersectPos, freePos);
    }

    private Triangle[] SolvePartialOverlapAndDivide(Triangle overlapped, Triangle overlapping)
    {
        Vector2 midPointOverlapping = (overlapping.B - overlapping.C) / 2 + overlapping.C;
        Vector2 divisionPoint = Intersect(overlapped.B, overlapped.C, overlapping.A, midPointOverlapping);

        Triangle t1 = new Triangle(overlapped.A, overlapped.B, divisionPoint);
        Triangle t2 = new Triangle(overlapped.A, divisionPoint, overlapped.C);

        t1 = SolvePartialOverlap(t1, overlapping);
        t2 = SolvePartialOverlap(t2, overlapping);

        Triangle[] retArr = {t1, t2};

        return retArr;
    }

    private Vector2 Intersect(Vector2 a, Vector2 b, Vector2 p, Vector2 r)
    {
        float dx1 = a.x - b.x;
        float mx1 = b.x;

        float dy1 = a.y - b.y;
        float my1 = b.y;

        float dx2 = r.x - p.x;
        float mx2 = p.x;

        float dy2 = r.y - p.y;
        float my2 = p.y;

        float t2 = (dx1 * (my2 - my1) - dy1 * (mx2 - mx1)) / (dx2 * dy1 - dy2 * dx1);

        return new Vector2(t2 * dx2 + mx2, t2 * dy2 + my2);
    }

    public Triangle[] GetTriangles()
    {
        return triangles.ToArray();
    }

    private void PrintTriangles()
    {
        int triangleCount = 0;
        foreach (Triangle t in triangles)
        {
            Debug.Log("Triangle " + ++triangleCount + ": " + t);
        }
    }

    public class Triangle
    {
        public readonly Vector2 A;
        public readonly Vector2 B;
        public readonly Vector2 C;

        public readonly bool IsValid;

        private readonly float _area;
        private readonly float _angle;

        private readonly float _sensitivity;

        private static float Area(Vector2 a, Vector2 b, Vector2 c)
        {
            return Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2);
        }

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            A = a;
            B = b;
            C = c;

            _area = Area(A, B, C);
            _angle = Vector2.Angle(B - A, C - A);

            _sensitivity = Mathf.Pow(10, -3f);

            IsValid = _angle > _sensitivity;
        }

        public bool PointIsInTriangle(Vector2 point)
        {
            float areaPab = Area(point, A, B);
            float areaPac = Area(point, A, C);
            float areaPbc = Area(point, B, C);

            // If you make triangles where one side is a side of the original triangle and one
            // point is the input point and the three new triangles have the same area as the original triangle,
            // then the input point is inside the triangle.
            return Mathf.Abs(_area - (areaPab + areaPac + areaPbc)) <= _sensitivity;
        }

        public bool PointIsInOrBehindTriangle(Vector2 point)
        {
            Vector2 normB = (B - A).normalized;
            Vector2 normC = (C - A).normalized;
            Vector2 normPHalf = (point - A).normalized / 2;

            Triangle temp = new Triangle(Vector2.zero, normB, normC);

            return temp.PointIsInTriangle(normPHalf);
        }

        public bool IsCorner(Vector2 point)
        {
            return B.Equals(point) || C.Equals(point);
        }

        public void DebugDrawTriangle(float y, float duration)
        {
            Debug.DrawLine(new Vector3(A.x, y, A.y), new Vector3(B.x, y, B.y), Color.green, duration);
            Debug.DrawLine(new Vector3(A.x, y, A.y), new Vector3(C.x, y, C.y), Color.green, duration);
            Debug.DrawLine(new Vector3(C.x, y, C.y), new Vector3(B.x, y, B.y), Color.green, duration);
        }

        public override string ToString()
        {
            return "{A: " + (A - A) + ", B: " + (B - A) + ", C: " + (C - A) + ", angle: " + _angle + ", is valid: " +
                   IsValid + "}";
        }
    }
}
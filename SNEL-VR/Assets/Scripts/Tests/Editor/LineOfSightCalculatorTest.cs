using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//namespace Tests
//{
//    [TestFixture]
//    public class LineOfSightCalculatorTest
//    {
//        private readonly LineOfSightCalculator _lineOfSightCalculator;
//
//        public LineOfSightCalculatorTest()
//        {
//            _lineOfSightCalculator = new LineOfSightCalculator();
//        }
//
//        [Test]
//        public void RunTests()
//        {
//            // Get all angles that is a multiple of PI/8.
//            List<float> angles = new List<float>();
//            for (int i = 0; i < 16; i++)
//            {
//                angles.Add(i * Mathf.PI / 8);
//            }
//
//            for (int i = 0; i < 4; i++)
//            {
//                angles.Add(Mathf.PI / 4 + i * Mathf.PI / 2);
//            }
//
//            // Get all combinations of +/- x and z.
//            List<Vector3> origins = new List<Vector3>();
//            float x = 5;
//            float z = 3;
//            for (int i = -1; i <= 1; i += 2)
//            {
//                for (int k = -1; k <= 1; k += 2)
//                {
//                    origins.Add(new Vector3(x * i, 0, z * k));
//                }
//            }
//
//            // Probably no need to have different sizes of obstacles, but made a list just in case.
//            List<float> obstacleSizes = new List<float> {0.25f};
//
//            // Triple-nested for-loops might not be the best thing in the world, but hey, it's only a test!
//            // Won't affect performance in the end product!
//            foreach (float angle in angles)
//            {
//                foreach (Vector3 origin in origins)
//                {
//                    foreach (float obstacleSize in obstacleSizes)
//                    {
//                        TestCorrectAmountOfTriangleSpawning(angle, origin, obstacleSize);
//
//                        TestObstacleCompleteCoverage(angle, origin, obstacleSize);
//
//                        TestObstaclePartialAndCompleteCoverage(angle, origin, obstacleSize, true);
//                        TestObstaclePartialAndCompleteCoverage(angle, origin, obstacleSize, false);
//                    }
//                }
//            }
//        }
//
//        private void TestCorrectAmountOfTriangleSpawning(float angle, Vector3 origin, float obstacleSize)
//        {
//            Vector3 childScale = Vector3.one * obstacleSize;
//            Vector3 childOffset = new Vector3(obstacleSize / 2, 0, 0);
//
//            Vector3 parentScale = Vector3.one;
//            Vector3 parentPos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized + origin;
//
//            GameObject obstacle = MakeMockObstacle(parentPos, parentScale, childOffset, childScale);
//            HashSet<GameObject> obstacles = new HashSet<GameObject> {obstacle};
//            _lineOfSightCalculator.CalculateLos(origin, obstacles);
//
//            float[] deltaAndLimitingAngle = GetDeltaAndLimitingAngle(angle, parentPos - origin, obstacleSize);
//
//            if (IsDeltaWithinLimit(deltaAndLimitingAngle[0], deltaAndLimitingAngle[1]))
//            {
//                Assert.True(_lineOfSightCalculator.GetTriangles().Length == 2,
//                    "NM: There is supposed to be two triangles, instead there were " +
//                    _lineOfSightCalculator.GetTriangles().Length + ", angle = " + angle * Mathf.Rad2Deg);
//            }
//            else
//            {
//                Assert.True(_lineOfSightCalculator.GetTriangles().Length == 1,
//                    "NM: There is supposed to be one triangle, instead there were " +
//                    _lineOfSightCalculator.GetTriangles().Length + ", angle = " + angle * Mathf.Rad2Deg);
//            }
//        }
//
//        private void TestObstacleCompleteCoverage(float angle, Vector3 origin, float obstacleSize)
//        {
//            Vector3 childScale = Vector3.one * obstacleSize;
//            Vector3 childOffset = new Vector3(obstacleSize, 0, 0);
//
//            Vector3 parentScale = Vector3.one;
//
//            Vector3 pos1 = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
//            Vector3 pos2 = pos1 * 2;
//
//            pos1 += origin;
//            pos2 += origin;
//
//            GameObject obs1 = MakeMockObstacle(pos1, parentScale, childOffset, childScale);
//            GameObject obs2 = MakeMockObstacle(pos2, parentScale, childOffset, childScale);
//
//            HashSet<GameObject> obstacles = new HashSet<GameObject> {obs1, obs2};
//            _lineOfSightCalculator.CalculateLos(origin, obstacles);
//
//            float[] deltaAndLimitingAngle = GetDeltaAndLimitingAngle(angle, pos1 - origin, obstacleSize);
//            LineOfSightCalculator.Triangle[] triangles = _lineOfSightCalculator.GetTriangles();
//
//            // Check for a proper amount of triangles.
//            if (IsDeltaWithinLimit(deltaAndLimitingAngle[0], deltaAndLimitingAngle[1]))
//            {
//                Assert.True(triangles.Length == 2,
//                    "CC: There is supposed to be two triangles, instead there were " +
//                    triangles.Length + ", angle = " + angle * Mathf.Rad2Deg +
//                    ", origin: " + origin);
//            }
//            else
//            {
//                Assert.True(triangles.Length == 1,
//                    "CC: There is supposed to be one triangle, instead there were " +
//                    triangles.Length + ", angle = " + angle * Mathf.Rad2Deg +
//                    ", origin: " + origin);
//            }
//
//            // Check for partial or complete overlap.
//            foreach (LineOfSightCalculator.Triangle t1 in triangles)
//            {
//                foreach (LineOfSightCalculator.Triangle t2 in triangles)
//                {
//                    if (t1.Equals(t2))
//                    {
//                        continue;
//                    }
//
//                    Assert.False(_lineOfSightCalculator.IsCovering(t1, t2), "CC: " + t2 + " is covering " + t1);
//                    Assert.False(_lineOfSightCalculator.IsCovering(t2, t1), "CC: " + t1 + " is covering " + t2);
//
//                    Assert.False(_lineOfSightCalculator.IsPartiallyOverlapping(t1, t2),
//                        "CC: " + t2 + " is partially covering " + t1);
//                    Assert.False(_lineOfSightCalculator.IsPartiallyOverlapping(t2, t1),
//                        "CC: " + t1 + " is partially covering " + t2);
//                }
//            }
//        }
//
//        private void TestObstaclePartialAndCompleteCoverage(float angle, Vector3 origin, float obstacleSize,
//            bool secondObstacleHasHigherAngle)
//        {
//            float secondAngle = angle;
//            secondAngle += secondObstacleHasHigherAngle
//                ? Mathf.Abs(Mathf.Atan(obstacleSize / (obstacleSize + 2)))
//                : Mathf.Abs(Mathf.Atan(obstacleSize / (obstacleSize + 2))) * -1;
//
//            Vector3 childScale = Vector3.one * obstacleSize;
//            Vector3 childOffset = new Vector3(obstacleSize / 2, 0, 0);
//
//            Vector3 parentScale = Vector3.one;
//
//            Vector3 pos1 = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized + origin;
//            Vector3 pos2 = new Vector3(Mathf.Cos(secondAngle), 0, Mathf.Sin(secondAngle)).normalized * 2 + origin;
//
//            GameObject obs1 = MakeMockObstacle(pos1, parentScale, childOffset, childScale);
//            GameObject obs2 = MakeMockObstacle(pos2, parentScale, childOffset, childScale);
//
//            HashSet<GameObject> obstacles = new HashSet<GameObject> {obs1, obs2};
//            _lineOfSightCalculator.CalculateLos(origin, obstacles);
//
//            LineOfSightCalculator.Triangle[] triangles = _lineOfSightCalculator.GetTriangles();
//            
//            // Check for partial or complete overlap.
//            foreach (LineOfSightCalculator.Triangle t1 in triangles)
//            {
//                foreach (LineOfSightCalculator.Triangle t2 in triangles)
//                {
//                    if (t1.Equals(t2))
//                    {
//                        continue;
//                    }
//
//                    Assert.False(_lineOfSightCalculator.IsCovering(t1, t2), 
//                        "PC/CC: " + t2 + " is covering " + t1);
//                    Assert.False(_lineOfSightCalculator.IsCovering(t2, t1), 
//                        "PC/CC: " + t1 + " is covering " + t2);
//
//                    Assert.False(_lineOfSightCalculator.IsPartiallyOverlapping(t1, t2),
//                        "PC/CC: " + t2 + " is partially covering " + t1);
//                    Assert.False(_lineOfSightCalculator.IsPartiallyOverlapping(t2, t1),
//                        "PC/CC: " + t1 + " is partially covering " + t2);
//                }
//            }
//        }
//
//        private bool IsDeltaWithinLimit(float delta, float limiting)
//        {
//            if (delta > Mathf.PI / 4)
//            {
//                limiting = Mathf.PI / 2 - limiting;
//                return delta < limiting;
//            }
//            else
//            {
//                return delta > limiting;
//            }
//        }
//
//        private float[] GetDeltaAndLimitingAngle(float angle, Vector3 relativePos, float obstacleSize)
//        {
//            // Limit the angle to be between 0 and PI/2 rad.
//            float deltaAngle = angle * Mathf.Rad2Deg % 90 * Mathf.Deg2Rad;
//
//            // Get the angle for the transition between having one triangle per obstacle to two triangles per obstacle.
//            float limitingAngle = Mathf.Asin(obstacleSize / relativePos.magnitude);
//
//            float[] retArr = {deltaAngle, limitingAngle};
//
//            return retArr;
//        }
//
//        private GameObject MakeMockObstacle(Vector3 parentPos, Vector3 parentScale, Vector3 childPos,
//            Vector3 childScale)
//        {
//            // Make the obstacle-segment.
//            GameObject co = new GameObject();
//            co.transform.position = childPos;
//            co.transform.localScale = childScale;
//
//            // Make the parent.
//            GameObject po = new GameObject();
//            po.transform.position = parentPos;
//            po.transform.localScale = parentScale;
//            po.AddComponent<BoxCollider>();
//
//            // Set the parent.
//            co.transform.parent = po.transform;
//
//            return co;
//        }
//    }
//}
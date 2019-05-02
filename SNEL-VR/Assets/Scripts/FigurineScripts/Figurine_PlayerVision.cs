using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figurine_PlayerVision : MonoBehaviour
{
    private bool _shouldUpdate = false;
    private bool _hasUpdated = false;

    private Vector3 _pos;
    private Vector3 _scale;
    private Vector3 _worldScale;
    private Vector3 _sphereCenter;

    private int _obstacleLayerMask;
    private float _visionRange;

    private Transform _parentTransform;
    private GameObject _figurine;

    private HashSet<Collider> _tempKnownObjectExitingRange;

    private HashSet<GameObject> _permKnownObjects;
    private HashSet<GameObject> _tempKnownObjects;

    private HashSet<GameObject> _obstaclesInRange;
    private HashSet<Collider> _collidersToBeProcessed;

    private LineOfSightCalculator _losCalc;

    private readonly float _debugDrawLineDuration = 1;

    // Called whenever the fog should be updated from the figurine's perspective.
    public void ShouldUpdate()
    {
        _shouldUpdate = true;
    }

    // Sets initial values and values that are never changed.
    private void OnEnable()
    {
        _parentTransform = transform.parent;
        _scale = transform.lossyScale;

        // Used for LOS calculations
        _worldScale = transform.parent.parent.lossyScale;

        // Used for the first FixedUpdate-call.
        _pos = _parentTransform.position;

        // Used for raycasts.
        _visionRange = GetComponent<SphereCollider>().radius * _scale.x * 0.95f;
        _sphereCenter = GetComponent<SphereCollider>().center;
        _sphereCenter.x *= _scale.x;
        _sphereCenter.y *= _scale.y;
        _sphereCenter.z *= _scale.z;
        _obstacleLayerMask = 1 << 9;

        // Used for LoS-updates in fog elements.
        _figurine = _parentTransform.gameObject;

        _tempKnownObjectExitingRange = new HashSet<Collider>();

        _permKnownObjects = new HashSet<GameObject>();
        _tempKnownObjects = new HashSet<GameObject>();
        _obstaclesInRange = new HashSet<GameObject>();
        _collidersToBeProcessed = new HashSet<Collider>();

        _losCalc = new LineOfSightCalculator(_worldScale);

        // Adds the attached figurine to the set of permanently known objects.
        foreach (Transform child in _parentTransform)
        {
            if (!child.gameObject.CompareTag("PlayerFigurine"))
            {
                continue;
            }

            _permKnownObjects.Add(child.gameObject);
            break;
        }
    }

    // Updates the position and checks if the script has updated the fog.
    private void FixedUpdate()
    {
        _pos = _parentTransform.position;
        if (!_hasUpdated)
        {
            return;
        }

        _losCalc.CalculateLos(_pos, _obstaclesInRange, _visionRange);
        _losCalc.DebugDrawTriangles(_pos.y + _scale.x * 0.5f, _debugDrawLineDuration);

        _obstaclesInRange.Clear();

        /*foreach (Collider c in _tempKnownObjectExitingRange)
        {
            UpdateColliderStatus(c);
        }

        _tempKnownObjectExitingRange.Clear();*/

        foreach (Collider c in _collidersToBeProcessed)
        {
            UpdateColliderStatus(c);
        }

        _collidersToBeProcessed.Clear();

        _shouldUpdate = false;
        _hasUpdated = false;
    }

    // Checks if a collider should have its visibility status updated.
    private void OnTriggerStay(Collider other)
    {
        // Does not update anything if the figurine has not moved.
        if (!_shouldUpdate)
        {
            return;
        }

        _hasUpdated = true;

        if (ColliderHasTag(other, "Fog") ||
            ColliderHasTag(other, "NPCFigurine") ||
            ColliderHasTag(other, "PlayerFigurine"))
        {
            _collidersToBeProcessed.Add(other);
        }
        else if (ColliderHasTag(other, "Obstacle"))
        {
            _collidersToBeProcessed.Add(other);
            _obstaclesInRange.Add(other.gameObject);
        }
    }

    // Checks if the collider belongs to an object that should be visible to this figurine's owner.
    // Returns true if the object is within the visibility range, and false if it is outside it.
    public bool UpdateColliderStatus(Collider other)
    {
        Vector3 otherPos = other.transform.position;

        if (ColliderHasTag(other, "Obstacle"))
        {
            Vector3 tmp = otherPos;
            tmp.y += 2;

            // Get the proper scale for the obstacle.
            Vector3 segmentScale = other.transform.parent.localScale;
            Vector3 parentScale = other.transform.parent.parent.localScale;
            Vector3 otherScale = new Vector3(segmentScale.x * parentScale.x, 0, segmentScale.z * parentScale.z);
            otherScale = Quaternion.AngleAxis(-other.transform.rotation.eulerAngles.y, Vector3.up) * otherScale;
            otherScale.x *= _scale.x;
            otherScale.z *= _scale.z;
            
            // Get the box size in the correct scale.
            Vector3 boxSize = other.gameObject.transform.parent.gameObject.GetComponent<BoxCollider>().size;
            boxSize = Quaternion.AngleAxis(-other.transform.rotation.eulerAngles.y, Vector3.up) * boxSize;
            boxSize.x *= otherScale.x;
            boxSize.z *= otherScale.z;

            // Calculate angle between middle of obstacle and figurine.
            Vector3 delta = otherPos - _pos;
            float angle = Mathf.Asin(delta.z / delta.magnitude);

            // Calculate radius of circle that can contain the obstacle.
            float radius = Mathf.Sqrt(Mathf.Pow(boxSize.x / 2, 2) + Mathf.Pow(boxSize.z / 2, 2));

            // Calculate offset needed in the x-axis to be able to detect the obstacle assuming no other obstacle is obscuring it.
            float offsetX = radius * Mathf.Cos(angle);
            if (offsetX < 0)
            {
                offsetX = Mathf.Max(offsetX, -boxSize.x / 2);
            }
            else
            {
                offsetX = Mathf.Min(offsetX, boxSize.x / 2);
            }

            // Calculate offset needed in the z-axis to be able to detect the obstacle assuming no other obstacle is obscuring it.
            float offsetZ = radius * Mathf.Sin(angle);
            if (offsetZ < 0)
            {
                offsetZ = Mathf.Max(offsetZ, -boxSize.z / 2);
            }
            else
            {
                offsetZ = Mathf.Min(offsetZ, boxSize.z / 2);
            }

            // Apply offsets with an extra bit to make sure it is not stuck "inside the wall".
            otherPos.x -= offsetX * 1.01f;
            otherPos.z -= offsetZ * 1.01f;

            Debug.DrawLine(tmp, new Vector3(otherPos.x, otherPos.y + 2, otherPos.z), Color.black, _debugDrawLineDuration);
        }
        
        float distance = (otherPos - (_sphereCenter + _pos)).magnitude;

        if (distance > _visionRange)
        {
            TellOutOfLOS(other.gameObject);
            return false;
        }
        
        if (_losCalc.PointIsInLos(otherPos))
        {
            TellInLOS(other.gameObject);
            Debug.DrawLine(otherPos, otherPos + Vector3.up / 5, Color.green, _debugDrawLineDuration);
        }
        else
        {
            TellOutOfLOS(other.gameObject);
            Debug.DrawLine(otherPos, otherPos + Vector3.up / 5, Color.red, _debugDrawLineDuration);
        }

        return true;
    }

    public bool UpdateObstacleColliderStatus(Collider other)
    {
        // Make sure the collider belongs to an obstacle.
        if (!ColliderHasTag(other, "Obstacle"))
        {
            return false;
        }

        // No need to do any more calls if the obstacle is already known.
        if (_permKnownObjects.Contains(other.gameObject))
        {
            return true;
        }

        Transform obstacleTransform = other.gameObject.GetComponent<Transform>().parent;
        Vector3 obstaclePos = obstacleTransform.position;
        Vector3 direction = obstaclePos - (_sphereCenter + _pos);
        float raycastRange = direction.magnitude;

        // Makes sure the object is in range.
        if (raycastRange > _visionRange)
        {
            return false;
        }
        else
        {
            // Gets the blocking part of of the object other belongs to.
            GameObject blocker = null;
            foreach (Transform child in obstacleTransform)
            {
                if (child.gameObject.layer == (int) Mathf.Log(_obstacleLayerMask, 2))
                {
                    blocker = child.gameObject;
                    break;
                }
            }

            // Makes sure that blocker is assigned some value.
            if (blocker == null)
            {
                return false;
            }

            // Disables the blocking part of the obstacle to prevent it from being "in the way" of itself.
            blocker.layer = 0;

            DoLOSRaycast(other, direction, raycastRange);

            // Enables the blocking part of the obstacle to allow it to block LOS to other objects.
            blocker.layer = (int) Mathf.Log(_obstacleLayerMask, 2);

            return true;
        }
    }

    // Calls the raycast and appropriate method based on its result.
    private void DoLOSRaycast(Collider other, Vector3 direction, float raycastRange)
    {
        // Checks if there is an obstacle between the figurine and fog element.
        if (Physics.Raycast(_sphereCenter + _pos, direction, raycastRange, _obstacleLayerMask))
        {
            // Is an obstacle.
            Debug.DrawLine(_sphereCenter + _pos, other.gameObject.GetComponent<Transform>().position, Color.red, 1);
            TellOutOfLOS(other.gameObject);
        }
        else
        {
            // Is no obstacle.
            Debug.DrawLine(_sphereCenter + _pos, other.gameObject.GetComponent<Transform>().position, Color.green, 1);
            TellInLOS(other.gameObject);
        }
    }

    // Sends a call to a fog element's script to tell it it is no longer visible when visited.
    private void OnTriggerExit(Collider other)
    {
        if (ColliderHasTag(other, "Fog") ||
            ColliderHasTag(other, "NPCFigurine") ||
            ColliderHasTag(other, "PlayerFigurine"))
        {
            // _tempKnownObjectExitingRange.Add(other);
            _collidersToBeProcessed.Add(other);
        }
    }

    // Tells a fog element it is no longer visible.
    private void TellOutOfLOS(GameObject other)
    {
        if (!other.CompareTag("Obstacle"))
        {
            _tempKnownObjects.Remove(other);
        }
    }

    // Tells a fog element it is visible.
    private void TellInLOS(GameObject other)
    {
        if (other.CompareTag("Fog"))
        {
            _permKnownObjects.Add(other);
            _tempKnownObjects.Add(other);
        }
        else if (other.CompareTag("Obstacle"))
        {
            _permKnownObjects.Add(other);
        }
        else
        {
            _tempKnownObjects.Add(other);
        }
    }

    // Checks if the other collider has the wanted tag.
    // If the collider is untagged it checks the tag of its gameObject.
    // If that gameObject is untagged it checks if that gameObjects parent-gameObject has the tag.
    // Returns false if any of the tags checked is neither "Untagged" nor tag, or if all are "Untagged", true otherwise.
    private bool ColliderHasTag(Collider other, string wantedTag)
    {
        if (other.CompareTag(wantedTag))
        {
            return true;
        }

        if (!other.CompareTag("Untagged"))
        {
            return false;
        }

        if (other.gameObject.CompareTag(wantedTag))
        {
            return true;
        }

        if (other.gameObject.CompareTag("Untagged"))
        {
            return other.gameObject.transform.parent.gameObject.CompareTag(wantedTag);
        }

        return false;
    }

    public HashSet<GameObject> GetTempKnownObjects()
    {
        return _tempKnownObjects;
    }

    public HashSet<GameObject> GetPermKnownObjects()
    {
        return _permKnownObjects;
    }
}
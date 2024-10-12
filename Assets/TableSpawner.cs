using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSpawner : MonoBehaviour
{
    public GameObject tablePrefab;
    public OVRSkeleton LeftSkeleton;
    public OVRSkeleton RightSkeleton;
    
    
    public void SpawnTable() {
        Transform leftThumbTipTransform = GetFingerTransformForSkeleton(LeftSkeleton, OVRSkeleton.BoneId.Hand_ThumbTip);
        Transform rightThumbTipTransform = GetFingerTransformForSkeleton(RightSkeleton, OVRSkeleton.BoneId.Hand_ThumbTip);
        
        Vector3 midpoint = (leftThumbTipTransform.position + rightThumbTipTransform.position) / 2f;

        Vector3 direction = leftThumbTipTransform.position - rightThumbTipTransform.position;
        Vector3 perpendicular = -Vector3.Cross(direction.normalized, Vector3.up);
        Quaternion rotation = Quaternion.LookRotation(perpendicular);
        
        // if a object with tag table exists, destroy it
        GameObject oldTable = GameObject.FindGameObjectWithTag("Table");
        if (oldTable != null) {
            Destroy(oldTable);
        }
        GameObject table = Instantiate(tablePrefab, midpoint, rotation);
    }

    public Transform GetFingerTransformForSkeleton(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId) {
        foreach (var b in skeleton.Bones) {
            if (b.Id == boneId) {
                return b.Transform;
            }
        }

        return null;
    }

}

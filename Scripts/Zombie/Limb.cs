using System.Collections.Generic;
using UnityEngine;

public enum LimbType
{
    Head,
    LeftArm,
    RightArm,
    Leg
}

public class Limb : MonoBehaviour
{
    public void GetHit()
    {
        if (childLimbs.Count > 0)
        {
            foreach (Limb limb in childLimbs)
            {
                if (limb != null)
                {
                    limb.GetHit();
                }
            }
        }

        if (limbPrefab != null)
        {
            Instantiate(limbPrefab, transform.position, transform.rotation);
        }

        transform.localScale = Vector3.zero;

        zombieController.OnLimbDestroyed(limbType);

        Destroy(this);
    }
}

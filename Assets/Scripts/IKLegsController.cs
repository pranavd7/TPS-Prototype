using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLegsController : MonoBehaviour
{
    [SerializeField] public Transform leftTarget;
    [SerializeField] public Transform rightTarget;

    [SerializeField] private IKLegTargetBiped[] legs;

    
    //even legs movable with legs inserted as all on one side first (eg. all left legs inserted first)
    bool evenLegMovable = true;
    //if ready to switch movable legs
    bool readySwitchMovable = true;

    //[SerializeField] Transform body;
    //Vector3 tempPos;
    //float tempHeight;
    //RaycastHit hit;
    //float moveSpeed = 5;

    // Start is called before the first frame update
    void Awake()
    {
        //StartCoroutine(AdjustBody());
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (legs.Length < 2)
                return;

            // Ordering steps
            foreach (IKLegTargetBiped leg in legs)
            {
                leg.movable = evenLegMovable;
                evenLegMovable = !evenLegMovable;
            }

            //if legMovable is true even legs(even index) are movable
            int index = evenLegMovable ? 0 : 1;

            // If the opposite foot step completes, switch the order to make a new step
            if (readySwitchMovable && legs[index].grounded)
            {
                evenLegMovable = !evenLegMovable;
                readySwitchMovable = false;
            }

            if (!readySwitchMovable && !legs[index].grounded)
            {
                readySwitchMovable = true;
            }
        }
    }

    //IEnumerator AdjustBody()
    //{
    //    while (true)
    //    {

    //        //if (Physics.Raycast(body.position + body.up, -body.up, out hit, Mathf.Infinity))
    //        //{
    //        //    Debug.DrawRay(body.position + body.up, -body.up * 10, Color.red, 10f);
    //        //    tempPos = hit.point - 0.05f * body.up;
    //        //}
    //        tempPos = Vector3.zero;
    //        foreach (IKLegTargetBiped leg in legs)
    //        {
    //            tempPos.z += leg.transform.position.z;
    //        }
    //        tempPos /= legs.Length;

    //        body.position = Vector3.Lerp(body.position, tempPos, moveSpeed * Time.deltaTime);
    //        //transform.position = tempPos;

    //        yield return new WaitForEndOfFrame();
    //    }
    //}
}

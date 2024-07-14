using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase : MonoBehaviour
{

    #region НОВЫЕ ПРОСТРАНСТВЕННЫЕ ФУНКЦИИ


    /// <summary>
    /// Rotate when target reached, before attacking
    /// </summary>
    /// <param name="target"></param>

    public float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public float VectorAngleHorizontal(Vector3 v1, Vector3 v2)
    {
        Vector3 v1x = v1;
        Vector3 v2x = v2;

        v1x.y = 0;
        v2x.y = 0;

        return Vector3.Angle(v1x, v2x);
    }


    /// <summary>
    /// Возвращаем дистанцию между юнитом и точкой назначения по горизонтали и вертикали отдельно
    /// </summary>
    /// <param name="position"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public Vector2 GetUnitDistance(Vector3 position, Vector3 destination)
    {
        Vector3 PositionXZ;
        Vector3 GuardLocXZ;
        Vector2 result;

        PositionXZ.y = 0;
        PositionXZ.x = position.x;
        PositionXZ.z = position.z;

        GuardLocXZ.y = 0;
        GuardLocXZ.x = destination.x;
        GuardLocXZ.z = destination.z;

        result.x = Vector3.Distance(PositionXZ, GuardLocXZ);

        result.y = Mathf.Abs(destination.y - position.y);

        return result;
    }

    /// <summary>
    /// Может ли GO A попасть в GO B на дистанции C (живу в PlayerStateBase)
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool CanHitTarget(GameObject from, GameObject to, float range)
    {
        
        //Debug.Log("check:  ");
        if (Vector3.Distance(from.transform.position, to.transform.position) > range) // Превышена дистанция
        {
            //Debug.Log("check: distance exceeded ");
            return false;
        }


        Vector3 TruePosition = from.transform.position;
        Vector3 TargetTruePosition = to.transform.position;

        // Если у стрелка есть центр фигуры, то берем ее как источник
        if (from.GetComponent<PlayerStateManager>() != null && from.GetComponent<PlayerStateManager>().FigureCenter != null)
        {
            TruePosition= from.GetComponent<PlayerStateManager>().FigureCenter.transform.position;
            //Debug.Log("check:  shooter has center");
            //GameObject PlasmaShot = GameObject.Instantiate(from.GetComponent<PlayerStateManager>().BlockEffect, TruePosition, from.transform.rotation);
        }

        // Если у цели есть центр фигуры, то берем ее как точку назначения
        if (to.GetComponent<PlayerStateManager>() != null && to.GetComponent<PlayerStateManager>().FigureCenter != null)
        {
            TargetTruePosition = to.GetComponent<PlayerStateManager>().FigureCenter.transform.position;
            //Debug.Log("check:  target has center");
            //GameObject PlasmaShot = GameObject.Instantiate(to.GetComponent<PlayerStateManager>().BlockEffect, TargetTruePosition, to.transform.rotation);
        }

        Vector3 toTarget = TargetTruePosition - TruePosition;

        if (Physics.Raycast(TruePosition, toTarget, out RaycastHit hit, 40000))
        {
            //Debug.Log("check:  raycast");
            if (hit.transform == to.transform)
            {
                //Debug.Log("check:  raycast hit");
                return true;
            }
            else
            {
                //Debug.Log("check:  raycast fail");
                //Debug.Log(hit.transform.name);
                return false;
            }
        }
        else
        {
            //Debug.Log("check:  no raycast");
            return false;
        }



    }



    #endregion
}

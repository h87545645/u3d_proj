using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AstarTest : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //穿透所有mesh直到地面
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (var hit in hits)
            {
                string name = hit.collider.gameObject.name;
                if (name == "Plane")
                {
                    //移动方块
                    navMeshAgent.SetDestination(hit.point);
                }
            }
        }
    }
}

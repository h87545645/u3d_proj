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
            //��͸����meshֱ������
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (var hit in hits)
            {
                string name = hit.collider.gameObject.name;
                if (name == "Plane")
                {
                    //�ƶ�����
                    navMeshAgent.SetDestination(hit.point);
                }
            }
        }
    }
}

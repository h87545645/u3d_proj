using UnityEngine;
using System.Collections;

public class StandingState : BaseState
{
    private FragHore _fragHore;
    public StandingState(FragHore frag)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetBool("standing", true);
        Debug.Log("------------------------FragHore in StandingState~!������վ��״̬����");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {
        if (_fragHore.isGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _fragHore.SetHeroineState(new ChargeState(_fragHore));
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class LandingState : BaseState
{
    private FragHore _fragHore;
    public LandingState(FragHore frag)
    {
        _fragHore = frag;
        _fragHore.fragAnim.SetBool("landing", true);
        Debug.Log("------------------------FragHore in LandingState~!��������½״̬����");
    }

    public void Update()
    {

    }

    public void HandleInput()
    {

    }
}

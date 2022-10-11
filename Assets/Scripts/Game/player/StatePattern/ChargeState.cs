using UnityEngine;
using System.Collections;

public class ChargeState : IBaseState
{

    private FragHero _fragHore;

    //private double chargeTime = .0f;
    public ChargeState(FragHero frag)
    {
        _fragHore = frag;
        //chargeTime = .0f;
        _fragHore.fragAnim.SetTrigger("power");
        _fragHore.fragAnim.SetBool("standing", false);
        Debug.Log("------------------------FragHore in ChargeState~!(½øÈëÐîÁ¦×´Ì¬£¡)");
    }

    public void Update()
    {
       
    }

    public void HandleInput()
    {
        //chargeTime += Time.deltaTime;
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    _fragHore.SetHeroineState(new JumpingState(_fragHore , chargeTime));
        //    chargeTime = .0f;
        //}
    }
}
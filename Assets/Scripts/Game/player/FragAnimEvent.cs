using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragAnimEvent : MonoBehaviour
{
    public FragHore fragHore;
    public void OnLandingFinish()
    {
        fragHore.SetHeroineState(new StandingState(fragHore));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragAnimEvent : MonoBehaviour
{
    public FragHero fragHore;
    public void OnLandingFinish()
    {
        fragHore.SetHeroineState(new StandingState(fragHore));
        fragHore.IsReady = true;
    }
}

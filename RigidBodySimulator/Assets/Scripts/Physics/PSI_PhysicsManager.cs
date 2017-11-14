using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PSI_PhysicsManager : MonoBehaviour {

    [SerializeField]
    private int TicksPerSecond = 30;

    private float pTimeBetweenTicks { get { return 1f / (float)TicksPerSecond; } }


    private float t = 0.0f;

    //----------------------------------------Unity Functions----------------------------------------

    private void Start()
    {
        StartCoroutine(PhysicsLoop());
    }


    //----------------------------------------Private Functions--------------------------------------

    private void Tick()
    {
        //print("Tick! - " + (1f / (Time.unscaledTime - t)));
        //t = Time.unscaledTime;
    }

    private IEnumerator PhysicsLoop()
    {
        int lastPrintSecond = 0;
        int tickCount = 0;

        float timeOfLastTick = -1f;

        while(true)
        {
            if(Time.unscaledTime - timeOfLastTick >= pTimeBetweenTicks)
            {
                timeOfLastTick = Time.unscaledTime;

                Tick();
                tickCount++;

                if (Mathf.FloorToInt(Time.timeSinceLevelLoad) > lastPrintSecond)
                {
                    print("Ticks this second: " + tickCount);
                    tickCount = 0;
                    lastPrintSecond = Mathf.FloorToInt(Time.timeSinceLevelLoad);
                }
            }


            yield return null;
            //yield return new WaitForSecondsRealtime(pTimeBetweenTicks);
        }
        
    }
}

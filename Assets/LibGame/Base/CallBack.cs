//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UniRx;
//using UnityEngine;

//public class CallBack : MonoBehaviour
//{

//    public static void Call(Action callBack, double timeDelay = 0)
//    {
//        Scheduler.MainThreadIgnoreTimeScale.Schedule(TimeSpan.FromSeconds(timeDelay), time =>
//        {
//            callBack.Invoke();
//        });
//    }
//}

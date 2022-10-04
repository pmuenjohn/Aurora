using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using GameCommon;

namespace GameCommon
{
    public class GameNetwork : SingletonBind<GameNetwork>
    {
        private static bool isConnected = false;
        private string linkTest = "http://google.com";

        private IEnumerator checkInternetConnection(Action<bool> action)
        {
            WWW www = new WWW(linkTest);
            yield return www;
            if (www.error != null)
            {
                action(false);
            }
            else
            {
                action(true);
            }
        }

        //private IEnumerator GetRequest(string uri, Action<bool> action)
        //{
        //    using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        //    {
        //        // Request and wait for the desired page.
        //        yield return webRequest.SendWebRequest();

        //        if (webRequest.isNetworkError)
        //        {
        //            action(false);
        //        }
        //        else
        //        {
        //            action(true);
        //        }
        //    }
        //}

        public void CheckInternet()
        {
            if (!isConnected)
            {
                StartCoroutine(checkInternetConnection((connected) =>
                    {
                        isConnected = connected;
                    }));
            }
        }

    }
}
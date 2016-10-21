using System;
using Assets.GoogleAnaliticsCrossPlatform.GoogleAnalitics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets
{
    public class TestGoogleAnalitics : MonoBehaviour
    {
        public Text text;
        private DateTime _start;
        private void OnEnable()
        {
            _start = DateTime.Now;
            GoogleAnaliticsService.Initialize("UA-75783218-1","testUser1");
            GoogleAnaliticsService.IsDebuging = true;
            StartCoroutine(GoogleAnaliticsService.TrackPageView("com.android.testga", "MainPage", "Hello vitek"));
            StartCoroutine(GoogleAnaliticsService.TrackScreenView("TestGa", "MainScreen","1.0", "com.android.testga", "com.android.testga"));
            Application.logMessageReceived += ApplicationOnLogMessageReceived;
        }

        private void ApplicationOnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            text.text += condition+"\n";
        }

        public void TrackEvent()
        {
            for (int i = 0; i < 10; i++)
            {
                StartCoroutine(GoogleAnaliticsService.TrackEvent("test  Event 1", "click    1" + Random.Range(0, 9)));    
            }
            
        }

        public void TrackTransaction()
        {
            StartCoroutine(GoogleAnaliticsService.TrackTransaction("vitek", "my store", 49.95f, 0.05f, 3.12f, "USD"));
        }

        public void TrackItem()
        {
            StartCoroutine(GoogleAnaliticsService.TrackItem("vitek", "pryanik", 1.15f, 60, "game.vitek", "loot", "USD"));
        }

        public void TrackSocial()
        {
            StartCoroutine(GoogleAnaliticsService.TrackSocial("Like", "Vas'Book", "boroda"));
        }

        public void TrackException()
        {
            try
            {
                var go = default(GameObject);
                var ch = go.transform.childCount;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                StartCoroutine(GoogleAnaliticsService.TrackException(e.Message, true));
            }
        }

        public void TrackTiming()
        {
            StartCoroutine(GoogleAnaliticsService.TrackTiming("timing", "start timing",
                (DateTime.Now - _start).Milliseconds));
        }

        
    }
}

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Assets.GoogleAnaliticsCrossPlatform.GoogleAnalitics
{
    public static class GoogleAnaliticsService
    {
        
        private const string GoogleAnaliticsProtocolVersion = "1";

        private static string GoogleAnaliticsURL
        {
            get
            {
                if (IsDebuging)
                {
                    return "http://www.google-analytics.com/debug/collect";//"?";
                }
                return "http://www.google-analytics.com/collect";//"?";
            }
        }

    
        private static string _googleAnaliticsId = "UA-63824095-1";
        private static string _googleAnaliticsUserGuid = "UA-63824095-1";
        private static bool _isInitialize;
        private static Random _random;
    
        /// <summary>
        /// if this flag = true analitics no tracking it's for test requests only
        /// </summary>
        public static bool IsDebuging;

        public static void Initialize(string gaId, string gaUserId)
        {
            _googleAnaliticsId = gaId;
            _googleAnaliticsUserGuid = gaUserId;
            _isInitialize = true;
            _random = new Random(DateTime.Now.Millisecond);
            
        }

        /// <param name="documentHostName"> host domain (ex google.com) for apps - application name (ex myGame)</param>
        /// <param name="page">relative page url (ex /home) for apps - view name (ex myView)</param>
        /// <param name="title">page title for apps - full name of view (ex My First View)</param>
        public static IEnumerator TrackPageView(string documentHostName = "", string page = "", string title="")
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.PageView, documentHostName, page, title);
            return SendResponce(gaString);
        }
        /// <param name="eventCategory">(ex UI)</param>
        /// <param name="eventAction">(ex click_left_button)</param>
        /// <param name="eventLabel">(ex MyMegaLeftButton)</param>
        /// <param name="eventValue">some int value</param>
        public static IEnumerator TrackEvent(string eventCategory, string eventAction, string eventLabel = null,int? eventValue = null)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.Event, eventCategory, eventAction, eventLabel,eventValue);
            return SendResponce(gaString);
        }

        /// <param name="transactionAffiliation">purchase service or store (ex AppStore,Google.Play)</param>
        /// <param name="transactionRevenue">full price of transaction</param>
        /// <param name="transactionShipping">price for shiping</param>
        /// <param name="transactionTax">nalogi</param>
        /// <param name="currencyCode">ex RUB GB TUG</param>
        public static IEnumerator TrackTransaction(string transactionId, string transactionAffiliation = "", float? transactionRevenue = null, float? transactionShipping = null, float? transactionTax = null, string currencyCode = null)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.Transaction, transactionId, transactionAffiliation, transactionRevenue, transactionShipping, transactionTax, currencyCode);
            return SendResponce(gaString);
        }

        /// <param name="itemQuantity"> tupo suko count</param>
        /// <param name="itemCode">item sku</param>
        /// <param name="currencyCode">ex RUB GB TUG</param>
        public static IEnumerator TrackItem(string transactionId, string itemName, float? itemPrice =  null, int? itemQuantity = null, string itemCode = null, string itemCategory=null, string currencyCode=null)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.Item, transactionId, itemName, itemPrice, itemQuantity,itemCode,itemCategory, currencyCode);
            return SendResponce(gaString);
        }

        /// <param name="socialTarget">//url or text</param>
        /// <returns></returns>
        public static IEnumerator TrackSocial(string socialAction,string socialNetwork,string socialTarget)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.Social, socialAction,socialNetwork,socialTarget);
            return SendResponce(gaString);
        }

        public static IEnumerator TrackException(string exceptionDescription = null, bool? isFatal = null)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.Exception, exceptionDescription,isFatal);
            return SendResponce(gaString);
        }

        public static IEnumerator TrackTiming(string timingCategory, string timingVariable, int time, string timingLabel=null)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.Timing, timingCategory, timingVariable,time,timingLabel);
            return SendResponce(gaString);
        }

        /// <param name="appInstallerId">ex com.android.game</param>
        /// <param name="contentDescription">view name or screen name</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator TrackScreenView(string appName, string contentDescription, string appVersion = null, string appId = null, string appInstallerId=null)
        {
            var gaString = GetPostParams(GoogleAnaliticsHitType.ScreenView, appName,appVersion,appId, appInstallerId,contentDescription);
            return SendResponce(gaString);
        }

        private static string GetPostParams(GoogleAnaliticsHitType hitType, params object[] parameters)
        {
            
            var baseString = string.Format("v={0}&tid={1}&cid={2}&t={3}", GoogleAnaliticsProtocolVersion, _googleAnaliticsId, _googleAnaliticsUserGuid, hitType.ToString().ToLower());
           

            switch (hitType)
            {
                case GoogleAnaliticsHitType.PageView:
                    baseString += CombineToString(parameters, "&dh=", "&dp=", "&dt=");
                    break;
                case GoogleAnaliticsHitType.Event:
                    baseString += CombineToString(parameters, "&ec=", "&ea=", "&el=","&ev=");
                    break;
                case GoogleAnaliticsHitType.Transaction:
                    baseString += CombineToString(parameters, "&ti=","&ta=","&tr=","&ts=","&tt=","&cu=");
                   break;
                case GoogleAnaliticsHitType.Item:
                    baseString += CombineToString(parameters, "&ti=","&in=","&ip=","&iq=","&ic=","&iv=","&cu=");
                    break;
                case GoogleAnaliticsHitType.Social:
                    baseString += CombineToString(parameters,"&sa=","&sn=","&st=");
                    break;
                case GoogleAnaliticsHitType.Exception:
                    baseString += CombineToString(parameters, "&exd=","&exf=");
                    break;
                case GoogleAnaliticsHitType.Timing:
                    baseString += CombineToString(parameters,"&utc=","&utv=","&utt=","&utl=");
                    break;
                case GoogleAnaliticsHitType.ScreenView:
                    baseString += CombineToString(parameters,"&an=","&av=","&aid=","&aiid=","&cd=");
                    break;
                default:
                    break;
            }

            var now = DateTime.Now;

            baseString += string.Format("&z={0}", now.Millisecond + _random.Next(now.Millisecond));

            return baseString;
        }
        
        private static IEnumerator SendResponce(string responce)
        {
            CheckInitialize();
            var www = default (WWW);
            if (!IsDebuging)
            {
                www = new WWW(GoogleAnaliticsURL, Encoding.UTF8.GetBytes(responce));
            }
            else
            {
                www = new WWW(GoogleAnaliticsURL + "?" +responce);
            }
            
            yield return www;
            if (IsDebuging)
            {
                Debug.Log(www.error);
                Debug.Log(www.text);
                Debug.Log(www.bytesDownloaded);
            }
        }

        private static void CheckInitialize() 
        {
            if (!_isInitialize)
            {
                throw new Exception("google analitic service not initialize");
            }
        }

        private static string CombineToString(object[] values,params string[] keys)
        {
            var result = "";
            for (int i = 0; i < keys.Length; i++)
            {
                var value = "";
                if (values[i].ObjeToStr(out value))
                {
                    result += keys[i] + (IsDebuging?WWW.EscapeURL(value):value);
                }
            }
            return result;
        }

        private static float RoundFloat(float currency)
        {
            var result = (int)(currency * 100.0f);
            return (float)result / 100.0f;
        }

        private static bool ObjeToStr(this object obj,out string value)
        {
            if (obj is string)
            {
                var stringValue = obj as string;
                if (!string.IsNullOrEmpty(stringValue))
                {
                    value = stringValue;
                    return true;
                }
            }
            else if (obj is float?)
            {
                var floatValue = obj as float?;
                if (floatValue.HasValue)
                {
                    value = RoundFloat(floatValue.Value).ToString();
                    return true;
                }
            }
            else if (obj is int?)
            {
                var intValue = obj as int?;
                if (intValue.HasValue)
                {
                    value = intValue.Value.ToString();
                    return true;
                }
            }
            else
            {
                if (obj != null)
                {
                    value = obj.ToString();
                    return true;
                }
            }
            value = "";
            return false;
        }
    }
}

namespace AdminPortalV8.Helpers
{
    public class AppConstants
    {
        //public const string EmailHost = "mail.axel-logic.com";
        //public const int EmailPort = 26; // 587
        //public const string EmailUserName = "apptest@axel-logic.com";
        //public const string EmailPassword = "Mquest0412*#";



        static AppConstants()
        {
            

        }

        public const string EmailHost = "smtp.gmail.com";
        public const int EmailPort = 587;
        public const string EmailUserName = "sgcares@blossomseeds.sg";
        public const string EmailPassword = "sbumxcuiyudscbup";
        public const string EmailUserName1 = "homehelp@blossomseeds.sg";
        public const string EmailPassword2 = "Blossom.4904";
        public const string WebUrlForCardReader = "http://45.32.105.34:5587" + "/{0}";

        

#if DEBUG
        #region 84 Server
        ////84 Server Web API
        //public const string APIUrl = "http://localhost:6000" + "/{0}";
        ////public const string APIUrl = "http://111.223.112.84:6000" + "/{0}";

        ////84 Server HTTP APK
        //public const string APIUrlForMETAndroid = "intent://111.223.112.84:5585/met/index/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http://111.223.112.84:5585/assets/apk/84METMQNFCWebX.apk;end";
        //public const string ApiUrlForMETDesktop = "http://111.223.112.84:5585/met/index";

        ////84 Server HTTPS APK
        ////public const string APIUrlForMETAndroid = "intent://demo.mquestsys.com/met/index/#Intent;scheme=smquest;package=webview.tinyapps.mquest;S.browser_fallback_url=https://demo.mquestsys.com/assets/apk/84METMQNFCWebX.apk;end";
        ////public const string ApiUrlForMETDesktop = "https://demo.mquestsys.com/met/index";

        ////84 Server HTTP Tablet
        //public const string APIUrlForInitialRegistrationAndroid = "intent://111.223.112.84:5587/InitialRegistration/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=https://111.223.112.84:5585/assets/apk/CentreMQNFCWebX.apk;end";
        //public const string APIUrlForInitialRegistrationDestop = "http://111.223.112.84:5587/InitialRegistration";
        //public const string APIUrlForAttendanceAndroid = "intent://111.223.112.84:5587/Attendance/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=https://111.223.112.84:5585/assets/apk/CentreMQNFCWebX.apk;end";
        //public const string ApiUrlForAttendanceDesktop = "http://111.223.112.84:5587/Attendance";
        #endregion
        #region 83 Server
        //84 Server Web API
        public const string APIUrl = "http://localhost:6000" + "/{0}";
        //public const string APIUrl = "https://202.73.50.124:5586" + "/{0}";
        //public const string APIUrl = "http://45.32.105.34:5586" + "/{0}";
        ////84 Server HTTP APK
        public const string APIUrlForMETAndroid = "intent://202.73.50.122:5585/met/index/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http://202.73.50.122:5585/assets/apk/83MET.apk;end";

        ////public const string ApiUrlForMETDesktop = "http://111.223.112.83:5585/met/index";
        public const string APIUrlForCBPAttendanceAndroid = "intent://202.73.50.122:5585/CBPAttendance/#Intent;scheme=mquest;package=webview.attendance.mquest;S.browser_fallback_url=http://202.73.50.122:5585/assets/apk/83CBPAttendance.apk;end";

        public const string ApiUrlForCBPAttendanceDesktop = "http://111.223.112.83:5585/CBPAttendance";

        //84 Server HTTPS APK
        //public const string APIUrlForMETAndroid = "intent://demo.mquestsys.com/met/index/#Intent;scheme=smquest;package=webview.tinyapps.mquest;S.browser_fallback_url=https://demo.mquestsys.com/assets/apk/84METMQNFCWebX.apk;end";
        public const string ApiUrlForMETDesktop = "https://demo.mquestsys.com/met/index";

        //84 Server HTTP Tablet
        //public const string APIUrlForInitialRegistrationAndroid = "intent://202.73.50.122:5587/InitialRegistration/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=https://202.73.50.122:5585/assets/apk/BS_Tablet.apk;end";
        public const string APIUrlForInitialRegistrationAndroid = "intent://202.73.50.122:5587/InitialRegistration/#Intent;scheme=mquest;package=webview.BSTablet.MQuest;S.browser_fallback_url=http://202.73.50.122:5585/assets/apk/BS_Tablet.apk;end";
        public const string APIUrlForInitialRegistrationDestop = "http://202.73.50.122:5587/InitialRegistration";
        public const string APIUrlForAttendanceAndroid = "intent://202.73.50.122:5587/Attendance/#Intent;scheme=mquest;package=webview.BSTablet.MQuest;S.browser_fallback_url=https://202.73.50.122:5585/assets/apk/BS_Tablet.apk;end";
        public const string ApiUrlForAttendanceDesktop = "http://202.73.50.122:5587/Attendance";
        public const string APIUrlForCBPInitialRegistration = "http://202.73.50.122:5587/CBPInitialRegistration/Index";
        #endregion
#else
        #region Cloud
        //Cloud Server Web API
        //public const string APIUrl = "http://localhost:5586" + "/{0}";
        //public const string APIUrl = "http://localhost:6000" + "/{0}";
        public const string APIUrl = "http://45.32.105.34:5586" + "/{0}";
       //public const string APIUrl = "https://202.73.50.124:5586" + "/{0}";

        //Cloud Server HTTP APK
        public const string APIUrlForMETAndroid = "intent://45.32.105.34:5585/met/index/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http://45.32.105.34:5585/assets/apk/LiveMET.apk;end";
        public const string ApiUrlForMETDesktop = "http://45.32.105.34:5585/MET/index";
        public const string APIUrlForCBPAttendanceAndroid = "intent://45.32.105.34:5585/CBPAttendance/#Intent;scheme=mquest;package=webview.attendance.mquest;S.browser_fallback_url=http://45.32.105.34:5585/assets/apk/LiveCBPAttendance.apk;end";
        public const string ApiUrlForCBPAttendanceDesktop = "http://45.32.105.34:5585/CBPAttendance";

        //Cloud Server HTTPS APK
        //public const string APIUrlForMETAndroid = "intent://demo.mquestsys.com/met/index/#Intent;scheme=smquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http%3A%2F%2Ftinyapps.info%2Fnt%2Fdownload.html;end";
        ////public const string APIUrlForMETAndroid = "intent://demo.mquestsys.com/met/index/#Intent;scheme=smquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http%3A%2F%2Ftinyapps.info%2Fnt%2Fdownload.html;end";
        //public const string ApiUrlForMETDesktop = "https://demo.mquestsys.com/met/index";

        //Cloud Server HTTP Tablet
        public const string APIUrlForInitialRegistrationAndroid = "intent://45.32.105.34:5587/InitialRegistration/#Intent;scheme=mquest;package=webview.BSTablet.MQuest;S.browser_fallback_url=http://45.32.105.34:5585/assets/apk/Live_BS_Tablet.apk;end";
        //public const string APIUrlForInitialRegistrationAndroid = "intent://45.32.105.34:5587/InitialRegistration/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http%3A%2F%2Ftinyapps.info%2Fnt%2Fdownload.html;end";
        public const string APIUrlForInitialRegistrationDestop = "http://45.32.105.34:5587/InitialRegistration";
        public const string APIUrlForAttendanceAndroid = "intent://45.32.105.34:5587/Attendance/#Intent;scheme=mquest;package=webview.BSTablet.MQuest;S.browser_fallback_url=http://45.32.105.34:5585/assets/apk/Live_BS_Tablet.apk;end";
        //public const string APIUrlForAttendanceAndroid = "intent://45.32.105.34:5587/Attendance/#Intent;scheme=mquest;package=webview.tinyapps.mquest;S.browser_fallback_url=http%3A%2F%2Ftinyapps.info%2Fnt%2Fdownload.html;end";
        public const string ApiUrlForAttendanceDesktop = "http://45.32.105.34:5587/Attendance";
        public const string APIUrlForCBPInitialRegistration = "http://45.32.105.34:5587/CBPInitialRegistration/Index";
        #endregion

#endif



        public const int PotraitOrientation = 1;
        public const int LandscapeOrientation = 2;

        public const bool EnablePhoto = false;
    }
}

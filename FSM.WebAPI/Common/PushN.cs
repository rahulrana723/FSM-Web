using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using PushSharp;
using PushSharp.Core;
using PushSharp.Apple;
using System.Configuration;
using System.Web.UI;

namespace FSM.WebAPI.Common
{
    public class PushN
    {
        public static string Result = "";
        /// <summary>
        /// Push Notification To I-Phone 
        /// </summary>
        /// <param name="DeviceToken"></param>
        /// <param name="Message"></param>
        public string PushToIphone(string DeviceToken, string Message, string Password, bool Production, string CertificatePath)
        {
            try
            {
                Result = "Failed";
                //Create our push services broker
                var push = new PushBroker();

                //Wire up the events for all the services that the broker registers
                push.OnNotificationSent += NotificationSent;
                push.OnChannelException += ChannelException;
                push.OnServiceException += ServiceException;
                push.OnNotificationFailed += NotificationFailed;
                push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                push.OnChannelCreated += ChannelCreated;
                push.OnChannelDestroyed += ChannelDestroyed;


                var Certificate = CertificatePath;
                //Fetch Certificate
                string temp = CertificatePath;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IPhoneCertificate\\" + Certificate);

                //string FullPath = temp + "\\" + Certificate;

                var appleCertificate = File.ReadAllBytes(temp);
                //var appleCertificate = File.ReadAllBytes("D:\\Latest Projects\\EnSource\\ENSourceAPI\\iPhoneCertificate/PushNotificationCert.p12");
                //IMPORTANT: If you are using a Development provisioning Profile, you must use the Sandbox push notification server 
                //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as 'false')
                //  If you are using an AdHoc or AppStore provisioning profile, you must use the Production push notification server
                //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 'true')
                string p12FilePassword = Password;
                bool isProductionMode = Production;

                push.RegisterAppleService(new ApplePushChannelSettings(isProductionMode, appleCertificate, p12FilePassword, true)); //Extension method

                //Fluent construction of an iOS notification
                //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets generated within your iOS app itself when the Application Delegate
                //  for registered for remote notifications is called, and the device token is passed back to you


                push.QueueNotification(new AppleNotification(DeviceToken)
                                           .ForDeviceToken(DeviceToken)
                                           .WithAlert(Message)
                                           .WithSound("default")
                                           .WithCustomItem("OutputCode", 2));


                Console.WriteLine("Waiting for Queue to Finish...");

                //Stop and wait for the queues to drains
                push.StopAllServices();

                Console.WriteLine("Queue Finished, press return to exit...");
                return Result;
                //Console.ReadLine();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Change Device Subscription
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldSubscriptionId"></param>
        /// <param name="newSubscriptionId"></param>
        /// <param name="notification"></param>
        static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            //Currently this event will only ever happen for Android GCM
            Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
        }

        /// <summary>
        /// Sent Notification 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        static void NotificationSent(object sender, INotification notification)
        {
            Result = "Notification sent successfully.";
            Console.Write("<script>alert('Hello');</script>");
            Console.WriteLine("Sent: " + sender + " -> " + notification);
            // ChannelDestroyed(sender);
        }

        /// <summary>
        /// In Case of Notification Failure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notification"></param>
        /// <param name="notificationFailureException"></param>
        void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
        {
            //  Result =
            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "alertMessage", "alert('Called from code-behind directly!');", true);
            Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
            Result = ((PushSharp.Apple.NotificationFailureException)(notificationFailureException)).ErrorStatusDescription;
            //ScriptManager.RegisterStartupScript((sender as Control), (sender as Control).GetType(), "alertMessage", "alert('Called from code-behind directly!');", true);
            //Response.Write("<script>alert('Hello');</script>");
            //ScriptManager.RegisterStartupScript((sender as Control), (sender as Control).GetType(), "err_msg", "alert('Dispatch assignment saved, but you forgot to click Confirm or Cancel!)');",
            //   true);

        }

        /// <summary>
        /// Exception in Channel or Network
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="channel"></param>
        /// <param name="exception"></param>
        static void ChannelException(object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        /// <summary>
        /// Exception in Service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="exception"></param>
        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
        }

        /// <summary>
        /// Device Subscription Expired
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="expiredDeviceSubscriptionId"></param>
        /// <param name="timestamp"></param>
        /// <param name="notification"></param>
        static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
        {
            Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
        }

        /// <summary>
        /// Channel Destroyed
        /// </summary>
        /// <param name="sender"></param>
        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine("Channel Destroyed for: " + sender);
        }

        /// <summary>
        /// Channel Created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="pushChannel"></param>
        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine("Channel Created for: " + sender);
        }

        /// <summary>
        /// Validate Server Certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /************************************************Android Notification********************************************************************************************************/
        /// <summary>
        /// Send GCM Notification for Andriod
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="message"></param>
        /// <param name="ProjectNumber"></param>
        /// <param name="Api_Key"></param>
        /// <returns></returns>
        public string SendGCMNotification(string deviceId, string message, string AndroiApiKey)
        {
            try
            {
                string SERVER_API_KEY = AndroiApiKey;
                var SENDER_ID = "1";
                var value = message;
                WebRequest tRequest;
                tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";
                Console.WriteLine(postData);
                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;

                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();

                if (sResponseFromServer.Contains("id="))
                {
                    Result = "Notification sent successfully.";
                }
                else
                {
                    Result = sResponseFromServer;
                }
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return Result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public string SendFCMNotification(string DeviceId, string Message, string postDataContentType = "application/json")
        //{
        //    ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateServerCertificate);
        //    // MESSAGE CONTENT
        //    string postData = "{\"registration_ids\": [ \"" + DeviceId + "\" ], " +
        //        "\"data\": {\"body\":\"" + Message + "\",\"title\":\"drugbee\",\"sound\":\"default\",\"color\":\"#000000\",\"icon\":\"notification_transparent\",\"tag\":\"drugbee\"}," +
        //        "\"notification\": {\"body\":\"" + Message + "\",\"title\":\"drugbee\",\"sound\":\"default\",\"color\":\"#000000\",\"icon\":\"notification_transparent\",\"click_action\":\"HandlePushNotification\",\"tag\":\"drugbee\"}}";
        //    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        //    // CREATE REQUEST
        //    HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //    Request.Method = "POST";
        //    Request.KeepAlive = false;
        //    Request.ContentType = postDataContentType;
        //    Request.Headers.Add(HttpRequestHeader.Authorization, "key=AIzaSyDZtrj8qbxpHRHJdlNkGMy6-rHYX-rEDB8");
        //    Request.ContentLength = byteArray.Length;
        //    Stream dataStream = Request.GetRequestStream();
        //    dataStream.Write(byteArray, 0, byteArray.Length);
        //    dataStream.Close();
        //    // SEND MESSAGE
        //    try
        //    {
        //        WebResponse Response = Request.GetResponse();
        //        HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
        //        if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
        //        {
        //            var text = "Unauthorized - need new token";
        //        }
        //        else if (!ResponseCode.Equals(HttpStatusCode.OK))
        //        {
        //            var text = "Response from web service isn't OK";
        //        }
        //        StreamReader Reader = new StreamReader(Response.GetResponseStream());
        //        string responseLine = Reader.ReadToEnd();
        //        Reader.Close();
        //        //  PushNotificationAndroid.Flag = 3;

        //        return responseLine;

        //    }
        //    catch (Exception ex)
        //    {

        //        //PushNotificationAndroid.Flag = 4;


        //    }
        //    return "error";
        //}
    }
}
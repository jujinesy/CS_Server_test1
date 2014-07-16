using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;

namespace SimpleHttp
{
    class LogHandling
    {
    }
    public sealed class mainLogSingleton
    {
        //private static volatile mainLogSingleton instance;
        //private static object syncRoot = new Object();

        //private mainLogSingleton() { }

        //public static mainLogSingleton Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new mainLogSingleton();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private mainLogSingleton()
        {
        }

        public static mainLogSingleton Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly mainLogSingleton instance = new mainLogSingleton();
        }







        private delegate void myDelegate2(object[] obj);
        private void updateProgress2(object[] obj)
        {
            
            StringBuilder sb = new StringBuilder();
            //Console.WriteLine("1 : " + theValue.ToString());
            //Form1.Instance.lbMainLog.Items.Add(str);
            //Console.Write("{0} ", score);            
            //foreach (string str in obj) // foreach(데이터형_변수 in 배열 이름)
            //{
            //    sb.Append(str);
            //}

            


            if (Form1.Instance.lbMainLog.Items.Count == Conf.lbMainLog)
                Form1.Instance.lbMainLog.Items.Clear();
            for (int i = 0; i < obj.Length ; i++)
            {
                sb.Append(obj[i].ToString());
            }
            Form1.Instance.lbMainLog.Items.Add(sb.ToString());
            //Form1.Instance.lbMainLog.Items.Add(obj[0]);
            Form1.Instance.lbMainLog.SetSelected(Form1.Instance.lbMainLog.Items.Count - 1, true);
        }

        public void addResponse(object[] obj)
        {
            Coordinate Coord;
            int idx;
            string data = obj[0].ToString();
            if (data.Equals("POST data"))
            {
                //string[] data2 = obj[2].ToString().Split('\n');
                //string dataSplit = data2[data2.Length - 1];
                //NameValueCollection qscoll = HttpUtility.ParseQueryString(dataSplit);
                NameValueCollection qscoll = HttpUtility.ParseQueryString(obj[2].ToString());
                if (qscoll["HTTP_URL"] == "ALIVE")
                {
                    if (!Database.Instance.select_getphonenumber(qscoll["PHONE_SERIAL_NUMBER"]).Equals(qscoll["PHONE_NUMBER"]))
                    {
                        if (!Database.Instance.update_phonenumber(new object[] { qscoll["PHONE_NUMBER"], qscoll["PHONE_SERIAL_NUMBER"] }))
                            Database.Instance.insert_phoneinfo(new object[] { qscoll["PHONE_SERIAL_NUMBER"], qscoll["PHONE_NUMBER"] });
                        Database.Instance.select_lvinit();
                    }
                    Uimanage.Instance.lvPhoneConnectTimeUpdate(qscoll["PHONE_SERIAL_NUMBER"]);                    //if (Database.Instance.select_getphonenumber(qscoll["PHONE_SERIAL_NUMBER"]).Equals(""))
                    //{
                    //    if (!Uimanage.Instance.getlvphonenumber(qscoll["PHONE_SERIAL_NUMBER"]).Equals(qscoll["PHONE_NUMBER"]))
                    //    {
                    //    }
                    //}
                    //else
                    //{
                    //    Database.Instance.insert_phoneinfo(new object[] { qscoll["PHONE_SERIAL_NUMBER"], qscoll["PHONE_NUMBER"] });
                    //    Database.Instance.select_lvinit();
                    //}
                    //Uimanage.Instance.lvPhoneConnectTimeUpdate(qscoll["PHONE_SERIAL_NUMBER"]);
                }
                else if(qscoll["HTTP_URL"] == "SMS")
                {
                    if (qscoll["CONTENTS"].Substring(0, 5).Equals("승인번호[") && qscoll["CONTENTS"].Substring(qscoll["CONTENTS"].Length - 5).Equals("[한게임]"))
                    {
                        idx = Database.Instance.select_GetIDXFromPhoneNumber(new object[] {qscoll["PHONE_NUMBER"], Conf.iSMS_TIME});
                        if (idx > 0)         
                        {   
                            Coord = Database.Instance.select_GetUserCoord(Database.Instance.select_GetUserPhoneNumberFromIDX(new object[] { idx }));
                            //회원일 경우에만.
                            if (Coord.AVAILABLE)
                            {
                                Database.Instance.update_LogPaymentApproval(new object[] { int.Parse(System.Text.RegularExpressions.Regex.Replace(qscoll["CONTENTS"].Substring(11), @"\D", "")), 
                                qscoll["CONTENTS"].Substring(5, 6), 
                                idx });


                                ThreadPoolQueue Start = ThreadPoolQueue.Instance;
                                Works JP = new Works(Coord, Coord.NAME + "님" + System.Environment.NewLine +
                                    "방금 요청하신 결제 번호는" + System.Environment.NewLine +
                                    "[" + qscoll["CONTENTS"].Substring(5, 6)  + "]입니다.");
                                Start.TellerLine.Enqueue(JP);
                            }
                        }
                        else
                        {
                            //인덱스가 없을경우 문자 결제 신청 안한거 미사용사용자 보낸거 또는 시간초과
                        }
                        //요금
                        //   string c = System.Text.RegularExpressions.Regex.Replace(qscoll["CONTENTS"].Substring(11), @"\D", "");
                        //스레드 큐
                        //ThreadPoolQueue Start = ThreadPoolQueue.Instance;
                        //Works JP = new Works(Conf.scn[0], qscoll["CONTENTS"]);
                        //Start.TellerLine.Enqueue(JP);
                        Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    }
                    else if (qscoll["CONTENTS"].Substring(0, 5).Equals("[한게임]"))
                    {
                        Database.Instance.update_LogPaymentFinish(new object[] { int.Parse(System.Text.RegularExpressions.Regex.Replace(qscoll["CONTENTS"].Substring(14), @"\D", "")),
                            qscoll["PHONE_NUMBER"], 
                             int.Parse(System.Text.RegularExpressions.Regex.Replace(qscoll["CONTENTS"].Substring(5,6), @"\D", ""))
                             });
                        Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    }
                    else if (qscoll["CONTENTS"].Substring(0, 1).Equals("!"))
                    {
                        int idxspace = qscoll["CONTENTS"].Substring(1).IndexOf(' ');
                        string cmd = qscoll["CONTENTS"].Substring(1, idxspace);
                        if (cmd.Equals("결제"))
                        {
                            Coord = Database.Instance.select_GetUserCoord(qscoll["FROM_PHONE_NUMBER"]);
                            //회원일 경우에만.
                            if (Coord.AVAILABLE)
                            {
                                try
                                {
                                    int request = int.Parse(qscoll["CONTENTS"].Substring(idxspace + 2));
                                    string requestPhoneNum = Database.Instance.select_SearchPhoneNumber(new object[] { request });

                                    if (!string.IsNullOrEmpty(requestPhoneNum))
                                    {
                                        //널이 아니면
                                        if (Database.Instance.select_GetIdentiTime(request) > Database.Instance.select_GetAvailableTime(request, Conf.iSMS_TIME, Conf.iAPPROVAL_TIME))
                                        {
                                            Database.Instance.insert_Log_Payment(new object[] { request, qscoll["FROM_PHONE_NUMBER"], requestPhoneNum });
                                            ThreadPoolQueue Start = ThreadPoolQueue.Instance;
                                            Works JP = new Works(Coord, Coord.NAME + "님" + System.Environment.NewLine +
                                                request.ToString() + "번 폰(" + requestPhoneNum + ")" + System.Environment.NewLine +
                                                "결제요청을 지금부터" + System.Environment.NewLine +
                                                Conf.iSMS_TIME + "초 동안 가능 합니다.");
                                            Start.TellerLine.Enqueue(JP);

                                        }
                                        else
                                        {
                                            ThreadPoolQueue Start = ThreadPoolQueue.Instance;
                                            Works JP = new Works(Coord, Coord.NAME + "님" + System.Environment.NewLine +
                                                (Database.Instance.select_GetAvailableTime(request, Conf.iSMS_TIME, Conf.iAPPROVAL_TIME) - Database.Instance.select_GetIdentiTime(request)).ToString() + "초 후에 가능 합니다.");
                                            Start.TellerLine.Enqueue(JP);
                                        }                                        
                                    }
                                    else
                                    {
                                        //널이면
                                        ThreadPoolQueue Start = ThreadPoolQueue.Instance;
                                        Works JP = new Works(Coord, Coord.NAME + "님" + System.Environment.NewLine +
                                            request.ToString() + "번은 결제가" + System.Environment.NewLine +
                                        "불가능한 순번 입니다." + System.Environment.NewLine +
                                        "다시 확인해 주세요.");
                                        Start.TellerLine.Enqueue(JP);
                                    }
                                }
                                catch (Exception e)
                                {
                                    //실패
                                }
                            }
                        }
                        Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    }
                }
            }
            else if (data.Equals("POST dddata"))
            {

            }


            
            //if (data.IndexOf("&") > -1)
            //{
            //    string[] data2 = data.Split('\n');
            //    string d = data2[data2.Length - 1];
            //    NameValueCollection qscoll = HttpUtility.ParseQueryString(d);

            //    if (qscoll["ALIVE"] != null && qscoll["ALIVE"] == "IAM")
            //        if (!this.db.exists("phoneNumber", qscoll["PHONE_SERIAL_NUMBER"]))
            //            this.db.write("phoneNumber", qscoll["PHONE_SERIAL_NUMBER"], qscoll["PHONE_NUMBER"]);
            //}

            //frm.Invoke(new myDelegate2(updateProgress2), new object[] { 1, 100 });
            Form1.Instance.Invoke(new myDelegate2(updateProgress2), new object[] { obj });

            
        }
    }
}
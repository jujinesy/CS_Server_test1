using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Threading;

namespace SimpleHttp
{
    public class Threads
    {
        Thread tpr;
        Thread th2;
        Thread th3;
        //Form1 frm;
        static bool bProgress;

        //private static volatile Threads instance;
        //private static object syncRoot = new Object();
        //private Threads() { }

        //public static Threads Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Threads();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private Threads()
        {
        }

        public static Threads Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Threads instance = new Threads();
        }




        // progress bar
        private delegate void DelegateProgress(int theValue, int theMax);
        private void updateProgress(int theValue, int theMax)
        {
            if (theMax != 0)
                Form1.Instance.progressBar1.Maximum = theMax;
            if (theValue > theMax)
                theValue = theMax;
            if (theValue < 0)
                theValue = 0;
            Form1.Instance.progressBar1.Value = theValue;
        }
        

        private void ThreadProgressRun()
        {
            bProgress = true;
            for (int i = 0, j = 20, k = 1;;)
            {
                while (bProgress)
                {
                    if (Form1.Instance.progressBar1.InvokeRequired)
                    {
                        Form1.Instance.Invoke(new DelegateProgress(updateProgress), new object[] { i += k, j });
                        //더미 데이터 넣는 부분입니다. 1초에 20~30건씩 쌓이는 것 표현
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264530&PHONE_NUMBER=01050860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264531&PHONE_NUMBER=01150860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264532&PHONE_NUMBER=01250860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264533&PHONE_NUMBER=01350860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264534&PHONE_NUMBER=01450860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264535&PHONE_NUMBER=01550860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264536&PHONE_NUMBER=01650860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264537&PHONE_NUMBER=01750860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264538&PHONE_NUMBER=01850860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264539&PHONE_NUMBER=01950860272" });
                        //mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", "HTTP_URL=ALIVE&ALIVE=IAM&PHONE_SERIAL_NUMBER=359797041264511&PHONE_NUMBER=02250860272" });
                        Thread.Sleep(100);
                    }
                    if (i > j+3 || i < 1)
                        k*=-1;
                }
                i = 0;
                if (Form1.Instance.progressBar1.InvokeRequired)
                {
                    Form1.Instance.Invoke(new DelegateProgress(updateProgress), new object[] { i, j });
                }
                break;
            }
        }



        // Th2

        private delegate void myDelegate2(int theValue, int theMax, Form1 frm);
        private void updateProgress2(int theValue, int theMax, Form1 frm)
        {
            //Console.WriteLine("1 : " + theValue.ToString());
            frm.lbMainLog.Items.Add(theValue.ToString());
            frm.lbMainLog.SetSelected(frm.lbMainLog.Items.Count - 1, true);
        }

        private void myThread2(Form1 frm)
        {
            for (int i = 0; i < 100; i++)
            {
                frm.Invoke(new myDelegate2(updateProgress2), new object[] { i, 100 }, frm);
                Thread.Sleep(100);
            }
            //th2.Abort();
        }


        // Th3
        private delegate void myDelegate3(int theValue, int theMax, Form1 frm);
        private void updateProgress3(int theValue, int theMax, Form1 frm)
        {
            //Console.WriteLine("2 : " + theValue.ToString());
            frm.lbMainLog.Items.Add("2 > " + theValue.ToString());
            frm.lbMainLog.SetSelected(frm.lbMainLog.Items.Count - 1, true);
        }

        private void myThread3(Form1 frm)
        {
            for (int i = 0; i < 100; i += 2)
            {
                frm.Invoke(new myDelegate2(updateProgress3), new object[] { i, 50 }, frm);
                Thread.Sleep(80);
            }
            //th1.Abort();
        }



        public void Start()
        {
            tpr = new Thread(new ThreadStart(ThreadProgressRun));
            //th2 = new Thread(new ThreadStart(myThread2));
            //th3 = new Thread(new ThreadStart(myThread3));
            
            tpr.Start();
            //th2.Start();
            //th3.Start();


        }

        public void Stop(bool closing)
        {
            if (closing)
                tpr.Abort();
            bProgress = false;
        }
    }    
}

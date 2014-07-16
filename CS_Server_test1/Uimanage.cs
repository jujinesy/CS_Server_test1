using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
    
namespace SimpleHttp
{
    public class Uimanage
    {
        //private static volatile Uimanage instance;
        //private static object syncRoot = new Object();
        //private Uimanage() { }

        //public static Uimanage Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Uimanage();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private Uimanage()
        {
        }

        public static Uimanage Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Uimanage instance = new Uimanage();
        }



        public void lvPhoneConnectTimeUpdate(string serial)
        {
            if (Form1.Instance.listView1.InvokeRequired)
            {
                Form1.Instance.Invoke(new DelegatelvPhoneConnectTimeUpdate(lvPhoneConnectTimeUpdateInvoke), serial);
            }
        }

        private delegate void DelegatelvPhoneConnectTimeUpdate(string serial);
        private void lvPhoneConnectTimeUpdateInvoke(string serial)
        {
            for (int i = 0; i < Form1.Instance.listView1.Items.Count; i++)
            {
                if (Form1.Instance.listView1.Items[i].SubItems[2].Text.Equals(serial))
                {
                    Form1.Instance.listView1.Items[i].SubItems[1].Text = DateTime.Now.ToLocalTime().ToLongTimeString().Substring(3);
                }
            }
        }




        public string getlvphonenumber(string serial)
        {
            if (Form1.Instance.listView1.InvokeRequired)
            {
                return (string)Form1.Instance.Invoke(new Delegategetlvphonenumber(getlvphonenumberInvoke), serial);
            }
            return "";
        }

        private delegate string Delegategetlvphonenumber(string serial);
        private string getlvphonenumberInvoke(string serial)
        {
            for (int i = 0; i < Form1.Instance.listView1.Items.Count; i++)
            {
                if (Form1.Instance.listView1.Items[i].SubItems[2].Text.Equals(serial))
                {
                    return Form1.Instance.listView1.Items[i].SubItems[3].Text.ToString();
                }
            }
            return "";
        }

    }
}

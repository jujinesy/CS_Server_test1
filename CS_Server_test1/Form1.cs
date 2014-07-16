using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleHttp
{
    public partial class Form1 : Form
    {
        static bool btStartAndStop = true;
        KMActivityHook actHook;

        //private static volatile Form1 instance;
        //private static object syncRoot = new Form1();
        //private Form1() {
        //    InitializeComponent();
        //}

        //public static Form1 Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Form1();
        //            }
        //        }
        //        return instance;
        //    }
        //}


        private Form1()
        {
            InitializeComponent();
        }

        public static Form1 Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Form1 instance = new Form1();
        }


        private void evLoad(object sender, EventArgs e)
        {
            // TODO: 이 코드는 데이터를 'dBDataSet.phone' 테이블에 로드합니다. 필요한 경우 이 코드를 이동하거나 제거할 수 있습니다.
            this.phoneTableAdapter.Fill(this.dBDataSet.phone);

            tbLocalIp.Text = Util.Instance.getclientIp;

            //폰 정보 갱신
            Database.Instance.select_lvinit();

            //구매자 정보 갱신
            Database.Instance.select_lvinitPayment();

            //거래중 정보 갱신
            Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);

            actHook = new KMActivityHook();
            actHook.OnMouseActivity += new MouseEventHandler(Mouse);

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //HttpServer
            
            if (btStartAndStop)
            {
                //프로그레스바 움직이기
                Threads.Instance.Start();

                //스레드풀 돌기
                ThreadPool.Instance.Start();

                //버튼 이벤트
                button1.Text = "중지";
                btStartAndStop = !btStartAndStop;

                //HttpServerMain 포트 넣어서 웹서버 돌림 포트는 이론상 0~65535
                HttpServerMain.Instance.Start(int.Parse(tbPort.Text));

                tbPort.ReadOnly = true;
            }
            else
            {
                Threads.Instance.Stop(false);

                //스레드풀 돌기 멈춤
                ThreadPool.Instance.Stop(false);

                button1.Text = "시작";
                btStartAndStop = !btStartAndStop;

                HttpServerMain.Instance.Stop();

                tbPort.ReadOnly = false;
            }

        }

        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            if (!btStartAndStop)
            {
                Threads.Instance.Stop(true);

                //스레드풀 돌기 멈춤
                ThreadPool.Instance.Stop(true);

                button1.Name = "시작";

                HttpServerMain.Instance.Stop();
            }
        }

        private void evKeyPress_PortFilter(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true; 
            }            
        }

        private void phoneBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.phoneBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dBDataSet);

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //Database.Instance.update_phonenumber(new object[] { "12","352454053189216"});
            //Database.Instance.select_lvinit();

            //"2014-02-26 오후 4:22"
            //listView2.Items[0].SubItems[2].Text = DateTime.Now.ToLocalTime().ToShortDateString() + " " + DateTime.Now.ToLocalTime().ToShortTimeString();
            //listView2.Items[0].SubItems[2].Text=Control.MousePosition.X.ToString();




            //ThreadPoolQueue Start = ThreadPoolQueue.Instance;
            //Works JP = new Works(Conf.scn[0], "문자내용이 이래요 ㅋ");
            //Start.TellerLine.Enqueue(JP);
            switch(int.Parse(textBox4.Text))
            {
                case 0:
                    //폰 순서 바꾸기
                    Database.Instance.update_PhoneOrder(new object[] { 3, "359797041264332" });
                    Database.Instance.select_lvinit();
                    break;
                case 1:
                    //결제 사용자 추가하기 이름, 입금돈, 거래중돈, 폰번호
                    Database.Instance.insert_payuserinfo(new object[] { "사라미름", 100000, 0, "01088112207"});
                    Database.Instance.select_lvinitPayment();
                    break;
                case 2:
                    //결제사용자 순서 바꾸기
                    Database.Instance.update_PayUserOrder(new object[] { 1, 1 });
                    Database.Instance.select_lvinitPayment();
                    break;
                case 3:
                    // IDX 1번 클릭위치 변경 
                    Conf.iCoordUpdate = 1;
                    break;
                case 4:                    
                    //식별자로 폰번호 찾기
                    MessageBox.Show( Database.Instance.select_SearchPhoneNumber(new object[] { 3 }) );
                    break;
                case 5:
                    //폰 식별자, 요청한 번호, 요청 할번호
                    Database.Instance.insert_Log_Payment(new object[] { 3, "0108811207", "01050763027" });
                    Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    break;
                case 6:
                    //진행중 갱싱
                    Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    break;
                case 7:
                    MessageBox.Show( Database.Instance.select_GetIdentiTime(3).ToString());
                    break;
                case 8:
                    if ( Database.Instance.select_GetIdentiTime(3) > Database.Instance.select_GetAvailableTime( 3, Conf.iSMS_TIME, Conf.iAPPROVAL_TIME ) )
                    {
                        Database.Instance.insert_Log_Payment(new object[] { 3, "01088112207", "01050763027" });
                    }
                    else
                    {
                        MessageBox.Show( (Database.Instance.select_GetAvailableTime(3, Conf.iSMS_TIME, Conf.iAPPROVAL_TIME) - Database.Instance.select_GetIdentiTime(3)).ToString() + "초 후에 가능함.");
                    }
                    Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    break;
                case 9:
                    //테이블 영향받은 갯수대로 나옴 어프로발
                    MessageBox.Show( Database.Instance.update_LogPaymentApproval(new object[] { 70000, "011233", 12 }).ToString() );
                    Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    break;
                case 10:
                    //테이블 영향받은 갯수대로 나옴 피니시
                    MessageBox.Show(Database.Instance.update_LogPaymentFinish(new object[] { 110000, "01050763027", 9660 }).ToString());
                    Database.Instance.select_lvinitLog_Payment(Conf.iSMS_TIME, Conf.iAPPROVAL_TIME);
                    break;
                case 11:
                    //문자온번호로 인데스 구하기
                    MessageBox.Show(Database.Instance.select_GetIDXFromPhoneNumber(new object[] {"01050763027", Conf.iSMS_TIME}).ToString());
                    break;
                case 12:
                    break;
                case 13:
                    break;
                default :
                    break;
            }        
        }

        public void Mouse(object sender, MouseEventArgs e)
        {            
            if ( Conf.iCoordUpdate > 0 && e.Button.ToString().Equals("Left") && e.Clicks > 0  )
            {
                Database.Instance.update_payuserpoint(new object[] { e.X, e.Y, Conf.iCoordUpdate });
                Conf.iCoordUpdate = 0;
                Database.Instance.select_lvinitPayment();
            } 
        }
    }
}
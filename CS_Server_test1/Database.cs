using System;
using System.Data.SqlServerCe;

//using System.Collections.Generic;
//using System.Text;
//Path 쓸려고
using System.IO;

namespace SimpleHttp
{
    //class Database
    //{
    //    private string dir;

    //    public Database()
    //    {
    //        this.dir = Path.Combine(Environment.CurrentDirectory, "data");

    //        if (!Directory.Exists(this.dir))
    //            Directory.CreateDirectory(this.dir);

    //        this.createCategory("phoneNumber");
    //    }

    //    public void createCategory(string category)
    //    {
    //        if (!Directory.Exists(this.dir + @"\" + category))
    //            Directory.CreateDirectory(this.dir + @"\" + category);
    //    }

    //    public void deleteCategory(string category)
    //    {
    //        if (Directory.Exists(this.dir + @"\" + category))
    //            Directory.Delete(this.dir + @"\" + category, true);
    //    }

    //    public bool exists(string category, string key)
    //    {
    //        return Directory.Exists(this.dir + @"\" + category + @"\" + key);
    //    }

    //    public void write(string category, string key, string value)
    //    {
    //        StreamWriter writer = new StreamWriter(this.dir + @"\" + category + @"\" + key+".txt");
    //        //\r\n
    //        writer.Write(value + System.Environment.NewLine + value);
    //        //writer.W
    //        writer.Close();
    //    }

    //    public string read(string category, string key)
    //    {
    //        StreamReader reader = new StreamReader(this.dir + @"\" + category + @"\" + key);
    //        string value = reader.ReadToEnd();

    //        reader.Close();

    //        return value;
    //    }

    //    public void delete(string category, string key)
    //    {
    //        File.Delete(this.dir + @"\" + category + @"\" + key);
    //    }
    //}

    public class Database
    {
        private static string connString = @"Data Source=|DataDirectory|\DB.sdf";
        //private static volatile Database instance;
        //private static object syncRoot = new Database();
        
        //private Database() { }

        //public static Database Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Database();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private Database()
        {
        }

        public static Database Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Database instance = new Database();
        }


        public bool insert_phoneinfo(object[] obj)
        {
            string sql =
                "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER], [NUMBER_CHANGE_COUNT], [PAYMENT_COUNT], [LEFT_CHARGE], [ORDER]) " +
            "VALUES (@SERIAL_NUMBER, @NUMBER, @NUMBER_CHANGE_COUNT, @PAYMENT_COUNT, @LEFT_CHARGE, @ORDER)";
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.Add("@SERIAL_NUMBER", obj[0].ToString());
            cmd.Parameters.Add("@NUMBER", obj[1].ToString());
            cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            cmd.Parameters.AddWithValue("@PAYMENT_COUNT", 0);
            cmd.Parameters.AddWithValue("@LEFT_CHARGE", 0);
            cmd.Parameters.AddWithValue("@ORDER", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        public bool insert_payuserinfo(object[] obj)
        {
            string sql =
                "INSERT INTO paymentconditions ([NAME], [BALANCE], [HANDLING_BALANCE], [PHONE_NUMBER], [POINT_X], [POINT_Y], [ORDER]) " +
            "VALUES (@NAME, @BALANCE, @HANDLING_BALANCE, @PHONE_NUMBER, @POINT_X, @POINT_Y, @ORDER)";
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.Add("@NAME", obj[0].ToString());
            cmd.Parameters.AddWithValue("@BALANCE", obj[1]);
            cmd.Parameters.AddWithValue("@HANDLING_BALANCE", obj[2]);
            cmd.Parameters.Add("@PHONE_NUMBER", obj[3].ToString());
            cmd.Parameters.AddWithValue("@POINT_X", 0);
            cmd.Parameters.AddWithValue("@POINT_Y", 0);
            cmd.Parameters.AddWithValue("@ORDER", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        public bool insert_Log_Payment(object[] obj)
        {
            string sql =
                "INSERT INTO log_payment( [IDENTIFICATION_NUMBER], [FROM_PHONE_NUMBER], [TO_PHONE_NUMBER], [SMS_TIME] )" +
            "VALUES (@IDENTIFICATION_NUMBER, @FROM_PHONE_NUMBER, @TO_PHONE_NUMBER, GETDATE() )";
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.AddWithValue("@IDENTIFICATION_NUMBER", obj[0]);
            cmd.Parameters.Add("@FROM_PHONE_NUMBER", obj[1].ToString());
            cmd.Parameters.Add("@TO_PHONE_NUMBER", obj[2].ToString());
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        public string select_getphonenumber(string serial_number)
        {
            string query = "SELECT [NUMBER] FROM phone WHERE [SERIAL_NUMBER] = @serial_number";

            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);
            conn.Open();

            cmd.Parameters.Add("@serial_number", serial_number);
            SqlCeDataReader rdr = cmd.ExecuteReader();
            try
            {
                //newProdID = (Int32)cmd.ExecuteScalar();
                while (rdr.Read())
                {
                    return rdr.GetString(0);
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }



            return "";

        }

        public bool select_lvinit()
        {
            //string query = "SELECT [Order ID], [Customer] FROM Orders";
            string query = "SELECT * FROM phone order by [ORDER]";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();


            Form1.Instance.listView1.Items.Clear();


            try
            {
                // Iterate through the results
                //

                while (rdr.Read())
                {
                    //newList.SubItems.Add(DateTime.Now.ToLongTimeString());
                    //int val1 = rdr.GetInt32(0);
                    //string val2 = rdr.GetString(1);
                    System.Windows.Forms.ListViewItem newList = new System.Windows.Forms.ListViewItem(rdr["ORDER"].ToString());
                    newList.SubItems.Add("접속 대기");


                    //if (!rdr.IsDBNull(3))
                    //newList.SubItems.Add(rdr.GetString(1));
                    if (!rdr["SERIAL_NUMBER"].Equals(""))
                        newList.SubItems.Add(rdr["SERIAL_NUMBER"].ToString());
                    if (!rdr["NUMBER"].Equals(""))
                        newList.SubItems.Add(rdr["NUMBER"].ToString());
                    if (!rdr["NUMBER_CHANGE_COUNT"].Equals(""))
                        newList.SubItems.Add(rdr["NUMBER_CHANGE_COUNT"].ToString());
                    if (!rdr["PAYMENT_COUNT"].Equals(""))
                        newList.SubItems.Add(rdr["PAYMENT_COUNT"].ToString());
                    if (!rdr["LEFT_CHARGE"].Equals(""))
                        newList.SubItems.Add(rdr["LEFT_CHARGE"].ToString());
                    if (!rdr["IDX"].Equals(""))
                        newList.SubItems.Add(rdr["IDX"].ToString());
                    //if (!rdr.IsDBNull(3))
                    //    newList.SubItems.Add(rdr.GetInt32(3).ToString());
                    //if (!rdr.IsDBNull(4))
                    //    newList.SubItems.Add(rdr.GetInt32(4).ToString());
                    //if (!rdr.IsDBNull(5))
                    //    newList.SubItems.Add(rdr.GetInt32(5).ToString());
                    Form1.Instance.listView1.Items.Add(newList);
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }

            return true;
        }

        public bool select_lvinitPayment()
        {
            //string query = "SELECT [Order ID], [Customer] FROM Orders";
            string query = "SELECT * FROM paymentconditions order by [ORDER]";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            SqlCeDataReader rdr = cmd.ExecuteReader();


            Form1.Instance.listView2.Items.Clear();


            try
            {
                // Iterate through the results
                //

                while (rdr.Read())
                {
                    //newList.SubItems.Add(DateTime.Now.ToLongTimeString());
                    //int val1 = rdr.GetInt32(0);
                    //string val2 = rdr.GetString(1);
                    System.Windows.Forms.ListViewItem newList = new System.Windows.Forms.ListViewItem(rdr["ORDER"].ToString());
                    if (!rdr["NAME"].Equals(""))
                        newList.SubItems.Add(rdr["NAME"].ToString());
                    if (!rdr["PHONE_NUMBER"].Equals(""))
                        newList.SubItems.Add(rdr["PHONE_NUMBER"].ToString());
                    if (!rdr["BALANCE"].Equals(""))
                        newList.SubItems.Add(rdr["BALANCE"].ToString());
                    if (!rdr["HANDLING_BALANCE"].Equals(""))
                        newList.SubItems.Add(rdr["HANDLING_BALANCE"].ToString());
                    if (!rdr["POINT_X"].Equals(""))
                        newList.SubItems.Add(rdr["POINT_X"].ToString());
                    if (!rdr["POINT_Y"].Equals(""))
                        newList.SubItems.Add(rdr["POINT_Y"].ToString());
                    if (!rdr["IDX"].Equals(""))
                        newList.SubItems.Add(rdr["IDX"].ToString());

                    Form1.Instance.listView2.Items.Add(newList);
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }

            return true;
        }

        public string select_SearchPhoneNumber(object[] obj)
        {
            if (int.Parse(obj[0].ToString()) < 0)
                return null;
            //string query = "SELECT [Order ID], [Customer] FROM Orders";
            string query = "SELECT * FROM phone Where [ORDER] = @ORDER";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.Add("@ORDER", obj[0]);
            SqlCeDataReader rdr = cmd.ExecuteReader();
            
            try
            {
                while (rdr.Read())
                {
                    return rdr["NUMBER"].ToString();
                }
            }
            finally
            {
                rdr.Close();
                conn.Close();
            }

            return null;
        }

        public bool select_lvinitLog_Payment(int SMS_TIME, int APPROVAL_TIME)
        {
            //SELECT  GETDATE() AS Expr1, IDX, IDENTIFICATION_NUMBER, FROM_PHONE_NUMBER, TO_PHONE_NUMBER, 
            //               SMS_TIME, CASH, APPROVAL_NUMBER, APPROVAL_TIME, BALANCE, FINISH_TIME
            //FROM     log_payment
            //WHERE  (DATEDIFF(second, SMS_TIME, GETDATE()) < 20) AND (FINISH_TIME IS NULL) OR
            //               (FINISH_TIME IS NULL) AND (DATEDIFF(second, APPROVAL_TIME, GETDATE()) < 40)
            //ORDER BY IDX
            string query = "SELECT * FROM log_payment " +
                            "WHERE DATEDIFF(second, [SMS_TIME], GETDATE()) < @SMS_TIME AND ([FINISH_TIME] is NULL) OR " +
                            "DATEDIFF(second, [APPROVAL_TIME], GETDATE()) < @APPROVAL_TIME AND ([FINISH_TIME] is NULL) " +
                            "order by [IDX]";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.AddWithValue("@SMS_TIME", SMS_TIME);
            cmd.Parameters.AddWithValue("@APPROVAL_TIME", APPROVAL_TIME);
            SqlCeDataReader rdr = cmd.ExecuteReader();


            Form1.Instance.listView3.Items.Clear();


            try
            {
                // Iterate through the results
                //
                while (rdr.Read())
                {
                    //newList.SubItems.Add(DateTime.Now.ToLongTimeString());
                    //int val1 = rdr.GetInt32(0);
                    //string val2 = rdr.GetString(1);
                    System.Windows.Forms.ListViewItem newList = new System.Windows.Forms.ListViewItem(rdr["IDX"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("IDENTIFICATION_NUMBER")))
                        newList.SubItems.Add(rdr["IDENTIFICATION_NUMBER"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("FROM_PHONE_NUMBER")))
                        newList.SubItems.Add(rdr["FROM_PHONE_NUMBER"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("TO_PHONE_NUMBER")))
                        newList.SubItems.Add(rdr["TO_PHONE_NUMBER"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("SMS_TIME")))
                        newList.SubItems.Add(rdr["SMS_TIME"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("CASH")))
                        newList.SubItems.Add(rdr["CASH"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("APPROVAL_NUMBER")))
                        newList.SubItems.Add(rdr["APPROVAL_NUMBER"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("APPROVAL_TIME")))
                        newList.SubItems.Add(rdr["APPROVAL_TIME"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("BALANCE")))
                        newList.SubItems.Add(rdr["BALANCE"].ToString());
                    if (!rdr.IsDBNull(rdr.GetOrdinal("FINISH_TIME")))
                        newList.SubItems.Add(rdr["FINISH_TIME"].ToString());

                    Form1.Instance.listView3.Items.Add(newList);
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }

            return true;
        }

        public int select_GetIdentiTime(int IDENTIFICATION_NUMBER)
        {
            //select MIN(DATEDIFF(second, b.TIME, GETDATE())) AS SMS_TIME
            //from (
            //select SMS_TIME AS TIME
            //from log_payment
            //where FINISH_TIME is NULL AND IDX in
            //(select DISTINCT a.IDX as IDX
            //from
            //(SELECT  MAX(IDX) AS IDX
            //FROM     log_payment
            //WHERE  (IDENTIFICATION_NUMBER = 3)
            //GROUP BY IDENTIFICATION_NUMBER ) as a)
            //UNION
            //select APPROVAL_TIME AS TIME
            //from log_payment
            //where FINISH_TIME is NULL AND IDX in
            //(select DISTINCT a.IDX as IDX
            //from
            //(SELECT  MAX(IDX) AS IDX
            //FROM     log_payment
            //WHERE  (IDENTIFICATION_NUMBER = 3)
            //GROUP BY IDENTIFICATION_NUMBER ) as a)
            //) AS b
            string query = "select MIN(DATEDIFF(second, b.TIME, GETDATE())) AS TIME " +
                            "from ( " +
                            "select SMS_TIME AS TIME " +
                            "from log_payment " +
                            "where FINISH_TIME is NULL AND IDX in " +
                            "(select DISTINCT a.IDX as IDX " +
                            "from " +
                            "(SELECT  MAX(IDX) AS IDX " +
                            "FROM     log_payment " +
                            "WHERE  (IDENTIFICATION_NUMBER = @IDENTIFICATION_NUMBER) " +
                            "GROUP BY IDENTIFICATION_NUMBER ) as a) " +
                            "UNION " +
                            "select APPROVAL_TIME AS TIME " +
                            "from log_payment " +
                            "where FINISH_TIME is NULL AND IDX in " +
                            "(select DISTINCT a.IDX as IDX " +
                            "from " +
                            "(SELECT  MAX(IDX) AS IDX " +
                            "FROM     log_payment " +
                            "WHERE  (IDENTIFICATION_NUMBER = @IDENTIFICATION_NUMBER) " +
                            "GROUP BY IDENTIFICATION_NUMBER ) as a) " +
                            ") AS b";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.AddWithValue("@IDENTIFICATION_NUMBER", IDENTIFICATION_NUMBER);
            SqlCeDataReader rdr = cmd.ExecuteReader();

            try
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(rdr.GetOrdinal("TIME")))
                        return int.Parse(rdr["TIME"].ToString());
                    return Conf.iAPPROVAL_TIME + 1;
                }
                else
                {
                    return Conf.iAPPROVAL_TIME + 1;
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }
        }

        public int select_GetAvailableTime(int IDENTIFICATION_NUMBER, int SMS_TIME, int APPROVAL_TIME)
        {
            //select APPROVAL_TIME
            //from log_payment
            //where  FINISH_TIME is NULL AND IDX in
            //(select DISTINCT a.IDX as IDX
            //from
            //(SELECT  MAX(IDX) AS IDX
            //FROM     log_payment
            //WHERE  (IDENTIFICATION_NUMBER = 3)
            //GROUP BY IDENTIFICATION_NUMBER ) as a)
            string query = "select [SMS_TIME], [APPROVAL_TIME] " +
                            "from log_payment " +
                            "where [FINISH_TIME] is NULL AND [IDX] in " +
                            "(select DISTINCT a.IDX as IDX " +
                            "from " +
                            "(SELECT  MAX([IDX]) AS IDX " +
                            "FROM     log_payment " +
                            "WHERE  ([IDENTIFICATION_NUMBER] = @IDENTIFICATION_NUMBER) " +
                            "GROUP BY [IDENTIFICATION_NUMBER] ) as a)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.AddWithValue("@IDENTIFICATION_NUMBER", IDENTIFICATION_NUMBER);
            SqlCeDataReader rdr = cmd.ExecuteReader();

            try
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(rdr.GetOrdinal("SMS_TIME")))
                    {
                        //SMS_TIME가 널이 아니면
                        if (!rdr.IsDBNull(rdr.GetOrdinal("APPROVAL_TIME")))
                        {
                            //APPROVAL_TIME가 널이 아니면
                            return APPROVAL_TIME;
                        }
                        else
                        {
                            //APPROVAL_TIME이 널이면
                            return SMS_TIME;
                        }
                    }
                    else
                    {
                        //SMS_TIME가 널이면
                        return APPROVAL_TIME;                        
                    }
                }
                else
                {
                    //검색결과가 없으면
                    return APPROVAL_TIME;
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }
        }

        public Coordinate select_GetUserCoord(string PHONE_NUMBER)
        {
            Coordinate result = new Coordinate();
            result.AVAILABLE = false;
            //SELECT  NAME, POINT_X, POINT_Y
            //FROM     paymentconditions
            //WHERE  (PHONE_NUMBER = '01088112207')
            string query = "SELECT  [NAME], [POINT_X], [POINT_Y] " +
                           "FROM     paymentconditions " +
                           "WHERE  ([PHONE_NUMBER] = @PHONE_NUMBER)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.Add("@PHONE_NUMBER", PHONE_NUMBER);
            SqlCeDataReader rdr = cmd.ExecuteReader();

            try
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(rdr.GetOrdinal("NAME")))
                    {
                        //NAME가 널이 아니면
                        result.AVAILABLE = true;
                        result.NAME = rdr["NAME"].ToString();
                        result.X = int.Parse(rdr["POINT_X"].ToString());
                        result.Y = int.Parse(rdr["POINT_Y"].ToString());
                        return result;
                    }
                    else
                    {
                        //NAME가 널이면
                        return result;
                    }
                }
                else
                {
                    //검색결과가 없으면
                    return result;
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }
        }

        public int select_GetIDXFromPhoneNumber(object[] obj)
        {
            //select IDX
            //from log_payment
            //where APPROVAL_TIME is NULL AND IDX in
            //(select DISTINCT a.IDX as IDX
            //from
            //(SELECT  MAX(IDX) AS IDX
            //FROM     log_payment
            //WHERE  (TO_PHONE_NUMBER = '01050763027')
            //GROUP BY IDENTIFICATION_NUMBER ) as a)
            string query = "select [IDX] " +
                            "from log_payment " +
                            "where [APPROVAL_TIME] is NULL AND DATEDIFF(second, SMS_TIME, GETDATE()) < @TIME AND [IDX] in " +
                            "(select DISTINCT a.IDX as IDX " +
                            "from " +
                            "(SELECT  MAX([IDX]) AS IDX " +
                            "FROM     log_payment " +
                            "WHERE  ([TO_PHONE_NUMBER] = @TO_PHONE_NUMBER) " +
                            "GROUP BY [IDENTIFICATION_NUMBER] ) as a)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.Add("@TO_PHONE_NUMBER", obj[0]);
            cmd.Parameters.AddWithValue("@TIME", obj[1]);
            SqlCeDataReader rdr = cmd.ExecuteReader();

            try
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(rdr.GetOrdinal("IDX")))
                    {
                        //IDX가 널이 아니면
                        return int.Parse(rdr["IDX"].ToString());
                    }
                    else
                    {
                        //IDX가 널이면
                        return 0;
                    }
                }
                else
                {
                    //검색결과가 없으면
                    return 0;
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }
        }

        public string select_GetUserPhoneNumberFromIDX(object[] obj)
        {
            //SELECT  FROM_PHONE_NUMBER
            //FROM     log_payment
            //WHERE  (IDX = 20)
            string query = "SELECT  FROM_PHONE_NUMBER " +
                           "FROM     log_payment " +
                           "WHERE  ([IDX] = @IDX)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(query, conn);

            conn.Open();
            cmd.Parameters.AddWithValue("@IDX", obj[0]);
            SqlCeDataReader rdr = cmd.ExecuteReader();

            try
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(rdr.GetOrdinal("FROM_PHONE_NUMBER")))
                    {
                        //IDX가 널이 아니면
                        return rdr["FROM_PHONE_NUMBER"].ToString();
                    }
                    else
                    {
                        //IDX가 널이면
                        return null;
                    }
                }
                else
                {
                    //검색결과가 없으면
                    return null;
                }
            }
            finally
            {
                // Always call Close when done reading
                //
                rdr.Close();

                // Always call Close when done reading
                //
                conn.Close();
            }
        }
        
        public bool update_phonenumber(object[] obj)
        {
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER], [NUMBER_CHANGE_COUNT], [PAYMENT_COUNT], [LEFT_CHARGE], [ORDER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER, @NUMBER_CHANGE_COUNT, @PAYMENT_COUNT, @LEFT_CHARGE, @ORDER)";
            int result = -1;
            string sql = "UPDATE phone SET [NUMBER] = @NUMBER, [NUMBER_CHANGE_COUNT] = [NUMBER_CHANGE_COUNT]+1 WHERE [SERIAL_NUMBER]=@SERIAL_NUMBER";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.Add("@NUMBER", obj[0].ToString());
            cmd.Parameters.Add("@SERIAL_NUMBER", obj[1].ToString());
            //cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                result = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            if (result > 0)
                return true;
            return false;
        }

        public bool update_payuserpoint(object[] obj)
        {
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER], [NUMBER_CHANGE_COUNT], [PAYMENT_COUNT], [LEFT_CHARGE], [ORDER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER, @NUMBER_CHANGE_COUNT, @PAYMENT_COUNT, @LEFT_CHARGE, @ORDER)";
            int result = -1;
            string sql = "UPDATE paymentconditions SET [POINT_X] = @POINT_X, [POINT_Y] = @POINT_Y WHERE [IDX]=@IDX";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.Add("@POINT_X", obj[0]);
            cmd.Parameters.Add("@POINT_Y", obj[1]);
            cmd.Parameters.Add("@IDX", obj[2]);
            //cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                result = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            if (result > 0)
                return true;
            return false;
        }

        public bool update_PhoneOrder(object[] obj)
        {
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER], [NUMBER_CHANGE_COUNT], [PAYMENT_COUNT], [LEFT_CHARGE], [ORDER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER, @NUMBER_CHANGE_COUNT, @PAYMENT_COUNT, @LEFT_CHARGE, @ORDER)";
            int result = -1;
            string sql = "UPDATE phone SET [ORDER] = @ORDER WHERE [SERIAL_NUMBER]=@SERIAL_NUMBER";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.Add("@ORDER", int.Parse(obj[0].ToString()));
            cmd.Parameters.Add("@SERIAL_NUMBER", obj[1].ToString());
            //cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                result = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            if (result > 0)
                return true;
            return false;
        }

        public bool update_PayUserOrder(object[] obj)
        {
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER], [NUMBER_CHANGE_COUNT], [PAYMENT_COUNT], [LEFT_CHARGE], [ORDER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER, @NUMBER_CHANGE_COUNT, @PAYMENT_COUNT, @LEFT_CHARGE, @ORDER)";
            int result = -1;
            string sql = "UPDATE paymentconditions SET [ORDER] = @ORDER WHERE [IDX]=@IDX";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.Add("@ORDER", obj[0]);
            cmd.Parameters.Add("@IDX", obj[1]);
            //cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                result = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }

            if (result > 0)
                return true;
            return false;
        }

        public int update_LogPaymentApproval(object[] obj)
        {
            //string sql =
            //    "INSERT INTO phone ([SERIAL_NUMBER], [NUMBER], [NUMBER_CHANGE_COUNT], [PAYMENT_COUNT], [LEFT_CHARGE], [ORDER]) " +
            //"VALUES (@SERIAL_NUMBER, @NUMBER, @NUMBER_CHANGE_COUNT, @PAYMENT_COUNT, @LEFT_CHARGE, @ORDER)";
            int result = -1;
            string sql = "UPDATE log_payment " +
                        "SET [CASH] = @CASH, [APPROVAL_NUMBER] = @APPROVAL_NUMBER, [APPROVAL_TIME] = GETDATE() " +
                        "WHERE [IDX] = @IDX";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.AddWithValue("@CASH", obj[0]);
            cmd.Parameters.Add("@APPROVAL_NUMBER", obj[1]);
            cmd.Parameters.AddWithValue("@IDX", obj[2]);
            //cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                result = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public int update_LogPaymentFinish(object[] obj)
        {
            int result = -1;
            //UPDATE log_payment 
            //SET BALANCE = 50000, FINISH_TIME = GETDATE()
            //WHERE IDX in
            //(select DISTINCT a.IDX as IDX
            //from
            //(SELECT  MAX(IDX) AS IDX
            //FROM     log_payment
            //WHERE  (TO_PHONE_NUMBER = 01050763027)
            //GROUP BY IDENTIFICATION_NUMBER ) as a,
            //log_payment as b
            //where a.IDX=b.IDX and b.CASH = 70000 and b.APPROVAL_TIME is NOT NULL)
            string sql = "UPDATE log_payment " +
                       "SET [BALANCE] = @BALANCE, [FINISH_TIME] = GETDATE() " +
                       "WHERE [IDX] in " +
                       "(select DISTINCT a.IDX as IDX " +
                       "from " +
                       "(SELECT  MAX([IDX]) AS IDX " +
                       "FROM     log_payment " +
                       "WHERE  ([TO_PHONE_NUMBER] = @TO_PHONE_NUMBER) " +
                       "GROUP BY [IDENTIFICATION_NUMBER] ) as a, " +
                       "log_payment as b " +
                       "where a.IDX=b.IDX and b.CASH = @CASH and b.APPROVAL_TIME is NOT NULL and b.FINISH_TIME is NULL)";
            SqlCeConnection conn = new SqlCeConnection(connString);
            SqlCeCommand cmd = new SqlCeCommand(sql, conn);
            conn.Open();

            SqlCeTransaction tran = conn.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Parameters.AddWithValue("@BALANCE", obj[0]);
            cmd.Parameters.Add("@TO_PHONE_NUMBER", obj[1]);
            cmd.Parameters.AddWithValue("@CASH", obj[2]);
            //cmd.Parameters.AddWithValue("@NUMBER_CHANGE_COUNT", 0);
            //cmd.Parameters.Add("@NUMBER_CHANGE_COUNT", (Int32)1);
            //cmd.Parameters.Add("@PAYMENT_COUNT", (Int32)1);
            //cmd.Parameters.Add("@LEFT_CHARGE", (Int32)1);
            //cmd.Parameters.Add("@ORDER", (Int32)1);

            try
            {
                result = cmd.ExecuteNonQuery();
                tran.Commit();
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public void test()
        {
            string dir = Path.Combine(Environment.CurrentDirectory, "DB.sdf");
            string connectionString = @"Data Source=|DataDirectory|\DB.sdf";
            //string connectionString = @dir;

            SqlCeConnection con = new SqlCeConnection(connectionString);
            con.Open();

            // 데이터베이스 커맨드 생성 
            SqlCeCommand cmd = new SqlCeCommand();

            // 커맨드에 커넥션을 연결 
            cmd.Connection = con;

            // 트랜잭션 생성 
            SqlCeTransaction tran = con.BeginTransaction();
            cmd.Transaction = tran;
            // 쿼리 생성 : Insert 쿼리 
            cmd.CommandText = "INSERT INTO Test VALUES('소녀시대')";

            // 쿼리 실행 
            cmd.ExecuteNonQuery();

            // 반복으로 몇개 더 넣어보겠습니다. 
            cmd.CommandText = "INSERT INTO Test VALUES('원더걸스')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO Test VALUES('카라')";
            cmd.ExecuteNonQuery();

            // 커밋 
            tran.Commit();

            // SELECT 쿼리로 변경 
            cmd.CommandText = "SELECT * FROM Test";

            // DataReader에 쿼리 결과값 저장 
            SqlCeDataReader reader = cmd.ExecuteReader();

            // 결과값 출력 
            while (reader.Read())
            {
                Console.WriteLine(reader["Name"]);
            }

            con.Close();
        }
    }





//    static void Read()
//{
//    try
//    {
//        string connectionString =
//            "server=.;" +
//            "initial catalog=employee;" +
//            "user id=sa;" +
//            "password=sa123";
//        using (SqlConnection conn =
//            new SqlConnection(connectionString))
//        {
//            conn.Open();
//            using (SqlCommand cmd =
//                new SqlCommand("SELECT * FROM EmployeeDetails", conn))
//            {
//                SqlDataReader reader = cmd.ExecuteReader();

//                if (reader.HasRows)
//                {
//                    while (reader.Read())
//                    {
//                        Console.WriteLine("Id = ", reader["Id"]);
//                        Console.WriteLine("Name = ", reader["Name"]);
//                        Console.WriteLine("Address = ", reader["Address"]);
//                    }
//                }

//                reader.Close();
//            }
//        }
//    }
//    catch (SqlException ex)
//    {
//        //Log exception
//        //Display Error message
//    }
//}

//static void Insert()
//{
//    try
//    {
//        string connectionString =
//            "server=.;" +
//            "initial catalog=employee;" +
//            "user id=sa;" +
//            "password=sa123";
//        using (SqlConnection conn =
//            new SqlConnection(connectionString))
//        {
//            conn.Open();
//            using (SqlCommand cmd =
//                new SqlCommand("INSERT INTO EmployeeDetails VALUES(" +
//                    "@Id, @Name, @Address)", conn))
//            {
//                cmd.Parameters.AddWithValue("@Id", 1);
//                cmd.Parameters.AddWithValue("@Name", "Amal Hashim");
//                cmd.Parameters.AddWithValue("@Address", "Bangalore");

//                int rows = cmd.ExecuteNonQuery();

//                //rows number of record got inserted
//            }
//        }
//    }
//    catch (SqlException ex)
//    {
//        //Log exception
//        //Display Error message
//    }
//}

//static void Update()
//{
//    try
//    {
//        string connectionString =
//            "server=.;" +
//            "initial catalog=employee;" +
//            "user id=sa;" +
//            "password=sa123";
//        using (SqlConnection conn =
//            new SqlConnection(connectionString))
//        {
//            conn.Open();
//            using (SqlCommand cmd =
//                new SqlCommand("UPDATE EmployeeDetails SET Name=@NewName, Address=@NewAddress" +
//                    " WHERE Id=@Id", conn))
//            {
//                cmd.Parameters.AddWithValue("@Id", 1);
//                cmd.Parameters.AddWithValue("@Name", "Munna Hussain");
//                cmd.Parameters.AddWithValue("@Address", "Kerala");

//                int rows = cmd.ExecuteNonQuery();

//                //rows number of record got updated
//            }
//        }
//    }
//    catch (SqlException ex)
//    {
//        //Log exception
//        //Display Error message
//    }
//}

//static void Delete()
//{
//    try
//    {
//        string connectionString =
//            "server=.;" +
//            "initial catalog=employee;" +
//            "user id=sa;" +
//            "password=sa123";
//        using (SqlConnection conn =
//            new SqlConnection(connectionString))
//        {
//            conn.Open();
//            using (SqlCommand cmd =
//                new SqlCommand("DELETE FROM EmployeeDetails " +
//                    "WHERE Id=@Id", conn))
//            {
//                cmd.Parameters.AddWithValue("@Id", 1);
                
//                int rows = cmd.ExecuteNonQuery();

//                //rows number of record got deleted
//            }
//        }
//    }
//    catch (SqlException ex)
//    {
//        //Log exception
//        //Display Error message
//    }
//}
    //public void insert()
    //{
    //    string query = "SELECT [Order ID], [Customer] FROM Orders";
    //    SqlCeConnection conn = new SqlCeConnection(connString);
    //    SqlCeCommand cmd = new SqlCeCommand(query, conn);

    //    conn.Open();
    //    SqlCeDataReader rdr = cmd.ExecuteReader();

    //    try
    //    {
    //        // Iterate through the results
    //        //
    //        while (rdr.Read())
    //        {
    //            int val1 = rdr.GetInt32(0);
    //            string val2 = rdr.GetString(1);
    //        }
    //    }
    //    finally
    //    {
    //        // Always call Close when done reading
    //        //
    //        rdr.Close();

    //        // Always call Close when done reading
    //        //
    //        conn.Close();
    //    }
    //}


//    static public int AddProductCategory(string newName, string connString)
//{
//    Int32 newProdID = 0;
//    string sql =
//        "INSERT INTO Production.ProductCategory (Name) VALUES (@Name); "
//        + "SELECT CAST(scope_identity() AS int)";
//    using (SqlConnection conn = new SqlConnection(connString))
//    {
//        SqlCommand cmd = new SqlCommand(sql, conn);
//        cmd.Parameters.Add("@Name", SqlDbType.VarChar);
//        cmd.Parameters["@name"].Value = newName;
//        try
//        {
//            conn.Open();
//            newProdID = (Int32)cmd.ExecuteScalar();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message);
//        }
//    }
//    return (int)newProdID;
//}
}

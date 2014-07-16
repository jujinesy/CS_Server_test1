using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Threading;
using System.Collections; // needed for Queue 
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
//using System.Threading.Tasks;

namespace SimpleHttp
{
    public struct Coordinate
    {
        public bool AVAILABLE;
        public int X, Y, IDENTI;
        public string NAME, PHONE_NUMBER;

        public Coordinate(bool available, int p1, int p2, string nm, int identi, string phone_number)
        {
            AVAILABLE = available;
            X = p1;
            Y = p2;
            NAME = nm;
            IDENTI = identi;
            PHONE_NUMBER = phone_number;
        }
    }

    public class Util
    {
        //private static volatile Util instance;
        //private static object syncRoot = new Object();
        //private Util() { }

        //public static Util Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Util();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private Util()
        {
        }

        public static Util Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Util instance = new Util();
        }


        public const Int32 CURSOR_SHOWING = 0x00000001;
        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public bool fIcon;         // Specifies whether this structure defines an icon or a cursor. A value of TRUE specifies 
            public Int32 xHotspot;     // Specifies the x-coordinate of a cursor's hot spot. If this structure defines an icon, the hot 
            public Int32 yHotspot;     // Specifies the y-coordinate of the cursor's hot spot. If this structure defines an icon, the hot 
            public IntPtr hbmMask;     // (HBITMAP) Specifies the icon bitmask bitmap. If this structure defines a black and white icon, 
            public IntPtr hbmColor;    // (HBITMAP) Handle to the icon color bitmap. This member can be optional if this 
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure. 
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
            public IntPtr hCursor;          // Handle to the cursor. 
            public Point ptScreenPos;       // A POINT structure that receives the screen coordinates of the cursor. 
        }

        [DllImport("user32.dll", EntryPoint = "GetCursorInfo")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", EntryPoint = "CopyIcon")]
        public static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", EntryPoint = "GetIconInfo")]
        public static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        public static Bitmap CaptureCursor()
        {
            Bitmap bmp;
            IntPtr hicon;
            CURSORINFO ci = new CURSORINFO();
            ICONINFO icInfo;
            int x, y;
            ci.cbSize = Marshal.SizeOf(ci);
            if (GetCursorInfo(out ci))
            {
                if (ci.flags == CURSOR_SHOWING)
                {
                    hicon = CopyIcon(ci.hCursor);
                    if (GetIconInfo(hicon, out icInfo))
                    {
                        x = ci.ptScreenPos.X - ((int)icInfo.xHotspot);
                        y = ci.ptScreenPos.Y - ((int)icInfo.yHotspot);
                        Icon ic = Icon.FromHandle(hicon);
                        bmp = ic.ToBitmap();

                        return bmp;
                    }
                }
            }
            return null;
        }
        /*************************************************************************
         * method for comparing 2 cursor bitmaps to see if they are the same. 
         * (actually, this method works to compare any two botmaps) First
         * we convert both images to a byte array, we then get their hash (their
         * hash should match if the images are the same), we then loop through
         * each item in the hash comparing with the 2nd Bitmap
         **************************************************************************/
        public static bool CompareCursorImages(ref Bitmap bmp1, ref Bitmap bmp2)
        {
            try
            {
                //create instance or System.Drawing.ImageConverter to convert
                //each image to a byte array
                ImageConverter converter = new ImageConverter();
                //create 2 byte arrays, one for each image
                byte[] imgBytes1 = new byte[1];
                byte[] imgBytes2 = new byte[1];

                //convert images to byte array
                imgBytes1 = (byte[])converter.ConvertTo(bmp1, imgBytes2.GetType());
                imgBytes2 = (byte[])converter.ConvertTo(bmp2, imgBytes1.GetType());

                //now compute a hash for each image from the byte arrays
                SHA256Managed sha = new SHA256Managed();
                byte[] imgHash1 = sha.ComputeHash(imgBytes1);
                byte[] imgHash2 = sha.ComputeHash(imgBytes2);

                //now let's compare the hashes
                for (int i = 0; i < imgHash1.Length && i < imgHash2.Length; i++)
                {
                    //whoops, found a non-match, exit the loop
                    //with a false value
                    if (!(imgHash1[i] == imgHash2[i]))
                        return false;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
            //we made it this far so the images must match
            return true;
        }

        public string getclientIp
        {
            get
            {
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                string clientIP = string.Empty;
                for (int i = 0; i < host.AddressList.Length; i++)
                {
                    // AddressFamily.InterNetworkV6 - IPv6
                    if (host.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        clientIP = host.AddressList[i].ToString();
                    }
                }
                return clientIP;
            }
        }

        public Point getcMousePointer
        {
            get
            {
                CURSORINFO ci = new CURSORINFO();
                ci.cbSize = Marshal.SizeOf(ci);
                GetCursorInfo(out ci);
                Point pi = new Point();
                pi.X = ci.ptScreenPos.X;
                pi.Y = ci.ptScreenPos.Y;
                return pi;
            }
        }
    }

    public class Conf
    {
        //private static volatile Conf instance;
        //private static object syncRoot = new Object();
        //private Conf() { }

        //public static Conf Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new Conf();
        //            }
        //        }
        //        return instance;
        //    }
        //}


        private Conf()
        {
        }

        public static Conf Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Conf instance = new Conf();
        }


        public static int lbMainLog = 500;
        public static int iCoordUpdate = 0;
        public static int iSMS_TIME = 60;
        public static int iAPPROVAL_TIME = 120;
    }
}

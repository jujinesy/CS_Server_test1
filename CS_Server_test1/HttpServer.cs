﻿using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

// offered to the public domain for any use with no restriction
// and also with no warranty of any kind, please enjoy. - David Jeske. 

// simple HTTP explanation
// http://www.jmarshall.com/easy/http/

namespace SimpleHttp
{

    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                try
                {
                    next_char = inputStream.ReadByte();
                    if (next_char == '\n') { break; }
                    if (next_char == '\r') { continue; }
                    if (next_char == -1) { Thread.Sleep(1); continue; };
                    data += Convert.ToChar(next_char);
                }
                catch (Exception e) 
                {
                    Console.WriteLine("Exception: " + e.ToString());
                    mainLogSingleton.Instance.addResponse(new object[] { "streamReadLine", " : ", e.ToString() });
                }
            }
            return data;
        }

        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                mainLogSingleton.Instance.addResponse(new object[] { "process", "process", e.ToString() });
                writeFailure();
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
            mainLogSingleton.Instance.addResponse(new object[] { "parseRequest", " : ", request });
        }

        public void readHeaders()
        {
            Console.WriteLine("readHeaders()");
            mainLogSingleton.Instance.addResponse(new object[] { "readHeaders()" });
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    mainLogSingleton.Instance.addResponse(new object[] { "got headers" });
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                mainLogSingleton.Instance.addResponse(new object[] { "header", " : ", name, ":", value });
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            mainLogSingleton.Instance.addResponse(new object[] { "get post data start" });
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);
                    mainLogSingleton.Instance.addResponse(new object[] { "starting Read, to_read", " : ", to_read });

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    mainLogSingleton.Instance.addResponse(new object[] { "read finished, numread", " : ", numread });
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            mainLogSingleton.Instance.addResponse(new object[] { "get post data end" });
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writeSuccess(string content_type = "text/html")
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: " + content_type);
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }

    public abstract class HttpServer
    {

        protected int port;
        TcpListener listener;
        bool is_active = true;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public void listenStart()
        {
            listener = new TcpListener(port);
            //listener.
            listener.Start();
            while (is_active)
            {
                TcpClient s = listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        public void listenStop()
        {
            listener.Stop();
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public class MyHttpServer : HttpServer
    {
        public MyHttpServer(int port)
            : base(port)
        {
        }
        public override void handleGETRequest(HttpProcessor p)
        {

            if (p.http_url.Equals("/Test.png"))
            {
                Stream fs = File.Open("../../Test.png", FileMode.Open);

                p.writeSuccess("image/png");


                //fs.CopyTo (p.outputStream.BaseStream);

                p.outputStream.BaseStream.Flush();
            }

            Console.WriteLine("request: {0}", p.http_url);
            mainLogSingleton.Instance.addResponse(new object[] { "request", p.http_url });
            p.writeSuccess();
            p.outputStream.WriteLine("<html><head><meta http-equiv=Content-Type content=text/html; charset=utf-8 /></head><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            System.Web.HttpUtility.UrlEncode(p.http_url);
            p.outputStream.WriteLine("url : {0}", p.http_url);

            p.outputStream.WriteLine("<form method=post action=/for11m>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");

            //System.Web.HttpUtility.UrlEncode
            p.outputStream.WriteLine("</form>");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("POST request: {0}", p.http_url);
            mainLogSingleton.Instance.addResponse(new object[] { "POST request", " : ", p.http_url });

            string data = inputData.ReadToEnd();
            String data2 = HttpUtility.UrlDecode(data);

            p.writeSuccess();
            p.outputStream.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            p.outputStream.WriteLine("<html><head><meta http-equiv=Content-Type content=text/html; charset=euc-kr /></head><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data2);

            Console.WriteLine(data2);
            mainLogSingleton.Instance.addResponse(new object[] { "POST data", " : ", data2 });
        }
    }

    //public class TestMain {
    //    public static int Main(String[] args) {
    //        HttpServer httpServer;
    //        if (args.GetLength(0) > 0) {
    //            httpServer = new MyHttpServer(Convert.ToInt16(args[0]));
    //        } else {
    //            httpServer = new MyHttpServer(8080);
    //        }
    //        Thread thread = new Thread(new ThreadStart(httpServer.listen));
    //        thread.Start();
    //        return 0;
    //    }
    //}

    public class HttpServerMain
    {
        HttpServer httpServer;
        Thread thread;

        //private static volatile HttpServerMain instance;
        //private static object syncRoot = new Object();
        //private HttpServerMain() { }

        //public static HttpServerMain Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new HttpServerMain();
        //            }
        //        }
        //        return instance;
        //    }
        //}

        private HttpServerMain()
        {
        }

        public static HttpServerMain Instance { get { return Nested.instance; } }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly HttpServerMain instance = new HttpServerMain();
        }



        public void Start(int port)
        {
            httpServer = new MyHttpServer(port);
            thread = new Thread(new ThreadStart(httpServer.listenStart));
            thread.Start();

        }

        public void Stop()
        {
            httpServer.listenStop();
            thread.Abort();
        }
    }

    //public class UiSender
    //{
    //    Form1 frm;
    //    public UiSender(Form1 frm)
    //    {
    //        this.frm = frm;
    //    }

    //    private delegate void myDelegate2(String str);
    //    private void updateProgress2(String str)
    //    {
    //        //Console.WriteLine("1 : " + theValue.ToString());
    //        frm.listBox1.Items.Add(str);
    //        frm.listBox1.SetSelected(frm.listBox1.Items.Count - 1, true);
    //    }

    //    public void Set(String str)
    //    {
    //        //frm.Invoke(new myDelegate2(updateProgress2), new object[] { 1, 100 });
    //        frm.Invoke(new myDelegate2(updateProgress2), str);
    //    }
    //}    
}
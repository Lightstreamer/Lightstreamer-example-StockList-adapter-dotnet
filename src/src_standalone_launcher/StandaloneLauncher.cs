﻿using Lightstreamer.DotNet.Server;
using System;
using System.Collections;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;

using NLog;
using System.IO;

namespace TestAdapter
{
    class StandaloneLauncher
    {
        private static Logger nLog = LogManager.GetCurrentClassLogger();
        
        public const string PREFIX1 = "-";
        public const string PREFIX2 = "/";

        public const char SEP = '=';

        public const string ARG_HELP_LONG = "help";
        public const string ARG_HELP_SHORT = "?";

        public const string ARG_HOST = "host";
        public const string ARG_TLS = "tls"; // will use lowercase
        public const string ARG_METADATA_RR_PORT = "metadata_rrport";
        public const string ARG_DATA_RR_PORT = "data_rrport";
        public const string ARG_USER = "user";
        public const string ARG_PASSWORD = "password";
        public const string ARG_NAME = "name";

        public static void Main(string[] args)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "TestAdapter.log" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            if (args.Length == 0) Help();

            nLog.Info("Lightstreamer StockListDemo .NET Adapter Standalone Server starting ...");

            Server.SetLoggerProvider(new Log4NetLoggerProviderWrapper());

            IDictionary parameters = new Hashtable();
            string host = null;
            bool isTls = false;
            int rrPortMD = -1;
            int rrPortD = -1;
            string username = null;
            string password = null;
            string name = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.StartsWith(PREFIX1) || arg.StartsWith(PREFIX2))
                {
                    arg = arg.Substring(1).ToLower();

                    if (arg.Equals(ARG_HELP_SHORT) || arg.Equals(ARG_HELP_LONG))
                    {
                        Help();
                    }
                    else if (arg.Equals(ARG_HOST))
                    {
                        i++;
                        host = args[i];

                        nLog.Debug("Found argument: '" + ARG_HOST + "' with value: '" + host + "'");
                    }
                    else if (arg.Equals(ARG_TLS)) {
                        isTls = true;

                        nLog.Debug("Found argument: '" + ARG_TLS + "'");
                    }
                    else if (arg.Equals(ARG_METADATA_RR_PORT))
                    {
                        i++;
                        rrPortMD = Int32.Parse(args[i]);

                        nLog.Debug("Found argument: '" + ARG_METADATA_RR_PORT + "' with value: '" + rrPortMD + "'");
                    }
                    else if (arg.Equals(ARG_DATA_RR_PORT))
                    {
                        i++;
                        rrPortD = Int32.Parse(args[i]);

                        nLog.Debug("Found argument: '" + ARG_DATA_RR_PORT + "' with value: '" + rrPortD + "'");
                    }
                    else if (arg.Equals(ARG_USER))
                    {
                        i++;
                        username= args[i];

                        nLog.Debug("Found argument: '" + ARG_USER + "' with value: '" + username + "'");
                    }
                    else if (arg.Equals(ARG_PASSWORD)) {
                        i++;
                        password= args[i];

                        nLog.Debug("Found argument: '" + ARG_PASSWORD + "' with value: '" + password + "'");
                    }
                    else if (arg.Equals(ARG_NAME))
                    {
                        i++;
                        name = args[i];

                        nLog.Debug("Found argument: '" + ARG_NAME + "' with value: '" + name + "'");
                    }

                }
                else
                {
                    int sep = arg.IndexOf(SEP);
                    if (sep < 1)
                    {
                        nLog.Warn("Skipping unrecognizable argument: '" + arg + "'");

                    }
                    else
                    {
                        string par = arg.Substring(0, sep).Trim();
                        string val = arg.Substring(sep + 1).Trim();
                        parameters[par] = val;

                        nLog.Debug("Found parameter: '" + par + "' with value: '" + val + "'");
                    }
                }
            }

            try
            {
                if ((username != null) != (password != null))
                {
                    throw new Exception("Incomplete setting of /user and /password arguments");
                }

                { 
                    MetadataProviderServer server = new MetadataProviderServer();
                    server.Adapter = new Lightstreamer.Adapters.Metadata.LiteralBasedProvider();
                    server.AdapterParams = parameters;
                    // server.AdapterConfig not needed by LiteralBasedProvider
                    if (parameters["name"] != null) server.Name = "Test:LBP";
                    if (username != null)
                    {
                        server.RemoteUser = username;
                        server.RemotePassword = password;
                    }
                    nLog.Debug("Remote Metadata Adapter initialized");

                    ServerStarter starter = new ServerStarter(host, isTls, rrPortMD);
                    starter.Launch(server);
                }

                {
                    DataProviderServer server = new DataProviderServer();
                    server.Adapter = new Lightstreamer.Adapters.StockListDemo.Data.StockListDemoAdapter();
                    // server.AdapterParams not needed by StockListDemoAdapter
                    // server.AdapterConfig not needed by StockListDemoAdapter
                    if (name != null) server.Name = name;
                    if (username != null)
                    {
                        server.RemoteUser = username;
                        server.RemotePassword = password;
                    }
                    nLog.Debug("Remote Data Adapter initialized");

                    ServerStarter starter = new ServerStarter(host, isTls, rrPortD);
                    starter.Launch(server);
                }
            }
            catch (Exception e)
            {
                nLog.Fatal("Exception caught while starting the server: " + e.Message + ", aborting...", e);
            }

            nLog.Info("Lightstreamer StockListDemo .NET Adapter Standalone Server running");
        }

        private static void Help()
        {
            nLog.Fatal("Lightstreamer StockListDemo .NET Adapter Standalone Server Help");
            nLog.Fatal("Usage: DotNetStockListDemoLauncher");
            nLog.Fatal("                     [/name <name>]");
            nLog.Fatal("                     /host <address> [/tls]");
            nLog.Fatal("                     /metadata_rrport <port> /data_rrport <port>");
            nLog.Fatal("                     [/user <username> /password <password>]");
            nLog.Fatal("                     [\"<param1>=<value1>\" ... \"<paramN>=<valueN>\"]");
            nLog.Fatal("Where: <name>        is the symbolic name for both the adapters (1)");
            nLog.Fatal("       <address>     is the host name or ip address of LS server (2)");
            nLog.Fatal("       <port>        is a tcp port number where LS proxy is listening on");
            nLog.Fatal("       /tls          if indicated, initiates a TLS-encrypted communication (3)");
            nLog.Fatal("       <username>    is sent, along with <password>, to the LS proxy (3)");
            nLog.Fatal("       <paramN>      is the Nth metadata adapter parameter name (4)");
            nLog.Fatal("       <valueN>      is the value of the Nth metadata adapter parameter (4)");
            nLog.Fatal("Notes: (1) The adapter name is optional, if it is not given the adapter will be");
            nLog.Fatal("           assigned a progressive number name like \"#1\", \"#2\" and so on");
            nLog.Fatal("       (2) The communication will be from here to LS, not viceversa");
            nLog.Fatal("       (3) TLS communication and user-password submission may or may not be needed");
            nLog.Fatal("           depending on the LS Proxy Adapter configuration");
            nLog.Fatal("       (4) The parameters name/value pairs will be passed to the LiteralBasedProvider");
            nLog.Fatal("           Metadata Adapter as a Hashtable in the \"parameters\" Init() argument");
            nLog.Fatal("           The StockListDemo Data Adapter requires no parameters");
            nLog.Fatal("Aborting...");

            System.Environment.Exit(9);
        }
    }

    public class ServerStarter : IExceptionHandler
    {
        private static Logger nLog = LogManager.GetCurrentClassLogger();

        private Server _server;
        private bool _closed;

        private string _host;
        private bool _isTls;
        private int _rrPort;

        public ServerStarter(string host, bool isTls, int rrPort)
        {
            _host = host;
            _isTls = isTls;
            _rrPort = rrPort;
        }

        public void Launch(Server server)
        {
            _server = server;
            _closed = false;
            _server.ExceptionHandler = this;
            Thread t = new Thread(new ThreadStart(Run));
            t.Start();
        }

        public void Run()
        {
            TcpClient _rrSocket = null;

            do
            {
                nLog.Info("Connecting...");

                try
                {
                    _rrSocket = CreateSocket(_host, _isTls, _rrPort);
                    nLog.Info("Connected");

                    break;
                }
                catch (SocketException)
                {
                    nLog.Info("Connection failed, retrying in 10 seconds...");
                    Thread.Sleep(10000);
                }

            } while (true);

            try
            {
                Stream _rrStream = GetProperStream(_rrSocket, _rrPort);
                _server.RequestStream = _rrStream;
                _server.ReplyStream = _rrStream;
            }
            catch (AuthenticationException e)
            {
                nLog.Error("TLS Authentication failed");
                throw e;
            }

            _server.Start();
        }

        private static TcpClient CreateSocket(string host, bool isTls, int port)
        {
            nLog.Info("Opening connection on port " + port + (isTls ? " with TLS" : "") + "...");
            TcpClient s = null;
            try {
                s = new TcpClient(host, port);
            } finally { 
                if (s != null) {
                    nLog.Info("Connection on port " + port + " opened");
                } else {
                    nLog.Info("Connection on port " + port + " failed");
                }
            }
            return s;
        }

        private Stream GetProperStream(TcpClient socket, int port)
        {
            Stream _stream = socket.GetStream();
            if (_isTls)
            {
                SslStream _sslStream = new SslStream(_stream);
                try {
                    _sslStream.AuthenticateAsClient(_host);
                    nLog.Info("TLS handshake done on port " + port);
                } catch (AuthenticationException e) {
                    nLog.Info("TLS handshake failed on port " + port);
                    throw e;
                }
                return _sslStream;
            }
            else
            {
                return _stream;
            }
        }

        public bool handleIOException(Exception exception)
        {
            lock (this)
            {
                if (_closed)
                {
                    return false;
                }
                else
                {
                    nLog.Info("Connection to Lightstreamer Server closed");
                    _closed = true;
                }
            }
            _server.Close();
            System.Environment.Exit(0);
            return false;
        }

        public bool handleException(Exception exception)
        {
            lock (this)
            {
                if (_closed)
                {
                    return false;
                }
                else
                {
                    nLog.Info("Caught exception: " + exception.Message, exception);
                    _closed = true;
                }
            }
            _server.Close();
            System.Environment.Exit(1);
            return false;
        }

        // Notes about exception handling.
        // 
        // In case of exception, the whole Remote Server process instance
        // can no longer be used;
        // closing it also ensures that the Proxy Adapter closes
        // (thus causing Lightstreamer Server to close)
        // or recovers by accepting connections from a new Remote
        // Server process instance.
        // 
        // Keeping the process instance alive and replacing the Server
        // class instances would be possible.
        // This would issue new connections with Lightstreamer Server.
        // However, new instances of the Remote Adapters would also be needed
        // and a cleanup of the current instances should be performed,
        // by invoking them directly through a custom method.
    }
}

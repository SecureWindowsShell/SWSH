using System;
using System.IO;
using Renci.SshNet;
using System.Security.Cryptography;
using System.Text;

namespace SWSH {
    class Program {
        static string _command = "", _version = "alpha-2.0", _mainDirectory = "swsh-data/";
        static void Main(string[] args) {
            Console.Title = "SWSH - " + _version;
            __version();
            Console.Write("swsh --help or -h for help.\n\n");
            __start();
        }
        public static void __start() {
            while (true) {
                try {
                    __color("swsh> ", ConsoleColor.DarkGray);
                    _command = Console.ReadLine();
                    if (_command.StartsWith("swsh")) {
                        _command = _command.Replace("swsh", "").Trim();
                        if (_command == "--version" || _command == "-v") __version();
                        else if (_command.StartsWith("--add") || _command.StartsWith("-a")) {
                            _command = (_command.StartsWith("--")) ? _command.Replace("--add", "").Trim() : _command.Replace("-a", "").Trim();
                            __color("exit", ConsoleColor.Red);
                            Console.Write(" or ");
                            __color("-e", ConsoleColor.Red);
                            Console.Write(" to cancel.\n");
                            var key = "";
                            if (_command.StartsWith("-password")) key = "-password";
                            else {
                                getPriKey:
                                Console.Write("Path to private key: ");
                                key = Console.ReadLine();
                                __checkexit(key);
                                if (!File.Exists(key)) {
                                    __color("ERROR: ", ConsoleColor.Red);
                                    Console.Write("SWSH -> {0} -> file is non existent.\n", key);
                                    goto getPriKey;
                                }
                            }
                            Console.Write("Username: ");
                            var usr = Console.ReadLine();
                            __checkexit(usr);
                            Console.Write("Server: ");
                            var svr = Console.ReadLine();
                            __checkexit(svr);
                            getNick:
                            Console.Write("Unique Nickname: ");
                            var nkn = Console.ReadLine();
                            if (File.Exists(_mainDirectory + nkn + ".swsh")) {
                                __color("ERROR: ", ConsoleColor.Red);
                                Console.WriteLine("SWSH -> {0} -> nickname exists", nkn);
                                goto getNick;
                            }
                            if (nkn.Trim() == string.Empty) goto getNick;
                            String[] data = new String[] { key, usr, svr };
                            if (!Directory.Exists(_mainDirectory)) Directory.CreateDirectory(_mainDirectory);
                            File.AppendAllLines(_mainDirectory + nkn + ".swsh", data);
                        } else if (_command == "--help" || _command == "-h") {
                            help();
                        } else if (_command.StartsWith("--connect") || _command.StartsWith("-c")) {
                            #region   SSH Control  
                            ConnectionInfo ccinfo;
                            string nickname = (_command.StartsWith("--connect")) ? _command.Remove(0, 10) : _command.Remove(0, 3);
                            if (File.Exists(_mainDirectory + nickname + ".swsh")) {
                                if (File.ReadAllLines(_mainDirectory + nickname + ".swsh")[0] == "-password") {
                                    Console.Write("Password for {0}: ", nickname);
                                    ccinfo = __CreateConnectionInfoPassword(nickname, Console.ReadLine());
                                } else ccinfo = __CreateConnectionInfoKey(nickname);
                                if (ccinfo != null) {
                                    Console.Write("Waiting for response from {0}@{1}...\n", ccinfo.Username, ccinfo.Host);
                                    using (var ssh = new SshClient(ccinfo)) {
                                        ssh.Connect();
                                        __color("Connected to " + ccinfo.Username + "@" + ccinfo.Host + "...\n", ConsoleColor.Green);
                                        string pwd = " ", home = "";
                                        home = pwd = ssh.CreateCommand("echo $HOME").Execute();
                                        while (true) {
                                            if (pwd == home) pwd = "~";
                                            __color(pwd, ConsoleColor.Green);
                                            Console.Write(":/ $ ");
                                            _command = Console.ReadLine();
                                            if (_command == "exit")
                                                break;
                                            else if (_command.StartsWith("cd ")) {
                                                _command = _command.Remove(0, 3);
                                                if (_command.StartsWith("/")) pwd = _command;
                                                else if (_command.StartsWith("./")) pwd += "/" + _command.Remove(0, 2);
                                                else if (_command.StartsWith("..")) {
                                                    __color("ERROR: ", ConsoleColor.Red);
                                                    Console.Write("SWSH -> cd {0} -> Operation not allowed. Run \"swsh -h\" and see Note #1.\n", _command);
                                                } else pwd += "/" + _command;
                                            } else if (_command == "clear") Console.Clear();
                                            else {
                                                var result = ssh.CreateCommand("cd " + pwd + "; " + _command).Execute();
                                                Console.Write(result);
                                            }
                                        }
                                        ssh.Disconnect();
                                    }
                                    __color("Connection to " + ccinfo.Username + "@" + ccinfo.Host + ", closed.\n", ConsoleColor.Yellow);
                                } else break;
                            } else {
                                __color("ERROR: ", ConsoleColor.Red);
                                Console.Write("SWSH -> {0} -> nickname does not exists\n", nickname);
                            }
                            #endregion
                        } else if (_command.StartsWith("--show")) {
                            _command = _command.Remove(0, 6);
                            if (_command.Trim() == string.Empty) {
                                foreach (var file in Directory.GetFiles(_mainDirectory)) {
                                    var data = File.ReadAllLines(file);
                                    Console.Write("\nDetails of {0}:\n", Path.GetFileNameWithoutExtension(file));
                                    for (int i = 0; i < Path.GetFileNameWithoutExtension(file).Length + 12; i++) Console.Write("=");
                                    if (data[0] == "-password") {
                                        Console.Write("\nUsername: {0}\nHost: {1}\n\n", data[1], data[2]);
                                    } else {
                                        Console.Write("\nPath to key: {0}\nUsername: {1}\nHost: {2}\nStatus: ", data[0], data[1], data[2]);
                                        var conInfo = __CreateConnectionInfoKey(Path.GetFileNameWithoutExtension(file));
                                        if (conInfo != null)
                                            using (var connection = new SshClient(conInfo)) {
                                                connection.Connect();
                                                __color("Working\n\n", ConsoleColor.Green);
                                            }
                                    }
                                }
                            } else {
                                _command = _command.Trim();
                                var file = _mainDirectory + _command + ".swsh";
                                if (File.Exists(file)) {
                                    Console.Write("Details of {0}:\n", _command);
                                    var data = File.ReadAllLines(file);
                                    for (int i = 0; i < _command.Length + 12; i++) Console.Write("=");
                                    Console.Write("\nPath to key: {0}\nUsername: {1}\nHost: {2}\nStatus: ", data[0], data[1], data[2]);
                                    var connection = new SshClient(__CreateConnectionInfoKey(_command));
                                    connection.Connect();
                                    __color("Working\n", ConsoleColor.Green);
                                    connection.Dispose();
                                } else {
                                    __color("ERROR: ", ConsoleColor.Red);
                                    Console.WriteLine("SWSH -> {0} -> nickname does not exists", _command);
                                }
                            }
                        } else if (_command.StartsWith("--delete")) {
                            if (File.Exists(_mainDirectory + _command.Replace("--delete", "").Trim() + ".swsh")) {
                                __color("Are you sure you want to delete this nickname? (y/n): ", ConsoleColor.Red);
                                var ans = Console.ReadLine().ToUpper();
                                if (ans == "Y") {
                                    Console.Write("Type the nickname to confirm: ");
                                    var name = Console.ReadLine();
                                    if (name != _command.Replace("--delete", "").Trim()) __color("Aborted.\n", ConsoleColor.Yellow);
                                    else {
                                        File.Delete(_mainDirectory + _command.Replace("--delete", "").Trim() + ".swsh");
                                        __color("Deleted.\n", ConsoleColor.Green);
                                    }
                                } else
                                    __color("Aborted.\n", ConsoleColor.Yellow);
                            } else {
                                __color("ERROR: ", ConsoleColor.Red);
                                Console.WriteLine("SWSH -> {0} -> nickname does not exists", _command.Replace("--delete", "").Trim());
                            }
                        }
                    } else if (_command == "clear") {
                        Console.Clear();
                        __version();
                        Console.Write("swsh --help or -h for help.\n\n");
                    } else if (_command == "exit") break;
                    else {
                        Console.ForegroundColor = ConsoleColor.Red;
                        help();
                        Console.ResetColor();
                    }
                } catch (Exception exp) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine(exp.Message);
                }
            }
        }
        public static void __version() {
            Console.Write("   ______       _______ __  __\n  / ___/ |     / / ___// / / /\n  \\__ \\| | /| / /\\__ \\/ /_/ / \n ___/ /| |/ |/ /___/ / __  /  \n/____/ |__/|__//____/_/ /_/   \n     Secure Windows Shell     \n");
            Console.Write("\nRelease: {0}\n{1}", _version, "(c) Muhammad Muzzammil & Nabeel Omer\n");
        }
        public static void __color(string message, ConsoleColor cc) {
            Console.ForegroundColor = cc;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void help() {
            Console.Write("SWSH [command] [arg]\n");
            Console.WriteLine("\t--version -v: -Check the version of swsh.");
            Console.WriteLine("\t--add [-password]* -a [-password]*: -Add a new connection either using private key or password (-password).");
            Console.WriteLine("\t--show [nickname]*: Show nicknames/Details of a nickname.");
            Console.WriteLine("\t--connect [nickname] -c [nickname]: -Connects to Server over SSH.");
            Console.WriteLine("\t--delete [nickname]: -Deleted connection's nickname.");
            Console.WriteLine("\t--help -h: -Displays this help.");
            Console.WriteLine("clear: Clears the console.");
            Console.WriteLine("exit: Exits.");
            Console.WriteLine("\n\nNOTES:\n[1] cd .. is not supported.\n[2] * = Optional.");
        }
        public static void __checkexit(string keyword) {
            if (keyword == "exit" || keyword == "-e") __start();
        }
        public static ConnectionInfo __CreateConnectionInfoKey(string nickname) {
            try {
                if (File.Exists(_mainDirectory + nickname + ".swsh")) {
                    string privateKeyFilePath = File.ReadAllLines(_mainDirectory + nickname + ".swsh")[0],
                    user = File.ReadAllLines(_mainDirectory + nickname + ".swsh")[1],
                    server = File.ReadAllLines(_mainDirectory + nickname + ".swsh")[2];
                    ConnectionInfo connectionInfo;
                    using (var stream = new FileStream(privateKeyFilePath, FileMode.Open, FileAccess.Read)) {
                        var privateKeyFile = new PrivateKeyFile(stream);
                        AuthenticationMethod authenticationMethod = new PrivateKeyAuthenticationMethod(user, privateKeyFile);
                        connectionInfo = new ConnectionInfo(server, user, authenticationMethod);
                    }
                    return connectionInfo;
                } else {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> {0} -> nickname does not exists", nickname);
                    __start();
                }
            } catch (Exception exp) { __color("ERROR: " + exp.Message + "\n", ConsoleColor.Red); }
            return null;
        }
        public static ConnectionInfo __CreateConnectionInfoPassword(string nickname, string password) {
            try {
                if (File.Exists(_mainDirectory + nickname + ".swsh")) {
                    string user = File.ReadAllLines(_mainDirectory + nickname + ".swsh")[1],
                    server = File.ReadAllLines(_mainDirectory + nickname + ".swsh")[2];
                    ConnectionInfo connectionInfo;
                    AuthenticationMethod authenticationMethod = new PasswordAuthenticationMethod(user, password);
                    connectionInfo = new ConnectionInfo(server, user, authenticationMethod);
                    return connectionInfo;
                } else {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> {0} -> nickname does not exists", nickname);
                    __start();
                }
            } catch (Exception exp) { __color("ERROR: " + exp.Message + "\n", ConsoleColor.Red); }
            return null;
        }
    }
}
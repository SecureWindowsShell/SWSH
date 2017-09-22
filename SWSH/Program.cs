using Renci.SshNet;
using System.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWSH {
    class Program {
        static string _command = "", _version = "pre-alpha-0.0.1", _mainDirectory = "swsh-data/";
        static void Main(string[] args) {
            __version();
            Console.Write("swsh --help or -h for help.\n\n");
            __start();
        }
        public static void __start() {
            while (true) {
                __color("swsh> ", ConsoleColor.DarkGray);
                _command = Console.ReadLine();
                if (_command.StartsWith("swsh")) {
                    _command = _command.Replace("swsh", "").Trim();
                    if (_command == "--version" || _command == "-v") __version();
                    else if (_command == "--add" || _command == "-a") {
                        __color("exit", ConsoleColor.Red);
                        Console.Write(" or ");
                        __color("-e", ConsoleColor.Red);
                        Console.Write(" to cancel.\n");
                        Console.Write("Path to private key: ");
                        var key = Console.ReadLine();
                        __checkexit(key);
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
                        File.WriteAllLines(_mainDirectory + nkn + ".swsh", data);
                    } else if (_command == "--help" || _command == "-h") {
                        Console.WriteLine("--version -v: -Check the version of swsh.");
                        Console.WriteLine("--add -a: -Add a new connection.");
                        Console.WriteLine("--connect [nickname] -c [nickname]: -Connects to Server over SSH.");
                        Console.WriteLine("--help -h: -Displays this help.");
                        Console.WriteLine("exit: Exits.");
                        Console.WriteLine("\n\nNOTES:\n[1] cd .. is not supported.");
                    } else if (_command.StartsWith("--connect") || _command.StartsWith("-c")) {
                        #region   SSH Control  
                        var ccinfo = (_command.StartsWith("--connect")) ? __CreateConnectionInfo(_command.Remove(0, 10)) : __CreateConnectionInfo(_command.Remove(0, 3));
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
                        #endregion
                    }
                } else if (_command == "exit") break;
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
        public static void __checkexit(string keyword) {
            if (keyword == "exit" || keyword == "-e") __start();
        }
        public static ConnectionInfo __CreateConnectionInfo(string nickname) {
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
                return null;
            }
        }
    }
}
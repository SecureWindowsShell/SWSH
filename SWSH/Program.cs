using System;
using System.IO;
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace SWSH {
    class Program {
        static string _command = "", _version = "beta-1.0", _mainDirectory = "swsh-data/", _home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), _workingDirectory = _home;
        static void Main(string[] args) {
            Console.Title = "SWSH - " + _version;
            __version();
            Console.Write("swsh --help or -h for help.\n\n");
            __start();
        }
        private void AddConnection() {
            //TODO @muhammedmuzzammil1998: I don't know why this is needed, please review
            _command = (_command.StartsWith("--")) ? _command.Replace("--add", "").Trim() : _command.Replace("-a", "").Trim();
            __color("exit", ConsoleColor.Red);
            Console.Write(" or ");
            __color("-e", ConsoleColor.Red);
            Console.Write(" to cancel.\n");

            var key = "";
            if (_command.StartsWith("-password")) key = "-password";
            else {
                while (true) {
                    Console.Write("Enter path to private key: ");
                    key = Console.ReadLine();
                    __checkexit(key);
                    if (!File.Exists(key)) {
                        __color("ERROR: ", ConsoleColor.Red);
                        Console.Write("SWSH -> {0} -> file is non existent.\n", key);
                        continue;
                    } else {
                        break;
                    }
                }
            }

            Console.Write("Username: ");
            var usr = Console.ReadLine();
            __checkexit(svr);
            if (usr == "") {
                __color("ERROR: ", ConsoleColor.Red);
                Console.WriteLine("Username should not be empty!");
            }

            Console.Write("Server: ");
            var svr = Console.ReadLine();
            __checkexit(svr);
            if (svr == "") {
                __color("ERROR: ", ConsoleColor.Red);
                Console.WriteLine("IP address or domain name should not be empty!");
            }

            while(true) {
                Console.Write("Unique Nickname: ");
                var nkn = Console.ReadLine();
                if (nkn.Trim() == string.Empty) {
                    continue;
                }
                else if (File.Exists(_mainDirectory + nkn + ".swsh")) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> {0} -> nickname exists", nkn);
                }
                else {
                    break;
                }
            }
            String[] data = new String[] { key, usr, svr };
            if (!Directory.Exists(_mainDirectory)) {
                Directory.CreateDirectory(_mainDirectory);
            }
            File.AppendAllLines(_mainDirectory + nkn + ".swsh", data);
        }
        public static void __start() {
            while (true) {
                try {
                    __color(_workingDirectory.Replace('\\', '/').Remove(0, 2) + ":", ConsoleColor.DarkCyan);
                    __color("swsh> ", ConsoleColor.DarkGray);
                    GetStdin();
                    if (_command.StartsWith("swsh")) {
                        _command = _command.Replace("swsh", "").Trim();
                        if (_command == "--version" || _command == "-v") __version();
                        else if (_command.StartsWith("--add") || _command.StartsWith("-a")) {
                            AddConnection();
                        } else if (_command.StartsWith("--help") || _command.StartsWith("-h")) {
                            _command = (_command.StartsWith("--help") ? _command.Remove(0, 6) : _command.Remove(0, 2)).Trim();
                            if (_command.Length > 0) {
                                var title = "Help for " + _command;
                                Console.WriteLine(title);
                                for (int i = 0; i < title.Length; i++) Console.Write("=");
                                Console.WriteLine();
                                switch (_command) {
                                    case "-v":
                                    case "--version": {
                                            Console.WriteLine("Syntax: swsh --version");
                                            Console.WriteLine("Checks the version of swsh.\n\nUsage: swsh --version\n");
                                            break;
                                        }
                                    case "-a":
                                    case "--add": {
                                            Console.WriteLine("Syntax: (swsh --add/swsh -a) [-password]");
                                            Console.WriteLine("Add a new connection either using private key or password.\n\nUsage to add using private "
                                                + "key: swsh --add\nUsage to add using a password: swsh --add -password\n\nYou'll be asked for password "
                                                + "each time as SWSH doesn't store them.\n");
                                            break;
                                        }
                                    case "--show": {
                                            Console.WriteLine("Syntax: swsh --show [nickname]");
                                            Console.WriteLine("Show nicknames if no arguments have passed. If nickname is provided, shows details of a n"
                                                + "ickname.\nUsage to check all nicknames: swsh --show\nUsage to check a nickname: swsh --show myserver");
                                            break;
                                        }
                                    case "-c":
                                    case "--connect": {
                                            Console.WriteLine("Syntax: (swsh --connect/swsh -c) [nickname]");
                                            Console.WriteLine("Connects to Server over SSH.\nUsage: swsh --connect myserver");
                                            break;
                                        }
                                    case "--delete": {
                                            Console.WriteLine("Syntax: swsh --delete [nickname]");
                                            Console.WriteLine("Deletes connection's nickname.\nUsage: swsh --delete myserver");
                                            break;
                                        }
                                    case "--edit": {
                                            Console.WriteLine("Syntax: swsh --edit [nickname] [arg]");
                                            Console.WriteLine("arg:\n\t-user [newUserName]\n\t-key [newKey]\n\t-server [newServer]");
                                            Console.WriteLine("Edits nickname, use one argument at a time.\nUsage: swsh --edit myserver -user newUSER");
                                            break;
                                        }
                                    case "-h":
                                    case "--help": {
                                            Console.WriteLine("Syntax: (swsh --help/swsh -h) [command]");
                                            Console.WriteLine("Displays this help or command details.\nUsage: swsh --help -h");
                                            break;
                                        }
                                    default:
                                        __color("ERROR: SWSH -> " + _command + " -> unknown command.\n", ConsoleColor.Red);
                                        break;
                                }
                            } else __help();
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
                                            pwd = Regex.Replace(ssh.CreateCommand("cd " + pwd + "; pwd").Execute(), @"\t|\n|\r", "");
                                            if (pwd == Regex.Replace(home, @"\t|\n|\r", "")) pwd = "~";
                                            __color(pwd, ConsoleColor.Green);
                                            Console.Write(":/ $ ");
                                            GetStdin();
                                            if (_command == "exit")
                                                break;
                                            else if (_command.StartsWith("cd")) {
                                                _command = _command.Remove(0, 3);
                                                if (_command.StartsWith("/")) pwd = _command;
                                                else if (_command.StartsWith("./")) pwd += "/" + _command.Remove(0, 2);
                                                else if (_command.StartsWith("..")) pwd = Regex.Replace(ssh.CreateCommand("cd " + pwd + "; dirname $(pwd)").Execute(), @"\t|\n|\r", "");
                                                else if (_command.Trim() == String.Empty) pwd = "~";
                                                else pwd += "/" + _command;
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
                                if (Directory.Exists(_mainDirectory)) {
                                    if (Directory.GetFiles(_mainDirectory).Length > 0) {
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
                                        __color("ERROR: ", ConsoleColor.Red);
                                        Console.WriteLine("SWSH -> no nickname(s) found. try swsh --help");
                                    }
                                } else {
                                    __color("ERROR: ", ConsoleColor.Red);
                                    Console.WriteLine("SWSH -> no nickname(s) found. try swsh --help");
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
                        } else if (_command.StartsWith("--edit")) {
                            _command = _command.Remove(0, 6);
                            String[] data = _command.Split(' ');
                            if (File.Exists(_mainDirectory + data[1] + ".swsh")) {
                                string[] arrLine = File.ReadAllLines(_mainDirectory + data[1] + ".swsh");
                                if (data[2] == "-user") arrLine[1] = data[3];
                                else if (data[2] == "-server") arrLine[2] = data[3];
                                else if (data[2] == "-key") {
                                    if (!Directory.Exists(data[3])) {
                                        __color("ERROR: ", ConsoleColor.Red);
                                        Console.Write("SWSH -> {0} -> file is non existent.\n", data[3]);
                                        __color("exit", ConsoleColor.Red);
                                        Console.Write(" or ");
                                        __color("-e", ConsoleColor.Red);
                                        Console.Write(" to cancel.\n");
                                        getKey:
                                        Console.Write("-key: ");
                                        var key = Console.ReadLine();
                                        if (key == "-e" || key == "exit") {
                                            __color("Aborted.\n", ConsoleColor.Yellow);
                                            goto end;
                                        }
                                        if (File.Exists(key)) arrLine[0] = key;
                                        else {
                                            __color("ERROR: ", ConsoleColor.Red);
                                            Console.Write("SWSH -> {0} -> file is non existent.\n", key);
                                            goto getKey;
                                        }
                                    }
                                }
                                File.WriteAllLines(_mainDirectory + data[1] + ".swsh", arrLine);
                                __color("Updated.\n", ConsoleColor.Green);
                                end:;
                            } else {
                                __color("ERROR: ", ConsoleColor.Red);
                                Console.WriteLine("SWSH -> {0} -> nickname does not exists", data[1]);
                            }
                        } else if (_command == "clear") {
                            Console.Clear();
                            __version();
                            Console.Write("swsh --help or -h for help.\n\n");
                        } else if (_command == "exit") break;
                        else {
                            Console.ForegroundColor = ConsoleColor.Red;
                            __help();
                            Console.ResetColor();
                        }
                    } else if (_command == "ls") {
                        if (Directory.GetFiles(_workingDirectory).Length > 0) {
                            __color("files: \n", ConsoleColor.Cyan);
                            foreach (var file in Directory.GetFiles(_workingDirectory))
                                Console.WriteLine(Path.GetFileName(file));
                        }
                        if (Directory.GetDirectories(_workingDirectory).Length > 0) {
                            __color("\ndirectories: \n", ConsoleColor.DarkCyan);
                            foreach (var dir in Directory.GetDirectories(_workingDirectory))
                                Console.WriteLine((dir.Replace(Path.GetDirectoryName(dir) + Path.DirectorySeparatorChar, "")).Replace('\\', '/'));
                        }
                        if (Directory.GetDirectories(_workingDirectory).Length == 0 && Directory.GetFiles(_workingDirectory).Length == 0) __color("No files or directories here.\n", ConsoleColor.Yellow);
                    } else if (_command.StartsWith("cd")) {
                        _command = _command.Remove(0, 3);
                        if (_command == "..") __changeWorkingDir(Path.GetDirectoryName(_workingDirectory));
                        else if (_command.StartsWith("./")) __changeWorkingDir(_workingDirectory + "/" + _command.Remove(0, 2));
                        else if (_command.StartsWith("/")) __changeWorkingDir(Path.GetPathRoot(_workingDirectory) + _command.Remove(0, 1));
                        else __changeWorkingDir(_workingDirectory + "/" + _command);
                    } else __color("ERROR: SWSH -> " + _command + " -> unknown command.\n", ConsoleColor.Red);
                } catch (Exception exp) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine(exp.Message);
                }
            }
        }
        public static void __changeWorkingDir(string path) {
            path = path.Replace('\\', '/');
            if (Directory.Exists(path)) _workingDirectory = path;
            else {
                __color("ERROR: ", ConsoleColor.Red);
                Console.WriteLine("SWSH -> {0} -> path does not exists", path);
            }
        }
        public static void __version() {
            Console.Write("   ______       _______ __  __\n  / ___/ |     / / ___// / / /\n  \\__ \\| | /| / /\\__ \\/ /_/ / \n ___/ /| |/ |/ /___/ / __"
                + "  /  \n/____/ |__/|__//____/_/ /_/   \n     Secure Windows Shell     \n");
            Console.Write("\nRelease: {0}\n{1}", _version, "(c) Muhammad Muzzammil & Nabeel Omer\n");
        }
        public static void __color(string message, ConsoleColor cc) {
            Console.ForegroundColor = cc;
            Console.Write(message);
            Console.ResetColor();
        }
        public static void __help() {
            Console.Write("swsh [command] [arg]\n");
            Console.WriteLine("\t-v --version:                  -Check the version of swsh.");
            Console.WriteLine("\t-a --add     [-password]*      -Add a new connection either using private key or password (-password).");
            Console.WriteLine("\t   --show    [nickname]*       -Show nicknames/Details of a nickname.");
            Console.WriteLine("\t-c --connect [nickname]        -Connects to Server over SSH.");
            Console.WriteLine("\t   --delete  [nickname]        -Deletes connection's nickname.");
            Console.WriteLine("\t   --edit    [nickname] [arg]  -Edits nickname, use one argument at a time.");
            Console.WriteLine("\t-h --help:   [command]*        -Displays this help or command details.");
            Console.WriteLine("\tclear                          -Clears the console.");
            Console.WriteLine("\texit                           -Exits.");
            Console.WriteLine("ls                                     -Lists all files and directories in working directory.");
            Console.WriteLine("cd [arg]                               -Changes directory to 'arg'. arg = directory name.");
            Console.WriteLine("\n\nNOTES:\n[1] * = Optional.");
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
        private void GetStdin() {
            _command = Console.ReadLine();
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
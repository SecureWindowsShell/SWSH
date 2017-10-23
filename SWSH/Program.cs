/*
    Copyright (C) 2017  Muhammad Muzzammil & Nabeel Omer
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using Renci.SshNet;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace SWSH {
    public static class Program {
        public const string _version = "1.3";
        public static string _command = "", _codename = "beta", _mainDirectory = "swsh-data/",
            _workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static void Main(string[] args) {
            Console.Title = "SWSH - " + __version();
            /* REMOVED FOR UNSTABLE BRANCH
             * __checkHash(args.Any((x) => x == "--IgnoreChecksumMismatch"));
             */
            Console.Write("swsh --help or -h for help.\n\n");
            __start();
        }
        private static void __start() {
            while (true) {
                try {
                    __color(_workingDirectory.Replace('\\', '/').Remove(0, 2).ToLower() + ":", ConsoleColor.DarkCyan);
                    __color("swsh> ", ConsoleColor.DarkGray);
                    _command = __getCommand();
                    if (_command.StartsWith("swsh")) {
                        _command = _command.Replace("swsh", "").Trim();
                        if (_command == "--version" || _command == "-v") __version();
                        else if (_command.StartsWith("--add") || _command.StartsWith("-a")) __addConnection();
                        else if (_command.StartsWith("--help") || _command.StartsWith("-h")) __interactiveHelp();
                        else if (_command.StartsWith("--connect") || _command.StartsWith("-c")) __connect();
                        else if (_command.StartsWith("--show")) __show();
                        else if (_command.StartsWith("--delete")) __delete();
                        else if (_command.StartsWith("--edit")) __edit();
                        else if (_command == "clear") __clear();
                        else if (_command == "exit") break;
                        else __help();
                    } else if (_command == "ls") __ls();
                    else if (_command.StartsWith("cd")) __cd();
                    else if (_command.StartsWith("upload")) __upload();
                    else if(_command.Trim() != "") __color("ERROR: SWSH -> " + _command + " -> unknown command.\n", ConsoleColor.Red);
                } catch (Exception exp) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine(exp.Message);
                }
            }
        }
        private static void __addConnection() {
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
                    key = __getCommand();
                    if (key.Trim() == String.Empty) {
                        __color("ERROR: ", ConsoleColor.Red);
                        Console.Write("SWSH -> key path should not be empty!\n", key);
                    } else {
                        __checkexit(key);
                        if (!File.Exists(key)) {
                            __color("ERROR: ", ConsoleColor.Red);
                            Console.Write("SWSH -> {0} -> file is non existent.\n", key);
                        } else {
                            break;
                        }
                    }
                }
            }

            var usr = "";
            while (true) {
                Console.Write("Username: ");
                usr = __getCommand();
                __checkexit(usr);
                if (usr.Trim() == String.Empty) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.Write("SWSH -> username should not be empty!\n");
                } else break;
            }

            var svr = "";
            while (true) {
                Console.Write("Server: ");
                svr = __getCommand();
                __checkexit(svr);
                if (svr.Trim() == String.Empty) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.Write("SWSH -> IP address or domain name should not be empty!\n");
                } else break;
            }

            var nkn = "";
            while (true) {
                Console.Write("Unique Nickname: ");
                nkn = __getCommand();
                __checkexit(nkn);
                if (nkn.Trim() == string.Empty) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.Write("SWSH -> nickname should not be empty!\n");
                } else if (File.Exists(_mainDirectory + nkn + ".swsh")) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> {0} -> nickname exists", nkn);
                } else {
                    break;
                }
            }
            String[] data = new String[] { key, usr, svr };
            if (!Directory.Exists(_mainDirectory)) Directory.CreateDirectory(_mainDirectory);
            File.AppendAllLines(__getNickname(nkn), data);
        }
        private static void __interactiveHelp() {
            _command = (_command.StartsWith("--help") ? _command.Remove(0, 6).Trim() : _command.Remove(0, 2)).Trim();
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
        }
        private static void __help() {
            Console.Write("swsh [command] [arg]\n");
            Console.WriteLine("\t-v --version:                  -Check the version of swsh.");
            Console.WriteLine("\t-a --add     [-password]*      -Add a new connection either using private key or password (-password).");
            Console.WriteLine("\t   --show    [nickname]*       -Show nicknames/Details of a nickname.");
            Console.WriteLine("\t-c --connect [nickname]        -Connects to Server over SSH.");
            Console.WriteLine("\t   --delete  [nickname]        -Deletes connection's nickname.");
            Console.WriteLine("\t   --edit    [nickname] [arg]  -Edits nickname, use one argument at a time.");
            Console.WriteLine("\t-h --help    [command]*        -Displays this help or command details.");
            Console.WriteLine("\tclear                          -Clears the console.");
            Console.WriteLine("\texit                           -Exits.");
            Console.WriteLine("ls                                     -Lists all files and directories in working directory.");
            Console.WriteLine("cd [arg]                               -Changes directory to 'arg'. arg = directory name.");
            Console.WriteLine("upload [args] [nickname]:[location]    -Uploads files and directories. 'upload -h' for help.");
            Console.WriteLine("\n\nNOTES:\n[1] * = Optional.");
        }
        private static void __connect() {
            ConnectionInfo ccinfo;
            string nickname = (_command.StartsWith("--connect")) ? _command.Remove(0, 10) : _command.Remove(0, 3);
            if (File.Exists(_mainDirectory + nickname + ".swsh")) {
                if (File.ReadAllLines(_mainDirectory + nickname + ".swsh")[0] == "-password") {
                    Console.Write("Password for {0}: ", nickname);
                    ccinfo = __CreateConnectionInfoPassword(nickname, __getCommand());
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
                            _command = __getCommand();
                            ssh.CreateCommand(String.Format("echo \"[{0} UTC]\t=>\t{1}\" >> ~/.swsh_history", DateTime.UtcNow, _command)).Execute();
                            if (_command == "exit")
                                break;
                            else if (_command.StartsWith("cd")) {
                                _command = _command.Remove(0, 3);
                                if (_command.StartsWith("/")) pwd = _command;
                                else if (_command.StartsWith("./")) pwd += "/" + _command.Remove(0, 2);
                                else if (_command.StartsWith("..")) pwd = Regex.Replace(ssh.CreateCommand("cd " + pwd + "; dirname $(pwd)").Execute(), @"\t|" +
                                    "\n|\r", "");
                                else if (_command.Trim() == String.Empty) pwd = "~";
                                else pwd += "/" + _command;
                            } else if (_command == "clear") Console.Clear();
                            else if (_command.StartsWith("swsh")) {
                                __color("ERROR: ", ConsoleColor.Red);
                                Console.Write("SWSH -> can't execute swsh while in connection\n", nickname);
                            } else {
                                var result = ssh.CreateCommand("cd " + pwd + "; " + _command).Execute();
                                Console.Write(result);
                            }
                        }
                        ssh.Disconnect();
                    }
                    __color("Connection to " + ccinfo.Username + "@" + ccinfo.Host + ", closed.\n", ConsoleColor.Yellow);
                }
            } else {
                __color("ERROR: ", ConsoleColor.Red);
                Console.Write("SWSH -> {0} -> nickname does not exists\n", nickname);
            }
        }
        private static void __show() {
            _command = _command.Remove(0, 6);
            if (_command.Trim() == string.Empty) {
                if (Directory.Exists(_mainDirectory) && Directory.GetFiles(_mainDirectory).Length > 0) {
                    foreach (var file in Directory.GetFiles(_mainDirectory)) {
                        try {
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
                        } catch (Exception exp) {
                            __color("ERROR: ", ConsoleColor.Red);
                            Console.WriteLine(exp.Message);
                        }
                    }
                } else {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> no nickname(s) found. try swsh --help");
                }
            } else {
                _command = _command.Trim();
                var file = _mainDirectory + _command + ".swsh";
                try {
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
                } catch (Exception exp) {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine(exp.Message);
                }
            }
        }
        private static void __delete() {
            try {
                if (File.Exists(__getNickname(_command.Replace("--delete", "").Trim()))) {
                    __color("Are you sure you want to delete this nickname? (y/n): ", ConsoleColor.Red);
                    var ans = __getCommand().ToUpper();
                    if (ans == "Y") {
                        Console.Write("Type the nickname to confirm: ");
                        var name = __getCommand();
                        if (name != _command.Replace("--delete", "").Trim()) __color("Aborted.\n", ConsoleColor.Yellow);
                        else {
                            File.Delete(__getNickname(_command.Replace("--delete", "").Trim()));
                            __color("Deleted.\n", ConsoleColor.Green);
                        }
                    } else __color("Aborted.\n", ConsoleColor.Yellow);
                } else {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> {0} -> nickname does not exists", _command.Replace("--delete", "").Trim());
                }
            } catch (Exception exp) {
                __color("ERROR: ", ConsoleColor.Red);
                Console.WriteLine(exp.Message);
            }
        }
        private static void __edit() {
            _command = _command.Remove(0, 6);
            String[] data = _command.Split(' ');
            if (File.Exists(__getNickname(data[1]))) {
                string[] arrLine = File.ReadAllLines(__getNickname(data[1]));
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
                        while (true) {
                            Console.Write("-key: ");
                            var key = __getCommand();
                            if (key == "-e" || key == "exit") {
                                __color("Aborted.\n", ConsoleColor.Yellow);
                                break;
                            }
                            if (File.Exists(key)) {
                                arrLine[0] = key;
                                break;
                            } else {
                                __color("ERROR: ", ConsoleColor.Red);
                                Console.Write("SWSH -> {0} -> file is non existent.\n", key);
                            }
                        }
                    }
                }
                File.WriteAllLines(__getNickname(data[1]), arrLine);
                __color("Updated.\n", ConsoleColor.Green);
            } else {
                __color("ERROR: ", ConsoleColor.Red);
                Console.WriteLine("SWSH -> {0} -> nickname does not exists", data[1]);
            }
        }
        private static void __clear() {
            Console.Clear();
            __version();
            Console.Write("swsh --help or -h for help.\n\n");
        }
        private static void __ls() {
            if (Directory.GetDirectories(_workingDirectory).Length > 0) {
                List<string> data = new List<string>();
                Directory.GetDirectories(_workingDirectory).ToList().ForEach(dir => data.Add(dir));
                Directory.GetFiles(_workingDirectory).ToList().ForEach(file => data.Add(file));
                data.Sort();
                Console.WriteLine("Size\tUser        Date Modified   Name\n====\t====        =============   ====");
                data.ForEach(x => {
                    if (File.Exists(x)) {
                        var info = new FileInfo(x);
                        if (!info.Attributes.ToString().Contains("Hidden")) {
                            var owner = File.GetAccessControl(x).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString().Split('\\')[1];
                            var size = ((info.Length > 1024) ? (((info.Length / 1024) > 1024) ? (info.Length / 1024) / 1024 : info.Length / 1024) : 
                            info.Length).ToString();
                            var toApp = "";
                            owner = (owner.Length >= 10) ? owner.Remove(5) + "..." + owner.Remove(0, owner.Length - 2) : owner;
                            if (owner.Length < 10) for (int i = 0; i < 10 - owner.Length; i++) toApp += " ";
                            owner += toApp;
                            if (size.Length < 4) for (int i = 0; i < 3 - size.Length; i++) toApp += " ";
                            size = toApp + size;
                            __color(size, ConsoleColor.Green);
                            __color((info.Length > 1024) ? (((info.Length / 1024) > 1024) ? "MB" : "KB") : "B", ConsoleColor.DarkGreen);
                            __color(String.Format("\t{0}  ", owner), ConsoleColor.Yellow);
                            __color(String.Format("{0}{1} {2} {3}", (
                                String.Format("{0:d}", info.LastWriteTime.Date).Split('/')[0].Length > 1 ? "" : " "),
                                String.Format("{0:d}", info.LastWriteTime.Date).Split('/')[0],
                                String.Format("{0:m}", info.LastWriteTime.Date).Remove(3),
                                String.Format("{0:HH:mm}    ", info.LastWriteTime.ToLocalTime())), ConsoleColor.Blue);
                            __color(info.Name, (Path.GetFileNameWithoutExtension(x).Length > 0) ? ConsoleColor.Magenta : ConsoleColor.Cyan);
                            Console.WriteLine();
                        }
                    } else if (Directory.Exists(x)) {
                        var info = new DirectoryInfo(x);
                        if (!info.Attributes.ToString().Contains("Hidden")) {
                            var owner = File.GetAccessControl(x).GetOwner(typeof(System.Security.Principal.NTAccount)).ToString().Split('\\')[1];
                            owner = (owner.Length >= 10) ? owner.Remove(5) + "..." + owner.Remove(0, owner.Length - 2) : owner;
                            var toApp = "";
                            if (owner.Length < 10) for (int i = 0; i < 10 - owner.Length; i++) toApp += " ";
                            owner += toApp;
                            __color("   -", ConsoleColor.DarkGray);
                            __color(String.Format("\t{0}  ", owner), ConsoleColor.Yellow);
                            __color(String.Format("{0}{1} {2} {3}", (
                                String.Format("{0:d}", info.LastWriteTime.Date).Split('/')[0].Length > 1 ? "" : " "),
                                String.Format("{0:d}", info.LastWriteTime.Date).Split('/')[0],
                                String.Format("{0:m}", info.LastWriteTime.Date).Remove(3),
                                String.Format("{0:HH:mm}    ", info.LastWriteTime.ToLocalTime())), ConsoleColor.Blue);
                            __color(info.Name, 
                                (info.Name.StartsWith(".")) ? ConsoleColor.DarkCyan : (info.GetFiles().Length > 0 || info.GetDirectories().Length > 0) ? 
                                ConsoleColor.White : ConsoleColor.DarkGray);
                            __color((info.GetFiles().Length == 0 && info.GetDirectories().Length == 0) ? "  <empty>" : "", ConsoleColor.DarkRed);
                            Console.WriteLine();
                        }
                    }
                });
            }
            if (Directory.GetDirectories(_workingDirectory).Length == 0 && Directory.GetFiles(_workingDirectory).Length == 0) __color("No file" +
                "s or directories here.\n", ConsoleColor.Yellow);
        }
        private static void __cd() {
            _command = _command.Remove(0, 3);
            if (_command == "..") __changeWorkingDir(Path.GetDirectoryName(_workingDirectory));
            else if (_command.StartsWith("./")) __changeWorkingDir(_workingDirectory + "/" + _command.Remove(0, 2));
            else if (_command.StartsWith("/")) __changeWorkingDir(Path.GetPathRoot(_workingDirectory) + _command.Remove(0, 1));
            else __changeWorkingDir(_workingDirectory + "/" + _command);
        }
        private static void __upload() {
            _command = _command.Remove(0, 7);
            if (_command == "-h") {
                Console.WriteLine("upload [--dir]* [args] [nickname]:[location]\n\n'args' are seperated using spaces ( ) and last 'arg' will be treated as s" +
                    "erver data which includes nickname as well as the location, part after the colon (:), where the data is to be uploaded. Use flag '" +
                    "--dir' to upload directiories. Do not use absolute paths for local path, change working directory to navigate.");
            } else {
                List<string> toupload = (_command.StartsWith("--dir")) ? _command.Replace("--dir", "").Trim().Split(' ').ToList() : _command.Trim().Split(' ')
                    .ToList();
                try {
                    var serverData = toupload.Pop().Split(':');
                    var nickname = serverData[0];
                    var location = serverData[1];
                    try {
                        if (File.Exists(__getNickname(nickname))) {
                            ConnectionInfo ccinfo;
                            if (File.ReadAllLines(_mainDirectory + nickname + ".swsh")[0] == "-password") {
                                Console.Write("Password for {0}: ", nickname);
                                ccinfo = __CreateConnectionInfoPassword(nickname, __getCommand());
                            } else ccinfo = __CreateConnectionInfoKey(nickname);
                            if (ccinfo != null) {
                                if (_command.StartsWith("--dir"))
                                    using (var sftp = new SftpClient(ccinfo)) {
                                        _command = _command.Replace("--dir", "");
                                        sftp.Connect();
                                        toupload.ForEach(x => {
                                            var path = _workingDirectory + "/" + x.Trim();
                                            location = serverData[1] + ((serverData[1].EndsWith("/")) ? "" : "/") + x.Trim();
                                            if (!sftp.Exists(location)) sftp.CreateDirectory(location);
                                            __color("Uploading <directory>: " + x.Trim() + "\n", ConsoleColor.Yellow);
                                            __uploadDir(sftp, path, location);
                                            __color("Done.\n", ConsoleColor.Green);
                                        });
                                    } else
                                    using (var scp = new ScpClient(ccinfo)) {
                                        scp.Connect();
                                        toupload.ForEach(x => {
                                            var path = _workingDirectory + "/" + x.Trim();
                                            if (File.Exists(path)) {
                                                __color("Uploading <file>: " + x.Trim(), ConsoleColor.Yellow);
                                                scp.Upload(new FileInfo(path), location);
                                                __color(" -> Done\n", ConsoleColor.Green);
                                            } else {
                                                __color("ERROR: ", ConsoleColor.Red);
                                                Console.WriteLine("SWSH -> {0} -> file does not exists", path.Replace('/', '\\'));
                                            }
                                        });
                                    }
                            }
                        } else {
                            __color("ERROR: ", ConsoleColor.Red);
                            Console.WriteLine("SWSH -> {0} -> nickname does not exists", nickname);
                        }
                    } catch (Exception exp) { __color("ERROR: " + exp.Message + "\n", ConsoleColor.Red); }
                } catch {
                    __color("ERROR: ", ConsoleColor.Red);
                    Console.WriteLine("SWSH -> upload {0} -> is not the correct syntax for this command", _command);
                }
            }
        }
        private static void __uploadDir(SftpClient client, string localPath, string remotePath) {
            new DirectoryInfo(localPath).EnumerateFileSystemInfos().ToList().ForEach(x => {
                if (x.Attributes.HasFlag(FileAttributes.Directory)) {
                    string subPath = remotePath + "/" + x.Name;
                    if (!client.Exists(subPath)) client.CreateDirectory(subPath);
                    __uploadDir(client, x.FullName, remotePath + "/" + x.Name);
                } else {
                    using (Stream fileStream = new FileStream(x.FullName, FileMode.Open)) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\tUploading <file>: {0} ({1:N0} bytes)", x, ((FileInfo)x).Length);
                        client.UploadFile(fileStream, remotePath + "/" + x.Name);
                        __color(" -> Done\n", ConsoleColor.Green);
                    }
                }
            });
        }
        private static string Pop(this List<string> list) {
            var retVal = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return retVal;
        }
        private static string __version() {
            Console.Write("   ______       _______ __  __\n  / ___/ |     / / ___// / / /\n  \\__ \\| | /| / /\\__ \\/ /_/ / \n ___/ /| |/ |/ /___/ / __"
                + "  /  \n/____/ |__/|__//____/_/ /_/   \n     Secure Windows Shell     \n");
            Console.Write("\nRelease: {0}-{1}\n{2}", _codename, _version, "(c) Muhammad Muzzammil & Nabeel Omer\nSWSH is licensed under the GNU General Publ" +
                "ic License v3.0\n");
            return _codename + "-" + _version;
        }
        private static void __changeWorkingDir(string path) {
            path = path.Replace('\\', '/');
            if (Directory.Exists(path)) _workingDirectory = path;
            else {
                __color("ERROR: ", ConsoleColor.Red);
                Console.WriteLine("SWSH -> {0} -> path does not exists", path);
            }
        }
        private static void __checkexit(string keyword) {
            if (keyword == "exit" || keyword == "-e") {
                __color("Aborted.\n", ConsoleColor.Yellow);
                __start();
            }
        }
        private static void __color(string message, ConsoleColor cc) {
            Console.ForegroundColor = cc;
            Console.Write(message);
            Console.ResetColor();
        }
        private static void __checkHash(bool ignore) {
            try {
                // Crash course to functional programming!
                // This took the both of us 30 minutes to write, never change it!
                if (!new System.Collections.Generic.List<byte>(
                    new System.Security.Cryptography.SHA1CryptoServiceProvider()
                    .ComputeHash(File.ReadAllBytes(System.Reflection.Assembly.GetExecutingAssembly().Location)))
                    .Select((x) => x.ToString("x2"))
                    .Aggregate((x, y) => x + y)
                    .Equals(new System.Net.WebClient().DownloadString("https://raw.githubusercontent.com/SecureWindowsShell/SWSH/master/checksum?" +
                    new Random().Next())))
                    throw new Exception();

            } catch (Exception) {
                __color("ERROR: Checksum Mismatch! This executable may be out of date or malicious!\n", ConsoleColor.Red);
                if (!ignore) {
                    Console.Read();
                    Environment.Exit(500);
                } else {
                    Console.WriteLine(new System.Collections.Generic.List<byte>(
                        new System.Security.Cryptography.SHA1CryptoServiceProvider()
                        .ComputeHash(File.ReadAllBytes(System.Reflection.Assembly.GetExecutingAssembly().Location)))
                        .Select((x) => x.ToString("x2"))
                        .Aggregate((x, y) => x + y));
                    Console.WriteLine(new System.Net.WebClient().DownloadString("https://raw.githubusercontent.com/SecureWindowsShell/SWSH/master/checksum?" +
                        new Random().Next()));
                }
            }
        }
        private static string __getNickname(string s) => _mainDirectory + s + ".swsh";
        private static string __getCommand() {
            var read = Console.ReadLine();
            File.AppendAllText(".swsh_history", String.Format("[{0} UTC]\t=>\t{1}\n", DateTime.UtcNow, read));
            return read;
        }
        private static ConnectionInfo __CreateConnectionInfoKey(string nickname) {
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
        private static ConnectionInfo __CreateConnectionInfoPassword(string nickname, string password) {
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
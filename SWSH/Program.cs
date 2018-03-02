/*
 *  SWSH - Secure Windows Shell
 *  Copyright (C) 2017  Muhammad Muzzammil
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using Renci.SshNet;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using System.Xml;

namespace SWSH {
    public static class Program {
        public static bool _keygenstatus;
        public const string
            _absVersion = "0.4",
            _version = "",
            _codename = "unstable-titan";
        public static string
            _command = "",
            _swshAppdata = Environment.GetFolderPath((Environment.SpecialFolder.ApplicationData)) + "/SWSH",
            _swshHistory = _swshAppdata + "/swsh_history",
            _swshKeys = _swshAppdata + "/swsh_keys",
            _swshLicense = _swshAppdata + "/LICENSE.txt",
            _workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        static void Main(string[] args) {
            if (!Directory.Exists(_swshAppdata)) Directory.CreateDirectory(_swshAppdata);
            Console.Title = "SWSH - Secure Windows Shell";
            __notice();
            Console.Write("\nType `license notice` to view this notice again.\n");
            for (int i = 0; i < 5; i++) {
                Console.Write($"\rStarting in {5 - (i + 1)}s");
                Thread.Sleep(1000);
            }
            Console.Clear();
            /* Downloading License; if does not exists. START */
            try {
                if (!File.Exists(_swshLicense)) {
                    Console.WriteLine("License file not found, downloading...");
                    new WebClient().DownloadFile(new Uri(Url.License), _swshLicense);
                    Console.Clear();
                }
            } catch (Exception exp) { __error($"Unable to download License, view online copy here: {Url.License}\nReason:{exp.Message}\n"); }
            /* Downloading License; if does not exists. END   */
            Console.Title = $"SWSH - {__version()}";
            if (!__unstable()) _keygenstatus = __checkHash(args.Any((x) => x == "--IgnoreChecksumMismatch"));
            Console.Write("Use `help` command for help.\n\n");
            try {
                var handle = ExternalFunctions.GetStdHandle(-11);
                ExternalFunctions.GetConsoleMode(handle, out var mode);
                ExternalFunctions.SetConsoleMode(handle, mode | 0x4);
                ExternalFunctions.GetConsoleMode(handle, out mode);
            } catch (Exception exp) { __error($"{exp.Message}\n"); }
            __start();
        }
        private static void __start() {
            while (true) {
                try {
                    __color($"{_workingDirectory.Replace('\\', '/').Remove(0, 2).ToLower()}:", ConsoleColor.DarkCyan);
                    __color("swsh> ", ConsoleColor.DarkGray);
                    _command = __getCommand();
                    if (_command.StartsWith("swsh")) {
                        __color("WARNING:\nThis type of commands is deprecated and will stop working in future.\nPlease take a look at our latest documentat" +
                            "ion or use `help` command.\n", ConsoleColor.Yellow);
                        if (_command.StartsWith("swsh --")) _command = _command.Remove(0, 7);
                    }
                    if (_command == "version") __version();
                    else if (_command.StartsWith("help")) __help();
                    else if (_command.StartsWith("connect")) __connect();
                    else if (_command.StartsWith("keygen")) __keygen();
                    else if (_command == "ls") __ls();
                    else if (_command.StartsWith("cd")) __cd();
                    else if (_command.StartsWith("upload")) __upload();
                    else if (_command == "clear") __clear();
                    else if (_command == "license") File.ReadAllLines(_swshLicense).ToList().ForEach(x => Console.WriteLine(x));
                    else if (_command == "license notice") __notice();
                    else if (_command == "pwd") Console.WriteLine(_workingDirectory.ToLower());
                    else if (_command.StartsWith("computehash")) __printHash();
                    else if (_command == "exit") Environment.Exit(0);
                    else if (_command.Trim() != "") __error($"SWSH -> {_command} -> unknown command.\n");
                } catch (Exception exp) { __error($"{exp.Message}\n"); }
            }
        }
        private static void __help() {
            _command = _command.Remove(0, 4).Trim();
            if (_command.Length > 0) {
                var title = $"Help for {_command}";
                Console.WriteLine(title);
                for (int i = 0; i < title.Length; i++) Console.Write("=");
                Console.WriteLine();
                switch (_command) {
                    case "version": {
                            Console.WriteLine("Syntax: version");
                            Console.WriteLine("Checks the version of swsh.\n\nUsage: version\n");
                            break;
                        }
                    case "connect": {
                            Console.WriteLine("Syntax: connect [user@host] (-p)");
                            Console.WriteLine("Connects to Server over SSH. Use `-p` for password connection.\nUsage: connect root@server.ip");
                            break;
                        }
                    case "keygen": {
                            Console.WriteLine("Syntax: keygen (options)");
                            Console.WriteLine("Generates, imports or show SSH RSA key pair. Requires swsh-keygen.exe.");
                            Console.WriteLine("Default values are provided in parentheses.");
                            Console.WriteLine("\nOptions:\n\timport\t\t- Imports RSA key pair.");
                            Console.WriteLine("\tshow [private]\t- Print RSA keys. By default, prints public key. Use `private` to print private key.");
                            break;
                        }
                    case "pwd": {
                            Console.WriteLine("Syntax: pwd");
                            Console.WriteLine("Prints working directory.\nUsage: pwd");
                            break;
                        }
                    case "computehash": {
                            Console.WriteLine("Syntax: computehash [(>/>>) path/to/file]");
                            Console.WriteLine("Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen.\n");
                            Console.WriteLine("Usage:\nTo overwrite-> computehash > path/to/file\nTo append-> computehash >> path/to/file");
                            break;
                        }
                    case "help": {
                            Console.WriteLine("Syntax: help [command]");
                            Console.WriteLine("Displays this help or command details.\nUsage: help add");
                            break;
                        }
                    default:
                        __error($"SWSH -> {_command} -> unknown command.\n");
                        break;
                }
            } else {
                Console.Write(
                      "Usage: <command> [arguments] (options)\n"
                    + "Available commands:\n"
                    + "  version                             -Check the version of swsh.\n"
                    + "  connect [user@host] (-p)            -Connects to Server over SSH.\n"
                    + "  keygen              (options)       -Generates, imports or show SSH RSA key pair. `help keygen` for more.\n"
                    + "  help    [command]                   -Displays this help or command details.\n"
                    + "  clear                               -Clears the console.\n"
                    + "  pwd                                 -Prints working directory.\n"
                    + "  computehash [(>/>>) path/to/file]   -Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen.\n"
                    + "  exit                                -Exits.\n"
                    + "  ls                                  -Lists all files and directories in working directory.\n"
                    + "  cd [arg]                            -Changes directory to 'arg'. arg = directory name.\n"
                    + "  upload [args] [nickname]:[location] -Uploads files and directories. 'upload -h' for help.\n");
            }
        }
        private static void __connect() {
            if (!File.Exists(_swshKeys)) {
                Console.Write("SWSH private key file not found. (I)mport or (G)enerate?: ");
                switch (Console.ReadKey().Key) {
                    case ConsoleKey.I:
                        __importKey();
                        break;
                    case ConsoleKey.G:
                        __keygen();
                        break;
                    default:
                        Console.WriteLine(" <= Invalid option.");
                        return;
                }
                return;
            }
                if (String.IsNullOrEmpty(__readKeys()[1])) __color("WARNING: No public key detected.\n", ConsoleColor.Yellow);

            ConnectionInfo ccinfo;
            ccinfo = __CreateConnection(_command.Remove(0, 8));
            if (ccinfo != null) {
                Console.Write($"Waiting for response from {ccinfo.Username}@{ccinfo.Host}...\n");
                using (var ssh = new SshClient(ccinfo)) {
                    ssh.Connect();
                    __color($"Connected to {ccinfo.Username}@{ccinfo.Host}...\n", ConsoleColor.Green);
                    var actual = ssh.CreateShellStream(
                        "xterm-256color",
                        (uint) Console.BufferWidth,
                        (uint) Console.BufferHeight,
                        (uint) Console.BufferWidth,
                        (uint) Console.BufferHeight,
                        Console.BufferHeight, null);
                    //Read Thread
                    var read = new Thread(() => {
                        if (actual.CanRead)
                            while (true)
                                Console.WriteLine(actual.ReadLine());
                    });
                    //Write Thread
                    new Thread(() => {
                        if (actual.CanWrite)
                            while (true) {
                                try {
                                    actual.WriteLine("");
                                    var input = Console.ReadLine();
                                    Console.Write("\b\r\b\r");
                                    actual.WriteLine(input);
                                    if (input == "exit") {
                                        actual.Dispose();
                                        read.Abort();
                                        throw new Exception();
                                    }
                                }
                                catch (Exception) {
                                    __color($"Connection to {ccinfo.Username}@{ccinfo.Host}, closed.\n",
                                        ConsoleColor.Yellow);
                                    __color("(E)xit SWSH - Any other key to reload SWSH: ", ConsoleColor.Blue);
                                    var key = Console.ReadKey();
                                    if (key.Key != ConsoleKey.E)
                                        Process.Start(Assembly.GetExecutingAssembly().Location);
                                    ssh.Disconnect();
                                    Environment.Exit(0);
                                }
                            }
                    }).Start();
                    read.Start();
                    while (true) ;
                }
            }
        }

        private static void __importKey() {
            string[] data = new string[2];
            Console.WriteLine("\nImporting keys...");
            while (true) {
                Console.Write("Enter path to private key: ");
                data[0] = __getCommand();
                if (data[0].Trim() == String.Empty)
                    __error("SWSH -> key path should not be empty!\n");
                else {
                    if (File.Exists(data[0]) && File.Exists($"{_workingDirectory}/{data[0]}"))
                        __error($"SWSH -> {data[0]} -> file path is ambiguous.\n");
                    else if (File.Exists($"{_workingDirectory}/{data[0]}")) {
                        data[0] = $"{_workingDirectory.Replace('\\', '/')}/{data[0]}";
                        break;
                    } else if (!File.Exists(data[0]))
                        __error($"SWSH -> {data[0]} -> file is non existent.\n");
                    else break;
                }
            }

            Console.Write("Import public key? (y/n): ");
            if (__getCommand().ToUpper() == "Y") {
                while (true) {
                    Console.Write("Enter path to public key: ");
                    data[1] = __getCommand();
                    if (data[1].Trim() == String.Empty)
                        __error("SWSH -> key path should not be empty!\n");
                    else {
                        if (File.Exists(data[1]) && File.Exists($"{_workingDirectory}/{data[1]}"))
                            __error($"SWSH -> {data[1]} -> file path is ambiguous.\n");
                        else if (File.Exists($"{_workingDirectory}/{data[0]}")) {
                            data[0] = $"{_workingDirectory.Replace('\\', '/')}/{data[0]}";
                            break;
                        } else if (!File.Exists(data[1]))
                            __error($"SWSH -> {data[1]} -> file is non existent.\n");
                        else break;
                    }
                }
            } else
                Console.Write("\r\b\rImport public key? (y/n): ...skipped\n");
            __writeKeys(data[0], data[1] == null ? "" : data[1]);
        }
        private static void __keygen() {
            _command = (_command.Length > 7) ? _command.Remove(0, 7):null;
            if (_command != null && _command.StartsWith("show")) {
                if (_command.Remove(0, 4).Trim() == "private") {
                    Console.WriteLine(__readKeys()[0]);
                } else {
                    if (!String.IsNullOrEmpty(__readKeys()[1])) Console.WriteLine(__readKeys()[1]);
                    else Console.WriteLine("No public key detected.");
                }
                return;
            }
            if (File.Exists(_swshKeys)) {
                __color("WARNING: This action will overwrite previously generated or imported keys in the data file but not the original keys. Continue? (y/n): ", ConsoleColor.Yellow);
                if (Console.ReadKey().Key != ConsoleKey.Y) {
                    Console.WriteLine();
                    return;
                }
            }
            if (_command == "import") {
                __importKey();
                return;
            }
            if (!_keygenstatus ^ __unstable()) {
                __color("Key generation is unavailable.\n", ConsoleColor.DarkBlue);
                return;
            }
            if (File.Exists("swsh-keygen.exe")) {
                if (!__checkHash(true) ^ __unstable()) return;
                Console.WriteLine("\nGenerating public/private rsa key pair.");
                string privateFile, publicFile;
                __color("exit", ConsoleColor.Red);
                Console.Write(" or ");
                __color("-e", ConsoleColor.Red);
                Console.Write(" to cancel.\n");
                do {
                    __color("Enter absolute path to save private key (%appdata%/SWSH/swsh.private):\t", ConsoleColor.Yellow);
                    privateFile = __getCommand();
                    if (privateFile == String.Empty) privateFile = _swshAppdata + "/swsh.private";
                    else if (privateFile == "-e" || privateFile == "exit") return;
                } while (!isWritable(privateFile));
                do {
                    __color("Enter absolute path to save public key (%appdata%/SWSH/swsh.public):\t", ConsoleColor.Yellow);
                    publicFile = __getCommand();
                    if (publicFile == String.Empty) publicFile = _swshAppdata + "/swsh.public";
                    else if (publicFile == "-e" || privateFile == "exit") return;
                } while (!isWritable(publicFile));
                bool isWritable(string path) {
                    if (File.Exists(path)) {
                        __color($"File exists: {new FileInfo(path).FullName}\n\n\nOverwrite? (y/n): ", ConsoleColor.Red);
                        return (__getCommand().ToUpper() == "Y") ? true : false;
                    } else return true;
                }
                var keygenProcess = new Process {
                    StartInfo = new ProcessStartInfo() {
                        FileName = "swsh-keygen.exe",
                        Arguments = $"-pub={new FileInfo(publicFile).FullName} -pri={new FileInfo(privateFile).FullName}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                keygenProcess.Start();
                keygenProcess.WaitForExit();
                if (keygenProcess.ExitCode != 0) {
                    __color($"WARNING: swsh-keygen exited with exit code {keygenProcess.ExitCode}.", ConsoleColor.Yellow);
                    return;
                }
                __color($"Your public key:\n\n{File.ReadAllLines(publicFile)[0]}\n", ConsoleColor.Green);
                __writeKeys(privateFile, publicFile);
            } else __error($"The binary 'swsh-keygen.exe' was not found. Are you sure it's installed?\nSee: {Url.Keygen}.\n");

            return;
        }
        private static void __clear() {
            Console.Clear();
            __version();
            Console.Write("Use `help` command for help.\n\n");
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
                            var owner = File.GetAccessControl(x).GetOwner(typeof(NTAccount)).ToString().Split('\\')[1];
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
                            __color($"\t{owner}  ", ConsoleColor.Yellow);
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
                            var owner = File.GetAccessControl(x).GetOwner(typeof(NTAccount)).ToString().Split('\\')[1];
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
            if (Directory.GetDirectories(_workingDirectory).Length == 0 && Directory.GetFiles(_workingDirectory).Length == 0)
                __color("No files or directories here.\n", ConsoleColor.Yellow);
        }
        private static void __cd() {
            if ((_command = _command.Remove(0, 3)) == "..") __changeWorkingDir(Path.GetDirectoryName(_workingDirectory));
            else if (_command.StartsWith("./")) __changeWorkingDir($"{_workingDirectory}/{_command.Remove(0, 2)}");
            else if (_command.StartsWith("/")) __changeWorkingDir(Path.GetPathRoot(_workingDirectory) + _command.Remove(0, 1));
            else __changeWorkingDir($"{_workingDirectory}/{_command}");
            void __changeWorkingDir(string path) {
                path = path.Replace('\\', '/');
                if (Directory.Exists(Path.GetFullPath(path))) _workingDirectory = Path.GetFullPath(path);
                else __error($"SWSH -> {path} -> path does not exists.\n");
            }
        }
        private static void __upload() {
            if ((_command = _command.Remove(0, 7)) == "-h") {
                Console.WriteLine("upload [--dir]* [args] [nickname]:[location]\n\n'args' are seperated using spaces ( ) and last 'arg' will be treated as s" +
                    "erver data which includes nickname as well as the location, part after the colon (:), where the data is to be uploaded. Use flag '--dir" +
                    "' to upload directiories. Do not use absolute paths for local path, change working directory to navigate.");
            } else {
                List<string> toupload = (_command.StartsWith("--dir")) ? _command.Replace("--dir", "").Trim().Split(' ').ToList() : _command.Trim().Split(' ')
                    .ToList();
                try {
                    var serverData = toupload.Pop().Split(':');
                    var data = serverData[0];
                    var location = serverData[1];
                    try {
                            ConnectionInfo ccinfo;
                            ccinfo = __CreateConnection(data);
                        if (ccinfo != null) {
                            if (_command.StartsWith("--dir"))
                                using (var sftp = new SftpClient(ccinfo)) {
                                    _command = _command.Replace("--dir", "");
                                    sftp.Connect();
                                    toupload.ForEach(x => {
                                        var path = $"{_workingDirectory}/{x.Trim()}";
                                        location = serverData[1] + ((serverData[1].EndsWith("/")) ? "" : "/") +
                                                   x.Trim();
                                        if (!sftp.Exists(location)) sftp.CreateDirectory(location);
                                        __color($"Uploading <directory>: {x.Trim()}\n", ConsoleColor.Yellow);
                                        __uploadDir(sftp, path, location);
                                        __color("Done.\n", ConsoleColor.Green);
                                    });
                                }
                            else
                                using (var scp = new ScpClient(ccinfo)) {
                                    scp.Connect();
                                    toupload.ForEach(x => {
                                        var path = $"{_workingDirectory}/{x.Trim()}";
                                        if (File.Exists(path)) {
                                            __color($"Uploading <file>: {x.Trim()}", ConsoleColor.Yellow);
                                            scp.Upload(new FileInfo(path), location);
                                            __color(" -> Done\n", ConsoleColor.Green);
                                        }
                                        else __error($"SWSH -> {path.Replace('/', '\\')} -> file does not exists.\n");
                                    });
                                }
                        }
                    } catch (Exception exp) { __error($"{exp.Message}\n"); }
                } catch { __error($"SWSH -> upload {_command} -> is not the correct syntax for this command.\n"); }
            }
            void __uploadDir(SftpClient client, string localPath, string remotePath) {
                new DirectoryInfo(localPath).EnumerateFileSystemInfos().ToList().ForEach(x => {
                    if (x.Attributes.HasFlag(FileAttributes.Directory)) {
                        string subPath = $"{remotePath}/{x.Name}";
                        if (!client.Exists(subPath)) client.CreateDirectory(subPath);
                        __uploadDir(client, x.FullName, $"{remotePath}/{x.Name}");
                    } else {
                        using (Stream fileStream = new FileStream(x.FullName, FileMode.Open)) {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("\tUploading <file>: {0} ({1:N0} bytes)", x, ((FileInfo)x).Length);
                            client.UploadFile(fileStream, $"{remotePath}/{x.Name}");
                            __color(" -> Done\n", ConsoleColor.Green);
                        }
                    }
                });
            }
        }
        private static string Pop(this List<string> list) {
            var retVal = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return retVal;
        }
        private static string __version() {
            Console.Write("   ______       _______ __  __\n  / ___/ |     / / ___// / / /\n  \\__ \\| | /| / /\\__ \\/ /_/ / \n ___/ /| |/ |/ /___/ / __  / " +
                " \n/____/ |__/|__//____/_/ /_/   \n     Secure Windows Shell     \n");
            Console.Write($"\nRelease: {_codename} {_version}\n");
            return $"{_codename} {_version}";
        }
        private static void __color(string message, ConsoleColor cc) {
            Console.ForegroundColor = cc;
            Console.Write(message);
            Console.ResetColor();
        }
        private static bool __checkHash(bool ignore) {
            bool compareHash(string path, string hash) => !__computeHash(path).Equals(hash.Trim());
            string getHash(string uri) => new WebClient().DownloadString($"{uri}?" + new Random().Next());
            string
                error = "ERROR: Checksum Mismatch! This executable *may* be out of date or malicious!\n",
                checksumfile = Url.Checksum,
                swshlocation = Assembly.GetExecutingAssembly().Location,
                keygenlocation = "swsh-keygen.exe";
            try {
                if (compareHash(swshlocation, getHash(checksumfile).Split(' ')[0]) || compareHash(keygenlocation, getHash(checksumfile).Split(' ')[1]))
                    throw new Exception();
                return true;
            } catch (Exception) {
                if (!File.Exists(keygenlocation)) {
                    __color("WARNING: Could not find swsh-keygen.exe. SSH key generation will not be available.\n", ConsoleColor.Yellow);
                }
                if (!ignore) {
                    __color(error, ConsoleColor.Red);
                    Console.Read();
                    Environment.Exit(500);
                }
                return false;
            }
        }
        private static void __printHash() {
            string action = _command.Remove(0, 11).Trim(), file;
            if (action.StartsWith(">") && File.Exists("swsh-keygen.exe")) {
                __color($"Exporting... ", ConsoleColor.Yellow);
                if (action.StartsWith(">>")) {
                    file = action.Remove(0, 2).Trim();
                    File.AppendAllText(file, $"{__computeHash(Assembly.GetExecutingAssembly().Location)} {__computeHash("swsh-keygen.exe")}");
                    Console.WriteLine(new FileInfo(file).FullName);
                    return;
                }
                file = action.Remove(0, 1).Trim();
                File.WriteAllText(file, $"{__computeHash(Assembly.GetExecutingAssembly().Location)} {__computeHash("swsh-keygen.exe")}");
                Console.WriteLine(new FileInfo(file).FullName);
                return;
            }
            Console.WriteLine($"{__computeHash(Assembly.GetExecutingAssembly().Location)} -- SHA1 -- SWSH.exe");
            if (File.Exists("swsh-keygen.exe"))
                Console.WriteLine($"{__computeHash("swsh-keygen.exe")} -- SHA1 -- swsh-keygen.exe");
        }
        private static string __computeHash(string path) =>
            new List<byte>(new SHA1CryptoServiceProvider()
                .ComputeHash(File.ReadAllBytes(path)))
                .Select((x) => x.ToString("x2"))
                .Aggregate((x, y) => x + y);
        private static void __notice() => Console.Write("SWSH - Secure Windows Shell\nCopyright (C) 2017  Muhammad Muzzammil\nThis program comes with ABSOLU" +
            "TELY NO WARRANTY; for details type `license'.\nThis is free software, and you are welcome to redistribute it\nunder certain conditions; type `l" +
            "icense' for details.\n\n");
        private static string __getCommand() {
            var list = new List<string>();
            var commands = new string[] { "version", "connect", "keygen", "help", "clear", "exit", "upload", "pwd", "comput" +
                "ehash" };
            foreach (var i in commands) list.Add(i);
            foreach (var i in Directory.GetDirectories(_workingDirectory)) list.Add($"cd {new DirectoryInfo(i).Name.ToLower()}");
            try {
                ReadLine.AutoCompletionHandler = (data, length) => {
                    var tList = new List<string>();
                    if (data.StartsWith("cd ") && (data.Contains("/") || data.Contains("\\")))
                        Directory.GetDirectories($"{_workingDirectory}/{Path.GetDirectoryName(data.Remove(0, 3))}").ToList()
                        .Where(x => new DirectoryInfo(x)
                            .FullName.ToLower().Contains(data.ToLower().Split(' ')[1].Replace('/', '\\'))).ToList()
                        .ForEach(x => tList.Add(x.Remove(0, ($"{_workingDirectory}/{Path.GetDirectoryName(data.Remove(0, 3))}").Length + 1).ToLower()));
                    if (data.Trim() == "help")
                        commands.ToList().ForEach(x => tList.Add(x));
                    list.Where(x => x.Contains(data)).ToList().ForEach(y => tList.Add(y.Remove(0, length)));
                    return tList.ToArray();
                };
            } catch (IndexOutOfRangeException) { }
            var read = ReadLine.Read();
            File.AppendAllText(_swshHistory, $"[{DateTime.UtcNow} UTC]\t=>\t{read}\n");
            if (read.Contains("%appdata%")) read = read.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace('\\', '/'));
            return read.TrimEnd().TrimStart().Trim();
        }

        private static string __getPassword(string prompt) {
            ReadLine.PasswordMode = true;
            var password = ReadLine.Read(prompt);
            ReadLine.GetHistory().Pop();
            ReadLine.PasswordMode = false;
            return password;
        }
        private static bool __unstable() => _codename.StartsWith("unstable");
        private static void __error(string err) {
            __color("ERROR: ", ConsoleColor.Red);
            Console.Write(err);
        }

        private static string[] __readKeys() {
            var xml = new XmlDocument();
            xml.Load(_swshKeys);
            return new string[] {xml.GetElementsByTagName("private")[0].InnerText, xml.GetElementsByTagName("public")[0].InnerText };
        }
        private static void __writeKeys(string privateFile, string publicFile) {
            var publickey = publicFile == null ? "" : File.ReadAllText(new FileInfo(publicFile).FullName);
            File.WriteAllLines(_swshKeys, new string[] {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<data>",
                $"<private>{File.ReadAllText(new FileInfo(privateFile).FullName)}</private>",
                $"<public>{publickey}</public>",
                "</data>"
            });
        }
        private static ConnectionInfo __CreateConnection(string data) {
            try {
                if (data.EndsWith("-p")) {
                    data = data.Split(' ')[0];
                    return new ConnectionInfo(
                        data.Split('@')[1],
                        data.Split('@')[0],
                        new PasswordAuthenticationMethod(
                            data.Split('@')[0],
                            __getPassword($"Password for {data}: ")));
                } else return new ConnectionInfo(
                        data.Split('@')[1],
                        data.Split('@')[0],
                        new PrivateKeyAuthenticationMethod(
                            data.Split('@')[0],
                            new PrivateKeyFile(new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(__readKeys()[0]))).BaseStream)));
            } catch (Exception exp) { __error($"{exp.Message}\n"); }
            return null;
        }
    }
}
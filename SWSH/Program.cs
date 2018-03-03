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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Xml;
using Renci.SshNet;

namespace SWSH {
    public static class Program {
        public static bool KeygenIsAvailable { get; set; }
        public static bool Unstable => Codename.StartsWith("unstable");
        public static string Version => "";
        public static string Codename => "unstable-titan";
        public static string AppDataDirectory =>
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/SWSH";
        public static string History => $"{AppDataDirectory}/swsh_history";
        public static string Keys => $"{AppDataDirectory}/swsh_keys";
        public static string License => $"{AppDataDirectory}/LICENSE.txt";
        public static string Command { get; set; }
        public static string WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static void Main(string[] args) {
            if (!Directory.Exists(AppDataDirectory)) Directory.CreateDirectory(AppDataDirectory);
            Console.Title = "SWSH - Secure Windows Shell";
            Notice();
            Console.Write("\nType `license notice` to view this notice again.\n");
            for (int i = 0; i < 5; i++) {
                Console.Write($"\rStarting in {5 - (i + 1)}s");
                Thread.Sleep(1000);
            }
            Console.Clear();
            /* Downloading License; if does not exists. START */
            try {
                if (!File.Exists(License)) {
                    Console.WriteLine("License file not found, downloading...");
                    new WebClient().DownloadFile(new Uri(Url.License), License);
                    Console.Clear();
                }
            } catch (Exception exp) { Error($"Unable to download License, view online copy here: {Url.License}\nReason:{exp.Message}\n"); }
            /* Downloading License; if does not exists. END   */
            Console.Title = $"SWSH - {GetVersion()}";
            if (!Unstable) KeygenIsAvailable = CheckHash(args.Any(x => x == "--IgnoreChecksumMismatch"));
            Console.Write("Use `help` command for help.\n\n");
            try {
                var handle = ExternalFunctions.GetStdHandle(-11);
                ExternalFunctions.GetConsoleMode(handle, out var mode);
                ExternalFunctions.SetConsoleMode(handle, mode | 0x4);
                ExternalFunctions.GetConsoleMode(handle, out mode);
            } catch (Exception exp) { Error($"{exp.Message}\n"); }
            Start();
        }
        private static void Start() {
            while (Command != "exit") {
                try {
                    Color($"{WorkingDirectory.Replace('\\', '/').Remove(0, 2).ToLower()}:", ConsoleColor.DarkCyan);
                    Color("swsh> ", ConsoleColor.DarkGray);
                    Command = GetCommand();
                    if (Command.StartsWith("swsh")) {
                        Color("WARNING:\nThis type of commands is deprecated and will stop working in future.\nPlease take a look at our latest documentation or use `help` command.\n", ConsoleColor.Yellow);
                        if (Command.StartsWith("swsh --")) Command = Command.Remove(0, 7);
                    }
                    if (Command == "version") GetVersion();
                    else if (Command.StartsWith("help")) Help();
                    else if (Command.StartsWith("connect")) Connect();
                    else if (Command.StartsWith("keygen")) Keygen();
                    else if (Command == "ls") Ls();
                    else if (Command.StartsWith("cd")) Cd();
                    else if (Command.StartsWith("upload")) Upload();
                    else if (Command == "clear") Clear();
                    else if (Command == "license") File.ReadAllLines(License).ToList().ForEach(Console.WriteLine);
                    else if (Command == "license notice") Notice();
                    else if (Command == "pwd") Console.WriteLine(WorkingDirectory.ToLower());
                    else if (Command.StartsWith("computehash")) PrintHash();
                    else if (Command.Trim() != "") Error($"SWSH -> {Command} -> unknown command.\n");
                } catch (Exception exp) { Error($"{exp.Message}\n"); }
            }
        }
        private static void Help() {
            Command = Command.Remove(0, 4).Trim();
            if (Command.Length > 0) {
                var title = $"Help for {Command}";
                Console.WriteLine(title);
                for (int i = 0; i < title.Length; i++) Console.Write("=");
                Console.WriteLine();
                switch (Command) {
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
                            Console.WriteLine("Displays this help or command details.\nUsage: help pwd");
                            break;
                        }
                    default:
                        Error($"SWSH -> {Command} -> unknown command.\n");
                        break;
                }
            } else {
                Console.Write(
                      "Usage: <command> [arguments] (options)\n"
                    + "Available commands:\n"
                    + "  version                     -Check the version of swsh.\n"
                    + "  connect [user@host] (-p)    -Connects to Server over SSH.\n"
                    + "  keygen (options)            -Generates, imports or show SSH RSA key pair. `help keygen` for more.\n"
                    + "  help [command]              -Displays this help or command details.\n"
                    + "  clear                       -Clears the console.\n"
                    + "  pwd                         -Prints working directory.\n"
                    + "  computehash [(>/>>) path]   -Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen.\n"
                    + "  exit                        -Exits.\n"
                    + "  ls                          -Lists all files and directories in working directory.\n"
                    + "  cd [arg]                    -Changes directory to 'arg'. arg = directory name.\n"
                    + "  upload [arguments]          -Uploads files and directories. 'upload -h' for help.\n");
            }
        }
        private static void Connect() {
            if (!Command.EndsWith("-p")) { 
                if (!File.Exists(Keys)) {
                    Console.Write("SWSH private key file not found. (I)mport or (G)enerate?: ");
                    switch (Console.ReadKey().Key) {
                        case ConsoleKey.I:
                            ImportKey();
                            break;
                        case ConsoleKey.G:
                            Keygen();
                            break;
                        default:
                            Console.WriteLine(" <= Invalid option.");
                            return;
                    }
                    return;
                }
                    if (string.IsNullOrEmpty(ReadKeys()[1])) Color("WARNING: No public key detected.\n", ConsoleColor.Yellow);
            }

            var ccinfo = CreateConnection(Command.Remove(0, 8));
            if (ccinfo == null) return;
            Console.Write($"Waiting for response from {ccinfo.Username}@{ccinfo.Host}...\n");
            using (var ssh = new SshClient(ccinfo)) {
                ssh.Connect();
                Color($"Connected to {ccinfo.Username}@{ccinfo.Host}...\n", ConsoleColor.Green);
                var actual = ssh.CreateShellStream(
                    "xterm-256color",
                    (uint) Console.BufferWidth,
                    (uint) Console.BufferHeight,
                    (uint) Console.BufferWidth,
                    (uint) Console.BufferHeight,
                    Console.BufferHeight, null);
                //Read Thread
                var read = new Thread(() => {
                    if (!actual.CanRead) return;
                    while (true)
                        Console.WriteLine(actual.ReadLine());
                });
                //Write Thread
                new Thread(() => {
                    if (!actual.CanWrite) return;
                    while (true) {
                        try {
                            actual.WriteLine("");
                            var input = Console.ReadLine();
                            Console.Write("\b\r\b\r");
                            actual.WriteLine(input);
                            if (input != "exit") continue;
                            actual.Dispose();
                            read.Abort();
                            throw new Exception();
                        }
                        catch (Exception) {
                            Color($"Connection to {ccinfo.Username}@{ccinfo.Host}, closed.\n",
                                ConsoleColor.Yellow);
                            Color("(E)xit SWSH - Any other key to reload SWSH: ", ConsoleColor.Blue);
                            var key = Console.ReadKey();
                            if (key.Key != ConsoleKey.E)
                                Process.Start(Assembly.GetExecutingAssembly().Location);
                            ssh.Disconnect();
                            Environment.Exit(0);
                        }
                    }
                }).Start();
                read.Start();
                while (true) { }
            }
        }
        private static void ImportKey() {
            string[] data = new string[2];
            Console.WriteLine("\nImporting keys...");
            while (true) {
                Console.Write("Enter path to private key: ");
                data[0] = GetCommand();
                if (data[0].Trim() == String.Empty)
                    Error("SWSH -> key path should not be empty!\n");
                else {
                    if (File.Exists(data[0]) && File.Exists($"{WorkingDirectory}/{data[0]}"))
                        Error($"SWSH -> {data[0]} -> file path is ambiguous.\n");
                    else if (File.Exists($"{WorkingDirectory}/{data[0]}")) {
                        data[0] = $"{WorkingDirectory.Replace('\\', '/')}/{data[0]}";
                        break;
                    } else if (!File.Exists(data[0]))
                        Error($"SWSH -> {data[0]} -> file is non existent.\n");
                    else break;
                }
            }
            Console.Write("Import public key? (y/n): ");
            if (GetCommand().ToUpper() == "Y") {
                while (true) {
                    Console.Write("Enter path to public key: ");
                    data[1] = GetCommand();
                    if (data[1].Trim() == String.Empty)
                        Error("SWSH -> key path should not be empty!\n");
                    else {
                        if (File.Exists(data[1]) && File.Exists($"{WorkingDirectory}/{data[1]}"))
                            Error($"SWSH -> {data[1]} -> file path is ambiguous.\n");
                        else if (File.Exists($"{WorkingDirectory}/{data[0]}")) {
                            data[0] = $"{WorkingDirectory.Replace('\\', '/')}/{data[0]}";
                            break;
                        } else if (!File.Exists(data[1]))
                            Error($"SWSH -> {data[1]} -> file is non existent.\n");
                        else break;
                    }
                }
            } else Console.Write("\r\b\rImport public key? (y/n): ...skipped\n");
            WriteKeys(data[0], data[1] ?? "");
        }
        private static void Keygen() {
            Command = Command.Length > 7 ? Command.Remove(0, 7):null;
            if (Command != null && Command.StartsWith("show")) {
                if (Command.Remove(0, 4).Trim() == "private") {
                    Console.WriteLine(ReadKeys()[0]);
                } else Console.WriteLine(!string.IsNullOrEmpty(ReadKeys()[1]) ? ReadKeys()[1] : "No public key detected.");
                return;
            }
            if (File.Exists(Keys)) {
                Color("WARNING: This action will overwrite previously generated or imported keys in the data file but not the original keys. Continue? (y/n): ", ConsoleColor.Yellow);
                if (Console.ReadKey().Key != ConsoleKey.Y) {
                    Console.WriteLine();
                    return;
                }
            }
            if (Command == "import") {
                ImportKey();
                return;
            }
            if (!KeygenIsAvailable ^ Unstable) {
                Color("Key generation is unavailable.\n", ConsoleColor.DarkBlue);
                return;
            }
            if (File.Exists("swsh-keygen.exe")) {
                if (!CheckHash(true) ^ Unstable) return;
                Console.WriteLine("\nGenerating public/private rsa key pair.");
                string privateFile, publicFile;
                Color("exit", ConsoleColor.Red);
                Console.Write(" or ");
                Color("-e", ConsoleColor.Red);
                Console.Write(" to cancel.\n");
                do {
                    Color("Enter absolute path to save private key (%appdata%/SWSH/swsh.private):\t", ConsoleColor.Yellow);
                    privateFile = GetCommand();
                    if (privateFile == String.Empty) privateFile = AppDataDirectory + "/swsh.private";
                    else if (privateFile == "-e" || privateFile == "exit") return;
                } while (!IsWritable(privateFile));
                do {
                    Color("Enter absolute path to save public key (%appdata%/SWSH/swsh.public):\t", ConsoleColor.Yellow);
                    publicFile = GetCommand();
                    if (publicFile == String.Empty) publicFile = AppDataDirectory + "/swsh.public";
                    else if (publicFile == "-e" || privateFile == "exit") return;
                } while (!IsWritable(publicFile));
                bool IsWritable(string path) {
                    if (!File.Exists(path)) return true;
                    Color($"File exists: {new FileInfo(path).FullName}\n\n\nOverwrite? (y/n): ", ConsoleColor.Red);
                    return GetCommand().ToUpper() == "Y";
                }
                var keygenProcess = new Process {
                    StartInfo = new ProcessStartInfo {
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
                    Color($"WARNING: swsh-keygen exited with exit code {keygenProcess.ExitCode}.", ConsoleColor.Yellow);
                    return;
                }
                Color($"Your public key:\n\n{File.ReadAllLines(publicFile)[0]}\n", ConsoleColor.Green);
                WriteKeys(privateFile, publicFile);
            } else Error($"The binary 'swsh-keygen.exe' was not found. Are you sure it's installed?\nSee: {Url.Keygen}.\n");
        }
        private static void Clear() {
            Console.Clear();
            GetVersion();
            Console.Write("Use `help` command for help.\n\n");
        }
        private static void Ls() {
            if (Directory.GetDirectories(WorkingDirectory).Length > 0) {
                var data = new List<string>();
                Directory.GetDirectories(WorkingDirectory).ToList().ForEach(dir => data.Add(dir));
                Directory.GetFiles(WorkingDirectory).ToList().ForEach(file => data.Add(file));
                data.Sort();
                Console.WriteLine("Size\tUser        Date Modified   Name\n====\t====        =============   ====");
                data.ForEach(x => {
                    if (File.Exists(x)) {
                        var info = new FileInfo(x);
                        if (info.Attributes.ToString().Contains("Hidden")) return;
                        var owner = File.GetAccessControl(x).GetOwner(typeof(NTAccount)).ToString().Split('\\')[1];
                        var size = (info.Length > 1024 ? (info.Length / 1024 > 1024 ? info.Length / 1024 / 1024 : info.Length / 1024) :
                            info.Length).ToString();
                        var toApp = "";
                        owner = owner.Length >= 10 ? owner.Remove(5) + "..." + owner.Remove(0, owner.Length - 2) : owner;
                        if (owner.Length < 10) for (int i = 0; i < 10 - owner.Length; i++) toApp += " ";
                        owner += toApp;
                        if (size.Length < 4) for (int i = 0; i < 3 - size.Length; i++) toApp += " ";
                        size = toApp + size;
                        Color(size, ConsoleColor.Green);
                        Color(info.Length > 1024 ? (info.Length / 1024 > 1024 ? "MB" : "KB") : "B", ConsoleColor.DarkGreen);
                        Color($"\t{owner}  ", ConsoleColor.Yellow);
                        Color(
                            $"{($"{info.LastWriteTime.Date:d}".Split('/')[0].Length > 1 ? "" : " ")}" +
                            $"{$"{info.LastWriteTime.Date:d}".Split('/')[0]} " +
                            $"{$"{info.LastWriteTime.Date:m}".Remove(3)} " +
                            $"{info.LastWriteTime.ToLocalTime():HH:mm}    ",
                            ConsoleColor.Blue);
                        Color(info.Name, Path.GetFileNameWithoutExtension(x).Length > 0 ? ConsoleColor.Magenta : ConsoleColor.Cyan);
                        Console.WriteLine();
                    } else if (Directory.Exists(x)) {
                        var info = new DirectoryInfo(x);
                        if (info.Attributes.ToString().Contains("Hidden")) return;
                        var owner = File.GetAccessControl(x).GetOwner(typeof(NTAccount)).ToString().Split('\\')[1];
                        owner = owner.Length >= 10 ? owner.Remove(5) + "..." + owner.Remove(0, owner.Length - 2) : owner;
                        var toApp = "";
                        if (owner.Length < 10) for (int i = 0; i < 10 - owner.Length; i++) toApp += " ";
                        owner += toApp;
                        Color("   -", ConsoleColor.DarkGray);
                        Color($"\t{owner}  ", ConsoleColor.Yellow);
                        Color(
                            $"{($"{info.LastWriteTime.Date:d}".Split('/')[0].Length > 1 ? "" : " ")}" +
                            $"{$"{info.LastWriteTime.Date:d}".Split('/')[0]} " +
                            $"{$"{info.LastWriteTime.Date:m}".Remove(3)} " +
                            $"{info.LastWriteTime.ToLocalTime():HH:mm}    ",
                            ConsoleColor.Blue);
                        Color(info.Name,
                            info.Name.StartsWith(".") ? ConsoleColor.DarkCyan : info.GetFiles().Length > 0 || info.GetDirectories().Length > 0 ?
                                ConsoleColor.White : ConsoleColor.DarkGray);
                        Color(info.GetFiles().Length == 0 && info.GetDirectories().Length == 0 ? "  <empty>" : "", ConsoleColor.DarkRed);
                        Console.WriteLine();
                    }
                });
            }
            if (Directory.GetDirectories(WorkingDirectory).Length == 0 && Directory.GetFiles(WorkingDirectory).Length == 0)
                Color("No files or directories here.\n", ConsoleColor.Yellow);
        }
        private static void Cd() {
            if ((Command = Command.Remove(0, 3)) == "..") ChangeWorkingDir(Path.GetDirectoryName(WorkingDirectory));
            else if (Command.StartsWith("./")) ChangeWorkingDir($"{WorkingDirectory}/{Command.Remove(0, 2)}");
            else if (Command.StartsWith("/")) ChangeWorkingDir(Path.GetPathRoot(WorkingDirectory) + Command.Remove(0, 1));
            else ChangeWorkingDir($"{WorkingDirectory}/{Command}");
            void ChangeWorkingDir(string path) {
                path = path.Replace('\\', '/');
                if (Directory.Exists(Path.GetFullPath(path))) WorkingDirectory = Path.GetFullPath(path);
                else Error($"SWSH -> {path} -> path does not exists.\n");
            }
        }
        private static void Upload() {
            if ((Command = Command.Remove(0, 7)) == "-h") {
                Console.WriteLine("upload [--dir]* [args] [user@host]:[location]\n\n'args' are seperated using spaces ( ) and last 'arg' will be treated as server data which includes username and host location as well as the location of data to upload, part after the colon (:), where the data is to be uploaded. Use flag '--dir' to upload directiories. Do not use absolute paths for local path, change working directory to navigate.");
            } else {
                var toupload = Command.StartsWith("--dir") ? Command.Replace("--dir", "").Trim().Split(' ').ToList() : Command.Trim().Split(' ').ToList();
                try {
                    var serverData = toupload.Pop().Split(':');
                    var data = serverData[0];
                    var location = serverData[1];
                    try {
                        var ccinfo = CreateConnection(data);
                        if (ccinfo != null) {
                            if (Command.StartsWith("--dir"))
                                using (var sftp = new SftpClient(ccinfo)) {
                                    Command = Command.Replace("--dir", "");
                                    sftp.Connect();
                                    toupload.ForEach(x => {
                                        var path = $"{WorkingDirectory}/{x.Trim()}";
                                        location = serverData[1] + (serverData[1].EndsWith("/") ? "" : "/") +
                                                   x.Trim();
                                        if (!sftp.Exists(location)) sftp.CreateDirectory(location);
                                        Color($"Uploading <directory>: {x.Trim()}\n", ConsoleColor.Yellow);
                                        UploadDir(sftp, path, location);
                                        Color("Done.\n", ConsoleColor.Green);
                                    });
                                }
                            else
                                using (var scp = new ScpClient(ccinfo)) {
                                    scp.Connect();
                                    toupload.ForEach(x => {
                                        var path = $"{WorkingDirectory}/{x.Trim()}";
                                        if (File.Exists(path)) {
                                            Color($"Uploading <file>: {x.Trim()}", ConsoleColor.Yellow);
                                            scp.Upload(new FileInfo(path), location);
                                            Color(" -> Done\n", ConsoleColor.Green);
                                        }
                                        else Error($"SWSH -> {path.Replace('/', '\\')} -> file does not exists.\n");
                                    });
                                }
                        }
                    } catch (Exception exp) { Error($"{exp.Message}\n"); }
                } catch { Error($"SWSH -> upload {Command} -> is not the correct syntax for this command.\n"); }
            }
            void UploadDir(SftpClient client, string localPath, string remotePath) {
                new DirectoryInfo(localPath).EnumerateFileSystemInfos().ToList().ForEach(x => {
                    if (x.Attributes.HasFlag(FileAttributes.Directory)) {
                        var subPath = $"{remotePath}/{x.Name}";
                        if (!client.Exists(subPath)) client.CreateDirectory(subPath);
                        UploadDir(client, x.FullName, $"{remotePath}/{x.Name}");
                    } else {
                        using (Stream fileStream = new FileStream(x.FullName, FileMode.Open)) {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("\tUploading <file>: {0} ({1:N0} bytes)", x, ((FileInfo)x).Length);
                            client.UploadFile(fileStream, $"{remotePath}/{x.Name}");
                            Color(" -> Done\n", ConsoleColor.Green);
                        }
                    }
                });
            }
        }
        private static string Pop(this IList<string> list) {
            var retVal = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return retVal;
        }
        private static string GetVersion() {
            Console.Write("   ______       _______ __  __\n  / ___/ |     / / ___// / / /\n  \\__ \\| | /| / /\\__ \\/ /_/ / \n ___/ /| |/ |/ /___/ / __  /  \n/____/ |__/|__//____/_/ /_/   \n     Secure Windows Shell     \n");
            Console.Write($"\nRelease: {Codename} {Version}\n");
            return $"{Codename} {Version}";
        }
        private static void Color(string message, ConsoleColor cc) {
            Console.ForegroundColor = cc;
            Console.Write(message);
            Console.ResetColor();
        }
        private static bool CheckHash(bool ignore) {
            bool CompareHash(string path, string hash) => !ComputeHash(path).Equals(hash.Trim());
            string GetHash(string uri) => new WebClient().DownloadString($"{uri}?" + new Random().Next());
            string
                error = "ERROR: Checksum Mismatch! This executable *may* be out of date or malicious!\n",
                checksumfile = Url.Checksum,
                swshlocation = Assembly.GetExecutingAssembly().Location,
                keygenlocation = "swsh-keygen.exe";
            try {
                if (CompareHash(swshlocation, GetHash(checksumfile).Split(' ')[0]) || CompareHash(keygenlocation, GetHash(checksumfile).Split(' ')[1]))
                    throw new Exception();
                return true;
            } catch (Exception) {
                if (!File.Exists(keygenlocation)) {
                    Color("WARNING: Could not find swsh-keygen.exe. SSH key generation will not be available.\n", ConsoleColor.Yellow);
                }
                if (!ignore) {
                    Color(error, ConsoleColor.Red);
                    Console.Read();
                    Environment.Exit(500);
                }
                return false;
            }
        }
        private static void PrintHash() {
            string action = Command.Remove(0, 11).Trim();
            if (action.StartsWith(">") && File.Exists("swsh-keygen.exe")) {
                Color("Exporting... ", ConsoleColor.Yellow);
                string file;
                if (action.StartsWith(">>")) {
                    file = action.Remove(0, 2).Trim();
                    File.AppendAllText(file, $"{ComputeHash(Assembly.GetExecutingAssembly().Location)} {ComputeHash("swsh-keygen.exe")}");
                    Console.WriteLine(new FileInfo(file).FullName);
                    return;
                }
                file = action.Remove(0, 1).Trim();
                File.WriteAllText(file, $"{ComputeHash(Assembly.GetExecutingAssembly().Location)} {ComputeHash("swsh-keygen.exe")}");
                Console.WriteLine(new FileInfo(file).FullName);
                return;
            }
            Console.WriteLine($"{ComputeHash(Assembly.GetExecutingAssembly().Location)} -- SHA1 -- SWSH.exe");
            if (File.Exists("swsh-keygen.exe")) Console.WriteLine($"{ComputeHash("swsh-keygen.exe")} -- SHA1 -- swsh-keygen.exe");
        }
        private static string ComputeHash(string path) =>
            new List<byte>(new SHA1CryptoServiceProvider()
                .ComputeHash(File.ReadAllBytes(path)))
                .Select(x => x.ToString("x2"))
                .Aggregate((x, y) => x + y);
        private static void Notice() => Console.Write("SWSH - Secure Windows Shell\nCopyright (C) 2017  Muhammad Muzzammil\nThis program comes with ABSOLUTELY NO WARRANTY; for details type `license'.\nThis is free software, and you are welcome to redistribute it\nunder certain conditions; type `license' for details.\n\n");
        private static string GetCommand() {
            var list = new List<string>();
            var commands = new[] { "version", "connect", "keygen", "help", "clear", "exit", "upload", "pwd", "computehash" };
            foreach (var i in commands) list.Add(i);
            foreach (var i in Directory.GetDirectories(WorkingDirectory)) list.Add($"cd {new DirectoryInfo(i).Name.ToLower()}");
            try {
                ReadLine.AutoCompletionHandler = (data, length) => {
                    var tList = new List<string>();
                    if (data.StartsWith("cd ") && (data.Contains("/") || data.Contains("\\")))
                        Directory.GetDirectories($"{WorkingDirectory}/{Path.GetDirectoryName(data.Remove(0, 3))}").ToList()
                        .Where(x => new DirectoryInfo(x)
                            .FullName.ToLower().Contains(data.ToLower().Split(' ')[1].Replace('/', '\\'))).ToList()
                        .ForEach(x => tList.Add(x.Remove(0, $"{WorkingDirectory}/{Path.GetDirectoryName(data.Remove(0, 3))}".Length + 1).ToLower()));
                    if (data.Trim() == "help")
                        commands.ToList().ForEach(x => tList.Add(x));
                    list.Where(x => x.Contains(data)).ToList().ForEach(y => tList.Add(y.Remove(0, length)));
                    return tList.ToArray();
                };
            } catch (IndexOutOfRangeException) { }
            var read = ReadLine.Read();
            File.AppendAllText(History, $"[{DateTime.UtcNow} UTC]\t=>\t{read}\n");
            if (read.Contains("%appdata%")) read = read.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace('\\', '/'));
            return read.TrimEnd().TrimStart().Trim();
        }
        private static string GetPassword(string prompt) {
            ReadLine.PasswordMode = true;
            var password = ReadLine.Read(prompt);
            ReadLine.GetHistory().Pop();
            ReadLine.PasswordMode = false;
            return password;
        }
        private static void Error(string err) {
            Color("ERROR: ", ConsoleColor.Red);
            Console.Write(err);
        }
        private static string[] ReadKeys() {
            var xml = new XmlDocument();
            xml.Load(Keys);
            return new[] {xml.GetElementsByTagName("private")[0].InnerText, xml.GetElementsByTagName("public")[0].InnerText };
        }
        private static void WriteKeys(string privateFile, string publicFile) {
            var publickey = publicFile == null ? "" : File.ReadAllText(new FileInfo(publicFile).FullName);
            File.WriteAllLines(Keys, new[] {
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>",
                "<data>",
                $"<private>{File.ReadAllText(new FileInfo(privateFile).FullName)}</private>",
                $"<public>{publickey}</public>",
                "</data>"
            });
        }
        private static ConnectionInfo CreateConnection(string data) {
            try {
                if (data.EndsWith("-p")) {
                    data = data.Split(' ')[0];
                    return new ConnectionInfo(
                        data.Split('@')[1],
                        data.Split('@')[0],
                        new PasswordAuthenticationMethod(
                            data.Split('@')[0],
                            GetPassword($"Password for {data}: ")));
                }
                return new ConnectionInfo(
                    data.Split('@')[1],
                    data.Split('@')[0],
                    new PrivateKeyAuthenticationMethod(
                        data.Split('@')[0],
                        new PrivateKeyFile(new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(ReadKeys()[0]))).BaseStream)));
            } catch (Exception exp) { Error($"{exp.Message}\n"); }
            return null;
        }
    }
}
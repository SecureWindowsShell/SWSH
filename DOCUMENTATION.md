# Documentation for SWSH
###### For Version beta-1.4

# Index
   * [List of Commands](#commands)
	   * [--version](#--version) 
	   * [--add](#--add) 
	   * [--show](#--show) 
	   * [--connect](#--connect) 
	   * [--delete](#--delete) 
	   * [--edit](#--edit) 
	   * [--keygen](#--keygen) 
	   * [--help](#--help) 
	   * [clear](#clear) 
	   * [exit](#exit)
	   * [ls](#ls)
	   * [cd](#cd)
	   * [upload](#upload) 

## Commands
```
swsh [command] [arg]
        -v --version:                  -Check the version of swsh.
        -a --add     [-password]*      -Add a new connection either using private key or password (-password).
           --show    [nickname]*       -Show nicknames/Details of a nickname.
        -c --connect [nickname]        -Connects to Server over SSH.
           --delete  [nickname]        -Deletes connection's nickname.
           --edit    [nickname] [arg]  -Edits nickname, use one argument at a time.
           --keygen                    -Generates SSH RSA key pair.
        -h --help    [command]*        -Displays this help or command details.
        clear                          -Clears the console.
        exit                           -Exits.
ls                                     -Lists all files and directories in working directory.
cd [arg]                               -Changes directory to 'arg'. arg = directory name.
upload [args] [nickname]:[location]    -Uploads files and directories. 'upload -h' for help.
```
##### * = Optional

### --version
```
Syntax: swsh --version
Checks the version of swsh.

Usage: swsh --version
```

### --add
```
Syntax: swsh --add [-password]
Add a new connection either using private key or password.

Usage to add using private key: swsh --add
Usage to add using a password: swsh --add -password

You'll be asked for password each time as SWSH doesn't store them.
```

### --show
```
Syntax: swsh --show [nickname]
Show nicknames if no arguments have passed. If nickname is provided, shows details of a nickname.

Usage to check all nicknames: swsh --show
Usage to check a nickname: swsh --show myserver
```

### --connect
```
Syntax: swsh --connect [nickname]
Connects to Server over SSH.

Usage: swsh --connect myserver
```

### --delete
```
Syntax: swsh --delete [nickname]
Deletes connection's nickname.

Usage: swsh --delete myserver
```

### --edit
```
Syntax: swsh --edit [nickname] [arg]
arg:
        -user [newUserName]
        -key [newKey]
        -server [newServer]
Edits nickname, use one argument at a time.

Usage: swsh --edit myserver -user newUSER
```

### --keygen
```
Syntax: swsh --keygen
Generates SSH RSA key pair. Requires swsh-keygen.exe.

Default values are provided in parentheses.
Usage: swsh --keygen
```

### --help
```
Syntax: swsh --help [command]
Displays help or command details.

Usage: swsh --help -h
```

### clear
```
Syntax: swsh clear
Clears the console.

Usage: swsh clear
```

### exit
```
Syntax: swsh exit
Exits.

Usage: swsh exit
```

### ls
```
Syntax: ls
Lists all files and directories in working directory.

Usage: ls
```

### cd
```
Syntax: cd
Changes directory to 'arg'. arg = directory name.

Usage: cd
```

### upload
```
Syntax: upload [--dir]* [args] [nickname]:[location]
Uploads files and directories. 'upload -h' for help.

'args' are seperated using spaces ( ) and last 'arg' will be treated as server data which includes nickname as well as the location, part after the colon (:), where the data is to be uploaded. Use flag '--dir' to upload directiories. Do not use absolute paths for local path, change working directory to navigate.

Usage: upload --dir files muzzammil:/var/files
```

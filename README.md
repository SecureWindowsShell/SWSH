<p align="center">
  <img src=".images/github banner.png"/>
  <br>
  <a href="https://ci.appveyor.com/project/muhammadmuzzammil1998/swsh/branch/master" target="_blank">
    <img src="https://ci.appveyor.com/api/projects/status/1f2uc16tue6h0r0l/branch/master?retina=true" alt="Build status">
  </a>
  <br>
  <a href="https://github.com/SecureWindowsShell/SWSH/issues" target="_blank">
    <img src="https://img.shields.io/github/issues/SecureWindowsShell/SWSH.svg?style=for-the-badge" alt="GitHub issues">
  </a>
  <a href="https://github.com/SecureWindowsShell/SWSH/network" target="_blank">
    <img src="https://img.shields.io/github/forks/SecureWindowsShell/SWSH.svg?style=for-the-badge" alt="GitHub forks">
  </a>
  <a href="https://github.com/SecureWindowsShell/SWSH/stargazers" target="_blank">
    <img src="https://img.shields.io/github/stars/SecureWindowsShell/SWSH.svg?style=for-the-badge" alt="GitHub stars">
  </a>
  <a href="https://github.com/SecureWindowsShell/SWSH/blob/master/LICENSE" target="_blank">
    <img src="https://img.shields.io/github/license/SecureWindowsShell/SWSH.svg?style=for-the-badge" alt="GitHub license">
  </a>
  <a href="https://github.com/SecureWindowsShell/SWSH/releases" target="_blank">
    <img src="https://img.shields.io/github/release/SecureWindowsShell/SWSH/all.svg?style=for-the-badge" alt="Latest release">
  </a>
  <img src="https://img.shields.io/github/languages/top/SecureWindowsShell/SWSH.svg?style=for-the-badge" alt="Top language">
  <a href="https://swsh.muzzammil.xyz/" target="_blank">
    <img src="https://img.shields.io/website-up-down-green-red/http/shields.io.svg?label=website&style=for-the-badge" alt="Website">
  </a>
</p>

SWSH is a console application that offers SSH-like connectivity with ease to users which grants them the ability to operate remotely on SSH protocol. It is also Open source, so feel free to contribute.

**If you are not using a [prebuilt SWSH binary](https://github.com/SecureWindowsShell/SWSH/releases), you will see SWSH complain about a checksum mismatch and exit, use `--IgnoreChecksumMismatch` to stop it from exiting.**

![SWSH just doing its thing](https://user-images.githubusercontent.com/12321712/36257898-bfaba952-127e-11e8-9bd9-b63d4885f649.png)
*SWSH (beta) just doing its thing*

## Getting Started

### Generating SSH keys

SSH keys serve as a means of identifying yourself to an SSH server. To Generate your private and public key, SWSH uses an add-on, swsh-keygen. You can [build swsh-keygen](https://github.com/SecureWindowsShell/swsh-keygen) yourself if you want and place the executable (.exe) in SWSH's root (installation) directory.

Use command ```keygen``` to tell SWSH that you want to generate a new RSA key pair for SSH connection after that just follow the prompts.
You'll be asked for locations to store your keys, leave it blank if you want it to be default.
Output will be similar to this:

```swsh
/users/muzzammil:swsh> keygen

Generating public/private rsa key pair.
exit or -e to cancel.
Enter absolute path to save private key (%appdata%/SWSH/swsh.private):
Enter absolute path to save public key (%appdata%/SWSH/swsh.public):
Your public key:

ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAAAgQCt2MxdswuuUvmaY4JK6kP4lYIqGy0KeHCqcx1NEjB4EcqH7+MIeXGbdikACvP3wlOAEAt+7PMEhBHf7nL2S2SsOybpegJw0piiMeOIPJwQxIQFaRWyz3xn0ESItzBizsQ4yxfQiG37sFkMeQVnP5fHuc2+Z4JZ5SD56Dh1xxgnEw==
```

### Importing SSH keys

If you already have SSH keys and want to use them instead of creating a new pair, you can! Use ```keygen import``` command to do so and just follow the prompts.

NOTE: DO **NOT** SHARE YOUR PRIVATE KEY!

### Connecting to a host

To connect run ```connect username@host```.

To use a password connection, use tag `-p` like this: ```connect username@host -p```.

If done properly, output would be similar to the following:

```swsh
Waiting for response from username@host...
Connected to username@host...
~:/ $
```

## Commands

| Command                                   | Description                                                           |
|:------------------------------------------|:----------------------------------------------------------------------|
| [version](#version)                       | Check the version of swsh.                                            |
| [connect [user@host] (-p)](#connect)      | Connects to Server over SSH.                                          |
| [keygen (options)](#keygen)               | Generates SSH RSA key pair.                                           |
| [help    [command]](#help)                | Displays this help or command details.                                |
| [clear](#clear)                           | Clears the console.                                                   |
| [pwd](#pwd)                               | Prints working directory.                                             |
| [computehash [(>/>>) path]](#computehash) | Uses SHA-1 hash function to generate hashes for SWSH and swsh-keygen. |
| [exit](#exit)                             | Exits.                                                                |
| [ls](#ls)                                 | Lists all files and directories in working directory.                 |
| [cd [arg]](#cd)                           | Changes directory to 'arg'. arg = directory name.                     |
| [upload [arguments]](#upload)             | Uploads files and directories. 'upload -h' for help.                  |

For more, see our [documentation](DOCUMENTATION.md).

# License

GPL v3

Copyright (C) 2017  Muhammad Muzzammil

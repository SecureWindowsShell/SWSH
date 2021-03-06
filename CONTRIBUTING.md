# Contributing to SWSH
:tada: First off, thanks for taking the time to contribute! :tada:

## Index
* [Code Of Conduct](#code-of-conduct)
* [Bug Reporting](#report-a-bug)
* [Feature Request](#add-new-feature)
* [Pull Request](#pull-request)
* [Contact](#contact)

## Code Of Conduct
We need you to strictly follow our code of conduct. Help us make a better community.
### What is Code of conduct?
A code of conduct defines standards for how to engage in a community. It signals an inclusive environment that respects all contributions. It also outlines procedures for addressing problems between members of your project's community. 
### Read our Code of conduct
You can read our code of conduct [here](./CODE_OF_CONDUCT.md)

## Report a bug
Before reporting, please ensure that:
- [ ] Bug is not fixed or mentioned in [unstable](https://github.com/muhammadmuzzammil1998/SWSH/tree/unstable) branch of SWSH.
- [ ] Bug is not on your computer only, test it on at least 3 computers.
- [ ] You are using latest version of SWSH.
- [ ] You check if someone already filed a bug report of same issue. (if someone has filed one, comment on it)
- [ ] You do your research to help us identify the problem.
- [ ] You are running the latest version of Windows, SWSH, and .NET framework.

### Format of Report
A report should contain the following:

* A suitable title,
* Your SWSH, Windows, and .NET Framework version,
* Description of what happened and what was supposed to happen,
* Exact steps,
* And, your thought on how to fix this. :)

#### Example
**Title**: `connect` command is not working.

**Description**: 

* SWSH Release: Titan
* Windows 10 1709 
* .NET framework 4.7

`connect` command is not working if there is a space before it.

Steps: just run ` connect` with space.

Thoughts: Trim input taken from user.

## Add new feature
> “The best way to predict your future is to create it.” ~*Abraham Lincoln*

Just [email us](mailto:swsh@muzzammil.xyz) the details and we will get right on it.

Details should include:

* What you want,
* How do you want it,
* Any research you have done for it,
* Your contact information (name, email and website.)
* Anything you want, as long as it follows our [code of conduct](#code-of-conduct).

## Pull Request
You can make a pull request, but it should follow guidelines described here and in our [code of conduct](#code-of-conduct).

### Somethings to remember when writing code:
* If a function is only required for one function, it should be a local function to the latter function.
Example:
```cs
// Instead of this:
public static string Name() {
	...
	OtherFunction(str);
	...
}
public static string OtherFunction(string s) {
	...
	// your code here
	...
}



// Do this:
public static string Name() {
	...
	OtherFunction(str);
	...
	string OtherFunction(string s) {
		...
		// your code here
		...
	}
}
```
* Try to use lambda.
* Use programmer-friendly variable names.
* Don't do unnecessary things.

## Contact
SWSH's email: [swsh@muzzammil.xyz](mailto:swsh@muzzammil.xyz)

My email: [email@muzzammil.xyz](mailto:email@muzzammil.xyz)

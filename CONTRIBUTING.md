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
**Title**: Bug in swsh --show, characters not displaying correctly.
**Description**: 

* SWSH Version: beta 3.0
* Windows 10 1709 
* .NET framework 4.7

When I use `swsh --show` it is not displaying output correctly, there are ?'s in between lines. 

Steps: just run `swsh --show`

Thoughts: You should change encoding of output or something.

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

* Global variable names starts with an underscore (_), except for local variables.

Example: `var _name = "Hi, I am a variable, I vary.";`

* Global functions/methods starts with two (2) underscores (__), except for local functions.

Example: 
```cs
public static string __name() {
	...
	// your code here
	...
}
```
* If a function is only required for one function, it should be a local function to the latter function.
Example:
```cs
// Instead of this:
public static string __name() {
	...
	__otherFunction(str);
	...
}
public static string __otherFunction(string s) {
	...
	// your code here
	...
}



// Do this:
public static string __name() {
	...
	otherFunction(str);
	...
	string otherFunction(string s) {
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

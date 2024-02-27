### NetLimiter Crack Tool (C#): Automated Patching
I coded a c# program that automatically patches NetLimiter for you.

#### Features:
- Automatically patches NetLimiter for free Premium.

#### How it works:
1. Access the `NetLimiter.dll` file within your NetLimiter installation folder.
2. Navigate to `NetLimiter (version) -> NetLimiter.dll -> NetLimiter.Service -> NLLicense -> NLLicense()`.
3. Right-click on `this.IsRegistered = false;`, edit with C#, and change it to `true;`.
4. Proceed to `NetLimiter.dll -> NetLimiter.Service -> NLServiceTemp -> InitLicense()`.
5. Right-click on `DateTime expiration = installTime.AddDays(28.0);`, select "edit IL instruction", and change `ldc.r8 28` to `ldc.r8 99999`.
6. Save all changes.
7. NetLimiter is now patched and fully activated.

[![image](https://gist.github.com/assets/45830921/14b124b7-3026-4b9e-bfe4-816b60d4e666)](https://gist.github.com/assets/45830921/28feb274-c0b9-4f2d-ac1b-3e9d492c2cb9)

![image](https://github.com/Baseult/NetLimiterCrack/assets/45830921/df62b060-dde4-41cd-8851-b74b7b7afa1e)

I coded a c# program that automatically patches NetLimiter for you.

[![image](https://gist.github.com/assets/45830921/14b124b7-3026-4b9e-bfe4-816b60d4e666)](https://gist.github.com/assets/45830921/28feb274-c0b9-4f2d-ac1b-3e9d492c2cb9)

![image](https://github.com/Baseult/NetLimiterCrack/assets/45830921/df62b060-dde4-41cd-8851-b74b7b7afa1e)

If you want to crack it yourself, open the NetLimiter.dll file inside your NetLimiter folder with DnSpy.

Navigate to NetLimiter (version) -> NetLimiter.dll -> NetLimiter.Service -> NLLicense -> NLLicense().
Right click the this.IsRegistered = false; and edit with C# and change it from false; to true;
After that go to NetLimiter.dll -> NetLimiter.Service -> NLServiceTemp -> InitLicense().
Right click at the DateTime expiration = installTime.AddDays(28.0); and "edit IL instruction".
Change the 'ldc.r8 28' to 'ldc.r8 99999' and press OK.
Go to File -> Save all.
That's it!

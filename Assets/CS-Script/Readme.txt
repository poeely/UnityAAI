NOTE: 
You should no longer use this, but use the modified mono mcs.dll by Jakub Rak instead which is located here ->
https://github.com/aeroson/mcs-ICodeCompiler .
It is a lot easier to set up and use.

I will keep this package available for educational reasons though.

Basic example on how to use CS-Script ( http://www.csscript.net/ ) inside Unity.

With CS-Script you are able to compile and run C# scripts at runtime. This way you can ship new C# code after release without building a new version.
This is also a great start for adding mod support to your game.
See comments inside samplescripts for more info on how it works. 

Note: 
Compiling C# at runtime will NOT work on iOS as it is permitted by apple (AOT <-> JIT). The same is true for consoles. If you need a runtime script solution which will work on all platforms, you should check out a Unity compatible LUA interpreter like http://www.moonsharp.org instead.

If you are thinking about using runtime C# compilation for modding purposes, you can still do that on iOS/consoles. You just have to give modders a tool to compile their mod (You can either use CS-Script or mcs.dll to do exactly that. Visual Studio/Mono will also work, of course). Then simply load the compiled .dll at runtime. All you loose is the ability to directly use the code without recompiling everytime. But you can still use uncompiled scripts on Desktop/Android and then let modders compile their scripts if they want to ship their mods to iOS/consoles.

This example ships with v3.11.0.0 of CS-Script but it is advised to always use the newest stable release from http://www.csscript.net/ .
Until Unity updates its version of Mono you have to use the NET 1.1 version of CS-Script. Also, don't forget to copy Mono.CSharp.dll from the CS-Script lib folder when updating.

by http://www.dotmos.org


{% include_relative _header.md %}

{% include_relative contents.md %}


fabulous-cli
-----
##### (topic last updated: pending)
<br /> 

With the `fabulous-cli` you can run LiveUpdate. 

Install `fabulous-cli`: 

        // install latest
        dotnet tool install -g fabulous-cli 
        // install explicit version
        dotnet tool install -g fabulous-cli --version {versionnumber}
		
Update `fabulous-cli`:

        dotnet tool update -g fabulous-cli
		
Uninstall `fabulous-cli`:

        dotnet tool uninstall -g fabulous-cli


Live Update
------

There is an experimental LiveUpdate mechanism available.  The aim of this is primarily to enable modifying the `view` function in order
to see the effect of adjusting of visual options.

At the time of writing this has been trialled with
* Visual Studio + Android (USB Device or Emulator)
* Visual Studio for Mac + Android (USB Device or Emulator)
* Visual Studio for Mac + iOS (USB Device or Emulator)
* Visual Studio + WPF

Some manual set-up is required.

1. Install or update `fabulous-cli` as a global tool

        dotnet tool install -g fabulous-cli
        dotnet tool update -g fabulous-cli

2. Install or update the NuGet package `Fabulous.XamarinForms.LiveUpdate` for all projects in your app.  
This is the default for apps created with templates 0.13.10 and higher. Do a clean build.

3. Add the following reference to enable live update:
       
       open Fabulous.XamarinForms.LiveUpdate

4. Uncomment or add the code in the `#if` section below:

       type App () =
           inherit Application()
           ....
       #if DEBUG
           do runner.EnableLiveUpdate ()
       #endif

5. If running on Android, forward requests from localhost to the Android Debug Bridge:

       USB:
           adb -d forward  tcp:9867 tcp:9867
       EMULATOR:
           adb -e forward  tcp:9867 tcp:9867

6. Launch your app in Debug mode (note: you can use Release mode but must set Linking options to `None` rather than `SDK Assemblies`)

7. Run the following from your core project directory (e.g. `SqueakyApp\SqueakyApp`)

        cd SqueakyApp\SqueakyApp
        fabulous --watch --send 
        
8. It may be necessary to launch Visual Studio with elevated permissions ("Run as administrator"); otherwise, `runner.EnableLiveUpdate()` may fail to start the HttpListener, which will cause LiveUpdate to fail.

Now, whenever you save a file in your core project directory, the `fabulous` watcher will attempt to recompile your changed file and
send a representation of its contents to your app via a PUT request to the given webhook.  The app then deserializes this representation and
adds the declarations to an F# interpreter. This interpreter will make some reflective calls into the existing libraries on device.

**To take effect as app changes, your code must have a single declaration in some module called `programLiveUpdate` or `program` taking no arguments.**  For example:

```fsharp
module App =
    ...
    let init() = ...

    let update model msg = ...

    let view model = ...

    let program = Program.statefulApplication init update view
```

If a declaration like this is found the `program` object replaces the currently running Elmish program and the view is updated.
The model state of the app is re-initialized.

### Known limitations

1. The F# interpreter used on-device has incompletenesses and behavioural differences:

   1. Object expressions may not be interpreted
   2. Implementations of ToString() and other overrides will be ignored
   3. Some other F# constructs are not supported (e.g. address-of operations, new delegate)
   4. Some overloading of methods by type is not supported (overloading by argument count is ok)

   You can move generally move problematic constructs to a utility library, which will then be executed as compiled code.

2. Changes to the resources in a project (e.g. images) require a rebuild

3. Changes to Android and iOS projects require a rebuild

4. You can't debug interpreted code from the IDE using breakpoints, stack inspection etc.  Restart for that.

5. You may need to mock any platform-specific helpers you pass through, e.g.

       module App =
           ...
           let init() = ...

           let update (helper1, helper2) model msg = ...

           let view model = ...

       #if DEBUG
       // The fake program, used when LiveUpdate is activated and a program change has been made
       module AppLiveUpdate =
           open App

           let mockHelper1 () = ...

           let mockHelper2 () = ...

           let programLiveUpdate = Program.mkProgram init (update (mockHelper1, mockHelper2)) view
       #endif

       type App (helper1, helper2) = 
           inherit Application()
           ....

           // The real program, used when LiveUpdate is not activated or a program change has not been made
           let program = Program.mkProgram App.init (App.update (helper1, helper2)) App.view

6. There may be issues running on networks with network policy restrictions

7. In Visual Studio 2019, by default you cannot edit files whilst debugging. To enable file edits, turn **OFF** Edit and Continue by going to *Tools->Options*, selecting *Debug->General* and **unchecking** *Enable Edit and Continue*.

### Troubleshooting

The LiveUpdate mechanism is very experimental.
- Debug output is printed to console by `fabulous`
- Debug output is printed to app-output by the on-device web server

**ERROR SENDING TO WEBHOOK: "System.Net.WebException: No connection could be made because the target machine actively refused it."**

If the LiveUpdate console displays this error there are multiple possilbe causes:
- Visual Studio needs elevated permissions to execute "EnableLiveUpdate()".
    - **Solution**: launch Visual Studio using the "Run as Administrator" option
- Connection to wrong webhook ip.
    - **Solution**: check the output window in Visual Studio and explicitly specify the webhook url: ![output-window](images/live-update/output-window-webhooks.png)
- Local firewall on Mac blocks connection.
    - **Solution**: add a firewall exception for tcp port 9867 
- Firewall blocks traffic (if Windows PC and Mac/iPhone are on different networks).
    - **Solution**: add a firewall exception for tcp port 9867

**Quacked: "couldn't quack! the evaluation of the declarations in the code package failed: System.IO.FileNotFoundException: Could not load the file 'netstandard'..."**

If the console displays this error then *check your iOS Build settings* and set the option *Linker Behaviour* to `Don't Link`:    

![ios-linker-options](images/live-update/ios-build-link-behaviour.png)

### Design

The fabulous watcher does this:

1. Cracks project options, listens for changes, then uses FSharp.Compiler.Service to compile

2. converts code output to PortaCode code model 

3. serializes PortaCode using Newtonsoft.Json

4. sends to device by http. 

Device app does this:

5. starts httplistener, which gets http request

6. deserializes PortaCode

7. uses Interpreter.fs to run.

8. looks for a "program" declaration in interpreted code and hacks into the currently running Elmish app and replaces the Fabulous "program" ie view/update/init logic. 

Device app continues to use whatever library dlls are on device via reflection.


Please contribute documentation, updates and fixes to make the experience simpler.

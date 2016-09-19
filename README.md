# whitespace_module_vb
VB.NET class for HTTP whitespace removal 

It is a plug n’ play HTTP VB module that works by simply adding the class to the App_Code folder. Created based on the C# version from Mads Kristensen: http://madskristensen.net/post/A-whitespace-removal-HTTP-module-for-ASPNET-20. Many thanks! By using this module, HTML is being compressed at the server.

How to use:

1. Download the vb module and copy it to your App_Code folder.
2. Make sure all dependencies are available in the BIN folder: AjaxMin.dll & WebMarkupMin.Core.dll
3. Add the module to the web.config:
      <system.webServer>
            <modules runAllManagedModulesForAllRequests="false">
                  <add name="WhitespaceModule" type="WhitespaceModule" preCondition="managedHandler" />
            </modules>
      </system.webServer>

You are now up and running. Enjoy.

Dependencies:
- https://www.nuget.org/packages/AjaxMin/
- https://www.nuget.org/packages/WebMarkupMin.Core/

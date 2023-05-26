# Lightstreamer - Stock-List Demo - .NET Adapter
<!-- START DESCRIPTION lightstreamer-example-stocklist-adapter-dotnet -->

The Stock-List demos simulate a market data feed and front-end for stock quotes. They show a list of stock symbols and updates prices and other fields displayed on the page in real-time.

This project contains the source code and all the resources needed to install a .NET version of the Stock-List Demo Data Adapter and Metadata Adapters.

As an example of [Clients Using This Adapter](#clients-using-this-adapter), you may refer to the [Lightstreamer - Basic Stock-List Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript#basic-stock-list-demo---html-client) and view the corresponding [Live Demo](http://demos.lightstreamer.com/StockListDemo_Basic).

## Details

Lightstreamer Server exposes native Java Adapter interfaces. The .NET interfaces are added through the [Lightstreamer Adapter Remoting Infrastructure (**ARI**)](https://lightstreamer.com/api/ls-generic-adapter/latest/ARI%20Protocol.pdf).

*The Architecture of Adapter Remoting Infrastructure for .NET.*

![General Architecture](generalarchitecture_new.png)

You'll find more details about the *SDK for .NET Standard Adapters* at *.NET Interfaces* in the [Lightstreamer - "Hello World" Tutorial - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-HelloWorld-adapter-dotnet/blob/master/README.md#net-interfaces) project.

### Dig the Code

This project includes the implementation of the IDataProvider interface and the IMetadataProvider interface for the *Stock-List Demo*. 

The application is divided into 4 main classes.

* `StockList.cs`: this is a C#/.NET porting of the [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-java). It inherits from the *IDataProvider* interface and calls back Lightstreamer through the *IItemEventListener* interface. Use it as a starting point to implement your custom data adapter.
* `ExternalFeed.cs`: this component simulates an external data feed that supplies quote values for all the stocks needed for the demos.
* `StandaloneLauncher.cs`: this is the Remote Server, that is, stand-alone executable that launches both the Data Adapter and the Metadata Adapter for the .NET Stock-List Demo example. It redirects sockets connections from Lightstreamer to the .NET Servers implemented in the LS .NET Standard Adapter SDK library.
* `Log4NetLoggerProviderWrapper.cs`: used by the Remote Server to forward the log produced by the LS .NET Standard Adapter SDK library to the application logging system, based on NLog.<br>

Check out the sources for further explanations.

The Metadata Adapter functionalities are absolved by the `LiteralBasedProvider`, a simple Metadata Adapter already included in the .NET Standard Adapter SDK binaries, which is enough for this demo.
See also [LiteralBasedProvider Metadata Adapter](https://github.com/Lightstreamer/Lightstreamer-lib-adapter-dotnet-remote#literalbasedprovider).

<!-- END DESCRIPTION lightstreamer-example-stocklist-adapter-dotnet -->

## Install

If you want to install a version of this demo in your local Lightstreamer server, follow these steps:
* Download the latest Lightstreamer distribution (Lightstreamer Server comes with a free non-expiring demo license for 20 connected users) from [Lightstreamer Download page](https://lightstreamer.com/download/), and install it, as explained in the `GETTING_STARTED.TXT` file in the installation home directory.
* Get the `deploy.zip` file of the ["Release for Lightstreamer 7.3" release](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet/releases) and unzip it.
* Plug the Proxy Data Adapter and the Proxy MetaData Adapter into the Server: go to the `Deployment_LS` folder and copy the `DotNetStockList` directory and all of its files to the `adapters` folder of your Lightstreamer Server installation.
* Alternatively, you may plug the **robust** versions of the Proxy Data Adapter and the Proxy MetaData Adapter: go to the `Deployment_LS(robust)` folder and copy the `DotNetStockList` directory and all of its files into `adapters`. This Adapter Set demonstrates the provided "robust" versions of the standard Proxy Data and Metadata Adapters. The robust Proxy Data Adapter can handle the case in which a Remote Data Adapter is missing or fails, by suspending the data flow and trying to connect to a new Remote Data Adapter instance. The robust Proxy Metadata Adapter can handle the case in which a Remote Metadata Adapter is missing or fails, by temporarily denying all client requests and trying to connect to a new Remote Data Adapter instance. See the comments embedded in the provided [`adapters.xml` file template](https://lightstreamer.com/docs/ls-server/latest_7_3/remote_adapter_robust_conf_template/adapters.xml), for details. Note that this extended Adapter Set also requires that the client is able to manage the case of missing data. Currently, only the [Lightstreamer - Stock-List Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript#stocklist-demo) and the [Lightstreamer - Framed Stock-List Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript#framed-stocklist-demo) front-ends have such ability.
* Run the `DotNetStockListDemoLauncher.bat` script under the `Deployment_DotNet_Adapters` directory. The script runs the Remote Server, that is, a .NET Core application which hosts both the Remote Data Adapter and the Remote Metadata Adapter for the .NET Stock-List Demo.
* Launch Lightstreamer Server. The Server startup will complete only after a successful connection between the Proxy Adapters and the Remote Adapters.
* Test the Adapter, launching one of the clients listed in [Clients Using This Adapter](#clients-using-this-adapter).
    * To make the Stock-List Demo front-end pages consult the newly installed Adapter Set, you need to modify the front-end pages and set the required Adapter Set name to STOCKLISTDEMO_REMOTE when creating the LightstreamerClient instance. So a line like this:<BR/>
`var sharingClient = new LightstreamerClient(hostToUse,"DEMO");`<BR/>
becomes like this:<BR/>
`var sharingClient = new LightstreamerClient(hostToUse, "STOCKLISTDEMO_REMOTE");`<BR/>
(you don't need to reconfigure the Data Adapter name, as it is the same in both Adapter Sets).
    * As the referred Adapter Set has changed, make sure that the front-end no longer shares the Engine with demos.
So a line like this:<BR/>
`sharingClient.connectionSharing.enableSharing("DemoCommonConnection","ls/","SHARE_SESSION", true);`<BR/>
should become like this:<BR/>
`  sharingClient.connectionSharing.enableSharing("RemoteStockListConnection","ls/","SHARE_SESSION", true);`<BR/>
The Stock-List Demo web front-end is now ready to be opened. The front-end will now get data from the newly installed Adapter Set.

Please note that the .NET Remote Adapters connects to Proxy Adapters, not vice versa.

### Available improvements

#### Add Encryption

Each TCP connection from a Remote Adapter can be encrypted via TLS. To have the Proxy Adapters accept only TLS connections, a suitable configuration should be added in adapters.xml in the <data_provider> block, like this:
```xml
  <data_provider>
    ...
    <param name="tls">Y</param>
    <param name="tls.keystore.type">JKS</param>
    <param name="tls.keystore.keystore_file">./myserver.keystore</param>
    <param name="tls.keystore.keystore_password.type">text</param>
    <param name="tls.keystore.keystore_password">xxxxxxxxxx</param>
    ...
  </data_provider>
```
and the same should be added in the <metadata_provider> block.

This requires that a suitable keystore with a valid certificate is provided. See the configuration details in the [provided template](https://lightstreamer.com/docs/ls-server/latest_7_3/remote_adapter_robust_conf_template/adapters.xml).
NOTE: For your experiments, you can configure the adapters.xml to use the same JKS keystore "myserver.keystore" provided out of the box in the Lightstreamer distribution. Since this keystore contains an invalid certificate, remember to configure your local environment to "trust" it.
The sample Remote Server provided in the `Deployment_DotNet_Adapters` directory in `deploy.zip` is already predisposed for TLS connection on all ports. You can rerun the demo with the new configuration after modifying DotNetStockListDemoLauncher.bat to run a command like this:<BR/>
  `dotnet TestAdapter.dll /host xxxxxxxx /tls /data_rrport 6661 /data_notifport 6662 /metadata_rrport 6663 max_bandwidth=40 max_frequency=3 buffer_size=30`<BR/>
where the same hostname supported by the provided certificate must be supplied.

#### Add Authentication

Each TCP connection from a Remote Adapter can be subject to Remote Adapter authentication through the submission of user/password credentials. To enforce credential check on the Proxy Adapters, a suitable configuration should be added in adapters.xml in the <data_provider> block, like this:
```xml
  <data_provider>
    ...
    <param name="auth">Y</param>
    <param name="auth.credentials.1.user">user1</param>
    <param name="auth.credentials.1.password">pwd1</param>
    ...
  </data_provider>
```
and the same should be added in the <metadata_provider> block.

See the configuration details in the [provided template](https://lightstreamer.com/docs/ls-server/latest_7_3/remote_adapter_robust_conf_template/adapters.xml).
The sample Remote Server provided in the `Deployment_DotNet_Adapters` directory in `deploy.zip` is already predisposed for credential submission on both adapters. You can rerun the demo with the new configuration after modifying DotNetStockListDemoLauncher.bat to run a command like this:<BR/>
  `dotnet TestAdapter.dll /host localhost /user user1 /password pwd1 /data_rrport 6661 /data_notifport 6662 /metadata_rrport 6663 max_bandwidth=40 max_frequency=3 buffer_size=30`<BR/>

Authentication can (and should) be combined with TLS encryption.

## Build 

### Build the .NET Stock-List Demo Data Adapter

To build your own version of `DotNetStockListDataAdapter.dll`, instead of using the one provided in the `deploy.zip` file from the [Install](#install) section above, follow these steps:
* Download this project.
* Create a project for ".NET Core library" template and name it "DotNetStockListDataAdapter".
* Include in the project the sources `src/src_data_adapter`.
* Get the binaries files of the Lightstreamer .NET Standard Adapters Server library from NuGet [Lightstreamer.DotNetStandard.Adapters](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Adapters/), copy it into the `lib` directory and add it as a reference for the project; or more simply, use directly the "NuGet Package Manager" looking for 'Lightstreamer Adapters' and installing the Lightstreamer.DotNetStandard.Adapters package.
* Build Solution

### Build the Stand-Alone Launcher (i.e. Remote Server)
To build your own version of the Remote Server, follow these steps:
* Create a project for ".NET Core App Console" template and name it "TestAdapter".
* Include in the project the source `src/src_standalone_launcher`
* Include reference to the .NET Stock-List Demo Data Adapter binaries you have built in the previous step. 
* Get the binaries files of the Lightstreamer .NET Standard Adapters Server library from NuGet [Lightstreamer.DotNetStandard.Adapters](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Adapters/), copy it into the `lib` directory and add it as a reference for the project; or more simply, use directly the "NuGet Package Manager" looking for 'Lightstreamer Adapters' and installing the Lightstreamer.DotNetStandard.Adapters package.
* Get the binaries files of the [NLog library from NuGet](https://www.nuget.org/packages/NLog/), copy it into the `lib` directory and add it as a reference for the project; or more simply, use directly the "NuGet Package Manager" looking for 'NLog' and installing the NLog package.
* Make sure that the startup object is the TestAdapter.StandaloneLauncher class.
* Build Solution

## See Also

* [Adapter Remoting Infrastructure Network Protocol Specification](https://lightstreamer.com/api/ls-generic-adapter/latest/ARI%20Protocol.pdf)
* [.NET Adapter API Reference](https://lightstreamer.com/api/ls-dotnetstandard-adapter/latest/index.html)

### Clients Using This Adapter

<!-- START RELATED_ENTRIES -->

* [Complete list of clients using this Adapter](https://github.com/Lightstreamer?utf8=%E2%9C%93&q=lightstreamer-example-stocklist-client&type=&language=)

<!-- END RELATED_ENTRIES -->
### Related Projects
* [LiteralBasedProvider Metadata Adapter - Java](https://github.com/Lightstreamer/Lightstreamer-lib-adapter-java-remote#literalbasedprovider-metadata-adapter)
* [LiteralBasedProvider Metadata Adapter - .NET](https://github.com/Lightstreamer/Lightstreamer-lib-adapter-dotnet-remote#literalbasedprovider)
* [Lightstreamer - Portfolio Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-java)
* [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-java)
* [Lightstreamer - Stock-List Demo - Java (JMS) Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-JMS)

## Lightstreamer Compatibility Notes

* Compatible with Lightstreamer SDK for .NET Standard Adapters version 1.12 to 1.14.
* For instructions compatible with Lightstreamer SDK for .NET Adapters version 1.11, please refer to [this tag](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet/releases/tag/for_standard_1.11).

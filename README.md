# Lightstreamer - Stock-List Demo - .NET Adapter
<!-- START DESCRIPTION lightstreamer-example-stocklist-adapter-dotnet -->

The Stock-List demos simulate a market data feed and front-end for stock quotes. They show a list of stock symbols and updates prices and other fields displayed on the page in real-time.

This project contains the source code and all the resources needed to install a .NET version of the Stock-List Demo Data Adapter and Metadata Adapters.

As an example of [Clients Using This Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet#clients-using-this-adapter), you may refer to the [Lightstreamer - Basic Stock-List Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript#basic-stock-list-demo---html-client) and view the corresponding [Live Demo](http://demos.lightstreamer.com/StockListDemo_Basic).

## Details
Lightstreamer Server exposes native Java Adapter interfaces. The .NET interfaces are added through the [Lightstreamer Adapter Remoting Infrastructure (**ARI**)](http://www.lightstreamer.com/docs/adapter_generic_base/ARI%20Protocol.pdf).

*The Architecture of Adapter Remoting Infrastructure for .NET.*

![General Architecture](generalarchitecture_new.png)

You'll find more details about the *SDK for .NET Standard Adapters* at [.NET Interfaces](https://github.com/Lightstreamer/Lightstreamer-example-HelloWorld-adapter-dotnet/blob/master/README.md#net-interfaces) in the [Lightstreamer - "Hello World" Tutorial - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-HelloWorld-adapter-dotnet) project.

### Dig the Code

This project includes the implementation of the [IDataProvider](http://www.lightstreamer.com/docs/adapter_dotnet_api/Lightstreamer_Interfaces_Data_IDataProvider.html) interface and the [IMetadataProvider](http://www.lightstreamer.com/docs/adapter_dotnet_api/Lightstreamer_Interfaces_Metadata_IMetadataProvider.html) interface for the *Stock-List Demo*. 

The application is divided into 4 main classes.

* `StockList.cs`: this is a C#/.NET porting of the [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-java). It inherits from the *IDataProvider* interface and calls back Lightstreamer through the *IItemEventListener* interface. Use it as a starting point to implement your custom data adapter.
* `ExternalFeed.cs`: this component simulates an external data feed that supplies quote values for all the stocks needed for the demos.
* `StandaloneLauncher.cs`: this is a stand-alone executable that launches both the Data Adapter and the Metadata Adapter for the .NET Stock-List Demo example. It redirects sockets connections from Lightstreamer to the .NET Servers implemented in the LS .NET SDK library and does not rely on the .NET Server wrapper provided.
* `Log4NetLoggerProviderWrapper.cs`: used by the stand-alone executable to forward the log produced by the LS .NET SDK library to the application logging system, based on NLog.<br>

Check out the sources for further explanations.

The Metadata Adapter functionalities are absolved by the `LiteralBasedProvider`, a simple Metadata Adapter already included in the .NET Adapter SDK binaries, which is enough for all demo clients.
See also [Lightstreamer - Reusable Metadata Adapters - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-ReusableMetadata-adapter-dotnet).

<!-- END DESCRIPTION lightstreamer-example-stocklist-adapter-dotnet -->

## Install

If you want to install a version of this demo in your local Lightstreamer server, follow these steps:
* Download the latest Lightstreamer distribution (Lightstreamer Server comes with a free non-expiring demo license for 20 connected users) from [Lightstreamer Download page](https://lightstreamer.com/download/), and install it, as explained in the `GETTING_STARTED.TXT` file in the installation home directory.
* Get the `deploy.zip` file of the [latest release](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet/releases) and unzip it
* Plug the Proxy Data Adapter and the Proxy MetaData Adapter into the Server: go to the `Deployment_LS` folder and copy the `DotNetStockList` directory and all of its files to the `adapters` folder of your Lightstreamer Server installation.
* Alternatively, you may plug the **robust** versions of the Proxy Data Adapter and the Proxy MetaData Adapter: go to the `Deployment_LS(robust)` folder and copy the `DotNetStockList` directory and all of its files into `adapters`. This Adapter Set demonstrates the provided "robust" versions of the standard Proxy Data and Metadata Adapters. The robust Proxy Data Adapter can handle the case in which a Remote Data Adapter is missing or fails, by suspending the data flow and trying to connect to a new Remote Data Adapter instance. The robust Proxy Metadata Adapter can handle the case in which a Remote Metadata Adapter is missing or fails, by temporarily denying all client requests and trying to connect to a new Remote Data Adapter instance. See the comments embedded in the generic `adapters.xml` file template, `DOCS-SDKs/adapter_remoting_infrastructure/doc/adapter_robust_conf_template/adapters.xml`, for details. Note that this extended Adapter Set also requires that the client is able to manage the case of missing data. Currently, only the [Lightstreamer - Stock-List Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript#stocklist-demo) and the [Lightstreamer - Framed Stock-List Demo - HTML Client](https://github.com/Lightstreamer/Lightstreamer-example-StockList-client-javascript#framed-stocklist-demo) front-ends have such ability.
* Run the `DotNetStockListDemoLauncher.bat` script under the `Deployment_DotNet_Adapters` directory. The script runs a .NET Core application which hosts both the Remote Data Adapter and the Remote Metadata Adapter for the .NET Stock-List Demo.
* Launch Lightstreamer Server. The Server startup will complete only after a successful connection between the Proxy Adapters and the Remote Adapters.
* Test the Adapter, launching one of the clients listed in [Clients Using This Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet#clients-using-this-adapter).
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

In case of need, the .NET StockList Demo Launcher prints on the log a help page if run with the following syntax: `dotnet TestAdapter.dll /help`.

Please note that the .NET Remote Adapters connects to Proxy Adapters, not vice versa.

## Build 

### Build the .NET Stock-List Demo Data Adapter

To build your own version of `DotNetStockListDataAdapter.dll`, instead of using the one provided in the `deploy.zip` file from the [Install](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet#install) section above, follow these steps:
* Download this project.
* Create a project for ".NET Core library" template and name it "DotNetStockListDataAdapter".
* Include in the project the sources `src/src_data_adapter`.
* Get the binaries files of the Lightstreamer .NET Standard Adapters Server library from NuGet [Lightstreamer.DotNetStandard.Adapters](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Adapters/), copy it into the `lib` directory and add it as a reference for the project; or more simply, use directly the "NuGet Package Manager" looking for 'Lightstreamer Adapters' and intalling the Lightstreamer.DotNetStandard.Adapters package.
* Build Solution

### Build the Stand-Alone Launcher
To build your own version of the Stand-Alone Launcher, follow these steps:
* Create a project for ".NET Core App Console" template and name it "TestAdapter".
* Include in the project the source `src/src_standalone_launcher`
* Include reference to the .NET Stock-List Demo Data Adapter binaries you have built in the previous step. 
* Get the binaries files of the Lightstreamer .NET Standard Adapters Server library from NuGet [Lightstreamer.DotNetStandard.Adapters](https://www.nuget.org/packages/Lightstreamer.DotNetStandard.Adapters/), copy it into the `lib` directory and add it as a reference for the project; or more simply, use directly the "NuGet Package Manager" looking for 'Lightstreamer Adapters' and intalling the Lightstreamer.DotNetStandard.Adapters package.
* Get the binaries files of the [NLog library from NuGet](https://www.nuget.org/packages/NLog/), copy it into the `lib` directory and add it as a reference for the project; or more simply, use directly the "NuGet Package Manager" looking for 'NLog' and intalling the NLog package.
* Make sure that the startup object is the TestAdapter.StandaloneLauncher class.
* Build Solution

## See Also

* [Adapter Remoting Infrastructure Network Protocol Specification](http://www.lightstreamer.com/docs/adapter_generic_base/ARI%20Protocol.pdf)
* [.NET Adapter API Reference](http://www.lightstreamer.com/docs/adapter_dotnet_api/frames.html)

### Clients Using This Adapter

<!-- START RELATED_ENTRIES -->

* [Complete list of clients using this Adapter](https://github.com/Lightstreamer?utf8=%E2%9C%93&q=lightstreamer-example-stocklist-client&type=&language=)

<!-- END RELATED_ENTRIES -->
### Related Projects
* [Lightstreamer - Reusable Metadata Adapters - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-ReusableMetadata-adapter-java)
* [Lightstreamer - Reusable Metadata Adapters - .NET Adapter](https://github.com/Lightstreamer/Lightstreamer-example-ReusableMetadata-adapter-dotnet)
* [Lightstreamer - Portfolio Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-Portfolio-adapter-java)
* [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-java)
* [Lightstreamer - Stock-List Demo - Java (JMS) Adapter](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-JMS)

## Lightstreamer Compatibility Notes

* Compatible with Lightstreamer SDK for .NET Standard Adapters version 1.11.
* For instructions compatible with Lightstreamer SDK for .NET Adapters version 1.10, please refer to [this tag](https://github.com/Lightstreamer/Lightstreamer-example-StockList-adapter-dotnet/tree/current_1.10).

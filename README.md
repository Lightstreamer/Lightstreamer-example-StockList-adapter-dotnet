# Lightstreamer - Stock-List Demo - .NET Adapter
<!-- START DESCRIPTION lightstreamer-example-stocklist-adapter-dotnet -->

The Stock-List demos simulate a market data feed and front-end for stock quotes. They show a list of stock symbols and updates prices and other fields displayed on the page in real-time.

This project contains the source code and all the resources needed to install a .NET version of the Stock-List Demo Data Adapter and Metadata Adapters.

As an example of [Clients Using This Adapter](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-dotnet#clients-using-this-adapter), you may refer to the [Lightstreamer - Basic Stock-List Demo - HTML Client](https://github.com/Weswit/Lightstreamer-example-StockList-client-javascript#basic-stock-list-demo---html-client) and view the corresponding [Live Demo](http://demos.lightstreamer.com/StockListDemo_Basic).

## Details
Lightstreamer Server exposes native Java Adapter interfaces. The .NET interfaces are added through the [Lightstreamer Adapter Remoting Infrastructure (**ARI**)](http://www.lightstreamer.com/docs/ARI_base/ARI%20Protocol.pdf). 

*The Architecture of Adapter Remoting Infrastructure for .NET.*

![General Architecture](generalarchitecture_new.png)

You'll find more details about the *SDK for .NET Adapters* at [.NET Interfaces](https://github.com/Weswit/Lightstreamer-example-HelloWorld-adapter-dotnet/blob/master/README.md#net-interfaces) in the [Lightstreamer - "Hello World" Tutorial - .NET Adapter](https://github.com/Weswit/Lightstreamer-example-HelloWorld-adapter-dotnet) project.

### Dig the Code

This project includes the implementation of the [IDataProvider](http://www.lightstreamer.com/docs/adapter_dotnet_api/Lightstreamer_Interfaces_Data_IDataProvider.html) interface and the [IMetadataProvider](http://www.lightstreamer.com/docs/adapter_dotnet_api/Lightstreamer_Interfaces_Metadata_IMetadataProvider.html) interface for the *Stock-List Demo*. 

The application is divided into 4 main classes.

* `StockList.cs`: this is a C#/.NET porting of the [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-java). It inherits from the *IDataProvider* interface and calls back Lightstreamer through the *IItemEventListener* interface. Use it as a starting point to implement your custom data adapter.
* `ExternalFeed.cs`: this component simulates an external data feed that supplies quote values for all the stocks needed for the demos.
* `StandaloneLauncher.cs`: this is a stand-alone executable that launches both the Data Adapter and the Metadata Adapter for the .NET Stock-List Demo example. It redirects sockets connections from Lightstreamer to the .NET Servers implemented in the LS .NET SDK library and does not rely on the .NET Server wrapper provided.
* `Log4NetLogging.cs`: used by the stand-alone executable to forward the log produced by the LS .NET SDK library to the application logging system, based on log4net.<br>

Check out the sources for further explanations.

The Metadata Adapter functionalities are absolved by the `LiteralBasedProvider`, a simple Metadata Adapter already included in the .NET Adapter SDK binaries, which is enough for all demo clients.
See also [Lightstreamer - Reusable Metadata Adapters - .NET Adapter](https://github.com/Weswit/Lightstreamer-example-ReusableMetadata-adapter-dotnet).

**NOTE: At this stage, the demo is based on a version of LS .NET SDK that is currently available only as a pre-release. Skip the notes below and refer to the [for_Lightstreamer_5.1](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-dotnet/tree/for_Lightstreamer_5.1) tag for a demo version suitable for building and deploying.**

<!-- END DESCRIPTION lightstreamer-example-stocklist-adapter-dotnet -->

## Install

If you want to install a version of this demo in your local Lightstreamer server, follow these steps:
* Download *Lightstreamer Server 6.0* (Lightstreamer Server comes with a free non-expiring demo license for 20 connected users) from [Lightstreamer Download page](http://www.lightstreamer.com/download.htm), and install it, as explained in the `GETTING_STARTED.TXT` file in the installation home directory.
* Get the `deploy.zip` file of the [latest release](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-dotnet/releases) and unzip it
* Plug the Proxy Data Adapter and the Proxy MetaData Adapter into the Server: go to the `Deployment_LS` folder and copy the `DotNetStockList` directory and all of its files to the `adapters` folder of your Lightstreamer Server installation.
* Alternatively, you may plug the **robust** versions of the Proxy Data Adapter and the Proxy MetaData Adapter: go to the `Deployment_LS(robust)` folder and copy the `DotNetStockList` directory and all of its files into `adapters`. This Adapter Set demonstrates the provided "robust" versions of the standard Proxy Data and Metadata Adapters. The robust Proxy Data Adapter can handle the case in which a Remote Data Adapter is missing or fails, by suspending the data flow and trying to connect to a new Remote Data Adapter instance. The robust Proxy Metadata Adapter can handle the case in which a Remote Metadata Adapter is missing or fails, by temporarily denying all client requests and trying to connect to a new Remote Data Adapter instance. See the comments embedded in `adapters.xml` for details. Note that this extended Adapter Set also requires that the client is able to manage the case of missing data. Currently, only the [Lightstreamer - Stock-List Demo - HTML Client](https://github.com/Weswit/Lightstreamer-example-StockList-client-javascript#stocklist-demo) and the [Lightstreamer - Framed Stock-List Demo - HTML Client](https://github.com/Weswit/Lightstreamer-example-StockList-client-javascript#framed-stocklist-demo) front-ends have such ability.
* Launch the Remote .NET Adapter Server. The .NET Server resources can be found under `Deployment_DotNet_Server`. Run the `DotNetServers.bat` script. The script runs the two instances of the .NET Server (one for the Remote Metadata Adapter and the other for the Remote Data Adapter).
* Alternatively, run the `DotNetCustomServer.bat` script under the `Deployment_DotNet_Server(custom)` directory. The script runs the DotNetStockListDemoLauncher_N2.exe Custom Launcher, which hosts both the Remote Data Adapter and the Remote Metadata Adapter for the .NET Stock-List Demo.
* Launch Lightstreamer Server. The Server startup will complete only after a successful connection between the Proxy Adapters and the Remote Adapters.
* Test the Adapter, launching one of the clients listed in [Clients Using This Adapter](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-dotnet#clients-using-this-adapter).
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

In case of need, the .NET Server prints on the log a help page if run with the following syntax: "DotNetServer /help".

Please note that the .NET Server connects to Proxy Adapters, not vice versa.

The standard type of configuration is shown, where the process that runs the Remote Adapters is manually launched beside Lightstreamer Server.
On the other hand, two different examples of manual launch of the remote process are shown, one based on the provided Remote Server and one based on a custom server program, also shown.

## Build 

### Build the .NET Stock-List Demo Data Adapter

To build your own version of `DotNetStockListDemo_N2.dll`, instead of using the one provided in the `deploy.zip` file from the [Install](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-dotnet#install) section above, follow these steps:
* Download this project.
* Create a project for a library target and name it "DotNetStockListDemo_N2",
* Include in the project the sources `src/src_data_adapter`.
* Get the Lightstreamer .NET Adapter Server library `DotNetAdapter_N2.dll` file from the `DOCS-SDKs/sdk_adapter_dotnet/lib` folder of the latest [Lightstreamer 6.0 (Beta)](http://download.lightstreamer.com/#next) distribution, and copy it into the `lib` directory.
* Include in the project the references to `DotNetAdapter_N2.dll` from the `lib` folder.
* Build Solution

### Build the Stand-Alone Launcher
To build your own version of the Stand-Alone Launcher, follow these steps:
* Create a project for a console application target and name it "DotNetStockListDemoLauncher_N2".
* Include in the project the source `src/src_standalone_launcher`
* Include references to the Lightstreamer .NET Adapter Server library binaries (see above) and .NET Stock-List Demo Data Adapter binaries you have built in the previous step. 
* Make sure that the entry point of the executable is the ServerMain class.
* Build Solution

## See Also

* [Adapter Remoting Infrastructure Network Protocol Specification](http://www.lightstreamer.com/docs/ARI_base/ARI%20Protocol.pdf)
* [.NET Adapter API Reference](http://www.lightstreamer.com/docs/adapter_dotnet_api/frames.html)

### Clients Using This Adapter

<!-- START RELATED_ENTRIES -->

* [Complete list of clients using this Adapter](https://github.com/Weswit?query=lightstreamer-example-stocklist-client)

<!-- END RELATED_ENTRIES -->
### Related Projects
* [Lightstreamer - Reusable Metadata Adapters - Java Adapter](https://github.com/Weswit/Lightstreamer-example-ReusableMetadata-adapter-java)
* [Lightstreamer - Reusable Metadata Adapters - .NET Adapter](https://github.com/Weswit/Lightstreamer-example-ReusableMetadata-adapter-dotnet)
* [Lightstreamer - Portfolio Demo - Java Adapter](https://github.com/Weswit/Lightstreamer-example-Portfolio-adapter-java)
* [Lightstreamer - Stock-List Demo - Java Adapter](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-java)
* [Lightstreamer - Stock-List Demo - Java (JMS) Adapter](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-JMS)

## Lightstreamer Compatibility Notes

* Compatible with Lightstreamer SDK for .NET Adapters version 1.9 or newer.
- For a version of this example compatible with Lightstreamer SDK for .NET Adapters version 1.7, please refer to [this tag](https://github.com/Weswit/Lightstreamer-example-StockList-adapter-dotnet/tree/for_Lightstreamer_5.1).

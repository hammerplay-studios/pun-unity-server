
# Photon Cloud, Unity3D Server
This project uses Photon Cloud (PUN) and runs a dedicated server by listening to Photon Cloud's Webhooks events.

The project can be both exported as standalone server and game client. It's just a fancy connection manager with command line arguments. Works both on Windows and Linux, haven't tested it on MacOS.

**Requirements**
 - Unity 2018.2 or higher 
 - Photon account and App Id
 - Any server that listens to Webhook events and fires a linux build,  Use our simple [NodeJS server](https://github.com/hammerplay-studios/pun-dedicated-server), that we use in this example.

**How it works**

The concept is fairly simple, Photon Cloud, in our case PUN, does not offer you a server client model, what you have is a MasterClient; All the clients connect to Photon Cloud, and we cannot run server side logic on Photon Cloud. This project is just a work around by assigning one of the client to run the server side logic (Authoritative logic). This raises another issue, if the MasterClient fails or has bad network connection, the other one suffers because that player is depended on MasterClient's connection.

The solution, Open a third instance of the game (optionally, headless mode) and make it MasterClient, and let it handle the Authoritative logic.
![FlowChart](https://raw.githubusercontent.com/hammerplay-studios/pun-unity-server/develop/Docs/images/flow-chart.png)

**How to**

Set you AppId, you can find this in your dashboard
![Set App Id for Photon](https://github.com/hammerplay-studios/pun-unity-server/blob/develop/Docs/images/AppId.png?raw=true)

Take a Linux build, and make sure you select Headless Mode option, this could work in Windows Servers as well.
![enter image description here](https://github.com/hammerplay-studios/pun-unity-server/blob/develop/Docs/images/Build.png?raw=true)

Now you need to upload this build to the [server](%28https://github.com/hammerplay-studios/pun-dedicated-server).

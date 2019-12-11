# running the SampleApp workshop

## table of contents 

- [running the SampleApp workshop](#running-the-SampleApp-workshop)
  - [table of contents](#table-of-contents)
  - [to begin, follow the workshop environment setup guide](#to-begin-follow-the-workshop-environment-setup-guide)
  - [openshift cli build + deployment pattern](#openshift-cli-build--deployment-pattern)
    - [setting the project context](#setting-the-project-context)
    - [creating the build](#creating-the-build)
    - [starting the build from a local directory](#starting-the-build-from-a-local-directory)
    - [creating the OpenShift application](#creating-the-OpenShift-application)
    - [checking on the deployment](#checking-on-the-deployment)
  - [spinning up the backend (DataService API)](#spinning-up-the-backend-DataService-API)
    - [ensure we can build the dotnet project locally](#ensure-we-can-build-the-dotnet-project-locally)
      - [verify ```dotnet``` tools availability](#verify-dotnet-tools-availability)
    - [retrieve solution locally](#retrieve-solution-locally)
      - [clone the repository from GitHub](#clone-the-repository-from-GitHub)
    - [(dotnet) build + (dotnet) publish solution](#dotnet-build--dotnet-publish-solution)
    - [s2i build + deployment + expose app to external route](#s2i-build--deployment--expose-app-to-external-route)
      - [create a new OpenShift build for DotNet API](#create-a-new-OpenShift-build-for-DotNet-API)
      - [start the newly-created OpenShift build from the locally-published artifacts](#start-the-newly-created-OpenShift-build-from-the-locally-published-artifacts)
      - [create a new OpenShift app to specify a deployment](#create-a-new-OpenShift-app-to-specify-a-deployment)
      - [expose the newly-deployed app](#expose-the-newly-deployed-app)
    - [cURL & verify API returns](#cURL--verify-API-returns)
      - [navigate to console + observe the external route](#navigate-to-console--observe-the-external-route)
  - [deploying the MongoDB (data tier)](#deploying-the-MongoDB-data-tier)
    - [create a MongoDB instance with the Red Hat template](#create-a-MongoDB-instance-with-the-Red-Hat-template)
    - [build + publish DotNet DataSeeder](#build--publish-DotNet-DataSeeder)
      - [navigate to the ```Perficient.OpenShift.WorkShop.DataSeeder``` solution directory](#navigate-to-the-PerficientOpenShiftWorkShopDataSeeder-solution-directory)
      - [once in the directory, build the DotNet solution](#once-in-the-directory-build-the-DotNet-solution)
      - [once the solution is built, publish the solution](#once-the-solution-is-built-publish-the-solution)
    - [create and start a new OpenShift build for the DotNet DataSeeder](#create-and-start-a-new-OpenShift-build-for-the-DotNet-DataSeeder)
      - [once the DataSeeder build has finished, create a new OpenShift DotNet app](#once-the-DataSeeder-build-has-finished-create-a-new-OpenShift-DotNet-app)
      - [now that the new DotNet application is running within in a Pod, we want to apply environmental settings to it using ```oc set env```](#now-that-the-new-DotNet-application-is-running-within-in-a-Pod-we-want-to-apply-environmental-settings-to-it-using-oc-set-env)
      - [we've created environmental settings, but lack **secrets**...](#weve-created-environmental-settings-but-lack-secrets)
    - [as of now, the MongoDB is deployed, but lacks a connection to the DataService API](#as-of-now-the-MongoDB-is-deployed-but-lacks-a-connection-to-the-DataService-API)
      - [since the DataService is already deployed and running, we will follow the same steps as the DataSeeder and set environmental variables + secrets](#since-the-DataService-is-already-deployed-and-running-we-will-follow-the-same-steps-as-the-DataSeeder-and-set-environmental-variables--secrets)
    - [verifying DataService-to-MongoDB connection](#verifying-DataService-to-MongoDB-connection)
      - [navigate to the DataService Pod within the OpenShift Web Console](#navigate-to-the-DataService-Pod-within-the-OpenShift-Web-Console)
      - [select ```Terminal``` from within the Pod menu and cURL the DataService API](#select-Terminal-from-within-the-Pod-menu-and-cURL-the-DataService-API)
  - [deploy the front-end (React app + Nginx)](#deploy-the-front-end-React-app--Nginx)
    - [```npm build``` the React app](#npm-build-the-React-app)
      - [navigate into the ```UI``` directory containing the front-end resources](#navigate-into-the-UI-directory-containing-the-front-end-resources)
      - [refer the the ```npm``` scripts we have available to us](#refer-the-the-npm-scripts-we-have-available-to-us)
    - [modifying environment variables to be set at run-time](#modifying-environment-variables-to-be-set-at-run-time)
      - [create a Config Map](#create-a-Config-Map)
  - [final verification](#final-verification)
    - [expose Nginx app to an external route for the API](#expose-Nginx-app-to-an-external-route-for-the-API)
    - ["tying it together" + summary](#%22tying-it-together%22--summary)

## to begin, follow the workshop environment setup guide

find the guide [here](./workshop-environment-setup.md) *(localized hyperlink)*

## openshift cli build + deployment pattern

### setting the project context

```powershell
# create a new OpenShift project
oc project <name-of-project>
```

### creating the build

```powershell
# this command creates a new build from which can create applications
oc new-build --name=<name-of-build> <base-image> --binary=true
```

### starting the build from a local directory

```powershell
# builds must complete error-free, we use `oc start-build` to kick-off a build
oc start-build <name-of-build> --from-dir=<local-solution-directory> --follow=true
```

* what is ```--from-dir=<local-solution-directory>```?
  * this specifies that we are starting a build from generated artifacts that exist in a local directory on the Development Environment

* what is ```--follow=true```?
  * by setting ```follow``` to ```true```, we can observe the log-trail of the build within the PowerShell window instead of having to navigate back to the OpenShift web console to observe the logs of a particular build

### creating the OpenShift application

```powershell
# here, we create a new app from a completed build
oc new-app <name-of-build> --name=<name-of-app>
```

### checking on the deployment

```powershell
# `oc status` allows us to check on all occurrences in the current project's scope
oc status
```

## spinning up the backend (DataService API)

### ensure we can build the dotnet project locally

#### verify ```dotnet``` tools availability

```powershell
dotnet
```

### retrieve solution locally

#### clone the repository from GitHub

```powershell
git clone https://github.com/perficient-msftnbu/OpenShiftOnAzure-Workshop.git <directory-of-cloned-repository>

cd <directory-of-cloned-repository>

ls
```

### (dotnet) build + (dotnet) publish solution

```powershell
dotnet build Perficient.OpenShift.Workshop.API.csproj
```

```powershell
dotnet publish Perficient.OpenShift.Workshop.API.csproj -c Release /p:MicrosoftNETPlatformLibrary=Microsoft.NETCore.App
```

### s2i build + deployment + expose app to external route

#### create a new OpenShift build for DotNet API

```powershell
oc new-build --name=workshop-api-build dotnet:2.2 --binary=true
```

#### start the newly-created OpenShift build from the locally-published artifacts

```powershell
oc start-build workshop-api-build --from-dir=bin/Release/netcoreapp2.2/publish
```

#### create a new OpenShift app to specify a deployment

```powershell
oc new-app workshop-api-build --name=workshop-api
```

#### expose the newly-deployed app

```powershell
# `oc expose` is a simple command that helps to set-up a service and external route for a particular app -- very helpful!
oc expose svc/workshop-api
```

### cURL & verify API returns

#### navigate to console + observe the external route

append ```/api/SampleData/WeatherForecast``` to the end of the public URL

also attempt appending ```/api/FetchCats/GetCats``` to the end of the public URL

## deploying the MongoDB (data tier)

### create a MongoDB instance with the Red Hat template

```powershell
oc process openshift//mongodb-persistent -p MONGODB_USER=forecastsuser -p MONGODB_PASSWORD=forecastpassword -p MONGODB_DATABASE=forecastdb -p MONGODB_ADMIN_PASSWORD=forecastpassword | oc create -f -
```

### build + publish DotNet DataSeeder

#### navigate to the ```Perficient.OpenShift.WorkShop.DataSeeder``` solution directory

```powershell
# assuming in the root repository directory...
cd Perficient.OpenShift.WorkShop.DataSeeder
```

#### once in the directory, build the DotNet solution

```powershell
dotnet build .\Perficient.OpenShift.WorkShop.DataSeeder.csproj
```

#### once the solution is built, publish the solution

```powershell
dotnet publish  -c Release /p:MicrosoftNETPlatformLibrary=Microsoft.NETCore.App
```

### create and start a new OpenShift build for the DotNet DataSeeder

```powershell
oc new-build --name=data-seeder dotnet:2.2 --binary=true
```

```powershell
oc start-build data-seeder --from-dir=bin/Release/netcoreapp2.2/publish --follow
```

#### once the DataSeeder build has finished, create a new OpenShift DotNet app

```powershell
oc new-app data-seeder
```

#### now that the new DotNet application is running within in a Pod, we want to apply environmental settings to it using ```oc set env```

```powershell
oc set env dc/data-seeder MongoDbSettings__HostName=mongodb.workshop-demo.svc.cluster.local
```

#### we've created environmental settings, but lack **secrets**...

```powershell
oc set env dc/data-seeder --from secret/mongodb
```

### as of now, the MongoDB is deployed, but lacks a connection to the DataService API

#### since the DataService is already deployed and running, we will follow the same steps as the DataSeeder and set environmental variables + secrets

```powershell
oc set env dc/workshop-api MongoDbSettings__HostName=mongodb.workshop-demo.svc.cluster.local
```

```powershell
oc set env dc/workshop-api --from secret/mongodb
```

### verifying DataService-to-MongoDB connection

#### navigate to the DataService Pod within the OpenShift Web Console

#### select ```Terminal``` from within the Pod menu and cURL the DataService API

```bash
curl http://workshop-api.workshop-demo.svc.cluster.local:8080/api/sampledata/WeatherForecasts 
```

## deploy the front-end (React app + Nginx)

### ```npm build``` the React app

#### navigate into the ```UI``` directory containing the front-end resources

```powershell
cd UI/ClientApp
```

#### refer the the ```npm``` scripts we have available to us

```cat``` allows us to display the content of a particular file

...in this case, we wish to examine the pre-defined ```npm``` scripts that pertain to building the React front-end application

```powershell
cat package.json
```

#### output should look like

```powershell
...
    extends": "react-app"
  },
  "scripts": {
    "start": "rimraf ./build && react-scripts start",
    "build": "react-scripts build",
    "test": "cross-env CI=true react-scripts test --env=jsdom",
    "eject": "react-scripts eject",
    "lint": "eslint ./src/",
    "deploy": "cp env.sh ./public/ && cp env-config.js ./public/ && react-scripts build",
    "deploy-old": "chmod +x ./env.sh && ./env.sh && cp env-config.js ./public/ && react-scripts build"
  }
}
```

#### install the ```npm``` packages

```powershell
npm install
```

#### ensure we can build + deploy the front-end

observe the ```env.sh``` script

```powershell
npm run-script build
```

#### sample output

```powershell
...
> my_new_app@0.1.0 deploy /mnt/c/Work/OpenShift/OpenShift-on-Azure-Workshop/UI/ClientApp
> cp env.sh ./public/ && cp env-config.js ./public/ && react-scripts build

Creating an optimized production build...
Compiled successfully.

File sizes after gzip:

  76.46 KB  build/static/js/main.32b41c6e.js
  23.23 KB  build/static/css/main.71231533.css
...
```

#### verify that the build has completed; navigate to the newly-created ```build``` folder

```powershell
cd ./build/

ls
```

### set up build for Nginx

#### create a new OpenShift build for Nginx server

```powershell
oc new-build --name=nginx-build openshift/nginx:1.12 --binary=true
```

### s2i build + deployment to Nginx

#### start the newly-created OpenShift build from the ```build``` folder

```powershell
oc start-build nginx-build --from-dir=build
```

#### create a new OpenShift app to specify a deployment for Nginx

```powershell
oc new-app nginx-build --name=nginx
```

#### expose the newly-deployed Nginx app

```powershell
oc expose svc/nginx
```

### modifying environment variables to be set at run-time

#### create a Config Map

1. navigate into the OpenShift Web Console in your browser
2. select your project from the right-hand navigation menu
3. on the left-hand navigation menu, hover over the *files* icon and select ConfigMaps
4. click the **Create Config Map** button
5. assign the Config Map a **Name**
6. assign the **Key** value to: ```env-config.js```
7. click on the multi-line editor and enter the following text:

   ```js
   window._env_ = {
     API_URL: "https:<your-name>-api.com"
   }
   ```

8. select **Save**, then select **Add to Application** just above
9. select the front-end Nginx server application name (*Nginx*)
10. select **Volume** enter: ```/opt/app-root/src/config```
11. select **Save**

## final verification

1. open the link to the external route created for the Nginx server and see that the specified environment variable in the ConfigMap sits on the website

### expose Nginx app to an external route for the API

we are going to create an additional route on the Nginx server

1. copy the full external web address of the Nginx server
2. navigate to the Routes page on the OpenShift Web Console while under your project
3. select **Create a Route**
4. specify a name for the new route
5. in **Hostname**, paste the copied external web address of the Nginx server and delete the ```https://``` part at the front of the copied text
6. specify: ```/api/``` for the path
7. select the DotNet Data Service API application name in the **Service** selector
8. select **Create** at the bottom of the form

### "tying it together" + summary

[tbd]
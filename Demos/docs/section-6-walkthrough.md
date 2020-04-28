# Section 6 - Managing secrets and environment variables

## Introduction

One of the challenges that we face as developers is the need to *externalize* application configuration as this will probably differ per environment or per deployment

In OpenShift, Docker, and Kubernetes, develoeprs have been externalizing this configuration into **Environment Variables**, allowing for each deployment to have different values for runtime configuration

This is a fantastic way to adhere to the third rule of the 12Factor methodology -- *"store configurations in the environment"*

So what type of configuration can OpenShift provide to an application or deployment? typically, configuration will fall under one of three categories

1. A singular configuration value expressed as an environment variable for the container
2. Command-line arguments in a container
3. Multiple configuration values typically set in a configuration file

It is easy to set environment variables on an application.  You can do it at the time of 
creation via oc new-app, or you can even set an environment variable after an application
has already been created.

## a. Provisioning techniques

### Secrets

the ```Secret``` object type within the OpenShift Container Platform provides a mechanism to hold sensitive information such as passwords, OpenShift configuration files, ```dockercfg``` files, private source repository credentials, etc...

**secrets** allow us to decouple sensitive content from the pods; we are able to mount secrets into containers using a volume plug-in or the OpenShift system can use secrets to perform actions on behalf of a pod

typical YAML secret object definition

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: my-secret
  namespace: my-namespace
type: Opaque
data:
  username: <username>
  password: <password>
stringData:
  hostname: myapp.mydomain.com
```

* the ```type: Opaque``` indicates the structure of the secret's key names and values
* the *allowable* format for the keys in the ```data``` field must meet the guidelines set forth in the ```DNS_SUBDOMAIN``` value in the Kubernetes identifiers glossary
* the value associated with keys in the ```data``` map must be base64 encoded
* the value associated with keys in the ```stringData``` map is made up of *plain-text strings*
* entries in the ```stringData``` map are automatically converted to base64 and the entry will then be moved to the data map
  * this field is write-only -- the value will only be returned via the data field

Creating a secret using the oc cli

```bash
$ oc create secret generic sql-server-sa-password-secret --from-literal=SA_PASSWORD=Password!23
```

### Creating an environment variable that references a secret
This is important, because at runtime, our .NET apps need to get their configuration values from environment variables.
If there are secret configuration values, we can push them into an environment variable.

```bash
$ oc set env dc/workshop-api --from secret/sql-server-sa-password-secret
```


### ConfigMaps

many applications require using a combination of configuration files, command-line arguments, and environment variables

these configuration artifacts should be decoupled from the image content in order to keep containerized applications portable

the OpenShift ```ConfigMap``` object provides mechanisms to *inject* containers with configuration data while keeping containers *agnostic* of the OpenShift Container Platform

a ```ConfigMap``` can be used to store fine-grained information like individual properties or coarse-grained information like entire configuration files or JSON blobs

the ```ConfigMap``` object holds key-value pairs of configuration data that can be consumed in pods or used to store configuration data for system components (such as controllers!)

**a ```ConfigMap``` is similar to secrets, but designed to more conveniently support working with strings that do not contain sensitive data**

example ConfigMap YAML object definition:

```yaml
kind: ConfigMap
apiVersion: v1
metadata:
  creationTimestamp: <yyyy-mm-ddThh:mm:ssZ>
  name: my-config
  namespace: my-namespace
data:
  my.first.property: hello
  my.second.property: world
  my.third.property.file: |-
    file.property.1=value-1
    file.property.2=value-2
    file.property.3=value-3
```

* ```data``` holds the configuration data

## b. Referencing secrets from an environment variable

### *Secrets*

to create a secret from a local ```.docker/config.json``` file, we can use the following command:

```bash
$ oc create secret generic dockerhub \
--from-file=.dockerconfigjson=<path/to/.docker/config.json> \
--type=kubernetes.io/dockerconfigjson
```

#### Properties of secrets

key properties include:

* secret data can be referenced independently from its definition
* secret data volumes are backed by temporary file-storage facilities (tmpfs) and never sit on a node
* secret data can be shared within a namespace

#### creating secrets

you must create a secret prior to creating the pods that depend on such secret

typical steps (overview):

* create a secret object with secret data
* update the pod's service account to allow the reference to that secret
* create a pod which consumes the secret as an environment variable or as a file (secret volume)

command to create a secret object from a JSON or YAML file:

```bash
$ oc create -f <filename>
```

#### updating secrets

when you modify the value of a secret, the value in an already-running pod will not dynamically change -- you must *delete the original pod and create a new pod*

### *ConfigMaps*

#### ConfigMap re-cap

a ConfigMap can be used to:

1. populate the value of environment variables
2. set command-line arguments in a container
3. populate the configuration files in a volume

> both users and system components may store configuration data in a ConfigMap

#### creating ConfigMaps

you can use the following command to easily create a ConfigMap from directories, specific files, or literal values:

```bash
$ oc create configmap <configmap-name> [options]
```

when creating ConfigMaps from directories, we can consider the following example:

```bash
$ ls .\example-files

game.properties
ui.properties

$ cat .\example-files/game.properties

enemies=aliens
lives=3
secret.code.passphrase=this-is-a-secret
secret.code.allowed=true

$ cat .\example-files/ui.properties

color.good=purple
color.bad=yellow
allow.textmode=true

$ oc create configmap game-configuration \
    --from-file=.\example-files/
```

* the ```--from-file``` option points to a directory, each file directly in the specified directory is used to populate a key in the ConfigMap, where the name of the key is the file name and the value of that key is the content of the file

```bash
$ oc describe configmaps game-configuration

Name:           game-configuration
Namespace:      default
Labels:         <none>
Annotations:    <none>

Data

game.properties:        121 bytes
ui.properties:          83 bytes
```

here, we can see the two keys in the map created from the file names in the previously-specified directory; since the content of those keys may be large, the output of ```oc describe``` only shows the names and sizes of the keys

if you wish to see the values, we can use the ```oc get``` command:

```bash
$ oc get configmaps game-config -o yaml

apiVersion: v1
data:
  game.properties:
    <...>
  ui.properties:
    <...>
kind: ConfigMap
<...>
```

alternatively, we can create ConfigMaps by specifying literal values:

```bash
$ oc create config literal-config \
    --from-literal=literal.how=very \
    --from-literal=literal.type=charming
```

and we can see the results:

```bash
$ oc get configmaps literal-config -o yaml

apiVersion: v1
data:
  literal.how: very
  literal.type: charming
kind: ConfigMap
<...>
```

## d. Demo

> Show techniques for deploying environment variables and secrets

### Creating a single-key secret
oc create secret generic my-single-secret --from-literal=key1=supersecret

### Creating a multiple-key secret
oc create secret generic my-multiple-secret --from-literal=key1=supersecret --from-literal=key2=topsecret

### Updating a secret
oc create secret generic my-single-secret --from-literal=key1=supersecret2 --dry-run -o yaml | oc replace -f -

### Consuming secrets as environment variables

> Show how to reference a secret from an environment variable as well as pass to a container (show how the SQL Server container was deployed in the eshopOnContainers app)

oc set env dc/data-seeder --from secret/mongodb

#### Data Seeder App
dotnet publish  -c Release /p:MicrosoftNETPlatformLibrary=Microsoft.NETCore.App
oc new-build --name=data-seeder dotnet:2.2 --binary=true
oc start-build data-seeder --from-dir=bin/Release/netcoreapp2.2/publish --follow
oc new-app data-seeder
oc set env dc/data-seeder MongoDbSettings__HostName=mongodb.workshop-demo.svc.cluster.local
oc set env dc/data-seeder --from secret/mongodb

#### API App
dotnet publish Perficient.OpenShift.Workshop.API.csproj -c Release /p:MicrosoftNETPlatformLibrary=Microsoft.NETCore.App
oc new-build --name=workshop-api dotnet:2.2 --binary=true
oc start-build workshop-api --from-dir=bin/Release/netcoreapp2.2/publish --follow
oc new-app workshop-api
oc set env dc/workshop-api MongoDbSettings__HostName=mongodb.workshop-demo.svc.cluster.local
oc set env dc/workshop-api --from secret/mongodb

## e. Lab

> Configure environment variables & secrets (in the OpenShift console) and deploy the Sample App data tier

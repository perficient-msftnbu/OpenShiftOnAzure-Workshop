# perficient ARO workshop outline

## section 1: business case for OpenShift on Azure

### the market

### the challenges

### how Azure Red Hat OpenShift helps

## section 2: what is *cloud native* and why it matters

### *cloud native* & microservices

### 12-factor applications

### an overview of containers

### an overview of Kubernetes

## section 3: the OpenShift platform

### what is the Red Hat OpenShift platform & why should I use it

### what is the Azure Red Hat OpenShift platform & why should I use it

### demo: show OpenShift cluster -- eShop instance

## section 4: development basics

### OpenShift core concepts

#### applications

#### pods

#### containers

#### services

#### routes

### OpenShift web console

### OpenShift CLI tool

### deploying Docker images

### deploying .NET Core apps

### OpenShift Pod logs

### source-to-image (S2I) concepts

### demo: show the CLI, deploy a Docker image, deploy a .NET Core "Hello World" app

### lab: deploy *your own* "Hello World" app

## section 5: advanced deployment techniques

### OpenShift template types; when / how to use them

### querying for items using the CLI

### YAML deployments & ```oc apply```

### demo: deploy the SampleApp UI (Nginx-React application)

### lab: deploy *your own* Nginx container

## section 6: managing secrets & environment variables

### provisioning techniques

### referencing secrets from an environment variable

#### ConfigMaps

#### Secrets

### passing secrets to a Docker image

### demo: show techniques for deploying environment variables & secrets

### lab: configure environment variables & secrets, deploy the SampleApp data tier

## section 7: persistent storage

### persistent volumes

### persistent volume claims

### adding storage to a container

### demo: create a persistent claim, add it to the MongoDB server container application

### lab: deploy SampleApp's data tier (MongoDB container)

## section 8: services & routes

### pod inter-communication

### public routes 

> expose an app to the external web

### demo: test service connectivity from an application's terminal window

### lab: deploy services & routes so that the SampleApp communicates across tiers

## section 9: Azure DevOps

### how to do repeatable deployments

### how to deploy from Azure DevOps (or any ALM platform)

#### connecting to OpenShift

### demo: Azure DevOps build & release pipeline to OpenShift cluster

### lab: add the SampleApp to a DevOps automated build

## section 10: advanced techniques & *gotcha's*

### using template files (JSON / YAML)

### forwarding headers

### demoD: show how to build + use the YAML / JSON files
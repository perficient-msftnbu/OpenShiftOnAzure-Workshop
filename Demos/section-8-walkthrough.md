# section 8 - services & routes

## introduction

Kubernetes ensures that pods are able to network with each other, allocating each pod an IP address from an internal network

this ensures that all containers in the pod behave as if they were on the same host

by giving each pod its own IP address means that pods can be treated like physical hosts or virtual machines in terms of port allocation, networking, naming, service discovery, load balancing, application configuration, and migration

if we run multiple services, such as a front-end and back-end services for use with multiple pods, environment variables are created for user-names, serivce IPs, and more so that the front-end pods can communicate with the back-end pods

### let's provide a hypothetical example

if a service is deleted and re-created, a new IP address is assigned to the service, requiring the fornt-end pods to also be re-created in order to pick up the updated values for the service IP environment variable

additionally, the back-end service must be created before any of the front-end pods to ensure that the service IP is generated properly, and that it can be provided to the front-end pods as an environment variable

for this reason, the OpenShift Container Platform has a built-in DNS so that any service can be reached by the service DNS as well as the service IP / port

## a. pod inter-communication

> a quick re-cap on pods relative to the OpenShift Container Platform

the OpenShift Container Platform leverages the Kubernetes concept of a *pod*, which is one or more containers deployed together on one *host*

the pod is the smallest compute unit that be be defined, deployed, and managed

pods are the rough equivalent of a single machine instance (physical or virtual) to a container; each pod is assigned its own internal IP address therefore allowing its own port space -- thereby...containers within the pods can share their local storage and networking

## b. public routes – exposing an app to the internet

### a quick introduction to Services on the OpenShift Container Platform

a Kubernetes *service* serves as an internal load balancer, identifying a set of replicated pods in order to proxy the connections it receives to them

*backing* pods can be added to removed from a service arbitrarily while the service remains consistently available, enabling anything that depends on the service to refer to it as a consistent address

the default service cluster IP address are sourced from the OpenShift Container Platform internal network and they are used to permit pods to access each other

### exposing services to the external web

to permit external access to a particular service, additional ```externalIP``` and ```ingressIP``` addresses that are external (i.e. external web traffic) to the cluster

like pods, services are REST objects, as we can see from a sample object defintion:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: docker-registry
spec:
  selector:
    docker-registry: default
  clusterIP: 172.30.136.123
  ports:
  - nodePort: 0
    port: 5000
    protocol: TCP
    targetPort: 5000
```

* the service name ```docker-registry``` is also used to construct an environment variable with the service IP that is inserted into other pods of the same namespace; the maximum length is 63 characters
* the label selector identifies all pods with the ```docker-registry=default``` label attached as its *backing* pods
* the virtual IP of the service is allocated automatically at creation from a pool of internal IPs
* the service will listen on port ```5000```
* the port on the backing pods that the service forwards connections is also ```5000```

### creating a project and service

> assuming we're authenticated to the cluster with the CLI

1. create a new project for our service:

   ```bash
    $ oc new-project <project-name>
   ```

2. use the ```oc new-app``` command to create a service

   ```bash
    $ oc new-app \
        -e MYSQL_USER=admin \
        -e MYSQL_PASSWORD=redhat \
        -e MYSQL_DATABASE=mysqldb \
        registry.access.redhat.com/openshift3/mysql-55-rhel7
   ```

3. run the following command to see that the new service is created:

   ```bash
    $ oc get svc

    NAME: mysql-55-rhel7
    CLUSTER-IP: 172.30.131.89
    EXTERNAL-IP: <none>
    PORT(S): 3306/TCP
    AGE: 13m
   ```

### expose a service to create a route

1. log into the project where the service we wish to expose is located

   ```bash
       $ oc project <project-name>
   ```

2. run the following command to expose the route:

   ```bash
       $ oc expose service <service-name>

       route "<service-name>" exposed
   ```

3. make sure that we are able to reach the service using the cluster IP address for the service:

   ```bash
       $ curl <pod-ip>:<port>
   ```

## c. demo

> test service connectivity from an app’s terminal window (to another app)
> show how to create a public route

## d. lab

> deploy services and routes so that the SampleApp can talk across tiers

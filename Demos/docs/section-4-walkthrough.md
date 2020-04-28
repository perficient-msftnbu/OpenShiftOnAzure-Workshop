# section 4 - development basics

## a. using the openshift cluster portal

navigate to a project within the openshift console

in the top-right corner, select *Add to Project*

click on *Browse Catalog*

search for *.NET Core Example*, no need to configure anything, click next until it is deployed

## b. using the cli tool

```oc login openshift.201e2035bd2d44cfb97b.centralus.azmosa.io```

the command line will return a URL with which we navigate to in order to retrieve a login key that is used back in the command prompt to authenticate the local CLI connection to the cluster

paste in the updated ```oc login _____``` command back in the command prompt, hit enter and observe that the OpenShift command line tools are now connected with the cluster

## c. deploying docker images

> this is relevant for devs that have out-grown their own machines and wish to leverage the power of OpenShift using Kubernetes

assuming that we have a running OpenShift environment, we can set up a new project to use...

### let's start with the new project command

**run the following command: ```oc new-project deploy-docker-images```**

we've now moved into the project we've just created automatically

say we wish to use the *docker.io/gitlab/gitlab-ce:latest* image:

**run the following command: ```oc new-app gitlab/gitlab-ce```**

**sample log:**

```bash
* Found Docker image _____ (X days old) from Docker Hub for 'gitlab/gitlab-ce'
* An image stream will be created as 'gitlab-ce:latest' that will track this image
* This image will be deployed in deployment config 'gitlab-ce'
* [WARNING] Image 'gitlab-ce' runs as the 'root' user which may not be permitted by your cluster adminstrator
* Ports 22/tcp, 443/tcp, and 80/tcp will be load balanced by service 'gitlab-ce'
* Creating resources with label app=gitlab-ce ... ImageStream 'gitlab-ce' created
* DeploymentConfig 'gitlab-ce' created
* Service 'gitlab-ce' created
* Success
```

### there's a *lot* to unpack here

just from specifying a particular Docker Image on Docker Hub, we've created a tracked image stream, services, a build configuration, routes, and a deployment configuration

for any pod deployment, we require a build configuration and a source image...in this case, with the *gitlab-ce* image, there have additional moving parts created for us -- specifically, the definition for a pod, service, DeploymentConfig, and replication controller

**let's take a look at the status of our app with ```oc status```**

**we can also observe the running pod containing our image: ```oc get pods```**

**sample output:**

NAME

> gitlab-ce-1-kekx2

READY

> 0/1

STATUS

> CrashLoopBackOff

RESTARTS

> 4

AGE

> 2m

#### navigate to container logs

we'll see something along the lines of:

```bash
# oc logs -p gitlab-ce-1-kekx2
Thank you for using GitLab Docker Image!
Current version: gitlab-ce=8.4.3-ce.0

Configure GitLab for your system by editing /etc/gitlab/gitlab.rb file
And restart this container to reload settings.
To do it use docker exec:

docker exec -it gitlab vim /etc/gitlab/gitlab.rb
docker restart gitlab

For a comprehensive list of configuration options please see the Omnibus GitLab readme
https://gitlab.com/gitlab-org/omnibus-gitlab/blob/master/README.md

If this container fails to start due to permission problems try to fix it by executing:

docker exec -it gitlab update-permissions
docker restart gitlab

Generating ssh_host_rsa_key...
No user exists for uid 1000530000
```

by default, all containers that we launch from within OpenShift are blocked from "RunAsAny" which means that they are not allowed to use a root user within the container environment -- this prevents root actions such as ```chown``` or ```chmod``` from being run and is a sensible default security precaution as should a user be able to perform a local exploit to break out of the container

we've determined that the failure was due to the ```gitlab-ce``` image attempting to run as root and perform a ```chown``` on the filesystems mounted into the container

### how to fix this

we ideally, we could fix the original Docker image to not run as root -- however, this may not be possible in the case of certain Docker images

we can tell OpenShift to allow this project to run as root using the below command to change the security context constraints: ```oc adm policy add-scc-to-user anyuid -z default```

with this command, we are relaxing a default setting which would normally prohibit containers running as root; the ```-z``` in the command indicates that we are going to add a capability in the service account and the ```anyuid``` parameter adds the "run as any user" capability, i.e. we are no longer prohibited from running as root

**let's verify the policy that we've just set using: ```oc edit scc anyuid```**

**let's now try and re-run the deployment of the *gitlab-ce* image: ```oc deploy gitlab-ce --latest```**

**now, we can cURL the URL of gitlab: ```curl http:/[the IP address]```**

## d. deploying .NET Core applications

```oc new-app --name=exampleapp dotnet:2.1~https://github.com/redhat-developer/s2i-dotnetcore-ex#dotnetcore-2.1 --build-env DOTNET_STARTUP_PROJECT=app```

## e. OpenShift Pod Logs

> self-explanatory

## f. Source-to-Image (s2i) concepts

Source-to_Image (S2I) is a tool used for building 'reproducible' Docker images

S2I produces 'ready-to-run' images by injecting application source into a Docker image and assembling it, incorporating the base Docker image (the builder) and the application source in order to ready the image for the ```docker run``` command

### advantages of S2I

* image flexibility
  * S2I scripts can be written to inject application code into almost any existing Docker image, taking advantage of the existing ecosystem
  * note that, currently, S2I relies on tar to inject application source, so the image needs to be able to process 'tarred' content
* speed
  * with S2I, the assemble process can perform a large number of complex operations without creating a new layer at each step, resulting in a fast process
  * in addition, S2I scripts can be written to re-use artifacts stored in a previous version of the application image, rather than having to download or build them each time the build is run
* patchability
  * S2I allows you to rebuild the application consistently if an underlying image needs a patch due to a security issue
* operational efficiency
  * by restricting build operations instead of allowing arbitrary actions, as a *Dockerfile* would allow, the PaaS operator can avoid accidental or intentional abuses of the build system
* operational security
  * building an arbitrary *Dockerfile* exposes the host system to root privilege escalation. This can be exploited by a malicious user because the entire Docker build process is run as a user with Docker privileges. S2I restricts the operations performed as a root user and can run the scripts as a non-root user
* user efficiency
  * S2I prevents developers from performing arbitrary ```yum install``` type operations, which could slow down development iteration, during their application build
* ecosystem
  * S2I encourages a shared ecosystem of images where you can leverage best practices for your applications

## g. demo: deploy a .NET Core "Hello World" app

### navigate to a working directory to store upcoming dotnet project + assets

**run the following command: ```dotnet new webapp```**

now, we publish the app:

```dotnet publish [project name] --output "publish\[project name]\MicrosoftNETPlatformLibrary=Microsoft.NETCore.App" --configuration Release```

now, we create a build configuration within OpenShift:

```oc new-build --name=workshop-demo-build dotnet:2.2 --binary=true```

#### navigate to the OpenShift console to see the newly-created build configuration

we want to initate a build from the published dotnet artifacts:

```oc start-build workshop-demo-build --from-dir=bin/Release/netcoreapp2.2/publish```

#### navigate to the OpenShift console to observe the newly-started build

#### once the build is finished, navigate to the *Builds --> Images* web console location in the left nav-menu

> we have now created an image based on the published dotnet binaries

we now want to create new OpenShift 'app' that is the formal deployment of our recently completed build

run the following command: ```oc new-app workshop-demo-build --name=workshop-demo```

#### navigate to the *Applications --> Deployments* to observe the pod that was spun-up to hold the .NET application

now, we can test the working dotnet web app

#### navigate to *Applications --> Services*, select the ```workshop-demo``` dotnet app and copy the ```Hostname``` local URL

#### then, navigate to the *Applications --> Pods* tab to select the running deployment of ```workshop-demo```

#### click on the 'Terminal' tab in the middle of UI

#### enter ```curl workshop-demo.workshop-demo.svc.cluster.local:8080``` in to the terminal and see that ```"Hello World"``` returns

review additional information within the particular pod page on the OpenShift web console

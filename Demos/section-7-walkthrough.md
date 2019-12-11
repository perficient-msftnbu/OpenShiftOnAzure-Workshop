# section 7 - persistent storage

## introduction

managing storage is a *distinct* problem from managing compute resources

the OpenShift Container Platofrm uses the Kubernetes persistent volume (PV) framework to allow cluster administrators to provision persistent storage for a cluster

developers can use persistent volume claims (PVCs) to request resources without having specific knowledge of the underlying storage infrastructure

in short, PVCs are not specific to a project and are created and used by developers as a means to use a PV

PV resources on their own are not scoped to any single project -- instead they are shared across the entire OpenShift Container Platform cluster and claimed from any project

PVs are resources in the cluster while PVCs are requests for those resources and also act as claim checks to the resource -- the interactions between PVs and PVCs follow this lifecycle

## a. persistent volumes

PVs are defined by a ```PersistentVolume``` API object, which represents a piece of existing, networked storage in the cluster that was provisioned by the cluster administrator

the PV is a resource in the cluster just like a node is a cluster resource

while PVs are volume plug-ins like ```Volumes```, they have a lifecycle independent of any individual pod that uses the PV

### PV specificities

each PV contains a ```spec``` and a ```status```, which is the specification and status of the volume, for example (in the object defintion):

```yaml
apiVersion: v1
kind: PersistentVolume
metadata:
  name: pv-0001
spec:
  capacity:
    storage: 5Gi
  accessModes:
  - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  <...>
status:
<...>
```

## b. persistent volume claims

PVCs are defined by a ```PersistentVolumeClaim``` API object, which represents a request for storage by a developer

it is similar to a pod in that pods consume node resources and PVCs consume PV resources

for example, pods can request specific levels of resources, such as CPU and memory, while PVCs can request specific storage capacity and access modes -- they can be mounted once ```read-write``` or many times ```read-only```

### PVC specificities

each PVC, like a PV, contain a ```spec``` and a ```status```

see the sample YAML object definition:

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: myClaim
spec:
  accessModes:
  - ReadWriteOnce
  resources:
    requests:
      storage: 5Gi
  storageClassName: gold
status:
<..>
```

* claims can optionally request a specific storage class by specifying the storage class' name in the ```storageClassName``` attribute
* only PVs of the requested class, ones with the *same* ```storageClassName``` as the PVC, can be bound to the PVC

## c. adding storage to a container

### prerequisites

before we can expand persistent volumes, the ```StorageClass``` must have the ```allowVolumeExpansion``` field set to ```true```

edit the ```StorageClass``` and add the ```allowVolumeExpansion``` attribute

* the following example demonstrates adding this line at the bottom of the ```StorageClass'``` configuration

  ```yaml
  apiVersion: storage.k8s.io/v1
  kind: StorageClass
  <...>
  parameters:
   type: gp2
  reclaimPolicy: Delete
  allowVolumeExpansion: true
  ```

### expanding PVCs with a file system

expanding PVCs based on volume types that need file system resizing is a two-step process; this process involves:

1. expanding volume objects in the cloud provider
2. expanding the file system on the actual node

> note: prior to expanding PVCs, the controlling ```StorageClass``` must have ```allowVolumeExpansion``` set to ```true```

edit the PVC and request a new size by editing ```spec.resources.requests``` in the YAML configuration

for example, the following configuration expands the ```ebs``` PVC to 8Gi

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: ebs
spec:
  storageClass: "storageClassWithFlagSet"
  accessModes:
  - ReadWriteOnce
  resources:
    requests:
      storage: 8Gi
```

...once the cloud provider object has finished resizing, the PVC is set to ```FileSystemResizePending```

the following command is used to check the condition of a particular PVC:

```bash
$ oc describe pvc <pvc-name>
```

when the cloud provider has finished resizing, the PVC object will reflect the newly-requested size in ```PersistentVolume.Spec.Capacity```

at this point, you can create or re-create a new pod from the PVC to finish the file system resizing process

> note: once the newly-sized pod is running, the ```FileSystemResizePending``` condition is removed from the PVC

## d. demo

> creating a persistent claim, adding it to a MongoDB Server container app

## e. lab

> deploy SampleApp (data tier) Mongo DB container

#!/bin/bash

for ((c=1; c<=5; c++))
do
    projectName="test-script-project-$c"
    userName="ARO_WSUser$c@msftnbu.com"

    az ad user create --display-name $userName --password Perfic3ntD3m0 --user-principal-name $userName

    oc new-project $projectName
    oc adm policy add-role-to-user edit $userName -n $projectName
done

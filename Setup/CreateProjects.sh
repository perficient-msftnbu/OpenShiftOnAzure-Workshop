#!/bin/bash

shouldDelete=0
while [ "$1" != "" ]; do
    case $1 in
        -d | --delete )         shouldDelete=1
                                ;;
    
        -p=* | --projectName=* ) 
           baseProjectName="${1#*=}"
          
           ;;
        -u=* | --userName=* )       
            baseUserName=${1#*=}
            
            ;;
        -e=* | --emailDomain=* )
            baseDomain=${1#*=}
            
            ;;
        -n=* | --numberOfProjects=* ) 
            stringProjs=${1#*=} 
            numberOfProjects=$(($stringProjs + 0)) 
           
            ;;
    esac
    shift
done




if (( $shouldDelete == 0 ));
then
    for ((c=1; c<=$numberOfProjects; c++))
    do
        projectName="$baseProjectName$c"
        userName="$baseUserName$c$baseDomain"

        az ad user create --display-name $userName --password Perfic3ntD3m0 --user-principal-name $userName

        oc new-project $projectName
        oc adm policy add-role-to-user edit $userName -n $projectName
    done
else
 for ((c=9; c<=$numberOfProjects; c++))
    do
        projectName="$baseProjectName$c"
        userName="$baseUserName$c$baseDomain"

        az ad user delete --upn-or-object-id $userName

        oc delete project $projectName
   
    done
fi
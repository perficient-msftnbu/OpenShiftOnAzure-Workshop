Param(
	[Parameter(Mandatory=$true)]
	[int] $Delete,
	[Parameter(Mandatory=$true)]
	[string] $BaseProjectName,
	[Parameter(Mandatory=$true)]
	[string] $BaseUsername,
	[Parameter(Mandatory=$true)]
	[string] $EmailDomain,
	[Parameter(Mandatory=$true)]
	[int] $NumberOfProjects,
	[Parameter(Mandatory=$true)]
	[string] $Password
)

if ($Delete -eq 1)
{
	Write-Host "Deleting projects and users..."
	for ($i = 1; $i -le $NumberOfProjects; $i++)
	{
		$projectName="$BaseProjectName$i"
		$userName="$BaseUserName$i$EmailDomain"
		oc delete project $projectName
		az ad user delete --id $userName
	}
}
else
{
	Write-Host "Creating projects and users..."
	for ($i = 1; $i -le $NumberOfProjects; $i++)
	{
		$projectName="$BaseProjectName$i"
		$userName="$BaseUserName$i$EmailDomain"
        Write-Host "Username:  $userName"
		az ad user create --display-name $userName --password $Password --user-principal-name $userName
		oc new-project $projectName
		oc adm policy add-role-to-user edit $userName -n $projectName
	}
}

<#
if (( $shouldDelete == 0 ));
then
    for ((c=1; c<=$numberOfProjects; c++))
    do
        projectName="$baseProjectName$c"
        userName="$baseUserName$c$baseDomain"

        az ad user create --display-name $userName --password $password --user-principal-name $userName

        oc new-project $projectName
        oc adm policy add-role-to-user edit $userName -n $projectName
    done
else
 for ((c=1; c<=$numberOfProjects; c++))
    do
        projectName="$baseProjectName$c"
        userName="$baseUserName$c$baseDomain"

        az ad user delete --upn-or-object-id $userName

        oc delete project $projectName
   
    done
fi
#>
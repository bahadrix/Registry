include .env

pack:
	dotnet pack --configuration Release

push: 
	@test $${FILE_PATH?Please set FILE_PATH}
	dotnet nuget push ${FILE_PATH} --api-key ${PACKAGE_API_KEY} --source https://api.nuget.org/v3/index.json
	

include .env

clean:
	rm -rf bin/Release

pack: clean
	dotnet pack --configuration Release

push: 
	@test $${FILE_PATH?Please set package path with FILE_PATH}
	dotnet nuget push ${FILE_PATH} --api-key ${PACKAGE_API_KEY} --source https://api.nuget.org/v3/index.json
	

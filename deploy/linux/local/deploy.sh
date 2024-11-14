#!/bin/bash

#The app runs using a linux system service called kvitta.service under /etc/systemd/system/ 
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu#create-the-service-file

echo "Stopping kvitta service"
sudo systemctl stop kvitta.service

echo

echo "Deploying application"
echo
dotnet publish -c Release --property:PublishDir=/home/apps/kvitta/ --os linux
echo

echo

#CURRENT_DIR=$(pwd)
#echo "Current directory: $CURRENT_DIR"
dotnet-ef migrations bundle -p ./Infrastructure  --self-contained -r linux-x64


if [ $? -eq 1 ]; then
    echo
    echo "EF Core migration failed"
    exit 1
fi

./efbundle

if [ $? -eq 0 ]; then
    sudo systemctl start kvitta.service
    else 
      echo
      echo "Failed to start kvitta.service"
fi

rm ./efbundle
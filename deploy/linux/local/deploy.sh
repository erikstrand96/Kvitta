#!/bin/bash

#The app runs using a linux system service under /etc/systemd/system/kvitta.service 
# https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-8.0&tabs=linux-ubuntu#create-the-service-file

echo "Stopping kvitta service"
sudo systemctl stop kvitta.service

echo

echo "Deploying application"
echo
dotnet publish -c Release --property:PublishDir=/home/apps/kvitta/ --os linux

echo
sudo systemctl start kvitta.service
sudo systemctl status kvitta.service

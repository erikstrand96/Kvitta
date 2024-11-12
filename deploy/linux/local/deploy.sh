#!/bin/bash

#The app runs using a linux system service under /etc/systemd/system/kvitta.service 

echo "Stopping kvitta service"
sudo systemctl stop kvitta.service

echo "Deploying application"
dotnet publish -c Release -o /home/apps/kvitta/ --os linux

sudo systemctl start kvitta.service
sudo systemctl status kvitta.service
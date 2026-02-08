#!/bin/bash

cd /home/nick/Documents/GitHub/shsdp

echo "Resetting local changes..."
git reset --hard HEAD

echo "Pulling latest code..."
git pull

echo "Publishing solution..."
dotnet publish SHSDP/SHSDP.csproj -c Release
dotnet publish SHSDP.API/SHSDP.API.csproj -c Release

echo "Stopping services..."
sudo systemctl stop shsdpapp
sudo systemctl stop shsdpapi

echo "Deploying UI (shsdpapp)..."
sudo rm -rf /opt/shsdp
sudo mkdir -p /opt/shsdp
sudo cp -r ./SHSDP/bin/Release/net10.0/publish/* /opt/shsdp

echo "Deploying API (shsdpapi)..."
sudo rm -rf /opt/shsdpapi
sudo mkdir -p /opt/shsdpapi
sudo cp -r ./SHSDP.API/bin/Release/net10.0/publish/* /opt/shsdpapi

echo "Starting services..."
sudo systemctl start shsdpapp
sudo systemctl start shsdpapi

echo "Redeployment complete."

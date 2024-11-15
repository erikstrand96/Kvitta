#!/bin/bash

docker compose down

docker build -t kvitta:latest .

docker compose up -d 

echo

MAX_RETRIES=5  
DELAY=1   

command="curl  http://localhost:8080/hello"

for ((i=1; i<=MAX_RETRIES; i++)); do

    $command && break  # If the command succeeds, exit the loop
    
    echo "Attempt $i/$MAX_RETRIES failed. Retrying in $DELAY seconds..."
    sleep $DELAY
    
done

if (( i > MAX_RETRIES )); then
    echo "Command failed after $MAX_RETRIES attempts."
    echo
    docker logs kvitta-api
    exit 1
else
    echo "Docker deployment succeeded."
fi


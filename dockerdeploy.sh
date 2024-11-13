#!/bin/bash

docker compose down

docker build -t kvitta:latest .

docker compose up -d 

echo

MAX_RETRIES=5  # Number of retry attempts
DELAY=1        # Delay in seconds between attempts

# Command to retry
command="curl  http://localhost:8080/hello"

# Retry loop
for ((i=1; i<=MAX_RETRIES; i++)); do
    # Run the command
    $command && break  # If the command succeeds, exit the loop
    
    echo "Attempt $i/$MAX_RETRIES failed. Retrying in $DELAY seconds..."
    sleep $DELAY
done

# Check final status
if (( i > MAX_RETRIES )); then
    echo "Command failed after $MAX_RETRIES attempts."
    docker logs kvitta-api
    exit 1
else
    echo "Docker deployment succeeded."
fi


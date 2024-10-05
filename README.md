**Tech Stack**</br>
.NET 8 </br>
PostgreSQL

**Local Development**</br>

Create a .env-file in project root that contains the following environment variables:

    POSTGRES_PASSWORD=my-password
    POSTGRES_DATABASE=my-database

Run setx KVITTA-DB-CONNECTION 'my-connection-string'  to set the environment variable for the connection string pointing to your development database.

    Example: 
    setx  KVITTA-DB-CONNECTION Host=host;Port=port;Database=database;Username=username;Password=password

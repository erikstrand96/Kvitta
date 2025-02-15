[![CI](https://github.com/erikstrand96/Kvitta/actions/workflows/CI.yml/badge.svg?branch=master)](https://github.com/erikstrand96/Kvitta/actions/workflows/CI.yml)&nbsp;

**Tech Stack**</br>
.NET 8 </br>
PostgreSQL

**Local Development**</br>

Create a .env-file in project root that contains the following environment variables:

    POSTGRES_PASSWORD=my-password
    POSTGRES_DATABASE=my-database

Set the environment variable for the connection string pointing to your development database.

    Windows: 
    setx  KvittaDbConnection "Host=localhost;Port=port;Database=database;Username=username;Password=password"
    REMARK: This will persist the variable across terminal sessions, but ony for the current user

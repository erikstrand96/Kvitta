[![CI](https://github.com/erikstrand96/Kvitta/actions/workflows/CI.yml/badge.svg?branch=master)](https://github.com/erikstrand96/Kvitta/actions/workflows/CI.yml)&nbsp;

> **Note:** This project is intended solely for learning and development purposes and is not production-ready.

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
    REMARK: This will persist the variable across terminal sessions, but only for the current user

**Kubernetes Deployment**</br>

Kubernetes manifests are available in `deploy/kubernetes` and are managed through Kustomize.

The deployment creates:

- Namespace: `kvitta`
- API deployment and service (`api-deploy`, `api-svc` on port 8080)
- PostgreSQL deployment and service (`db-deploy`, `db-svc` on port 5432)
- Persistent volume claim for PostgreSQL data (`db-pvc`)
- Kubernetes secret containing database credentials (`kvitta-secret`)

**1. Configure Secrets & Storage**

Before deploying, update `deploy/kubernetes/secret.yaml` with your database password (ensuring the password matches across both keys):

```yaml
stringData:
  POSTGRES_PASSWORD: "your-password"
  KvittaDbConnection: "Host=db-svc;Port=5432;Database=kvitta;Username=postgres;Password=your-password"
```

*Note: If your cluster requires an explicit StorageClass, uncomment and specify `storageClassName` in `deploy/kubernetes/db-pvc.yaml`.*

**2. Deploy**

Apply all resources using Kustomize:

```bash
kubectl apply -k deploy/kubernetes
```

**3. Verify Status**

Check that pods, deployments, and services in the `kvitta` namespace are running and healthy:

```bash
kubectl get all -n kvitta
```

**4. Access the API**

Port-forward the API service to access it locally on port 8080:

```bash
kubectl port-forward -n kvitta svc/api-svc 8080:8080
```

The API will then be available at `http://localhost:8080`.

**5. Teardown**

To tear down all resources and remove the `kvitta` namespace:

```bash
kubectl delete -k deploy/kubernetes
```

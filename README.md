# JirafeAPI

API .NET pour la gestion du backend du projet Jirafe

## Prerequis systeme

- .NET SDK `10.0.x`
- Outil EF Core CLI `dotnet-ef` en version `10.0.8`
- PostgreSQL `17`
- Docker + Docker Compose (si déploiement postgreSQL via conteneur)

## Installation et configuration

1. Cloner le projet puis se placer a la racine :

```powershell
git clone https://github.com/LucasAstley/JirafeAPI.git

cd JirafeAPI
```

2. Restaurer les dependances :

```powershell
dotnet restore
```

3. Configurer les variables d'environnement dans `.env` (a la racine).

```powershell
cp .env.example .env
```

Exemple minimal PostgreSQL :

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_PROVIDER=PostgreSQL
POSTGRESQL_CONNECTION_STRING=Host=localhost;Port=5432;Database=jirafe_prod;Username=jirafe_user;Password=your_password
JWT_SECRET=your_very_secure_secret_key_change_this_in_production_at_least_32_characters_long
JWT_ISSUER=JirafeAPI
JWT_AUDIENCE=JirafeClient
JWT_EXPIRATION_MINUTES=15
CORS_ALLOWED_ORIGINS=http://localhost:5173,http://localhost:3000
API_PORT=5000
API_HOST=localhost
```

4. (Option recommandée) Demarrer PostgreSQL + pgAdmin via Docker :

```powershell
docker compose --env-file infra-prod/postgres/.env -f infra-prod/postgres/docker-compose.yml up -d
```

Services par defaut :
- PostgreSQL : `localhost:5432`
- pgAdmin : `http://localhost:5050`

## Commandes migrations EF Core

Verifier les migrations existantes :

```powershell
dotnet ef migrations list
```

Appliquer les migrations sur la base configuree dans `.env` :

```powershell
dotnet ef database update
```

Creer une nouvelle migration :

```powershell
dotnet ef migrations add NomDeLaMigration
```

Supprimer la derniere migration non appliquee :

```powershell
dotnet ef migrations remove
```

## Procedures de lancement

### Option A - Developpement avec SQLite

Mettre dans `.env` :

```env
ASPNETCORE_ENVIRONMENT=Development
DATABASE_PROVIDER=Sqlite
SQLITE_CONNECTION_STRING=Data Source=jirafe.db
```

Puis lancer :

```powershell
dotnet run
```

### Option B - Production avec PostgreSQL

1. Demarrer la stack DB :

```powershell
docker compose --env-file infra-prod/postgres/.env -f infra-prod/postgres/docker-compose.yml up -d
```

2. Verifier `.env` API :
- `DATABASE_PROVIDER=PostgreSQL`
- `POSTGRESQL_CONNECTION_STRING=...`

3. Appliquer les migrations :

```powershell
dotnet ef database update
```

4. Lancer l'API :

```powershell
dotnet run
```

## URLs utiles

- Swagger UI : `http://localhost:5000/swagger`
- Health check : `http://localhost:5000/health`
- pgAdmin : `http://localhost:5050` (si docker compose utilisé)

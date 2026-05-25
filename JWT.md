# Documentation JWT - JirafeAPI

## 1) Sur quoi repose le système

Le système d'authentification repose sur :

- Un **Access Token JWT** (courte durée) signé en `HS256`
- Un **Refresh Token** aléatoire stocké en base
- La validation automatique par le middleware `JwtBearer` de ASP.NET Core

## 2) Variables d'environnement utilisées

Le JWT est configuré par ces variables dans `.env` :

- `JWT_SECRET` : clé secrète de signature
- `JWT_ISSUER` : émetteur attendu
- `JWT_AUDIENCE` : audience attendue
- `JWT_EXPIRATION_MINUTES` : durée de vie de l'access token

Ces valeurs sont lues dans `Program.cs` puis injectées dans `JwtHelper`

## 3) Contenu du JWT

Le token est généré dans `JwtHelper.GenerateAccessToken(...)` avec les claims :

- `NameIdentifier` : Identifiant utilisateur
- `Name` : Username
- `Email` : Email
- `Role` : Rôle (ex : `User`)

Algorithme de signature : `HmacSha256` avec la clé `JWT_SECRET`

## 4) Validation du JWT côté API

La validation est activée dans `Program.cs` via :

- `builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`
- Vérification de :
  - Signature (`ValidateIssuerSigningKey`)
  - Issuer
  - Audience
  - Expiration (`ValidateLifetime`)
- `ClockSkew = TimeSpan.Zero` (pas de tolérance supplémentaire)

Pour SignalR (`/hubs/board`), le token peut être lu dans la query string `access_token` via `OnMessageReceived`

## 5) Cycle d'authentification (endpoints)

Endpoints dans `AuthController` :

- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/refresh`
- `POST /api/auth/logout`

### Register / Login

`AuthService` :

1. Vérifie l'utilisateur
2. Génère un access token JWT
3. Génère un refresh token aléatoire (32 bytes, Base64)
4. Stocke le refresh token en base (`RefreshTokens`) avec :
   - `ExpiresAt = UtcNow + 7 jours`
   - `IsRevoked = false`
5. Retourne :
   - `AccessToken`
   - `RefreshToken`
   - Infos utilisateur

### Refresh

`AuthService.RefreshTokenAsync` :

1. Vérifie que le refresh token existe, n'est pas révoqué et n'est pas expiré
2. Régénère un nouvel access token
3. Révoque l'ancien refresh token (`IsRevoked = true`)
4. Crée un nouveau refresh token

=> Mécanisme de **rotation des refresh tokens**

### Logout

`AuthService.LogoutAsync(userId)` révoque tous les refresh tokens actifs de l'utilisateur

## 6) Appel des routes protégées

Le frontend doit envoyer :

```http
Authorization: Bearer <access_token>
```

Les controllers/procédures protégés sont marqués avec `[Authorize]`

# MLD

* **Users** (**Id_User**, Username, Email, PasswordHash, Role, CreatedAt)
* **RefreshTokens** (**Id_RefreshToken**, Token, ExpiresAt, CreatedAt, IsRevoked, `#Id_User`)
* **Workspaces** (**Id_Workspace**, Name, Description, CreatedAt)
* **WorkspaceMembers** (**Id_WorkspaceMember**, `#Id_Workspace`, `#Id_User`, Role, JoinedAt)
* **Boards** (**Id_Board**, Title, CreatedAt, `#Id_Workspace`)
* **BoardMembers** (**Id_BoardMember**, `#Id_Board`, `#Id_User`, JoinedAt)
* **Lists** (**Id_List**, Title, Position, CreatedAt, `#Id_Board`)
* **Cards** (**Id_Card**, Title, Description, DueDate, Position, CreatedAt, `#Id_List`)
* **CardMembers** (**Id_CardMember**, `#Id_Card`, `#Id_User`)
* **Labels** (**Id_Label**, Name, ColorHex, `#Id_Board`)
* **CardLabels** (**Id_CardLabel**, `#Id_Card`, `#Id_Label`)
* **Comments** (**Id_Comment**, Content, CreatedAt, UpdatedAt, `#Id_Card`, `#Id_User`)

# Détail des tables

#### `Users`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire performante pour les jointures. |
| `Username` | VARCHAR(50) | - | NOT NULL, UNIQUE | Limité à 50 caractères. Indexé pour les recherches. |
| `Email` | VARCHAR(255) | - | NOT NULL, UNIQUE | Format standard. Indexé pour accélérer l'authentification. |
| `PasswordHash` | VARCHAR(255) | - | NOT NULL | Stockage sécurisé du mot de passe hashé. |
| `Role` | VARCHAR(20) | - | NOT NULL | Rôle global (ex: "Admin", "User") pour le middleware d'autorisation. |
| `CreatedAt` | DATETIME | - | NOT NULL | Date d'inscription de l'utilisateur. |

#### `RefreshTokens`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire. |
| `Token` | VARCHAR(255) | - | NOT NULL, UNIQUE | Jeton unique indexé pour sécuriser le renouvellement d'Access Token. |
| `ExpiresAt` | DATETIME | - | NOT NULL | Date limite de validité du Refresh Token. |
| `IsRevoked` | BOOLEAN | - | NOT NULL (Default: False) | Permet d'invalider une session instantanément si nécessaire. |
| `UserId` | INT | **FK** | NOT NULL, Référence `Users(Id)` | Suppression en cascade (`Cascade Delete`) si le compte utilisateur est supprimé. |

#### `Workspaces`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire de l'espace de travail. |
| `Name` | VARCHAR(100) | - | NOT NULL | Nom de l'organisation ou de l'équipe. Max 100 caractères. |
| `Description` | VARCHAR(500) | - | NULL | Optionnel. Description textuelle courte de l'espace. |
| `CreatedAt` | DATETIME | - | NOT NULL | Date de création du workspace. |

#### `WorkspaceMembers`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Table d'association (Many-to-Many) entre Users et Workspaces. |
| `WorkspaceId` | INT | **FK** | NOT NULL, Référence `Workspaces(Id)` | Clé étrangère liée au workspace (`Cascade Delete`). |
| `UserId` | INT | **FK** | NOT NULL, Référence `Users(Id)` | Clé étrangère liée à l'utilisateur (`Cascade Delete`). |
| `Role` | VARCHAR(20) | - | NOT NULL | Rôle au sein du Workspace (ex: "Owner", "Admin", "Collaborator"). |
| `JoinedAt` | DATETIME | - | NOT NULL | Date d'intégration du membre dans l'espace. |

#### `Boards`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire du tableau. |
| `Title` | VARCHAR(100) | - | NOT NULL | Titre du projet / thématique. Max 100 caractères. |
| `CreatedAt` | DATETIME | - | NOT NULL | Date de création du tableau. |
| `WorkspaceId` | INT | **FK** | NOT NULL, Référence `Workspaces(Id)` | Un tableau est obligatoirement rattaché à un Workspace (`Cascade Delete`). |

#### `BoardMembers`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Table d'association (Many-to-Many) pour les accès aux tableaux. |
| `BoardId` | INT | **FK** | NOT NULL, Référence `Boards(Id)` | Indexé pour charger instantanément les membres lors de l'ouverture du tableau. |
| `UserId` | INT | **FK** | NOT NULL, Référence `Users(Id)` | Clé étrangère liée à l'utilisateur invité. |
| `JoinedAt` | DATETIME | - | NOT NULL | Date d'affectation au tableau. |

#### `Lists`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire de la colonne. |
| `Title` | VARCHAR(100) | - | NOT NULL | Nom de l'étape du workflow (ex: "À faire", "En cours"). |
| `Position` | INT | - | NOT NULL | **Optimisation Drag & Drop :** Indexé pour trier rapidement les colonnes (`ORDER BY Position`). |
| `CreatedAt` | DATETIME | - | NOT NULL | Date de création de la liste. |
| `BoardId` | INT | **FK** | NOT NULL, Référence `Boards(Id)` | Liaison au tableau. La suppression du tableau détruit ses listes. |

#### `Cards`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire de la tâche. |
| `Title` | VARCHAR(150) | - | NOT NULL | **Optimisation :** Limité à 150 caractères maximum pour alléger l'index. |
| `Description` | TEXT | - | NULL | Type `TEXT` pour stocker des notes riches ou du Markdown sans limite. |
| `DueDate` | DATETIME | - | NULL | Date d'échéance. Indexée pour le filtrage et la vue calendrier. |
| `Position` | INT | - | NOT NULL | **Optimisation Drag & Drop :** Géré via un **index composite** `(ListId, Position)`. |
| `CreatedAt` | DATETIME | - | NOT NULL | Date de création de la carte. |
| `ListId` | INT | **FK** | NOT NULL, Référence `Lists(Id)` | Liaison à la colonne active. Migre lors du changement de colonne. |

#### `CardMembers`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Table d'association Many-to-Many pour l'assignation des tâches. |
| `CardId` | INT | **FK** | NOT NULL, Référence `Cards(Id)` | Clé étrangère liée à la carte (`Cascade Delete`). |
| `UserId` | INT | **FK** | NOT NULL, Référence `Users(Id)` | Clé étrangère liée au membre assigné (`Cascade Delete`). |

#### `Labels`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire du label. |
| `Name` | VARCHAR(50) | - | NOT NULL | Nom de la catégorie (ex: "Bug", "Urgent"). Max 50 caractères. |
| `ColorHex` | VARCHAR(7) | - | NOT NULL | Code couleur hexadécimal strict (ex: `#FF5733`). Stockage très léger. |
| `BoardId` | INT | **FK** | NOT NULL, Référence `Boards(Id)` | Les étiquettes sont cloisonnées par tableau (`Cascade Delete`). |

#### `CardLabels`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Table d'association (Many-to-Many) entre Cards et Labels. |
| `CardId` | INT | **FK** | NOT NULL, Référence `Cards(Id)` | Clé étrangère liée à la carte. |
| `LabelId` | INT | **FK** | NOT NULL, Référence `Labels(Id)` | Clé étrangère liée au label appliqué. |

#### `Comments`
| Colonne | Type SQL | Clé | Contraintes / Propriétés | Objectif & Optimisation |
| :--- | :--- | :--- | :--- | :--- |
| `Id` | INT | **PK** | Auto-incrémenté, NOT NULL | Clé primaire du commentaire. |
| `Content` | TEXT | - | NOT NULL | Corps du message rédigé par le collaborateur. |
| `CreatedAt` | DATETIME | - | NOT NULL | Date d'envoi du message. |
| `UpdatedAt` | DATETIME | - | NULL | Renseigné uniquement si le commentaire a subi une édition. |
| `CardId` | INT | **FK** | NOT NULL, Référence `Cards(Id)` | Lié à la tâche (`Cascade Delete`). Si la carte est supprimée, les échanges s'effacent. |
| `UserId` | INT | **FK** | NULL | **Optimisation Rétention :** Configuré en `Set Null` ou `Restrict`. Si un compte utilisateur est supprimé, le contenu de son commentaire reste lisible (Auteur anonymisé). |

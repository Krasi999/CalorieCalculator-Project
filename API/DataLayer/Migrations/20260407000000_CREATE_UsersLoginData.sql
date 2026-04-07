CREATE TABLE IF NOT EXISTS "Users" (

    "Id"                  INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "Email"               VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash"        VARCHAR(512) NOT NULL,
    "ActivationDate"      TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastPasswordLogin"   TIMESTAMP    NULL,
    "IsBiometricEnabled"  BOOLEAN      NOT NULL DEFAULT FALSE,
    "IsActive"            BOOLEAN      NOT NULL DEFAULT TRUE,
    "CreatedAt"           TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"           TIMESTAMP    NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "User_Id" ON "Users" ("Id");
	
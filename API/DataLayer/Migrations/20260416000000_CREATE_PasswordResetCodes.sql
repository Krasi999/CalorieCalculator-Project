CREATE TABLE IF NOT EXISTS "PasswordResetCodes" (
    "Id"         UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "UserId"     UUID         NOT NULL,
    "Code"       VARCHAR(6)   NOT NULL,
    "ExpiresAt"  TIMESTAMP    NOT NULL,
    "IsUsed"     BOOLEAN      NOT NULL DEFAULT FALSE,
    "CreatedAt"  TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "FK_PasswordResetCodes_Users"
        FOREIGN KEY ("UserId")
        REFERENCES "Users"("Id")
        ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_PasswordResetCodes_UserId"
    ON "PasswordResetCodes"("UserId");
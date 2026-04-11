CREATE TABLE IF NOT EXISTS "UserDetails" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "UserId"        UUID         NOT NULL,
    "Nickname"      VARCHAR(50)  NOT NULL,
    "Gender"        SMALLINT     NOT NULL CHECK ("Gender" IN (1, 2)),
    "DateOfBirth"   DATE         NOT NULL,
    "HeightCm"      DECIMAL(5,2) NULL,
    "HeightFt"      DECIMAL(5,2) NULL,
    "WeightKg"      DECIMAL(5,2) NULL,
    "WeightLbs"     DECIMAL(5,2) NULL,
    "ActivityLevel" SMALLINT     NOT NULL DEFAULT 1
                                 CHECK ("ActivityLevel" BETWEEN 1 AND 5),
    "CreatedAt"     TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"     TIMESTAMP    NULL,

    CONSTRAINT "FK_UserDetails_Users"
        FOREIGN KEY ("UserId")
        REFERENCES "Users"("Id")
        ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "UX_UserDetails_UserId"
    ON "UserDetails"("UserId");

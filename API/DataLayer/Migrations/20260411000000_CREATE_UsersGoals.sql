CREATE TABLE IF NOT EXISTS "UserGoals" (
    "Id"              UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "UserId"          UUID         NOT NULL,
    "GoalType"        SMALLINT     NOT NULL CHECK ("GoalType" BETWEEN 1 AND 4),
    "TargetWeightKg"  DECIMAL(5,2) NULL,
    "TargetWeightLbs" DECIMAL(5,2) NULL,
    "StartDate"       DATE         NOT NULL,
    "EndDate"         DATE         NOT NULL,
    "IsActive"        BOOLEAN      NOT NULL DEFAULT TRUE,
    "CreatedAt"       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "FK_UserGoals_Users"
        FOREIGN KEY ("UserId")
        REFERENCES "Users"("Id")
        ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "UX_UserGoals_ActiveUser"
    ON "UserGoals"("UserId")
    WHERE "IsActive" = TRUE;
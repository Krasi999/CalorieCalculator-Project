CREATE TABLE IF NOT EXISTS "DailyCalories" (
    "Id"            UUID         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    "UserId"        UUID         NOT NULL,
    "Date"          DATE         NOT NULL,
    "CaloriesEaten" INT          NOT NULL DEFAULT 0,
    "CreatedAt"     TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt"     TIMESTAMP    NULL,

    CONSTRAINT "FK_DailyCalories_Users"
        FOREIGN KEY ("UserId")
        REFERENCES "Users"("Id")
        ON DELETE CASCADE
);


CREATE UNIQUE INDEX IF NOT EXISTS "UX_DailyCalories_UserDate"
    ON "DailyCalories"("UserId", "Date");
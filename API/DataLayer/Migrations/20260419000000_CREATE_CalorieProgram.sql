CREATE TABLE IF NOT EXISTS "CaloriePrograms" (
    "ProgramID" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "CaloriesPerDay" INT NOT NULL DEFAULT 0,
    "CarbsPerDay" INT NOT NULL DEFAULT 0,
    "ProteinPerDay" INT NOT NULL DEFAULT 0,
    "FatsPerDay" INT NOT NULL DEFAULT 0,
    "ProgramDate" DATE NOT NULL DEFAULT CURRENT_DATE,
    "UserID" UUID NOT NULL,
    CONSTRAINT "FK_CaloriePrograms_Users"
        FOREIGN KEY ("UserID")
        REFERENCES "Users"("Id")
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "CalorieProgramMeals" (
    "MealID" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "MealType" INT NOT NULL,
    "CalorieProgramID" INT NOT NULL,
    CONSTRAINT "FK_CalorieProgramMeals_CaloriePrograms"
        FOREIGN KEY ("CalorieProgramID")
        REFERENCES "CaloriePrograms"("ProgramID")
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "MealFoods" (
    "MealFoodID" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "MealID" INT NOT NULL,
    "ProductID" INT NOT NULL,
    CONSTRAINT "FK_MealFoods_CalorieProgramMeals"
        FOREIGN KEY ("MealID")
        REFERENCES "CalorieProgramMeals"("MealID")
        ON DELETE CASCADE,
    CONSTRAINT "FK_MealFoods_FoodProducts"
        FOREIGN KEY ("ProductID")
        REFERENCES "FoodProducts"("ProductID")
        ON DELETE CASCADE
);
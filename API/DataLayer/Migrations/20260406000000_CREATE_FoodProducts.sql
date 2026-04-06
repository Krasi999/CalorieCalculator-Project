CREATE TABLE IF NOT EXISTS "FoodCategories" (
	"CategoryID" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	"Name" VARCHAR (255) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS "FoodProducts" (
	"ProductID" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	"Name" VARCHAR (255) NOT NULL,
	"Description" VARCHAR (255),
	"CategoryID" INT NOT NULL,
	"Calories" INT NOT NULL,
	"Fats" DECIMAL NOT NULL,
	"Protein" DECIMAL NOT NULL,
	"Carbs" DECIMAL NOT NULL,
	"Weight" INT NOT NULL,
	"CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	CONSTRAINT fk_category FOREIGN KEY ("CategoryID") REFERENCES "FoodCategories"("CategoryID")
);

INSERT INTO "FoodCategories" ("Name") 
VALUES
	('Месо и местни продукти'),
	('Риба и морски храни'),
	('Млечни и яйчни'),
	('Зърнени и тестени'),
	('Растителни храни'),
	('Подправки');
	
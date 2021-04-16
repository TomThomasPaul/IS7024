# MOM'S SPAGHETTI
  https://momsspaghetti.azurewebsites.net/


---

## Team Members
_Anirudh C Nair_ 

_Shivani Mishra_  

_Tom Thomas Paul_

---

## Introduction

Nothing comes close to each of our mom's cooking, but with the new Mom's Spaghetti site we can help you achieve something close:

We have abducted some of the best home cooks from across the world and coerced them to share their finger-licking recipes for all of us to recreate.
- Search for your cravings and we will provide you with the right recipes.
- View all the ingredient details for any recipe of your choice.
- No more hassle of doing mental athletics to figure out how many ingredients would be required for increased servings. As we have dynamic recipes which change ingredients quantity and cooking duration as per servings.
- You can add/remove your fav recipes.
- You can view cooking directions for each recipe.
- You can view nutrient information for any food item of your choice.
- If you do not like the recipe, you can send us feedback and we will not release these wonderful moms till they get it to your satisfaction.


Now everyone can make food which would make their mom proud.

---

## External Data Feeds

https://forkify-api.herokuapp.com/api/search?q=

https://forkify-api.herokuapp.com/api/get?rId=

## External Group API endpoint- hosted in Azure by NutrientDiary
https://nutrientdiary20210416013720.azurewebsites.net/?query=

## Our JSON feed and JSON schema - Most Visted Recipes

https://momsspaghetti.azurewebsites.net/?getMostSearched=true
https://momsspaghetti.azurewebsites.net/?getSchema=true


---

## Functional Requirements

### Requirement 1.0 : Search for recipes

#### Scenario

As a user I want to search for a particular recipe

#### Dependencies

Recipe data is available and easily accessible

#### Example 1.1 :

**Given** recipe data is available 

**When** I search for "Pizza"

**Then** I should receive all the recipes related to Pizza 


### Requirement 2.0 : View ingredients,servings and cooking time for recipes

#### Scenario

As a user I want to view the ingredients and the cooking time required for the number of servings for the recipe. The cooking time will vary based on the number of ingredients.

#### Dependencies

Ingredients and related data is available for all recipes.

#### Example 2.1 :

**Given** ingredients data is available 

**When** I view "Pizza" recipe

**Then** I should receive all the ingredients and cooking time based on the number of servings for "Pizza". 

### Requirement 3.0 : Option to update servings for recipes

#### Scenario

As a user I want to increase or decrease the number of servings for each recipe and see the respective changes in the amount of ingredients required for the same.

#### Dependencies

Ingredients and related data is available for all recipes.

#### Example 3.1 :

**Given** ingredients data is available 

**When** I view "Pizza" recipe and increase servings.

**Then** I should see the corresponding increase in the amount of ingredients required for the same.

### Requirement 4.0 : Add/Remove recipes to/from Favourites

#### Scenario

As a user I want to add recipes to "Favourites".

#### Dependencies

Recipe data is available and easily accessible.

#### Example 4.1 :

**Given** Recipe data is available 

**When** I add "Pizza" to favourites

**Then** "Pizza" should be available in "Favourites" list.

**When** I remove "Pizza" from favourites

**Then** "Pizza" should be removed from "Favourites" list.


### Requirement 5.0 : View Cooking directions for recipes

#### Scenario

As a user I want to view the cooking directions for the recipe I am interested in.

#### Dependencies

Ingredients and related data is available for all recipes.

#### Example 5.1 :

**Given** ingredients data is available 

**When** I view "Pizza" recipe and click on cooking directions.

**Then** I should see the cooking directions for the recipe I am interested in.


### Requirement 6.0 : View recipes in pages

#### Scenario

As a user I want to view all recipes for a particular food item formatted in pages.

#### Dependencies

Recipe data is available for all recipes.

#### Example 6.1 :

**Given** ingredients data is available 

**When** I search for "Pizza".

**Then** I should see all recipes with pagination implemented for the same.


### Requirement 7.0 : Search for Nutrients

#### Scenario

As a user I want to search for a food item and retrieve it's nutrient information.

#### Dependencies

NutrientDiary Application hosted on Azure is up and running successfully:
https://nutrientdiary20210416013720.azurewebsites.net/?query=


#### Example 7.1 :

**Given** NutrientDiary Application in Azure is up and running 

**When** I search for "Apple".

**Then** I should see all nutrients for Apple.

---

## Scrum Roles

- Scrum Master: Anirudh C Nair
- Frontend Developer: Shivani Mishra
- Backend Developer: Tom Thomas Paul

---

## Weekly Meeting

Monday Wednesday 11:00 am to 12:00 pm






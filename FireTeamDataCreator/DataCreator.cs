using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FireTeamDataCreator
{
    class DataCreator
    {
        static List<Ingredient> knownIngredients = new List<Ingredient>();
        static List<Recipie> knownRecipies = new List<Recipie>();

        static void Main(string[] args)
        {
            Console.WriteLine("===========================================");
            Console.Write("JSON file path: ");
            var path = Console.ReadLine();
            JsonData jsonData;
            string jsonString = "";

            try
            {
                if (!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
                {
                    Console.WriteLine("JSON path is invalid!");
                    return;
                }

                if (File.Exists(path))
                {
                    Console.WriteLine("Reading existing JSON file...");
                    jsonString = File.ReadAllText(path);
                    jsonData = JsonSerializer.Deserialize<JsonData>(jsonString);

                    if (jsonData.ingredients != null)
                    {
                        knownIngredients = new List<Ingredient>();
                        knownIngredients.AddRange(jsonData.ingredients);
                    }
                    if (jsonData.recipies != null)
                    {
                        knownRecipies = new List<Recipie>();
                        knownRecipies.AddRange(jsonData.recipies);
                    }
                }
                else
                {
                    Console.WriteLine("Creating new JSON file...");
                    File.Create(path).Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading or creating JSON file.\n{e.Message}");
                return;
            }

            Console.WriteLine("===========================================");
            Console.WriteLine("Data creator for Fire team's app project.\nTo create new ingredient enter 'new ingredient'. To create new recipie enter 'new recipie'\nTo list ingredients type 'list ingredients'. To list recipies type 'list recipies'\nTo exit type 'exit'");
            Console.WriteLine("===========================================");

            do
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (input == "exit")
                    break;

                if (input == "new ingredient")
                    knownIngredients.Add(CreateIngredient(knownIngredients.Count));
                if (input == "new recipie")
                    knownRecipies.Add(CreateRecipie(knownRecipies.Count));
                if (input == "list ingredients")
                    foreach (var ingredient in knownIngredients)
                        Console.WriteLine(JsonSerializer.Serialize(ingredient));
                if (input == "list recipies")
                    foreach (var recipie in knownRecipies)
                        Console.WriteLine(JsonSerializer.Serialize(recipie));

            } while (true);

            jsonData = new JsonData()
            {
                ingredients = knownIngredients.ToArray(),
                recipies = knownRecipies.ToArray()
            };

            jsonString = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }

        [Serializable]
        class JsonData
        {
            public Ingredient[] ingredients { get; set; }
            public Recipie[] recipies { get; set; }
        }

        static Ingredient CreateIngredient(int id)
        {
            Console.Write("Ingredient type: ");
            var type = Console.ReadLine();

            Console.Write("Ingredient name: ");
            var name = Console.ReadLine();

            Console.Write("Ingredient description (enter empty line to finish): ");
            string description = "";
            string descriptionLine;
            while ((descriptionLine = Console.ReadLine()) != "")
            {
                description += descriptionLine + "\n";
            }

            Console.Write("Ingredient nutritional value (enter empty line to finish): ");
            string nutrition = "";
            string nutritionLine;
            while ((nutritionLine = Console.ReadLine()) != "")
            {
                nutrition += nutritionLine + "\n";
            }

            var ingredient = new Ingredient()
            {
                ID = id,
                Type = type,
                Name = name,
                Description = description,
                Nutrition = nutrition
            };

            Console.WriteLine($"\nCreated new recipie with id: {ingredient.ID}");

            return ingredient;
        }

        static Recipie CreateRecipie(int id)
        {
            Console.Write("Recipie name: ");
            var name = Console.ReadLine();

            Console.Write("Recipie description (enter empty line to finish): ");
            string description = "";
            string descriptionLine;
            while ((descriptionLine = Console.ReadLine()) != "")
            {
                description += descriptionLine + "\n";
            }

            Console.Write("Recipie cooking instructions (enter empty line to finish): ");
            string instruction = "";
            string instructionLine;
            while ((instructionLine = Console.ReadLine()) != "")
            {
                instruction += instructionLine + "\n";
            }

            Console.Write("Required ingredients. Enter id or full name (enter empty line to finish): ");
            List<int> ids = new List<int>();
            string input = "";
            while ((input = Console.ReadLine()) != "")
            {
                var ingredient = knownIngredients.Find((ingr) =>
                {
                    int id = -1;
                    if (int.TryParse(input, out id) && id == ingr.ID)
                        return true;
                    else if (input == ingr.Name)
                        return true;

                    return false;
                });

                if (ingredient != null)
                {
                    ids.Add(ingredient.ID);
                    Console.WriteLine($"Added required ingredient: {ingredient.Name}:{ingredient.ID}");
                }
                else
                    Console.WriteLine("Entered invalid ingredient!");
            }

            var recipie = new Recipie()
            {
                ID = id,
                ingredientIDs = ids.ToArray(),
                Name = name,
                Description = description,
                CookingDirectives = instruction
            };

            Console.WriteLine($"\nCreated new recipie with id: {recipie.ID}");

            return recipie;
        }
    }

    [Serializable]
    public class Ingredient
    {
        public string Type { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Nutrition { get; set; }
    }

    [Serializable]
    public class Recipie
    {
        public int ID { get; set; }
        public int[] ingredientIDs { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CookingDirectives { get; set; }
    }
}

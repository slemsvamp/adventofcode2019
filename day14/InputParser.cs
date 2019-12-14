using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace day14
{
    public class InputParser
    {
        public static ChemBook Parse(string filename)
        {
            var lines = File.ReadAllLines(filename);

            var regex = new Regex(@"(\d+) (\w+)");
            var allChemicals = new Dictionary<string, Chemical>();
            var allRecipes = new Dictionary<string, ChemicalRecipe>();

            foreach (var line in lines)
            {
                string[] recipeParts = line.Split(new[] { " => " }, StringSplitOptions.None);

                string produced = recipeParts[1];

                string[] consumed = recipeParts[0].Split(new[] { ", " }, StringSplitOptions.None);

                var producedMatch = regex.Match(produced);
                var produce = producedMatch.Groups[2].Value;

                Chemical chemical = new Chemical
                {
                    Name = produce
                };

                ChemicalRecipe recipe = new ChemicalRecipe
                {
                    Produced = chemical,
                    Quantity = int.Parse(producedMatch.Groups[1].Value),
                    Ingredients = new Dictionary<string, int>()
                };

                foreach (var consumedChemical in consumed)
                {
                    var match = regex.Match(consumedChemical);
                    var ingredient = match.Groups[2].Value;
                    var quantity = int.Parse(match.Groups[1].Value);

                    recipe.Ingredients.Add(ingredient, quantity);

                    if (!allChemicals.ContainsKey(ingredient))
                    {
                        allChemicals.Add(ingredient, new Chemical
                        {
                            Name = ingredient
                        });
                    }
                }

                if (!allChemicals.ContainsKey(produce))
                {
                    allChemicals.Add(produce, chemical);
                }

                allRecipes.Add(produce, recipe);
            }

            return new ChemBook
            {
                AllChemicals = allChemicals,
                AllRecipes = allRecipes
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace day14
{
    class ChemCooker
    {
        private Dictionary<string, long> _stash;
        private Dictionary<string, long> _claim;
        private readonly ChemBook _chemBook;

        public ChemCooker(string filename)
        {
            _chemBook = InputParser.Parse(filename);

            _stash = new Dictionary<string, long>();
            _claim = new Dictionary<string, long>();

            foreach (var chemical in _chemBook.AllChemicals.Keys)
            {
                _stash.Add(chemical, 0);
                _claim.Add(chemical, 0);
            }
        }

        internal long FuelForOre(long trillionOre)
        {
            // I have no other excuse than that I was tired and felt like brute-forcing it. If I return to this to
            // refactor this I would most likely try to work with decimals and just try to math it out. Instead of
            // producing and claiming to count for leftovers, just calculate the needed ratios and somehow when it's
            // all done you just get a Fuel coefficient, you divide the trillionOre by the coefficient and you probably
            // get the correct result. I might have gotten that backwards, but that's for another day.

            long cooked = 0;

            while (trillionOre - GetFromStash("ORE") > 0)
            {
                Cook(_chemBook, 1, "FUEL");
                cooked++;
            }

            return cooked - 1;
        }

        public long GetFromStash(string chemical)
        {
            return _stash[chemical];
        }

        public long OrePerFuel(int fuel)
        {
            Cook(_chemBook, fuel, "FUEL");

            return GetFromStash("ORE");
        }

        public void Cook(ChemBook chemBook, long needToProduce, string produceName)
        {
            if (chemBook.AllRecipes.ContainsKey(produceName))
            {
                var recipe = chemBook.AllRecipes[produceName];
                var recipeCreatesQuantity = recipe.Quantity;

                var neededIngredients = new List<(string, long)>();
                var batches = (long)Math.Ceiling((double)needToProduce / recipeCreatesQuantity);

                foreach (var requiredIngredient in recipe.Ingredients)
                {
                    neededIngredients.Add((requiredIngredient.Key, requiredIngredient.Value * batches));
                }

                foreach ((string ingredientName, long ingredientQuantityNeeded) in neededIngredients)
                {
                    _claim[ingredientName] += ingredientQuantityNeeded;

                    if (chemBook.AllRecipes.ContainsKey(ingredientName))
                    {
                        var ingredientRecipe = chemBook.AllRecipes[ingredientName];
                        var diff = _claim[ingredientName] - _stash[ingredientName];

                        if (_claim[ingredientName] > _stash[ingredientName])
                        {

                            Cook(chemBook, diff, ingredientName);
                        }

                        var ingredientBatches = (long)Math.Ceiling((double)diff / ingredientRecipe.Quantity);
                        _stash[ingredientName] += ingredientRecipe.Quantity * ingredientBatches;
                    }
                    else
                    {
                        if (_claim[ingredientName] > _stash[ingredientName])
                        {
                            _stash[ingredientName] += ingredientQuantityNeeded;
                        }
                    }
                }
            }
        }
    }
}

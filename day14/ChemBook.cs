using System;
using System.Collections.Generic;
using System.Text;

namespace day14
{
    public class ChemBook
    {
        public Dictionary<string, Chemical> AllChemicals { get; set; }
        public Dictionary<string, ChemicalRecipe> AllRecipes { get; set; }
    }
}

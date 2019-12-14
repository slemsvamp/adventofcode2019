using System;
using System.Collections.Generic;
using System.Text;

namespace day14
{
    public class ChemicalRecipe
    {
        public Chemical Produced { get; set; }
        public int Quantity { get; set; }
        public Dictionary<string, int> Ingredients { get; set; }
    }
}

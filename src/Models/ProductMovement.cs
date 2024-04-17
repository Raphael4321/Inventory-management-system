﻿using System;
using System.Collections.Generic;

namespace Gerenciador_de_estoque.src.Models
{
    public class ProductMovement
    {
        public int IdMovement { get; set; }
        public Supplier Supplier { get; set; }
        public string Type { get; set; }
        public List<SelectedProd> ProductsList { get; set; }
        public string Date { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Features.Polymorphism
{
    public abstract class ShipmentItem
    {
        // alternative solution : use it instead of $type
        public string Discriminator => GetType().Name; 
    }

    public class Crate : ShipmentItem
    {
        public double Weight { get; set; }
    }

    public class Car : ShipmentItem
    {

    }

    public class Shipment
    {
        public string Id { get; set; }

        public List<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
    }

    public class SpecialShipment
    {
        public string Id { get; set; }

        public ShipmentItem Item { get; set; }
    }
}

using System;

namespace Inventory.WebApi.Models
{
    public class ItemModel
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public string ItemType { get; set; }
        public string ExpirationDate { get; set; }
        public string Url { get; set; }
    }
}
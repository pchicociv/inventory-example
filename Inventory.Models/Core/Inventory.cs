using System.Collections.Generic;
using System.Linq;

namespace Inventory.Models.Core
{
    public class Inventory
    {
        static Inventory()
        {
            Instance = new Inventory();
        }
        private Inventory()
        {
            _data = new List<Item>();
            _dataById = new Dictionary<long, Item>();
            _dataByLabel = new Dictionary<string, Item>();
        }
        public static Inventory Instance { get; private set; }
        private readonly Dictionary<long, Item> _dataById = new Dictionary<long, Item>();
        private readonly Dictionary<string, Item> _dataByLabel = new Dictionary<string, Item>();
        private readonly List<Item> _data = new List<Item>();
        private long _nextId;

        //TODO: This lock is for demo purposes only as we don't have a database 
        //      but still want to support some sort of concurrent data access
        private object inventoryLock = new object();

        public Item Add(Item newItem)
        {
            lock (inventoryLock)
            {
                var nextId = NextId();
                newItem.Id = nextId;
                _data.Add(newItem);
                _dataById.Add(nextId, newItem);
                _dataByLabel.Add(newItem.Label, newItem);
                return newItem;
            }
        }

        public IEnumerable<Item> GetAll()
        {
            lock (inventoryLock)
            {
                return _data.ToList();
            }
        }

        public Item Get(string label)
        {
            lock (inventoryLock)
            {
                Item outItem;
                _dataByLabel.TryGetValue(label, out outItem);
                return outItem;
            }
        }

        public Item Get(long id)
        {
            lock (inventoryLock)
            {
                Item outItem;
                _dataById.TryGetValue(id, out outItem);
                return outItem;
            }
        }

        public Item Remove(Item item)
        {
            lock (inventoryLock)
            {
                if (_dataById.ContainsKey(item.Id))
                {
                    _data.Remove(item);
                    _dataByLabel.Remove(item.Label);
                    _dataById.Remove(item.Id);
                    return item;
                }
                return null;
            }
        }

        private long NextId()
        {
            return ++_nextId;
        }

    }
}

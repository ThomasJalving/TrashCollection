using System;
using System.Collections.Generic;
using DumpTruck.HelperClasses;

namespace DumpTruck.Datastructures
{
    // From (and then modified:) https://gamedev.stackexchange.com/a/162987/76152
    public class WeightedRandomBag<T>
    {
        private readonly List<Entry> _entries = new List<Entry>();
        private readonly Random _rand = RandomHelper.Random;
        private double _accumulatedWeight;

        //Adds an element to the bag with a certain weight
        public void AddEntry(T item, double weight)
        {
            _accumulatedWeight += weight;
            _entries.Add(new Entry {Item = item, AccumulatedWeight = _accumulatedWeight});
        }

        //Gets a random element from the bag with a chance based on the weight
        public T GetRandom()
        {
            var random = _rand.NextDouble() * _accumulatedWeight;

            foreach (var entry in _entries)
                if (entry.AccumulatedWeight >= random)
                    return entry.Item;

            return default; //should only happen when there are no entries
        }

        //Contains the accumulatedWeight at the time of adding the element
        private struct Entry
        {
            public double AccumulatedWeight;
            public T Item;
        }
    }
}
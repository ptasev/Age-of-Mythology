using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMProtoEditor
{
    /// <summary>
    /// This is a dictionary guaranteed to have only one of each value and key. 
    /// It may be searched either by TFirst or by TSecond, giving a unique answer because it is 1 to 1.
    /// </summary>
    /// <typeparam name="string">The type of the "key"</typeparam>
    /// <typeparam name="int">The type of the "value"</typeparam>
    public class StringTable2
    {
        private SortedList<string, int> stringLookup;

        public StringTable2()
        {
            stringLookup = new SortedList<string, int>();
        }
        public StringTable2(int capacity)
        {
            stringLookup = new SortedList<string, int>(capacity);
        }
        public StringTable2(IEqualityComparer<string> comparer)
        {
            //stringLookup = new SortedList<string, int>(comparer);
        }
        public StringTable2(int capacity, IEqualityComparer<string> comparer)
        {
            //stringLookup = new SortedList<string, int>(capacity, comparer);
        }


        #region Exception throwing methods

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Throws an exception if either element is already in the dictionary
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public void Add(string first, int second)
        {
            if (stringLookup.ContainsKey(first))
                throw new ArgumentException("Duplicate string!");

            stringLookup.Add(first, Count);
        }

        /// <summary>
        /// Find the TSecond corresponding to the TFirst first
        /// Throws an exception if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <returns>the value corresponding to first</returns>
        public int GetByFirst(string first)
        {
            int second;
            if (!stringLookup.TryGetValue(first, out second))
                throw new ArgumentException("String not found!");

            return second;
        }

        /// <summary>
        /// Find the TFirst corresponing to the Second second.
        /// Throws an exception if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <returns>the value corresponding to second</returns>
        public string GetBySecond(int second)
        {
            return stringLookup.Keys[second];
            foreach (KeyValuePair<string, int> pair in stringLookup)
            {
                if (pair.Value == second)
                    return pair.Key;
            }

            throw new ArgumentException("String ID not found");
        }


        /// <summary>
        /// Remove the record containing first.
        /// If first is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="first">the key of the record to delete</param>
        public void RemoveByFirst(string first)
        {
            int second;
            if (!stringLookup.TryGetValue(first, out second))
                throw new ArgumentException("first");

            stringLookup.Remove(first);
        }

        /// <summary>
        /// Remove the record containing second.
        /// If second is not in the dictionary, throws an Exception.
        /// </summary>
        /// <param name="second">the key of the record to delete</param>
        public void RemoveBySecond(int second)
        {
            string first = string.Empty;
            bool found = false;
            foreach (KeyValuePair<string, int> pair in stringLookup)
            {
                if (pair.Value == second)
                {
                    first = pair.Key;
                    found = true;
                }
            }

            if (!found)
                throw new ArgumentException("second");

            stringLookup.Remove(first);
        }

        #endregion

        #region Try methods

        /// <summary>
        /// Tries to add the pair to the dictionary.
        /// Returns false if either element is already in the dictionary        
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>true if successfully added, false if either element are already in the dictionary</returns>
        public Boolean TryAdd(string first, int second)
        {
            if (stringLookup.ContainsKey(first))
                return false;

            stringLookup.Add(first, Count);
            return true;
        }


        /// <summary>
        /// Find the TSecond corresponding to the TFirst first.
        /// Returns false if first is not in the dictionary.
        /// </summary>
        /// <param name="first">the key to search for</param>
        /// <param name="second">the corresponding value</param>
        /// <returns>true if first is in the dictionary, false otherwise</returns>
        public Boolean TryGetByFirst(string first, out int second)
        {
            return stringLookup.TryGetValue(first, out second);
        }

        /// <summary>
        /// Find the TFirst corresponding to the TSecond second.
        /// Returns false if second is not in the dictionary.
        /// </summary>
        /// <param name="second">the key to search for</param>
        /// <param name="first">the corresponding value</param>
        /// <returns>true if second is in the dictionary, false otherwise</returns>
        public Boolean TryGetBySecond(int second, out string first)
        {
            first = string.Empty;
            bool found = false;

            foreach (KeyValuePair<string, int> pair in stringLookup)
            {
                if (pair.Value == second)
                {
                    first = pair.Key;
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// Remove the record containing first, if there is one.
        /// </summary>
        /// <param name="first"></param>
        /// <returns> If first is not in the dictionary, returns false, otherwise true</returns>
        public Boolean TryRemoveByFirst(string first)
        {
            int second;
            if (!stringLookup.TryGetValue(first, out second))
                return false;

            stringLookup.Remove(first);
            return true;
        }

        /// <summary>
        /// Remove the record containing second, if there is one.
        /// </summary>
        /// <param name="second"></param>
        /// <returns> If second is not in the dictionary, returns false, otherwise true</returns>
        public Boolean TryRemoveBySecond(int second)
        {
            string first = string.Empty;
            bool found = false;

            foreach (KeyValuePair<string, int> pair in stringLookup)
            {
                if (pair.Value == second)
                {
                    first = pair.Key;
                    found = true;
                }
            }

            if (!found)
                return false;

            stringLookup.Remove(first);
            return true;
        }

        #endregion

        /// <summary>
        /// The number of pairs stored in the dictionary
        /// </summary>
        public Int32 Count
        {
            get { return stringLookup.Count; }
        }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        public void Clear()
        {
            stringLookup.Clear();
        }
    }
}

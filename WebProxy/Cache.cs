using System;
using System.Collections.Generic;
using System.Text;

namespace WebProxy
{
    /// <summary>
    /// Class for caching data
    /// </summary>
    class Cache
    {
        /// <summary>
        /// Byte arrays of data stored in this array
        /// </summary>
        byte[][] data;

        /// <summary>
        /// Matches a key to an index in the array of data. Allows for quick lookup
        /// </summary>
        IDictionary<string, int> indices;

        /// <summary>
        /// Reverse of the first dictionary. Allows for lookup in reverse, searching by index in the data array
        /// </summary>
        IDictionary<int, string> indicesRev;

        /// <summary>
        /// Used to calculate the least recently used index of the byte array
        /// </summary>
        int[,] leastRecentlyUsed;

        /// <summary>
        /// Size of the cache
        /// </summary>
        public int size { get; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">Size of the cache</param>
        public Cache(int size)
        {
            //Min size is 1
            if (size < 1)
            {
                size = 1;
            }
            this.size = size;

            data = new byte[size][];
            indices = new Dictionary<string, int>();
            indicesRev = new Dictionary<int, string>();
            leastRecentlyUsed = new int[size, size];

            //Initialize LRU matrix to 0s
            for (int i = 0; i < leastRecentlyUsed.GetLength(0); i++)
            {
                for (int j = 0; j < leastRecentlyUsed.GetLength(1); j++)
                {
                    leastRecentlyUsed[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// Insert a data array into the cache
        /// </summary>
        /// <param name="request">Unique request string. Used as the key</param>
        /// <param name="bytes">Data to be stored</param>
        public void InsertIntoCache(string request, byte[] bytes)
        {
            Console.WriteLine("Inserting into cache");
            string t = Encoding.ASCII.GetString(bytes);

            //If the cache already contains data from this request, update it instead
            if (indices.ContainsKey(request))
            {
                UpdateCacheEntry(request, bytes);
            }
            else
            {
                //Insert the data into the least recently used index
                int lru = FindLeastRecentlyUsed();
                if (data[lru] != null)
                {
                    Console.WriteLine("Cache overflow");
                    bool success = false;
                    string value = "";
                    while (!success)
                    {
                        success = indicesRev.TryGetValue(lru, out value);
                    }
                    indicesRev.Remove(lru);
                    indices.Remove(value);
                }
                data[lru] = bytes;
                indices.Add(request, lru);
                indicesRev.Add(lru, request);
            }
        }


        /// <summary>
        /// Retrieve data from the cache
        /// </summary>
        /// <param name="request">Unique request to retrieve</param>
        /// <returns></returns>
        public byte[] GetFromCache(string request)
        {
            Console.WriteLine("Getting from cache");
            if (!indices.TryGetValue(request, out int value))
            {
                return null;
            }
            else
            {
                string t = Encoding.ASCII.GetString(data[value]);
                return data[value];
            }
        }


        /// <summary>
        /// Mark an index as used most recently
        /// </summary>
        /// <param name="request">Unique request to mark</param>
        public void MarkReused(string request)
        {
            Console.WriteLine("Marking as reused");
            //Only do so if the request exists in the cache
            if (indices.TryGetValue(request, out int value))
            {
                MarkUsed(value);
            }
        }


        /// <summary>
        /// Update the data in the cache for a request
        /// </summary>
        /// <param name="request">Unique request to update</param>
        /// <param name="bytes">Data to update with</param>
        public void UpdateCacheEntry(string request, byte[] bytes)
        {
            Console.WriteLine("Updating cache");
            if (indices.TryGetValue(request, out int value))
            {
                data[value] = bytes;
                MarkUsed(value);
            }
            //If the request does not already exist, insert it instead
            else
            {
                InsertIntoCache(request, bytes);
            }
        }


        /// <summary>
        /// Find the least recently used index of the array
        /// </summary>
        /// <returns>Index of the least recently used entry</returns>
        private int FindLeastRecentlyUsed()
        {
            //The least recently used index corresponds to the row of the matrix containing only 0s
            for (int i = 0; i < leastRecentlyUsed.GetLength(0); i++)
            {
                bool isLRU = true;
                for (int j = 0; j < leastRecentlyUsed.GetLength(1) && isLRU; j++)
                {
                    if (leastRecentlyUsed[j, i] == 1)
                    {
                        isLRU = false;
                    }
                }
                if (isLRU) return i;
            }
            return -1;
        }


        /// <summary>
        /// Mark an index as most recently used
        /// </summary>
        /// <param name="index">Index of the most recently used entry</param>
        private void MarkUsed(int index)
        {
            //First, set the column corresponding to the index to all 1s
            for (int i = 0; i < leastRecentlyUsed.GetLength(0); i++)
            {
                leastRecentlyUsed[i, index] = 1;
            }
            //Then, set the corresponding row to all 0s
            for (int i = 0; i < leastRecentlyUsed.GetLength(1); i++)
            {
                leastRecentlyUsed[index, i] = 0;
            }
        }
    }
}

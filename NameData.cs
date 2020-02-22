using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NameGenerator
{
    [CreateAssetMenu(menuName = "Name Generator/Name Data", fileName = "Name Data"), ExecuteInEditMode]
    public class NameData : ScriptableObject
    {
        [SerializeField] TextAsset source;
        [SerializeField] char separator = '\n';
        [SerializeField] int maxChunkSize = 8;
        [SerializeField] int minLength = 3;

        Dictionary<string, List<string>> charDistribution;

        /// <summary>
        /// Generates a distribution from the source file.
        /// </summary>
        public void LoadData()
        {
            if(source == null)
            {
                Debug.LogError("Source is null for " + name);
                return;
            }
            string[] names = source.ToString().Split(separator);
            GenerateDistribution(names);
            Debug.Log("Disribution generated!");
        }

        /// <summary>
        /// Add all substring permutations of the given names to the distribution.
        /// </summary>
        /// <param name="names"></param>
        void GenerateDistribution(string[] names)
        {
            charDistribution = new Dictionary<string, List<string>>();
            charDistribution.Add("_start_", new List<string>());
            foreach(string name in names)
            {
                string lower = name.ToLower();
                char[] nameChars = lower.ToCharArray();

                /**1. Loop through all characters in a name.
                 * 2. Get all chunks of characters before the current character of length 1 until the start of the name.
                 * 3. Repeat for chunks after this character.
                 * 4. Add all combinations of 1 pre-chunk and 1 post-chunk to the distribution **/
                for(int startIndex = 0; startIndex < nameChars.Length; startIndex++)
                {
                    string chunk = "";
                    for(int chunkSize = 0; chunkSize < maxChunkSize; chunkSize++)
                    {
                        if(startIndex - chunkSize == -1)
                        {
                            string[] next = GetNextChars(nameChars, 0);

                            foreach(string nextString in next)
                            {
                                charDistribution["_start_"].Add(nextString);
                            }

                            continue;
                        }

                        if(startIndex - chunkSize >= 0)
                        {
                            chunk = nameChars[startIndex - chunkSize].ToString() + chunk;

                            if (!charDistribution.ContainsKey(chunk))
                                charDistribution.Add(chunk, new List<string>());

                            string[] next = GetNextChars(nameChars, startIndex);

                            foreach (string nextString in next)
                                charDistribution[chunk].Add(nextString);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates all substrings of string with a length from 1 to maxChunkSize starting from character i.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="startIndex">Index of the first character.</param>
        /// <returns></returns>
        string[] GetNextChars(char[] word, int startIndex)
        {
            string chunk = "";
            List<string> chars = new List<string>();
            for(int chunkSize = 1; chunkSize <= maxChunkSize; chunkSize++)
            {
                if(startIndex + chunkSize < word.Length)
                {
                    chunk += word[startIndex + chunkSize].ToString();
                    chars.Add(chunk);
                }
                if(startIndex + chunkSize == word.Length)
                {
                    chunk += "_end_";
                    chars.Add(chunk);
                    return chars.ToArray();
                }
            }
            return chars.ToArray();
        }

        /// <summary>
        /// Generates a random name.
        /// </summary>
        /// <param name="start">Start of the name.</param>
        /// <returns></returns>
        public string GenerateName(string start = "")
        {
            string name = start;

            string next = "";

            if(name == "")
                next = Random("_start_");

            int tries = 0;

            string nextTry;

            while((name.Length < minLength || (next.Length < 5 || next.Substring(next.Length - 5) != "_end_")) && tries < 1000)
            {
                if(next.Length < 5 || next.Substring(next.Length - 5) != "_end_")
                {
                    name += next;
                }
                nextTry = Random(name.Length > maxChunkSize ? name.Substring(maxChunkSize) : name);
                if(nextTry == null)
                {
                    break;
                }
                next = nextTry;
                tries++;
            }

            if (next.Length >= 5 && next.Substring(next.Length - 5) == "_end_")
                name += next.Remove(next.Length - 5);
            return name;
        }

        /// <summary>
        /// Gets a random string that would likely follow the given text.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string Random(string text)
        {
            for(int i = 0; i < text.Length; i++)
            {
                string chunk = text.Substring(i);
                if (charDistribution.ContainsKey(chunk))
                    return charDistribution[chunk][UnityEngine.Random.Range(0, charDistribution[chunk].Count - 1)];
            }
            return null;
        }
    }
}

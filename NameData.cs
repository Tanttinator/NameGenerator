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
        /// Generates a string distribution from a collection of strings.
        /// </summary>
        /// <param name="data"></param>
        void GenerateDistribution(string[] data)
        {
            charDistribution = new Dictionary<string, List<string>>();
            charDistribution.Add("_start_", new List<string>());
            foreach(string name in data)
            {
                string lower = name.ToLower();
                char[] chars = lower.ToCharArray();
                for(int i = 0; i < chars.Length; i++)
                {
                    string s_char = "";
                    for(int j = 0; j < maxChunkSize; j++)
                    {
                        if(i - j == -1)
                        {
                            string[] next = GetNextChars(chars, 0);

                            foreach(string nextString in next)
                            {
                                charDistribution["_start_"].Add(nextString);
                            }

                            continue;
                        }

                        if(i - j >= 0)
                        {
                            s_char = chars[i - j].ToString() + s_char;

                            if (!charDistribution.ContainsKey(s_char))
                                charDistribution.Add(s_char, new List<string>());

                            string[] next = GetNextChars(chars, i);

                            foreach (string nextString in next)
                                charDistribution[s_char].Add(nextString);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates all substrings of string with a length from 1 to maxChunkSize starting from character i.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="i">Index of the first character.</param>
        /// <returns></returns>
        string[] GetNextChars(char[] word, int i)
        {
            string s_next = "";
            List<string> chars = new List<string>();
            for(int j = 1; j <= maxChunkSize; j++)
            {
                if(i + j < word.Length)
                {
                    s_next += word[i + j].ToString();
                    chars.Add(s_next);
                }
                if(i + j == word.Length)
                {
                    s_next += "_end_";
                    chars.Add(s_next);
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

            int i = 0;

            string nextTry;

            while((name.Length < minLength || (next.Length < 5 || next.Substring(next.Length - 5) != "_end_")) && i < 1000)
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
                i++;
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
                if (charDistribution.ContainsKey(text.Substring(i)))
                    return charDistribution[text.Substring(i)][UnityEngine.Random.Range(0, charDistribution[text.Substring(i)].Count - 1)];
            }
            return null;
        }
    }
}

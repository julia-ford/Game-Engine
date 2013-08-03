using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Meta.ParsingAndPrinting
{
	public class StringManipulator
	{
		/// <summary>
		/// Capitalizes the first letter of a given string.
		/// </summary>
		/// <param name="givenString">the string to capitalize</param>
		/// <returns>the given string, but with the first letter capitalized</returns>
		public static string CapitalizeFirstLetter(string givenString)
		{
			return givenString.Substring(0, 1).ToUpper() + givenString.Substring(1);
		}

		/// <summary>
		/// Manually enforces an 80-character-per-line limit by inserting
		/// newline characters between words. This will fail if the string
		/// contains a word that is 80 characters or more.
		/// </summary>
		/// <param name="givenString">the string to enforce the 80-character-per-line limit on</param>
		/// <returns>the newly-formatted string</returns>
		public static string Enforce80CharLineLimit(string givenString)
		{
			string[] lines = givenString.Split('\n');
			string finalString = enforce80CharLineLimit(lines[0]);
			for (int i = 1; i < lines.Length; i++)
			{
				finalString += '\n' + enforce80CharLineLimit(lines[i]);
			}
			return finalString;
		}

		/// <summary>
		/// A helper method for Enforce80CharLineLimit, this function takes a
		/// section of words with no line breaks and enforces the limit on it.
		/// </summary>
		/// <param name="givenString">the string to enforce the 80-character-per-line limit on</param>
		/// <returns>the newly-formatted string</returns>
		private static string enforce80CharLineLimit(string givenString)
		{
			string finalString = "";
			string[] words = givenString.Split(' ');
			int charsOnCurrentLine = 0;

			foreach (string word in words)
			{
				if(charsOnCurrentLine + 1 + word.Length >= 80)
				{
					finalString += '\n' + word;
					charsOnCurrentLine = word.Length;
				}
				else
				{
					finalString += ' ' + word;
					charsOnCurrentLine += word.Length + 1;
				}
			}

			return finalString;
		}

		/// <summary>
		/// Determines if the given string starts with a vowel or not.
		/// </summary>
		/// <param name="givenString">the string to check</param>
		/// <returns>true if the string starts with a vowel; false otherwise</returns>
		public static bool StartsWithVowel(string givenString)
		{
			char firstChar = givenString.ToLower().ElementAt(0);

			if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
			{
				return true;
			}
			return false;
		}
	}
}

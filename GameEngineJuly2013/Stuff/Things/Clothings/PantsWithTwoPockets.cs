using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using Stuff.Things.Containers;

namespace Stuff.Things.Clothings
{
	/// <summary>
	/// Some pants with two pockets.
	/// </summary>
	class PantsWithTwoPockets : Pants, Searchable
	{
		BasicContainer leftPocket;
		BasicContainer rightPocket;

		public PantsWithTwoPockets(string name, string description, string[] parsedNames = null,
			bool isSpecific = false, bool isPlural = true, bool isProper = false,
			bool canBeTaken = true, bool canBeDropped = true, bool canBeRemoved = true)
			: base(name, description, parsedNames:parsedNames,
			isSpecific: isSpecific, isPlural: isPlural, isProper: isProper,
			canBeTaken: canBeTaken, canBeDropped: canBeDropped, canBeRemoved: canBeRemoved)
		{
			this.leftPocket  = new BasicContainer("left pocket",  "It's a pocket.", new string[] { "left pocket",  "pocket", "left pocket of "  + name, "left pocket of the "  + name }, canBeTaken:false);
			this.rightPocket = new BasicContainer("right pocket", "It's a pocket.", new string[] { "right pocket", "pocket", "right pocket of " + name, "right pocket of the " + name }, canBeTaken:false);
		}

		/// <summary>
		/// Returns a HashSet of the two pockets.
		/// </summary>
		/// <returns>a HashSet of the two pockets</returns>
		public HashSet<Thing> GetContents()
		{
			return new HashSet<Thing>(new Thing[]
			{ this.leftPocket, this.rightPocket });
		}

		/// <summary>
		/// Returns a HashSet of the two pockets.
		/// </summary>
		/// <returns>a HashSet of the two pockets</returns>
		public HashSet<Thing> GetVisibleContents()
		{
			return new HashSet<Thing>(new Thing[]
			{ this.leftPocket, this.rightPocket });
		}

		/// <summary>
		/// Returns a HashSet of the two pockets and all of their contents.
		/// </summary>
		/// <returns>
		/// a HashSet of the two pockets and all of their contents.
		/// </returns>
		public HashSet<Thing> GetRecursiveContents()
		{
			HashSet<Thing> temp = new HashSet<Thing>();
			temp.Add(leftPocket);
			temp.Add(rightPocket);
			temp.UnionWith( leftPocket.GetRecursiveContents());
			temp.UnionWith(rightPocket.GetRecursiveContents());
			return temp;
		}

		/// <summary>
		/// Returns a HashSet of the two pockets.
		/// </summary>
		/// <returns>a HashSet of the two pockets</returns>
		public HashSet<Thing> GetVisibleRecursiveContents()
		{
			return new HashSet<Thing>(new Thing[]
			{ this.leftPocket, this.rightPocket });
		}

	}
}

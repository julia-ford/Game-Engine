using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Interfaces;
using Meta.ParsingAndPrinting;
using Meta.Exceptions.PuttingIntoExceptions;

namespace Stuff.Things.Containers
{
	/// <summary>
	/// Represents some kind of container that can't be opened or closed, like a basket, or a pocket.
	/// </summary>
	public class BasicContainer : Thing, Container
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// The contents of the container.
		/// </summary>
		private HashSet<Thing> contents;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new basic container with the specified name and description.
		/// </summary>
		/// <param name="name">the name of the container</param>
		/// <param name="description">the description of the container</param>
		public BasicContainer(
			string name, string description, string[] parsedNames = null,
			bool isSpecific=false, bool isPlural=false, bool isProper=false,
			bool canBeTaken=true,bool canBeDropped=true,bool isTwoHanded=false) :
			base(name, description, parsedNames:parsedNames,
			isSpecific: isSpecific, isPlural: isPlural, isProper: isProper,
			canBeTaken:canBeTaken,canBeDropped:canBeDropped,isTwoHanded:isTwoHanded)
		{
			this.contents = new HashSet<Thing>();
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for the container's contents.
		/// </summary>
		/// <returns>a set of all the things in the container</returns>
		public HashSet<Thing> GetContents()
		{
			return new HashSet<Thing>(this.contents);
		}

		/// <summary>
		/// Gets a set of only the visible contents of the container.
		/// In the case of the BasicContainer, this does the exact same thing as GetContents().
		/// </summary>
		/// <returns>a set of all the visible things in the container</returns>
		public virtual HashSet<Thing> GetVisibleContents()
		{
			return new HashSet<Thing>(this.contents);
		}

		/// <summary>
		/// Gets a list of all of this container's contents, and the contents of any containers inside of it, recursively.
		/// </summary>
		/// <returns>the list described in the summary</returns>
		public HashSet<Thing> GetRecursiveContents()
		{
			HashSet<Thing> allContents = new HashSet<Thing>(this.contents);
			foreach (Thing item in this.contents)
			{
				if (typeof(Searchable).IsAssignableFrom(item.GetType()))
				{
					allContents.UnionWith(((Searchable)item).GetRecursiveContents());
				}
			}
			return allContents;
		}

		/// <summary>
		/// Gets a recursive list of the container's contents, but only the contents marked as visible.
		/// </summary>
		/// <returns>a recursive list of the container's visible contents</returns>
		public virtual HashSet<Thing> GetVisibleRecursiveContents()
		{
			HashSet<Thing> allContents = new HashSet<Thing>(this.contents);
			foreach (Thing item in this.contents)
			{
				if (typeof(Searchable).IsAssignableFrom(item.GetType()))
				{
					allContents.UnionWith(((Searchable)item).GetVisibleRecursiveContents());
				}
			}
			return allContents;
		}

		/// <summary>
		/// This override prefaces the container's name with the word "empty"
		/// if the contaainer is empty, or append a list of the container's
		/// contents if it is not empty.
		/// </summary>
		/// <returns>the name of the container</returns>
		public override string GetName()
		{
			// this will hold the name when done
			string name = "";
			// optional "empty"
			if (this.IsEmpty()) { name += "empty "; }
			// name
			name += base.GetName();
			// contents, if any
			if (!this.IsEmpty())
			{
				name += " (in which ";
				// one item
				if (this.contents.Count == 1)
				{
					name += this.contents.ElementAt(0).GetConjugatedVerb(VerbSet.ToBe) + ' ' + this.contents.ElementAt(0).GetQualifiedName();
				}
				// two items
				else if (this.contents.Count == 2)
				{
					name += "are " + this.contents.ElementAt(0).GetQualifiedName() + " and " + this.contents.ElementAt(1).GetQualifiedName();
				}
				// three or more items
				else
				{
					name += "are ";
					for (int i = 0; i < this.contents.Count - 1; i++)
					{
						name += this.contents.ElementAt(i).GetQualifiedName() + ", ";
					}
					name += "and " + this.contents.ElementAt(this.contents.Count - 1).GetQualifiedName();
				}
				name += ")";
			}
			// done
			return name;
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Determines whether or not a given <see cref="Thing"/> is in the container.
		/// </summary>
		/// <param name="item">the item to check for</param>
		/// <returns>true if the container contains the item; false otherwise</returns>
		public bool Contains(Thing item)
		{
			return this.contents.Contains(item);
		}

		/// <summary>
		/// Determines whether or not there is anything inside of the container.
		/// </summary>
		/// <returns>true if the container is empty; false otherwise</returns>
		public bool IsEmpty()
		{
			if(this.contents == null) {
				return true; }
			if (this.contents.Count == 0) {
				return true; }
			return false;
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// Adds the specified <see cref="Thing"/> to the contents of the container.
		/// It is intended that derived classes will override this function, and do more error checking.
		/// Throws exceptions on trying to re-add an item inside it and on trying to add itself to its contents.
		/// </summary>
		/// <param name="item">the item to add</param>
		public virtual void AddThing(Thing item)
		{
			// if the item is already inside the container, throw an exception
			if (this.contents.Contains(item)) {
				throw new Exception("Error: Attempted to put something into the continer it was already inside of."); }

			// don't allow the item to be added to itself
			else if (this == item) {
				throw new Exception("Error: Attempted to put something inside of itself."); }

			// if no exceptions were thrown, execute as normal
			else {
				this.RemoveParsedName(this.GetName());
				this.RemoveParsedName(this.GetQualifiedName());
				this.RemoveParsedName(this.GetSpecificName());

				this.contents.Add(item);

				this.AddParsedName(this.GetName());
				this.AddParsedName(this.GetQualifiedName());
				this.AddParsedName(this.GetSpecificName()); }
		}

		/// <summary>
		/// Removes the specified <see cref="Thing"/> from the contents of the container.
		/// Throws an exception if the item is not inside the container.
		/// </summary>
		/// <param name="item">the thing to remove</param>
		public virtual void RemoveThing(Thing item)
		{
			if (!this.contents.Remove(item)) {
				throw new Exception("Error: attempted to remove something from  a container when the item was not inside the container."); }
		}

	}
}

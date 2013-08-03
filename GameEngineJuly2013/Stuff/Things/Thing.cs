using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Interfaces;
using Meta.ParsingAndPrinting;
using Stuff.Things.Actors;

namespace Stuff.Things
{
	/// <summary>
	/// One of the most low-level classes in the engine, "Thing" represents any
	/// object that can be interacted with. All other interactable classes,
	/// including Person, derive from Thing.
	/// </summary>
	public class Thing : Describable
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// The <see cref="Container"/> that contains the <see cref="Thing"/>.
		/// Can be null if the <see cref="Thing"/> has not come into play yet,
		/// if it has been removed from play, or if it is abstract, like an
		/// <see cref="Actor"/>'s <see cref="Inventory"/>.
		/// </summary>
		private Container location;

		/// <summary>
		/// This is the owner of the <see cref="Thing"/>. If no one owns it,
		/// it will be set to null.
		/// </summary>
		private Actor owner;

		/// <summary>
		/// The printed name of the <see cref="Thing"/>.
		/// </summary>
		private string name;

		/// <summary>
		/// A brief description of the <see cref="Thing"/>.
		/// </summary>
		private string description;

		/// <summary>
		/// This is an array of names for the parser to recognize as referring
		/// to this <see cref="Thing"/>. By default, it only contains the
		/// printed name of the <see cref="Thing"/>.
		/// </summary>
		private string[] parsedNames;

		/// <summary>
		/// Indicates whether to use "the" or "a(n)"
		/// for this <see cref="Thing"/>'s article.
		/// </summary>
		private bool isSpecific;

		/// <summary>
		/// Indicates whether this <see cref="Thing"/>
		/// is a singular or plural noun.
		/// </summary>
		private bool isPlural;

		/// <summary>
		/// Indicates whether or not this <see cref="Thing"/>'s name
		/// should be preceded by an article.
		/// </summary>
		private bool isProper;

		/// <summary>
		/// Determines whether this thing can be randomly selected if a name
		/// conflict occurs.
		/// </summary>
		private bool isGeneric;

		/// <summary>
		/// Indicates whether or not the <see cref="Thing"/> can be taken.
		/// </summary>
		private bool canBeTaken;

		/// <summary>
		/// Indicates whether or not the <see cref="Thing"/> can be dropped.
		/// Used for creating cursed/undroppable items.
		/// </summary>
		private bool canBeDropped;

		/// <summary>
		/// Idicates whether the <see cref="Thing"/> requires two hands in
		/// order to be carried by a <see cref="Person"/>.
		/// </summary>
		/// 
		/// <seealso cref="IsTwoHanded"/>
		private bool isTwoHanded;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new <see cref="Thing"/> with the specified name and description.
		/// Default state for isSpecific and isPlural is false.
		/// Default state for canBeTaken is true.
		/// </summary>
		/// <param name="name">the name of the <see cref="Thing"/></param>
		/// <param name="description">a brief description of the <see cref="Thing"/></param>
		/// <param name="isSpecific">whether to use a specific article,
		/// like "the," or a generic one, like "a"</param>
		/// <param name="isPlural">whether to use a plural article,
		/// like "some," or a plural one, like "a"</param>
		/// <param name="canBeTaken">whether or not this <see cref="Thing"/> can be taken</param>
		public Thing(string name, string description, string[] parsedNames = null,
			bool isSpecific = false, bool isPlural = false, bool isProper = false,
			bool canBeTaken = true, bool canBeDropped = true, bool isTwoHanded = false)
		{
			this.name         = name;
			this.description  = description;
			this.isSpecific   = isSpecific;
			this.isPlural     = isPlural;
			this.isProper     = isProper;
			this.canBeTaken   = canBeTaken;
			this.canBeDropped = canBeDropped;
			this.isTwoHanded  = isTwoHanded;

			this.isGeneric = false;

			// if no parsed names are given,
			// make the list of default parsed names
			if (parsedNames == null) {
				parsedNames = this.GetGenericParsedNames(); }

			this.parsedNames = parsedNames;
		}

		/// <summary>
		/// Creates a generic thing.
		/// </summary>
		public Thing()
		{
			this.name         = this.GetGenericName();
			this.description  = this.GetGenericDescription();
			this.parsedNames  = this.GetGenericParsedNames();
			this.isSpecific   = false;
			this.isPlural     = this.GetGenericIsPlural();
			this.isProper     = false;
			this.isGeneric    = true;
			this.canBeTaken   = this.GetGenericCanBeTaken();
			this.canBeDropped = true;
			this.isTwoHanded  = this.GetGenericIsTwoHanded();
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for the <see cref="Thing"/>'s current location.
		/// </summary>
		/// <returns>
		/// a reference to the <see cref="Container"/> that contains the
		/// <see cref="Thing"/>
		/// </returns>
		public Container GetLocation()
		{
			return this.location;
		}

		/// <summary>
		/// Accessor for the <see cref="Actor"/> that owns the
		/// <see cref="Thing"/>.
		/// </summary>
		/// <returns>
		/// a reference to the <see cref="Actor"/> that owns the 
		/// <see cref="Thing"/>, if this <see cref="Thing"/> is owned;
		/// otherwise, null
		/// </returns>
		public Actor GetOwner()
		{
			return this.owner;
		}

		/// <summary>
		/// Accessor for the <see cref="Thing"/>'s name.
		/// </summary>
		/// <returns>the name of the <see cref="Thing"/></returns>
		public virtual string GetName()
		{
			return this.name;
		}

		/// <summary>
		/// Accessor for the <see cref="Thing"/>'s description. It is intended that derived
		/// classes override this with more sophisticated functionality.
		/// </summary>
		/// <returns>a brief description of the <see cref="Thing"/></returns>
		public virtual string GetDescription()
		{
			return this.description;
		}

		/// <summary>
		/// Determines the appropriate article for the <see cref="Thing"/>.
		/// </summary>
		/// <returns>an article</returns>
		public string GetAppropriateArticle()
		{
			if (this.isProper) { return null; }
			else if (this.isSpecific) { return "the"; }
			else if (this.isPlural) { return "some"; }
			else if (StringManipulator.StartsWithVowel(this.GetName())) { return "an"; }
			else { return "a"; }
		}

		/// <summary>
		/// Gets the name of the <see cref="Thing"/> preceded by an
		/// appropriate article.
		/// </summary>
		/// 
		/// <returns>
		/// the name of the <see cref="Thing"/>, preceded by an article
		/// </returns>
		public string GetQualifiedName()
		{
			if (this.GetAppropriateArticle() == null) { return this.GetName(); }
			else { return this.GetAppropriateArticle() + ' ' + this.GetName(); }
		}

		/// <summary>
		/// Gets the name of the <see cref="Thing"/> preceded by the word "the"
		/// unless the <see cref="Thing"/> is proper-named, in which case it
		/// just returns the name of the <see cref="Thing"/>.
		/// </summary>
		/// 
		/// <returns>
		/// the name of the <see cref="Thing"/>, preceded by "the " if the
		/// <see cref="Thing"/> is not proper-named.
		/// </returns>
		public string GetSpecificName()
		{
			if (this.isProper) {
				return this.GetName(); }
			return "the " + this.GetName();
		}

		/// <summary>
		/// Gets a name for the <see cref="Thing"/> that does not match any of
		/// the names for the things passed as a parameter.
		/// </summary>
		/// <param name="itemSet">the othere things to check against</param>
		/// <returns>
		/// a unique name for this thing, if one exists,
		/// or null if it has no unique name
		/// </returns>
		public string GetUniqueName(Thing[] itemSet)
		{
			HashSet<string> nameSet = new HashSet<string>();

			foreach (Thing item in itemSet) {
				nameSet.UnionWith(item.parsedNames); }

			foreach (string name in this.parsedNames) {
				if (!nameSet.Contains(name)) {
					return name; } }

			return null;
		}

		/// <summary>
		/// Gets an array of the unique names of all of the items
		/// given as a parameter.
		/// </summary>
		/// <param name="itemSet">the items to find unique names for</param>
		/// <returns>an array of the unique names</returns>
		public static string[] GetUniqueNames(List<Thing> itemSet)
		{
			List<string> names = new List<string>();
			foreach (Thing item in itemSet) {
				HashSet<Thing> otherItems = new HashSet<Thing>(itemSet);
				otherItems.Remove(item);
				names.Add(item.GetUniqueName(otherItems.ToArray())); }
			return names.ToArray();
		}

		/// <summary>
		/// The generic name of this class.
		/// </summary>
		/// <returns>the generic name of this class</returns>
		public virtual string GetGenericName()
		{
			return "thing";
		}

		/// <summary>
		/// The generic description of this class.
		/// </summary>
		/// <returns>the generic description of this class</returns>
		public virtual string GetGenericDescription()
		{
			return "A generic " + this.GetGenericName() + '.';
		}

		/// <summary>
		/// Get the list of names to parse for
		/// a generic instance of this class.
		/// </summary>
		/// <returns>list of names to parse</returns>
		public virtual string[] GetGenericParsedNames()
		{
			List<string> list = new List<string>();
			list.Add(name);
			if (!list.Contains(this.GetName())) {
				list.Add(this.GetName()); }
			if (!list.Contains(this.GetQualifiedName())) {
				list.Add(this.GetQualifiedName()); }
			if (!list.Contains(this.GetSpecificName())) {
				list.Add(this.GetSpecificName()); }
			return list.ToArray();
		}

		/// <summary>
		/// Get the default status for "isPlural" for a this class.
		/// </summary>
		/// <returns>the default status for "isPlural"</returns>
		public virtual bool GetGenericIsPlural()
		{
			return false;
		}

		/// <summary>
		/// Get the default status for "canBeTaken" for a this class.
		/// </summary>
		/// <returns>the default status for "canBeTaken"</returns>
		public virtual bool GetGenericCanBeTaken()
		{
			return true;
		}

		/// <summary>
		/// Get the default status for "isTwoHanded" for a this class.
		/// </summary>
		/// <returns>the default status for "isTwoHanded"</returns>
		public virtual bool GetGenericIsTwoHanded()
		{
			return false;
		}

		/// <summary>
		/// Conjugates a given verb, using the <see cref="Thing"/> as the
		/// subject.
		/// </summary>
		/// 
		/// <param name="verb">the verb to conjugate</param>
		/// 
		/// <returns>the conjugated verb</returns>
		public virtual string GetConjugatedVerb(VerbSet verb)
		{
			return VerbSet.GetForm(verb, (this.isPlural) ? PronounSet.GetPluralSet() : PronounSet.GetNeuterSet());
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Determines whether an <see cref="Actor"/> can take this
		/// <see cref="Thing"/>.
		/// </summary>
		/// 
		/// <returns>
		/// true if the <see cref="Thing"/> can be taken;
		/// false if it cannot be taken
		/// </returns>
		public bool CanBeTaken()
		{
			return this.canBeTaken;
		}

		/// <summary>
		/// Determines whether an <see cref="Actor"/> can drop this
		/// <see cref="Thing"/>.
		/// Used for creating cursed/undroppable items.
		/// </summary>
		/// 
		/// <returns>
		/// true if the <see cref="Thing"/> can be dropped;
		/// false if it cannot be dropped
		/// </returns>
		public bool CanBeDropped()
		{
			return this.canBeDropped;
		}

		/// <summary>
		/// Determines whether the <see cref="Thing"/> requires two hands in
		/// order to be carried by a <see cref="Person"/>.
		/// </summary>
		/// 
		/// <returns>
		/// true if the <see cref="Thing"/> requires two hands to be carried by
		/// a <see cref="Person"/>; false otherwise
		/// </returns>
		public bool IsTwoHanded()
		{
			return this.isTwoHanded;
		}

		/// <summary>
		/// Checks if the <see cref="Thing"/>'s name matches the given name.
		/// This function is intended to be overwritten by inherited classes,
		/// so that <see cref="Thing"/>s with multiple names can check to see
		/// if they match a given player command.
		/// </summary>
		/// <param name="name">
		/// the string to check against the thing's name
		/// </param>
		/// <returns>
		/// true if the string matches the name of the thing;
		/// false otherwise
		/// </returns>
		public bool IsNamed(string name)
		{
			foreach (string parsedName in this.parsedNames) {
				if (parsedName.ToLower() == name.ToLower()) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether this is a generic item that can be selected at
		/// random when multiple items match a player's command.
		/// </summary>
		/// <returns></returns>
		public bool IsGeneric()
		{
			return this.isGeneric;
		}

		/// <summary>
		/// Determines whether or not the <see cref="Thing"/> is currently
		/// owned by someone.
		/// </summary>
		/// <returns>
		/// true if the <see cref="Thing"/> has an owner; false otherwise
		/// </returns>
		public bool HasOwner()
		{
			return this.owner != null;
		}

		/// <summary>
		/// Determines whether, in a group of things,
		/// each thing has a unique name.
		/// </summary>
		/// <param name="thingList">the group of things</param>
		/// <returns>
		/// true if each thing has at least one unique name; false otherwise
		/// </returns>
		public static bool CanBeDifferentiated(List<Thing> thingList)
		{
			foreach (Thing item in thingList) {
				bool itemHasUniqueName = false;
				HashSet<Thing> tempSet = new HashSet<Thing>(thingList);
				tempSet.Remove(item);
				HashSet<string> names = new HashSet<string>();

				foreach(Thing otherItem in tempSet) {
					names.UnionWith(otherItem.parsedNames); }

				foreach (string name in item.parsedNames) {
					if (!names.Contains(name)) {
						itemHasUniqueName = true; } }

				if (!itemHasUniqueName) {
					return false; }
			}

			return true;
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// Mutator for a <see cref="Thing"/>'s current location.
		/// Intended to be overrided by its children with more secure mutators.
		/// </summary>
		/// <param name="location">the new location for the thing</param>
		public void SetLocation(Container location)
		{
			this.location = location;
		}

		/// <summary>
		/// Sets the owner of the <see cref="Thing"/> to the specified
		/// <see cref="Actor"/>.
		/// </summary>
		/// <param name="owner">
		/// the <see cref="Actor"/> to give ownership to
		/// </param>
		public void SetOwner(Actor owner)
		{
			this.owner = owner;
		}

		/// <summary>
		/// Add another name to the list of names that the parser recognizes
		/// as referring to this <see cref="Thing"/>.
		/// </summary>
		/// <param name="name">the name to add</param>
		public void AddParsedName(string name)
		{
			HashSet<string> temp = new HashSet<string>(this.parsedNames);
			temp.Add(name);
			this.parsedNames = temp.ToArray();
		}

		/// <summary>
		/// Remove a name from the list of names that the parser recognizes
		/// as referring to this <see cref="Thing"/>.
		/// </summary>
		/// <param name="name">the name to remove</param>
		public void RemoveParsedName(string name)
		{
			HashSet<string> temp = new HashSet<string>(this.parsedNames);
			temp.Remove(name);
			this.parsedNames = temp.ToArray();
		}

		/// <summary>
		/// Change a name on the list of names that the parser recognizes
		/// as referring to this <see cref="Thing"/> to a different name.
		/// </summary>
		/// <param name="name">the name to change</param>
		public void ChangeParsedName(string oldName, string newName)
		{
			this.RemoveParsedName(oldName);
			this.AddParsedName(newName);
		}

	}
}

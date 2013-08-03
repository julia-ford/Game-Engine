using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Interfaces;
using Meta.ParsingAndPrinting;

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
		/// The name of the <see cref="Thing"/>.
		/// </summary>
		private string name;

		/// <summary>
		/// A brief description of the <see cref="Thing"/>.
		/// </summary>
		private string description;

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
		/// Indicates whether or not the <see cref="Thing"/> can be taken.
		/// </summary>
		private bool canBeTaken;

		/// <summary>
		/// Indicates whether or not the <see cref="Thing"/> can be dropped.
		/// Used for creating cursed/undroppable items.
		/// </summary>
		private bool canBeDropped;

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
		public Thing(string name, string description,
			bool isSpecific = false, bool isPlural = false, bool isProper = false,
			bool canBeTaken = true, bool canBeDropped = true)
		{
			this.name         = name;
			this.description  = description;
			this.isSpecific   = isSpecific;
			this.isPlural     = isPlural;
			this.isProper     = isProper;
			this.canBeTaken   = canBeTaken;
			this.canBeDropped = canBeDropped;
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for the <see cref="Thing"/>'s current location.
		/// </summary>
		/// <returns>a reference to the <see cref="Container"/> that contains the <see cref="Thing"/></returns>
		public Container GetLocation()
		{
			return this.location;
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
		/// Gets the name of the <see cref="Thing"/> preceded by an appropriate article.
		/// </summary>
		/// <returns>the name of the <see cref="Thing"/>, preceded by an article</returns>
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
		/// <returns></returns>
		public string GetSpecificName()
		{
			if (this.isProper) {
				return this.GetName(); }
			return "the " + this.GetName();
		}

		/// <summary>
		/// Conjugates a given verb, using the <see cref="Thing"/> as the subject.
		/// </summary>
		/// <param name="verb">the verb to conjugate</param>
		/// <returns>the conjugated verb</returns>
		public virtual string GetConjugatedVerb(VerbSet verb)
		{
			return VerbSet.GetForm(verb, (this.isPlural) ? PronounSet.GetPluralSet() : PronounSet.GetNeuterSet());
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Determines whether an <see cref="Actor"/> can take this <see cref="Thing"/>.
		/// </summary>
		/// <returns>true if the <see cref="Thing"/> can be taken; false if it cannot be taken</returns>
		public bool CanBeTaken()
		{
			return this.canBeTaken;
		}

		/// <summary>
		/// Determines whether an <see cref="Actor"/> can drop this <see cref="Thing"/>.
		/// Used for creating cursed/undroppable items.
		/// </summary>
		/// <returns>true if the <see cref="Thing"/> can be dropped; false if it cannot be dropped</returns>
		public bool CanBeDropped()
		{
			return this.canBeDropped;
		}

		/// <summary>
		/// Checks if the <see cref="Thing"/>'s name matches the given name.
		/// This function is intended to be overwritten by inherited classes,
		/// so that <see cref="Thing"/>s with multiple names can check to see
		/// if they match a given player command.
		/// </summary>
		/// <param name="name">the string to check against the thing's name</param>
		/// <returns>true if the string matches the name of the thing; false otherwise</returns>
		public virtual bool IsNamed(string name)
		{
			if (String.Equals(name, this.name, StringComparison.OrdinalIgnoreCase)) {
				return true; }
			else if (String.Equals(name, this.GetName(), StringComparison.OrdinalIgnoreCase)) {
				return true; }
			else if (String.Equals(name, this.GetQualifiedName(), StringComparison.OrdinalIgnoreCase)) {
				return true; }
			else if (String.Equals(name, this.GetSpecificName(), StringComparison.OrdinalIgnoreCase)) {
				return true; }
			return false;
		}

		/// <summary>
		/// The idea behind this function is that if there is no way for
		/// the player to tell the difference between two things, the parser
		/// should just randomly pick one when deciding how to parse the command.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual bool CannotBeDistinguishedFrom(Thing other)
		{
			if (this.GetType() != other.GetType()) {
				return false; }

			if (this.name != other.name) {
				return false; }

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

	}
}

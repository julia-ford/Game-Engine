using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things.Containers;
using Meta.ParsingAndPrinting;
using Meta.Exceptions;
using Meta.Exceptions.MovingExceptions;
using Meta.Exceptions.OpeningAndClosingExceptions;
using Meta.Exceptions.PuttingIntoExceptions;
using Meta.Exceptions.TakingAndDroppingExceptions;
using Interfaces;

namespace Stuff.Things.Actors
{
	/// <summary>
	/// Represents any entity that can move around the game world
	/// and interact with <see cref="Thing"/>s.
	/// </summary>
	public abstract class Actor : Thing, Searchable
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// the set of pronouns to be used when printing information
		/// about the <see cref="Actor"/>
		/// </summary>
		private PronounSet pronouns;

		// TODO: I am seriously considering doing away with the inventory.
		// If I do, actors will be allowed to pick one thing up in each hand,
		// with some items being flagged as requiring both hands,
		// and everything else will have to be stored in containers.
		// Pockets, bandoliers, backpacks, sheath, belts, etc.

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new <see cref="Actor"/> with the specified name, description, and <see cref="PronounSet"/>.
		/// Default state for canBeTaken is false.
		/// </summary>
		/// <param name="name">the name of the <see cref="Actor"/></param>
		/// <param name="description">a brief description of the <see cref="Actor"/></param>
		/// <param name="pronouns">the set of pronouns to be used for the <see cref="Actor"/></param>
		public Actor(string name, string description, PronounSet pronouns,
			string[] parsedNames = null,
			bool isSpecific = false, bool isPlural = false, bool isProper = false,
			bool canBeTaken = false, bool canBeDropped = true, bool isTwoHanded = true)
			: base(name, description, parsedNames,
			isSpecific:isSpecific, isPlural:isPlural, isProper:isProper,
			canBeTaken:canBeTaken, canBeDropped:canBeDropped, isTwoHanded:isTwoHanded)
		{
			this.pronouns = pronouns;
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for the <see cref="Actor"/>'s subject pronoun.
		/// </summary>
		/// <returns>the <see cref="Actor"/>'s subject pronoun</returns>
		public string GetSubjectPronoun()
		{
			return this.pronouns.GetSubjectForm();
		}

		/// <summary>
		/// Accessor for the <see cref="Actor"/>'s direct/indirect object pronoun.
		/// </summary>
		/// <returns>the <see cref="Actor"/>'s (in)direct object pronoun</returns>
		public string GetObjectPronoun()
		{
			return this.pronouns.GetObjectForm();
		}

		/// <summary>
		/// Accessor for the <see cref="Actor"/>'s possessive pronoun.
		/// </summary>
		/// <returns>the <see cref="Actor"/>'s possessive pronoun</returns>
		public string GetPossessivePronoun()
		{
			return this.pronouns.GetPossessiveForm();
		}

		/// <summary>
		/// Accessor for the <see cref="Actor"/>'s possessive direct/indirect object pronoun.
		/// </summary>
		/// <returns>the <see cref="Actor"/>'s possessive (in)direct pronoun</returns>
		public string GetPossessiveObjectPronoun()
		{
			return this.pronouns.GetPossessiveObjectForm();
		}

		/// <summary>
		/// Accessor for the <see cref="Actor"/>'s reflexive pronoun.
		/// </summary>
		/// <returns>the <see cref="Actor"/>'s relfexive pronoun</returns>
		public string GetReflexivePronoun()
		{
			return this.pronouns.GetReflexiveForm();
		}

		/// <summary>
		/// Gets a HashSet of all of the <see cref="Actor"/>'s possessions.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of all the items that the <see cref="Actor"/> has
		/// possession of
		/// </returns>
		public abstract HashSet<Thing> GetContents();

		/// <summary>
		/// Gets a HashSet of all of the <see cref="Actor"/>'s visible
		/// possessions.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of all the visible items that the <see cref="Actor"/>
		/// has possession of
		/// </returns>
		public abstract HashSet<Thing> GetVisibleContents();

		/// <summary>
		/// Get a HashSet of all of the <see cref="Actor"/>'s possessions,
		/// recursively.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of all the items that the <see cref="Actor"/> has
		/// possession of
		/// </returns>
		public abstract HashSet<Thing> GetRecursiveContents();

		/// <summary>
		/// Gets a HashSet of all of the <see cref="Actor"/>'s visible
		/// possessions, recursively.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of all the visible items that the <see cref="Actor"/>
		/// has possession of
		/// </returns>
		public abstract HashSet<Thing> GetVisibleRecursiveContents();

		/// <summary>
		/// This override makes the function use the <see cref="Actor"/>'s own
		/// <see cref="PronounSet"/> for selecting the verb form.
		/// </summary>
		/// <param name="verb">the verb to conjugate</param>
		/// <returns>the conjugated verb</returns>
		public override string GetConjugatedVerb(VerbSet verb)
		{
			return VerbSet.GetForm(verb, this.pronouns);
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		// none yet

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// Moves the <see cref="Actor"/> in the given <see cref="Direction"/>.
		/// </summary>
		/// <param name="dir">the <see cref="Direction"/> to move in</param>
		/// <exception cref="MovingDirectionallyFromNullException">if the <see cref="Actor"/>'s location is null</exception>
		/// <exception cref="MovingInInvalidDirectionException">if there is no <see cref="Room"/> in the specified <see cref="Direction"/></exception>
		public void Move(Direction dir)
		{
			// if the item's current location is "null," throw an exception
			if(this.GetLocation() == null) {
				throw new MovingDirectionallyFromNullException(); }
			// if the thing trying to move is in a container instead of a room,
			// throw an exception for now until I add vehicle code
			else if (!typeof(Room).IsAssignableFrom(this.GetLocation().GetType())) {
				throw new Exception("Vehicle movement not coded yet."); }
			// if there is no room in the given direction, throw an exception
			else if (!((Room)(this.GetLocation())).HasAdjacent(dir)) {
				throw new MovingInInvalidDirectionException(); }
			// otherwise, move normally
			else {
				this.GetLocation().RemoveThing(this);
				this.SetLocation(((Room)(this.GetLocation())).GetAdjacent(dir));
				this.GetLocation().AddThing(this);
				// display the new room description when the player moves
				if (GameManager.IsPlayer(this)) {
					GameManager.Look(); }
				else {
					GameManager.ReportIfVisible(this, StringManipulator.CapitalizeFirstLetter(this.GetQualifiedName()) + ' ' + VerbSet.GetForm(VerbSet.ToGo, this.pronouns) + ' ' + dir + '.'); }
			} // end normal movement
		}

		/// <summary>
		/// Has the <see cref="Actor"/> try to take a <see cref="Thing"/>
		/// that they can see.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to take</param>
		public abstract void Take(Thing item);

		/// <summary>
		/// Has the <see cref="Actor"/> try to drop
		/// a given <see cref="Thing"/>.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to drop</param>
		public abstract void Drop(Thing item);

		/// <summary>
		/// Has the <see cref="Actor"/> try to open
		/// a given <see cref="Thing"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to open</param>
		/// <exception cref="InteractingWithUnseenThingException">
		/// if the <see cref="Actor"/> cannot see the <see cref="Thing"/>
		/// </exception>
		/// <exception cref="OpeningSomethingBesidesOpenableException">
		/// if the <see cref="Thing"/> is not <see cref="Openable"/>
		/// </exception>
		public void Open(Thing item)
		{
			// if the item is not visible, throw an exception
			if (!GameManager.CanSee(this, item)) {
				throw new InteractingWithUnseenThingException(); }

			// if the item is not openable, throw an exception
			else if (!typeof(Openable).IsAssignableFrom(item.GetType())) {
				throw new OpeningSomethingBesidesOpenableException(); }

			// the item is visible and openable; execute as normal
			else {
				// TODO: Make sure the item being opened isn't someone else's clothing.
				((Openable)item).Open();
				GameManager.ReportIfVisible(this, this.GetSpecificName() + ' ' + this.GetConjugatedVerb(VerbSet.ToOpen) + ' ' + item.GetSpecificName() + '.'); }
		}

		/// <summary>
		/// Has the <see cref="Actor"/> try to close
		/// a given <see cref="Thing"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to close</param>
		/// <exception cref="InteractingWithUnseenThingException">
		/// if the <see cref="Actor"/> cannot see the <see cref="Thing"/>
		/// </exception>
		/// <exception cref="OpeningSomethingBesidesOpenableException">
		/// if the <see cref="Thing"/> is not <see cref="Openable"/>
		/// </exception>
		public void Close(Thing item)
		{
			// if the item is not visible, throw an exception
			if (!GameManager.CanSee(this, item)) {
				throw new InteractingWithUnseenThingException(); }

			// if the item is not openable, throw an exception
			else if (!typeof(Openable).IsAssignableFrom(item.GetType())) {
				throw new ClosingSomethingBesidesOpenableException(); }

			// the item is visible and openable; execute as normal
			else {
				// TODO: Make sure the item being closed isn't someone else's clothing.
				((Openable)item).Close();
				GameManager.ReportIfVisible(this, VerbSet.ToClose, item); }
		}

	}
}

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
using Meta.Exceptions.TakingExceptions;
using Interfaces;

namespace Stuff.Things.Actors
{
	/// <summary>
	/// Represents any entity that can move around the game world
	/// and interact with <see cref="Thing"/>s.
	/// </summary>
	public class Actor : Thing, Searchable
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// the set of pronouns to be used when printing information
		/// about the <see cref="Actor"/>
		/// </summary>
		private PronounSet pronouns;

		/// <summary>
		/// all of the <see cref="Thing"/>s that an <see cref="Actor"/>
		/// is currently holding
		/// </summary>
		private Inventory inventory;

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
			bool isSpecific = false, bool isPlural = false, bool isProper = false,
			bool canBeTaken = false) : base(name, description, isSpecific, isPlural, isProper, canBeTaken)
		{
			this.pronouns = pronouns;
			this.inventory = new Inventory(this);
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
		/// Accessor for the <see cref="Actor"/>'s <see cref="Inventory"/>.
		/// </summary>
		/// <returns>a new HashSet containing all the <see cref="Thing"/>s in the <see cref="Actor"/>'s <see cref="Inventory"/></returns>
		public HashSet<Thing> GetInventory()
		{
			return new HashSet<Thing>(this.inventory.GetContents());
		}

		/// <summary>
		/// Get the contents of the <see cref="Actor"/>'s inventory.
		/// </summary>
		/// <returns>a HashSet of the contents of the <see cref="Actor"/>'s <see cref="Inventory"/></returns>
		public virtual HashSet<Thing> GetContents()
		{
			return this.inventory.GetContents();
		}

		/// <summary>
		/// Get the visible contents of the <see cref="Actor"/>'s <see cref="Inventory"/>.
		/// </summary>
		/// <returns>
		/// a HashSet of the visible contents of the <see cref="Actor"/>'s
		/// <see cref="Inventory"/>
		/// </returns>
		public virtual HashSet<Thing> GetVisibleContents()
		{
			return this.inventory.GetVisibleContents();
		}

		/// <summary>
		/// Get the contents of the <see cref="Actor"/>'s <see cref="Inventory"/>, recursively.
		/// </summary>
		/// <returns>a HashSet of the recursive contents of the <see cref="Actor"/>'s <see cref="Inventory"/></returns>
		public virtual HashSet<Thing> GetRecursiveContents()
		{
			return this.inventory.GetRecursiveContents();
		}

		/// <summary>
		/// Get the visible contents of the <see cref="Actor"/>'s <see cref="Inventory"/>, recursively.
		/// </summary>
		/// <returns>a HashSet of the visible recursive contents of the <see cref="Actor"/>'s <see cref="Inventory"/></returns>
		public virtual HashSet<Thing> GetVisibleRecursiveContents()
		{
			return this.inventory.GetVisibleRecursiveContents();
		}

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

		/// <summary>
		/// Checks if a given <see cref="Thing"/> is in the 
		/// <see cref="Actor"/>'s <see cref="Inventory"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to check for</param>
		/// <returns>
		/// true if the given <see cref="Thing"/> is in the
		/// <see cref="Actor"/>'s <see cref="Inventory"/>; false otherwise
		/// </returns>
		public bool Carries(Thing item)
		{
			return this.inventory.Contains(item);
		}

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
		/// <param name="item">the <see cref="Thing"/> to take</param>
		/// <exception cref="TakingItemFlaggedUntakeableException">
		/// if the <see cref="Thing"/> has been flagged as untakeable
		/// </exception>
		/// <exception cref="TakingThingNotVisibleException">
		/// if the <see cref="Actor"/> cannot see the <see cref="Thing"/>
		/// </exception>
		public void Take(Thing item)
		{
			// if the actor can't see the item, throw an exception
			if (!GameManager.CanSee(this, item)) {
				throw new InteractingWithUnseenThingException(); }

			// if the item can never be taken, throw an exception
			else if (!item.CanBeTaken()) {
				throw new TakingItemFlaggedUntakeableException(); }

			// if the actor is already carrying the item, throw an exception
			else if (this.Carries(item)) {
				throw new TakingItemAlreadyHeldException(); }

			// if the actor is a person who is wearing the item,
			// throw an exception for now
			else if (typeof(Person).IsAssignableFrom(this.GetType())
				&& ((Person)this).Wears(item)) {
				throw new Exception("Taking off is not coded for yet."); }

			// if the item is worn by someone else or is inside something
			// worn by someone else, throw an exception, for now
			else if (typeof(Clothes) == GameManager.GetTopMostVisibleContainer(item.GetLocation()).GetType()) {
				throw new Exception("Theft has not been coded yet."); }

			// The actor can see the item, and it is not being worn or carried.
			// This is the "happy path." Execute as normal.
			else {
				item.GetLocation().RemoveThing(item);
				item.SetLocation(this.inventory);
				this.inventory.AddThing(item);
				GameManager.ReportIfVisible(this, StringManipulator.CapitalizeFirstLetter(this.GetQualifiedName()) + ' ' + VerbSet.GetForm(VerbSet.ToTake, this.pronouns) + ' ' + item.GetSpecificName() + '.'); }
		}

		/// <summary>
		/// Has the <see cref="Actor"/> try to drop
		/// a given <see cref="Thing"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to drop</param>
		/// <exception cref="DroppingItemNotHeldException">
		/// if the <see cref="Thing"/> is not in the <see cref="Actor"/>'s
		/// <see cref="Inventory"/>
		/// </exception>
		/// <exception cref="DroppingCursedUndroppableItemException">
		/// if the <see cref="Thing"/> is cursed/undroppable
		/// </exception>
		public void Drop(Thing item)
		{
			// if the actor isn't carrying the item, throw an exception
			if(!this.Carries(item)) {
				throw new DroppingItemNotHeldException(); }

			// if the item is cursed/undroppable, throw an exception
			if(!item.CanBeDropped()) {
				throw new DroppingCursedUndroppableItemException(); }

			// actor is carrying the item and it is not cursed
			else {
				this.inventory.RemoveThing(item);
				item.SetLocation(this.GetLocation());
				this.GetLocation().AddThing(item);
				GameManager.ReportIfVisible(this, this.GetName() + ' ' +  this.GetConjugatedVerb(VerbSet.ToDrop) + ' ' + item.GetSpecificName() + '.'); }
		}

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

		/// <summary>
		/// Has the <see cref="Actor"/> try to put
		/// a given <see cref="Thing"/> into a given
		/// <see cref="Container"/>.
		/// </summary>
		/// <param name="item">the <see cref="Thing"/> to place</param>
		/// <param name="container">the <see cref="Container"/> to put it into</param>
		public void PutInto(Thing item, Thing container)
		{
			// if the actor can't see the item, throw an exception
			if(!GameManager.CanSee(this, item)) {
				throw new InteractingWithUnseenThingException(); }

			// if the actor can't see the container, throw an exception
			if(!GameManager.CanSee(this, container)) {
				throw new InteractingWithUnseenThingException(); }

			// if the container is not actually a Container, throw an exception
			if(!typeof(Container).IsAssignableFrom(container.GetType())) {
				throw new PuttingIntoNonContainerException(); }

			// if the item is already in the containter, throw an exception
			if (item.GetLocation() == container) {
				throw new PuttingItemAlreadyInsideException(); }

			// if the item is the container, throw an exception
			if (item == container) {
				throw new PuttingItemIntoItselfException(); }

			// if the actor isn't carrying the item, try taking it instead
			if(!this.Carries(item)) {
				// if the actor is the player, report the auto-correction
				if (GameManager.IsPlayer(this)) {
					GameManager.ReportIfVisible(this, "((You aren't carrying " + item.GetSpecificName() + ". Trying to take it instead...))"); }
				this.Take(item);
				return; }

			// if the container is closed, try to open it instead
			if (typeof(OpenableContainer).IsAssignableFrom(container.GetType()) && !((OpenableContainer)container).IsOpen()) {
				// if the actor is the player, report the auto-correction
				if (GameManager.IsPlayer(this)) {
					GameManager.ReportIfVisible(this, "((" + StringManipulator.CapitalizeFirstLetter(container.GetSpecificName()) + " is currently closed. Trying to open it instead..."); }
				this.Open(container);
				return; }

			// TODO: Should actors automatically try to unlock containers?

			// if nothing went wrong, execute normally
			item.GetLocation().RemoveThing(item);
			((Container)container).AddThing(item);
			item.SetLocation((Container)container);
			GameManager.ReportIfVisible(this, VerbSet.ToPut, item, " into ", container);
		}

		//===================================================================//
		//                           Protected                               //
		//===================================================================//

		/// <summary>
		/// This method allows the derived class, <see cref="Person"/>, and any
		/// other derived classes I end up making, to modify their
		/// <see cref="Inventory"/>  via other functions. For example,
		/// <see cref="Person.TakeOff"/> needs to be able to move an item of
		/// <see cref="Clothing"/> from a <see cref="Person"/>'s
		/// <see cref="Clothes"/> to their <see cref="Inventory"/>.
		/// 
		/// This method is completely unchecked, save for the checks performed
		/// by any of the functions it calls.
		/// 
		/// This method does all three parts of moving the item:
		/// it removes the item from its old location,
		/// adds the item to the new location,
		/// and updates the item's location.
		/// </summary>
		/// <param name="item">
		/// the <see cref="Thing"/> to add to the <see cref="Actor"/>'s
		/// <see cref="Inventory"/>
		/// </param>
		protected void AddToInventory(Thing item)
		{
			item.GetLocation().RemoveThing(item);
			this.inventory.AddThing(item);
			item.SetLocation(this.inventory);
		}

	}
}

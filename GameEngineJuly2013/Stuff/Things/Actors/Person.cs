using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things.Containers;
using Stuff.Things.Clothings;
using Meta.ParsingAndPrinting;
using Meta.Exceptions;
using Meta.Exceptions.TakingAndDroppingExceptions;
using Meta.Exceptions.PuttingIntoExceptions;
using Meta.Exceptions.WearingAndTakingOffExceptions;
using Interfaces;

namespace Stuff.Things.Actors
{
	/// <summary>
	/// This class represents a <see cref="Person"/> specifically,
	/// as opposed to an animal or a monster.
	/// A <see cref="Person"/> is assumed to be a bipedal humanoid.
	/// </summary>
	public class Person : Actor
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// All of the items of <see cref="Clothing"/> that the
		/// <see cref="Person"/> is wearing.
		/// </summary>
		private Clothes clothes;

		/// <summary>
		/// Represents the <see cref="Person"/>'s hands.
		/// </summary>
		private Hands hands;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new <see cref="Person"/> with the specified name,
		/// description, and set of pronouns.
		/// </summary>
		/// <param name="name">the name of the <see cref="Person"/></param>
		/// <param name="description">a brief description of the <see cref="Person"/></param>
		/// <param name="pronouns">the set of pronouns to be used for the <see cref="Person"/></param>
		public Person(string name, string description, PronounSet pronouns,
			string[] parsedNames = null,
			bool isSpecific=false, bool isPlural=false, bool isProper=true)
			: base(name, description, pronouns, parsedNames,
			isSpecific: isSpecific, isPlural: isPlural, isProper: isProper,
			canBeTaken: false)
		{
			this.clothes = new Clothes(this);
			this.hands   = new Hands(this);

			this.clothes.AddThing(
				new PantsWithTwoPockets(
					pronouns == PronounSet.GetSecondPersonSet() ? "your pants" : !isProper ? "the " : "" + name + "'s pants",
					"A pair of pants. It has two pockets: a left pocket, and a right pocket.",
					pronouns == PronounSet.GetSecondPersonSet() ? new string[] { "my pants", "pants" } : new string[] { !isProper ? "the " : "" + name + "'s pants", "pants" },
					isProper:true));
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for a <see cref="Person"/>'s <see cref="Clothes"/>.
		/// </summary>
		/// <returns>a set of all the <see cref="Clothing"/> a person is wearing</returns>
		public HashSet<Clothing> GetClothes()
		{
			HashSet<Clothing> tempClothes =  new HashSet<Clothing>();
			foreach (Clothing item in this.clothes.GetContents())
			{
				tempClothes.Add(item);
			}
			return tempClothes;
		}

		/// <summary>
		/// Returns a set of all the visible <see cref="Clothing"/> that a
		/// <see cref="Person"/> is wearing.
		/// </summary>
		/// <returns>
		/// a set of all the visible <see cref="Clothing"/> that a
		/// <see cref="Person"/> is wearing
		/// </returns>
		public HashSet<Clothing> GetVisibleClothes()
		{
			HashSet<Clothing> visibleClothes = new HashSet<Clothing>();

			foreach (Clothing item in this.clothes.GetContents())
			{
				if (item.HasType(ClothingSlot.bracelet) || item.HasType(ClothingSlot.cincher) || item.HasType(ClothingSlot.glasses) || item.HasType(ClothingSlot.gloves) || item.HasType(ClothingSlot.hat) || item.HasType(ClothingSlot.jacket) || item.HasType(ClothingSlot.necklace) || item.HasType(ClothingSlot.pants) || item.HasType(ClothingSlot.ring) || item.HasType(ClothingSlot.shirt) || item.HasType(ClothingSlot.shoes) || item.HasType(ClothingSlot.skirt) || item.HasType(ClothingSlot.socks) || item.HasType(ClothingSlot.tights))
				{
					visibleClothes.Add(item);
				}
				else if(item.HasType(ClothingSlot.undershirt) && !this.WearsSomethingOfType(ClothingSlot.shirt))
				{
					visibleClothes.Add(item);
				}
				else if (item.HasType(ClothingSlot.underpants) && !(this.WearsSomethingOfType(ClothingSlot.tights) || this.WearsSomethingOfType(ClothingSlot.skirt) || this.WearsSomethingOfType(ClothingSlot.pants)))
				{
					visibleClothes.Add(item);
				}
			}
			return visibleClothes;
		}

		/// <summary>
		/// This override makes the description of the <see cref="Person"/>
		/// describe their visible <see cref="Clothing"/>.
		/// </summary>
		/// <returns>a description of the <see cref="Person"/></returns>
		public override string GetDescription()
		{
			return this.DescribeVisibleClothes() + ' ' +
				this.DescribeCarriedItems();
		}

		/// <summary>
		/// Gets the contents of the <see cref="Person"/>'s hands and
		/// <see cref="Clothes"/>.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of the contents of the <see cref="Person"/>'s hands and
		/// <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetContents()
		{
			HashSet<Thing> temp =
				new HashSet<Thing>(this.clothes.GetContents());

			temp.UnionWith(hands.GetContents());

			return temp;
		}

		/// <summary>
		/// Gets the visible contents of the <see cref="Person"/>'s hands and
		/// <see cref="Clothes"/>.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of the visible contents of the <see cref="Person"/>'s
		/// hands and <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetVisibleContents()
		{
			HashSet<Thing> temp =
				new HashSet<Thing>(this.clothes.GetVisibleContents());

			temp.UnionWith(this.hands.GetVisibleContents());
			
			// TODO: Add code for concealed carried items?

			return (temp);
		}

		/// <summary>
		/// Get the contents of the <see cref="Person"/>'s hands and
		/// <see cref="Clothes"/>, recursively.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of the recursive contents of the <see cref="Person"/>'s
		/// hands and <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetRecursiveContents()
		{
			HashSet<Thing> temp =
				new HashSet<Thing>(this.clothes.GetRecursiveContents());

			temp.UnionWith(this.hands.GetRecursiveContents());

			return (temp);
		}

		/// <summary>
		/// Get the visible contents of the <see cref="Person"/>'s hands and
		/// <see cref="Clothes"/>, recursively.
		/// </summary>
		/// 
		/// <returns>
		/// a HashSet of the visible recursive contents of the
		/// <see cref="Person"/>'s hands and <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetVisibleRecursiveContents()
		{
			// first, add the clothes
			HashSet<Thing> temp =
				new HashSet<Thing>(this.clothes.GetVisibleRecursiveContents());

			temp.UnionWith(this.hands.GetVisibleRecursiveContents());

			return (temp);
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Checks if the <see cref="Person"/> is wearing the given
		/// <see cref="Clothing"/>.
		/// </summary>
		/// <param name="item">the <see cref="Clothing"/> to check for</param>
		/// <returns>
		/// true if the <see cref="Person"/> is wearing the
		/// <see cref="Clothing"/>; false otherwise
		/// </returns>
		public bool Wears(Thing item)
		{
			return this.clothes.Contains(item);
		}

		/// <summary>
		/// Determines if the <see cref="Person"/> is wearing some
		/// <see cref="Clothing"/> of the specified type.
		/// </summary>
		/// <param name="slot">the type to check for</param>
		/// <returns>
		/// true if the <see cref="Person"/> wears something of the specifed
		/// type; false otherwise
		/// </returns>
		public bool WearsSomethingOfType(ClothingSlot slot)
		{
			foreach (Clothing item in this.clothes.GetContents()) {
				if (item.HasType(slot)) {
					return true; }
			} // end foreach
			return false;
		}

		/// <summary>
		/// Checks if a given <see cref="Thing"/> is in one or both of a 
		/// <see cref="Person"/>'s hands.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to check for</param>
		/// 
		/// <returns>
		/// true if the given <see cref="Thing"/> is in one or both of the
		/// <see cref="Person"/>'s hands; false otherwise
		/// </returns>
		public bool Carries(Thing item)
		{
			return this.hands.Contains(item);
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//
		
		/// <summary>
		/// Has the <see cref="Person"/> try to take a <see cref="Thing"/>
		/// that they can see.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to take</param>
		/// 
		/// <exception cref="InteractingWithUnseenThingException">
		/// if the <see cref="Person"/> cannot see the <see cref="Thing"/>
		/// </exception>
		/// 
		/// <exception cref="TakingItemFlaggedUntakeableException">
		/// if the <see cref="Thing"/> has been flagged as untakeable
		/// </exception>
		/// 
		/// <exception cref="TakingWithHandsFullException">
		/// if the <see cref="Person"/>'s hands are too full
		/// </exception>
		public override void Take(Thing item)
		{
			// if the person can't see the item, throw an exception
			if (!GameManager.CanSee(this, item)) {
				throw new InteractingWithUnseenThingException(); }

			// if the item can never be taken, throw an exception
			else if (!item.CanBeTaken()) {
				throw new TakingItemFlaggedUntakeableException(); }

			// if the person is already carrying the item, throw an exception
			else if (this.Carries(item)) {
				throw new TakingItemAlreadyHeldException(); }

			// if the personis wearing the item, try taking it off instead
			else if (this.Wears(item)) {
				// if the person is the player, report the auto-correction
				if(GameManager.IsPlayer(this)) {
					GameManager.ReportIfVisible(this,
						"((I think you meant \"take off\" instead of " +
						"\"take.\" Trying that instead...))"); }
				this.TakeOff(item); }

			// if the item is worn by someone else or is inside something
			// worn by someone else, throw an exception, for now
			else if (typeof(Clothes) == GameManager.GetTopMostVisibleContainer(
				item.GetLocation()).GetType()) {
				throw new Exception("Theft has not been coded yet."); }

			// if neither hand is free or if only one hand is free and the item
			// requires two hands, throw an exception
			else if (!hands.HasFreeHand() ||
				(item.IsTwoHanded() && !hands.IsEmpty())) {
				throw new TakingWithHandsFullException(); }

			// No problems encountered; execute normally.
			else {
				item.GetLocation().RemoveThing(item);
				this.hands.AddThing(item);
				item.SetLocation(this.hands);
				GameManager.ReportIfVisible(this,
					StringManipulator.CapitalizeFirstLetter(
					this.GetQualifiedName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToPickUp) + ' ' +
					item.GetSpecificName() + '.'); }
		}

		/// <summary>
		/// Has the <see cref="Person"/> try to drop
		/// a given <see cref="Thing"/>.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to drop</param>
		/// 
		/// <exception cref="DroppingItemNotHeldException">
		/// if the <see cref="Thing"/> is not in the <see cref="Person"/>'s
		/// <see cref="Hands"/>
		/// </exception>
		/// 
		/// <exception cref="DroppingCursedUndroppableItemException">
		/// if the <see cref="Thing"/> is cursed/undroppable
		/// </exception>
		public override void Drop(Thing item)
		{
			// if the actor isn't carrying the item, throw an exception
			if(!this.Carries(item)) {
				throw new DroppingItemNotHeldException(); }

			// if the item is cursed/undroppable, throw an exception
			if(!item.CanBeDropped()) {
				throw new DroppingCursedUndroppableItemException(); }

			// actor is carrying the item and it is not cursed
			else {
				this.hands.RemoveThing(item);
				item.SetLocation(this.GetLocation());
				this.GetLocation().AddThing(item);
				GameManager.ReportIfVisible(this, VerbSet.ToDrop, item); }
		}
		
		/// <summary>
		/// Has the <see cref="Actor"/> try to put
		/// a given <see cref="Thing"/> into a given
		/// <see cref="Container"/>.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to place</param>
		/// 
		/// <param name="container">
		/// the <see cref="Container"/> to put it into
		/// </param>
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
					GameManager.ReportIfVisible(this, "((You aren't carrying "
						+ item.GetSpecificName() +
						". Trying to take it instead...))"); }
				this.Take(item);
				return; }

			// if the container is closed, try to open it instead
			if (typeof(OpenableContainer).IsAssignableFrom(container.GetType()) && !((OpenableContainer)container).IsOpen()) {
				// if the actor is the player, report the auto-correction
				if (GameManager.IsPlayer(this)) {
					GameManager.ReportIfVisible(this, "((" +
						StringManipulator.CapitalizeFirstLetter(
						container.GetSpecificName()) + " is currently " +
						"closed. Trying to open it instead..."); }
				this.Open(container);
				return; }

			// TODO: Should actors automatically try to unlock containers?

			// if nothing went wrong, execute normally
			item.GetLocation().RemoveThing(item);
			((Container)container).AddThing(item);
			item.SetLocation((Container)container);
			GameManager.ReportIfVisible(this, VerbSet.ToPut, item,
				" into ", container);
		}

		/// <summary>
		/// Has the <see cref="Person"/> try to wear the given
		/// <see cref="Thing"/>.
		/// If the <see cref="Person"/> is not carrying the
		/// <see cref="Thing"/>, it has them try to take the
		/// <see cref="Thing"/> instead.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to try wearing</param>
		/// 
		/// <exception cref="InteractingWithUnseenThingException">
		/// if the <see cref="Person"/> can't see the <see cref="Thing"/>
		/// </exception>
		/// <exception cref="WearingSomethingAlreadyWornException">
		/// if the <see cref="Person"/> is already wearing the
		/// <see cref="Thing"/>
		/// </exception>
		/// <exception cref="WearingSomethingBesidesClothingException">
		/// if the <see cref="Thing"/> is not <see cref="Clothing"/>
		/// </exception>
		public void Wear(Thing item)
		{
			// if the actor can't see the item, throw an exception
			if (!GameManager.CanSee(this, item)) {
				throw new InteractingWithUnseenThingException(); }

			// if the actor is already wearing the item, throw an exception
			if (this.Wears(item)) {
				throw new WearingSomethingAlreadyWornException(); }

			// if the item isn't actually clothing, throw an exception
			if (!typeof(Clothing).IsAssignableFrom(item.GetType())) {
				throw new WearingSomethingBesidesClothingException(); }

			// if the actor is not carrying the item, try taking it instead
			if (!this.Carries(item)) {
				if (GameManager.IsPlayer(this)) {
					GameManager.ReportIfVisible(this, "((You aren't carrying "
						+ item.GetSpecificName() +
						"; trying to take it instead...))"); }
				this.Take(item);
				return; }

			// if thr actor is wearing something that can't be worn at the
			// same time as the item, throw an exception
			foreach(Clothing wornItem in this.clothes.GetContents()) {
				if (wornItem.CannotBeWornWith((Clothing)item)) {
					throw new WearingWithConflictingItemException(wornItem,
						item); } }

			// "happy path"
				this.clothes.AddThing(item);
				item.GetLocation().RemoveThing(item);
				item.SetLocation(this.clothes);
				GameManager.ReportIfVisible(this, this.GetSpecificName() + ' '
					+ this.GetConjugatedVerb(VerbSet.ToPut) + " on " +
					item.GetSpecificName() + '.');
		}

		/// <summary>
		/// Has the <see cref="Person"/> try to take off the given
		/// <see cref="Thing"/>.
		/// </summary>
		/// <remarks>
		/// This function makes no attempt to check if the <see cref="Thing"/>
		/// is actually <see cref="Clothing"/>. There is an unchecked cast to
		/// to <see cref="Clothing"/>. Given how thoroughly wearing is checked,
		/// this should never be an issue, but I thought it best to make a note
		/// of it nonetheless.
		/// </remarks>
		/// <param name="item">the <see cref="Thing"/> to try taking off</param>
		/// <exception cref="RemovingSomethingNotWornException">
		/// if the <see cref="Person"/> is not wearing the <see cref="Thing"/>
		/// </exception>
		public void TakeOff(Thing item)
		{
			// if the person isn't wearing the item, throw an exception
			if (!this.Wears(item)) {
				throw new RemovingSomethingNotWornException(); }

			// if the item is cursed/unremovable, throw an exception
			if (!((Clothing)item).CanBeRemoved()) {
				throw new RemovingingCursedUnremovableItemException(); }

			// if the person doesn't have a free hand, throw an exception
			if (!this.hands.HasFreeHand()) {
				throw new RemovingWithHandsFullException(); }

			// TODO: Add code for taking off other people's clothes?

			// At the very least, I need some sort of body-looting capability,
			// though I could always cheat and replace people with containers
			// on death, with appropriate name and flavor text.

			// It would also be good to have an armor-removing attempt;
			// like allowing someone to knock off an opponent's helmet.
			// However, this would probably be better accomplished
			// with a different verb.

			// Theft should also be allowed for things like hats and necklaces,
			// though this might also be handled better with another verb,
			// possibly with a redirect from here.

			// if no exceptions got thrown, remove as normal
			this.clothes.RemoveThing(item);
			this.hands.AddThing(item);
			item.SetLocation(hands);
			GameManager.ReportIfVisible(this, VerbSet.ToTakeOff, item);
		}

		//===================================================================//
		//                            Helpers                                //
		//===================================================================//

		/// <summary>
		/// Helper method for <see cref="GetDescription"/> that describes a
		/// <see cref="Person"/>'s visible <see cref="Clothing"/>.
		/// </summary>
		/// <returns>a description of the <see cref="Person"/>'s visible <see cref="Clothing"/></returns>
		/// <seealso cref="Person.GetDescription"/>
		public string DescribeVisibleClothes()
		{
			HashSet<Clothing> visibleClothing = this.GetVisibleClothes();

			if (visibleClothing.Count == 0) {
				return StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) +
					"n't wearing anything of interest."; }

			else if (visibleClothing.Count == 1) {
				return StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " +
					visibleClothing.ElementAt(0).GetQualifiedName() + "."; }

			else if (visibleClothing.Count == 2) {
				return StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) + " wearing "
					+ visibleClothing.ElementAt(0).GetQualifiedName() + " and "
					+ visibleClothing.ElementAt(1).GetQualifiedName() + "."; }

			else {
				string returnStr = StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " +
					visibleClothing.ElementAt(0).GetQualifiedName();
				for (int i = 1; i < visibleClothing.Count - 1; i++) {
					returnStr += ", " +
						visibleClothing.ElementAt(i).GetQualifiedName(); }
				returnStr += ", and " +
					visibleClothing.ElementAt(visibleClothing.Count - 1)
					.GetQualifiedName() + '.';
				return returnStr; }
		}

		/// <summary>
		/// Helper for printing the player's "inventory."
		/// </summary>
		/// 
		/// <returns>
		/// a description of everything the person is wearing
		/// </returns>
		public string DescribeAllClothes()
		{
			HashSet<Clothing> clothing = this.GetClothes();

			if (clothing.Count == 0) {
				return StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) +
					"n't wearing anything of interest."; }

			else if (clothing.Count == 1) {
				return StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " +
					clothing.ElementAt(0).GetQualifiedName() + "."; }

			else if (clothing.Count == 2) {
				return StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) + " wearing "
					+ clothing.ElementAt(0).GetQualifiedName() + " and "
					+ clothing.ElementAt(1).GetQualifiedName() + "."; }

			else {
				string returnStr = StringManipulator.CapitalizeFirstLetter(
					this.GetSpecificName()) + ' ' +
					this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " +
					clothing.ElementAt(0).GetQualifiedName();
				for (int i = 1; i < clothing.Count - 1; i++) {
					returnStr += ", " +
						clothing.ElementAt(i).GetQualifiedName(); }
				returnStr += ", and " +
					clothing.ElementAt(clothing.Count - 1)
					.GetQualifiedName() + '.';
				return returnStr; }
		}

		/// <summary>
		/// Describes the items that the person is carrying.
		/// </summary>
		/// <returns>
		/// a description of the items the person is carrying
		/// </returns>
		public string DescribeCarriedItems()
		{
			return this.hands.GetDescription();
		}
	}
}

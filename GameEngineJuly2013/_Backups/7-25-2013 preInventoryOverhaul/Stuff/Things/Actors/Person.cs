using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things.Containers;
using Meta.ParsingAndPrinting;
using Meta.Exceptions;
using Meta.Exceptions.WearingExceptions;

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
			bool isSpecific = false, bool isPlural = false, bool isProper = true)
			: base(name, description, pronouns, isSpecific, isPlural, isProper: true, canBeTaken: false)
		{
			this.clothes = new Clothes(this);
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
			return StringManipulator.CapitalizeFirstLetter(this.GetQualifiedName()) + describeClothes();
		}

		/// <summary>
		/// Gets the contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>.
		/// </summary>
		/// <returns>
		/// a HashSet of the contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetContents()
		{
			HashSet<Thing> temp = new HashSet<Thing>(base.GetContents());
			temp.UnionWith(this.clothes.GetContents());
			return temp;
		}

		/// <summary>
		/// Gets the visible contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>.
		/// </summary>
		/// <returns>
		/// a HashSet of the visible contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetVisibleContents()
		{
			HashSet<Thing> temp = new HashSet<Thing>(base.GetVisibleContents());
			temp.UnionWith(this.clothes.GetVisibleContents());
			return (temp);
		}

		/// <summary>
		/// Get the contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>, recursively.
		/// </summary>
		/// <returns>
		/// a HashSet of the recursive contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetRecursiveContents()
		{
			HashSet<Thing> temp = new HashSet<Thing>(base.GetRecursiveContents());
			temp.UnionWith(this.clothes.GetRecursiveContents());
			return (temp);
		}

		/// <summary>
		/// Get the visible contents of the <see cref="Person"/>'s
		/// <see cref="Inventory"/> and <see cref="Clothes"/>, recursively.
		/// </summary>
		/// <returns>
		/// a HashSet of the visible recursive contents of the
		/// <see cref="Person"/>'s <see cref="Inventory"/> and
		/// <see cref="Clothes"/>
		/// </returns>
		public override HashSet<Thing> GetVisibleRecursiveContents()
		{
			HashSet<Thing> temp = new HashSet<Thing>(base.GetVisibleRecursiveContents());
			temp.UnionWith(this.clothes.GetVisibleRecursiveContents());
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

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

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
			else if (this.Wears(item)) {
				throw new WearingSomethingAlreadyWornException(); }

			// if the item isn't actually clothing, throw an exception
			else if (!typeof(Clothing).IsAssignableFrom(item.GetType())) {
				throw new WearingSomethingBesidesClothingException(); }

			// if the actor is not carrying the item, try taking it instead
			else if (!this.Carries(item)) {
				if (GameManager.IsPlayer(this)) {
					GameManager.ReportIfVisible(this, "((You aren't carrying "
						+ item.GetSpecificName() + "; trying to take it instead...))");
					this.Take(item); }
				} // end "if the actor is not carrying the item"

			// "happy path"
			else {
				this.clothes.AddThing(item);
				item.GetLocation().RemoveThing(item);
				item.SetLocation(this.clothes);
				GameManager.ReportIfVisible(this, this.GetSpecificName() + ' ' + this.GetConjugatedVerb(VerbSet.ToPut) + " on " + item.GetSpecificName() + '.');
			}
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
			this.AddToInventory(item);
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
		private string describeClothes()
		{
			HashSet<Clothing> visibleClothing = this.GetVisibleClothes();

			if (visibleClothing.Count == 0)
			{
				return ' ' + this.GetConjugatedVerb(VerbSet.ToBe) + "n't wearing anything of interest.";
			}
			else if (visibleClothing.Count == 1)
			{
				return ' ' + this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " + visibleClothing.ElementAt(0).GetQualifiedName() + ".";
			}
			else if (visibleClothing.Count == 2)
			{
				return ' ' + this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " + visibleClothing.ElementAt(0).GetQualifiedName()
					+ " and" + visibleClothing.ElementAt(1).GetQualifiedName() + ".";
			}
			else
			{
				string returnStr = ' ' + this.GetConjugatedVerb(VerbSet.ToBe) + " wearing " + visibleClothing.ElementAt(0).GetQualifiedName();
				for (int i = 1; i < visibleClothing.Count - 1; i++)
				{
					returnStr += ", " + visibleClothing.ElementAt(i).GetQualifiedName();
				}
				returnStr += ", and " + visibleClothing.ElementAt(visibleClothing.Count - 1).GetQualifiedName() + '.';
				return returnStr;
			}
		}

	}
}

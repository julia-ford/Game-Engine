using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things.Actors;
using Meta.Exceptions.WearingExceptions;
using Meta.ParsingAndPrinting;
using Interfaces;

namespace Stuff.Things.Containers
{
	/// <summary>
	/// Represents all of the <see cref="Clothing"/> that a <see cref="Person"/> is wearing.
	/// </summary>
	class Clothes : BasicContainer
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// the <see cref="Person"/> wearing the <see cref="Clothes"/>
		/// </summary>
		private Person owner;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new <see cref="Clothes"/> object belonging to the specified <see cref="Person"/>.
		/// </summary>
		/// <param name="owner">the <see cref="Person"/> to whom the <see cref="Clothes"/> belong</param>
		public Clothes(Person owner)
			: base(owner.GetName() + "'s clothes", "The clothes that " + owner.GetName() + " is wearing.", isProper: true, canBeTaken: false)
		{
			this.owner = owner;
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for the owner of the <see cref="Clothes"/>.
		/// </summary>
		/// <returns>the <see cref="Person"/> to whom the <see cref="Clothes"/> belong</returns>
		public Person GetOwner()
		{
			return this.owner;
		}

		/// <summary>
		/// This override prefaces the word clothes with their owner's specific name.
		/// </summary>
		/// <returns></returns>
		public override string GetName()
		{
			return this.owner.GetSpecificName() + "'s clothes";
		}

		/// <summary>
		/// This override takes into account clothing overlapping when deciding
		/// whether a given <see cref="Clothing"/> item is visible.
		/// </summary>
		/// <returns>a set of all the visible <see cref="Clothing"/></returns>
		public override HashSet<Thing> GetVisibleContents()
		{
			HashSet<Thing> visibleClothes = new HashSet<Thing>();

			foreach (Clothing item in base.GetVisibleContents()) {
				// bracelets, cinchers, glasses, gloves, hats, jackets,
				// necklaces, pants, rings, shirts, shoes, skirts, socks, and
				// tights are always considered visible
				if (   item.HasType(ClothingSlot.bracelet) || item.HasType(ClothingSlot.cincher) || item.HasType(ClothingSlot.glasses)
					|| item.HasType(ClothingSlot.gloves)   || item.HasType(ClothingSlot.hat)     || item.HasType(ClothingSlot.jacket)
					|| item.HasType(ClothingSlot.necklace) || item.HasType(ClothingSlot.pants)   || item.HasType(ClothingSlot.ring)
					|| item.HasType(ClothingSlot.shirt)    || item.HasType(ClothingSlot.shoes)   || item.HasType(ClothingSlot.skirt)
					|| item.HasType(ClothingSlot.socks)    || item.HasType(ClothingSlot.tights)) {
					visibleClothes.Add(item); }
				// undershirts are blocked by shirts
				else if (item.HasType(ClothingSlot.undershirt) && !this.ContainsSomethingOfType(ClothingSlot.shirt)) {
					visibleClothes.Add(item); }
				// underpants are blocked by tights, skirts, and pants 
				else if (item.HasType(ClothingSlot.underpants) && !(this.ContainsSomethingOfType(ClothingSlot.tights)
					|| this.ContainsSomethingOfType(ClothingSlot.skirt) || this.ContainsSomethingOfType(ClothingSlot.pants))) {
					visibleClothes.Add(item); }
			} // end foreach

			return visibleClothes;
		}

		/// <summary>
		/// This override takes into account clothing overlapping when deciding
		/// whether a given <see cref="Clothing"/> item is visible.
		/// </summary>
		/// <returns>a set of all the visible <see cref="Clothing"/>, and its contents</returns>
		public override HashSet<Thing> GetVisibleRecursiveContents()
		{
			HashSet<Thing> recursiveSet = this.GetVisibleContents();
			foreach(Thing item in this.GetVisibleContents()) {
				if (typeof(Searchable).IsAssignableFrom(item.GetType())) {
					recursiveSet.UnionWith(((Searchable)item).GetVisibleRecursiveContents()); }
			} // end foreach
			return recursiveSet;
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// This override ensures that the <see cref="Clothes"/> will never
		/// show up on a list and will never be interpreted as the subject
		/// of a player command.
		/// </summary>
		/// <param name="name">the name to check</param>
		/// <returns>false</returns>
		public override bool IsNamed(string name)
		{
			return false;
		}

		/// <summary>
		/// Determines if the <see cref="Clothes"/> contains some
		/// <see cref="Clothing"/> of the specified type.
		/// </summary>
		/// <param name="slot">the type to check for</param>
		/// <returns>
		/// true if the <see cref="Clothes"/> contains something
		/// of the specifed type; false otherwise
		/// </returns>
		public bool ContainsSomethingOfType(ClothingSlot slot)
		{
			foreach (Clothing item in this.GetContents()) {
				if (item.HasType(slot)) {
					return true; }
			} // end foreach
			return false;
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// This override ensures that only <see cref="Clothing"/> can be added to the Clothes.
		/// </summary>
		/// <param name="item">the item to be added</param>
		public override void AddThing(Thing item)
		{
			if (!typeof(Clothing).IsAssignableFrom(item.GetType())) {
				throw new WearingSomethingBesidesClothingException(); }

			else {
				base.AddThing(item); }
		}

	}
}

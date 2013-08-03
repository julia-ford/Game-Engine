using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Meta.ParsingAndPrinting;

namespace Stuff.Things
{
	/// <summary>
	/// Represents anything that can be worn by a <see cref="Person"/>.
	/// TODO: Create system for animal clothing, like dog collars?
	/// </summary>
	public class Clothing : Thing
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// Whether or not the <see cref="Clothing"/> can be taken off.
		/// Used for creating cursed/unremovable items.
		/// </summary>
		private bool canBeRemoved;

		/// <summary>
		/// A set of all of the <see cref="ClothingSlot"/>s the
		/// <see cref="Clothing"/> covers.
		/// </summary>
		private HashSet<ClothingSlot> slots;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new clothing item with the specified name and description.
		/// </summary>
		/// <param name="name">the name of the clothing</param>
		/// <param name="description">a brief description of the thing</param>
		public Clothing(string name, string description,
			bool isSpecific = false, bool isPlural = false, bool isProper = false,
			bool canBeTaken = true, bool canBeDropped = true, bool canBeRemoved = true,
			params ClothingSlot[] slots) :
			base(name, description,
			isSpecific:isSpecific, isPlural:isPlural, isProper:isProper,
			canBeTaken:canBeTaken, canBeDropped:canBeDropped)
		{
			this.canBeRemoved = canBeRemoved;
			this.slots = new HashSet<ClothingSlot>(slots);
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Determines whether or not it is possible for two pieces of clothing to be worn together.
		/// </summary>
		/// <param name="other">the other piece of clothing</param>
		/// <returns>true if the two pieces of clothing cannot be worn together; false otherwise</returns>
		public bool CannotBeWornWith(Clothing other)
		{
			foreach (ClothingSlot slot in this.slots)
			{
				if (other.slots.Contains(slot))
				{
					return true;
				}
			}
			if (   (this.slots.Contains(ClothingSlot.tights) && other.slots.Contains(ClothingSlot.socks))
				|| (this.slots.Contains(ClothingSlot.socks) && other.slots.Contains(ClothingSlot.tights))   )
			{
				return true;
			}
			if ((this.slots.Contains(ClothingSlot.pants) && other.slots.Contains(ClothingSlot.skirt))
				|| (this.slots.Contains(ClothingSlot.skirt) && other.slots.Contains(ClothingSlot.pants)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether this clothing will get in the way of putting on or taking off another piece of clothing.
		/// </summary>
		/// <param name="other">the other piece of clothing</param>
		/// <returns>true if this does get in the way; false otherwise</returns>
		public bool GetsInTheWayOf(Clothing other)
		{
			if (this.CannotBeWornWith(other))
			{
				return true;
			}
			else if (this.slots.Contains(ClothingSlot.shoes) && (other.slots.Contains(ClothingSlot.socks) || other.slots.Contains(ClothingSlot.tights)))
			{
					return true;
			}
			else if (this.slots.Contains(ClothingSlot.tights) && other.slots.Contains(ClothingSlot.underpants))
			{
				return true;
			}
			else if (this.slots.Contains(ClothingSlot.pants) && (other.slots.Contains(ClothingSlot.underpants) || other.slots.Contains(ClothingSlot.tights)))
			{
				return true;
			}
			else if (this.slots.Contains(ClothingSlot.shirt) && other.slots.Contains(ClothingSlot.undershirt))
			{
				return true;
			}
			else if (this.slots.Contains(ClothingSlot.jacket) && (other.slots.Contains(ClothingSlot.shirt) || other.slots.Contains(ClothingSlot.undershirt)))
			{
				return true;
			}
			else if (this.slots.Contains(ClothingSlot.ring) && other.slots.Contains(ClothingSlot.gloves))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines if the clothing covers the specified slot.
		/// </summary>
		/// <param name="type">the slot to check</param>
		/// <returns>true if it covers the slot; false otherwise</returns>
		public bool HasType(ClothingSlot type)
		{
			if (this.slots.Contains(type))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines whether the <see cref="Clothing"/> can be taken off.
		/// Used fro creating cursed/unremovable items.
		/// </summary>
		/// <returns>true if the <see cref="Clothing"/> can be taken off; false otherwise</returns>
		public bool CanBeRemoved()
		{
			return this.canBeRemoved;
		}
	}
}

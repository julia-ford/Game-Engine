using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things.Actors;

namespace Stuff.Things.Containers
{
	/// <summary>
	/// Represents all of the <see cref="Thing"/>s that an <see cref="Actor"/>
	/// is holding.
	/// </summary>
	class Inventory : BasicContainer
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		/// <summary>
		/// the <see cref="Actor"/> to whom the <see cref="Inventory"/> belongs
		/// </summary>
		private Actor owner;

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates a new <see cref="Inventory"/> belonging to the specified <see cref="Actor"/>.
		/// </summary>
		/// <param name="owner">the <see cref="Actor"/> to whom the <see cref="Inventory"/> belongs</param>
		public Inventory(Actor owner) : base(owner.GetName() + "'s inventory", owner.GetName() + "'s inventory", isProper: true, canBeTaken: false)
		{
			this.owner = owner;
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// Accessor for the owner of the <see cref="Inventory"/>.
		/// </summary>
		/// <returns>the <see cref="Actor"/> to whom the <see cref="Inventory"/> belongs</returns>
		public Actor GetOwner()
		{
			return this.owner;
		}

		/// <summary>
		/// This override names the <see cref="Inventory"/> after its owner.
		/// </summary>
		/// <returns>the name of the <see cref="Inventory"/></returns>
		public override string GetName()
		{
			return this.GetSpecificName() + "'s inventory";
		}

		/// <summary>
		/// This override makes it so that an <see cref="Actor"/>'s
		/// <see cref="Inventory"/> is not considered visible.
		/// </summary>
		/// <returns>an empty HashSet</returns>
		public override HashSet<Thing> GetVisibleContents()
		{
			return new HashSet<Thing>();
		}

		/// <summary>
		/// This override makes it so that an <see cref="Actor"/>'s
		/// <see cref="Inventory"/> is not considered visible.
		/// </summary>
		/// <returns>an empty HashSet</returns>
		public override HashSet<Thing> GetVisibleRecursiveContents()
		{
			return new HashSet<Thing>();
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// This override ensures that the <see cref="Inventory"/> will never show up
		/// on a list and will never be interpreted as the subject of a player
		/// command.
		/// </summary>
		/// <param name="name">the name to check</param>
		/// <returns>false</returns>
		public override bool IsNamed(string name)
		{
			return false;
		}
	}
}

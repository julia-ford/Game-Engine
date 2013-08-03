using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// internal
using Stuff.Things.Actors;
using Meta.Exceptions.WearingAndTakingOffExceptions;
using Meta.ParsingAndPrinting;
using Interfaces;

namespace Stuff.Things.Containers
{
	/// <summary>
	/// Represents both of a <see cref="Person"/>'s hands.
	/// </summary>
	class Hands : BasicContainer
	{
		//===================================================================//
		//                           Variables                               //
		//===================================================================//

		// none anymore

		//===================================================================//
		//                         Constructors                              //
		//===================================================================//

		/// <summary>
		/// Creates some new <see cref="Hands"/> belonging to the specified
		/// <see cref="Person"/>.
		/// </summary>
		/// 
		/// <param name="owner">
		/// the <see cref="Person"/> to whom the <see cref="Hands"/> belong
		/// </param>
		public Hands(Person owner) : base(
			owner.GetSpecificName() + "'s hands",
			owner.GetSpecificName() + "'s hands.",
			parsedNames: new string[0],
			isProper: true, canBeTaken: false)
		{
			this.SetOwner(owner);
		}

		//===================================================================//
		//                            Getters                                //
		//===================================================================//

		/// <summary>
		/// This override prefaces the word "hands" with its owner's name.
		/// </summary>
		/// 
		/// <returns>the printed name of the <see cref="Hands"/></returns>
		public override string GetName()
		{
			return this.GetOwner().GetSpecificName() + "'s hands";
		}

		/// <summary>
		/// This override makes the description describe the contents of the
		/// <see cref="Hands"/>.
		/// </summary>
		/// 
		/// <returns>
		/// a description of the contents of the <see cref="Hands"/>
		/// </returns>
		public override string GetDescription()
		{
			return StringManipulator.CapitalizeFirstLetter(
				this.GetOwner().GetSubjectPronoun()) + ' ' +
				this.GetOwner().GetConjugatedVerb(VerbSet.ToBe) +
				(this.IsEmpty() ? "n't carrying anything." : " carrying " +
				this.GetContents().ElementAt(0).GetQualifiedName() +
				(this.GetContents().Count == 1 ? "." : " and " +
				this.GetContents().ElementAt(1).GetQualifiedName() + '.'));
		}

		//===================================================================//
		//                            Booleans                               //
		//===================================================================//

		/// <summary>
		/// Determines whether the owner of the hands currently has a free hand
		/// available.
		/// </summary>
		/// <returns>
		/// true if one or both hands are free; false otherwise
		/// </returns>
		public bool HasFreeHand()
		{
			if (this.IsEmpty()) {
				return true; }

			else if (this.GetContents().Count > 1) {
				return false; }

			else {
				return !this.GetContents().ElementAt(0).IsTwoHanded(); }
		}

		//===================================================================//
		//                            Actions                                //
		//===================================================================//

		/// <summary>
		/// This override ensures that at most two one-handed
		/// <see cref="Thing"/>s or one two-handed <see cref="Thing"/> can be
		/// held in the <see cref="Hands"/>.
		/// </summary>
		/// 
		/// <param name="item">the <see cref="Thing"/> to be added</param>
		public override void AddThing(Thing item)
		{
			if (!this.HasFreeHand() ||
				(!this.IsEmpty() && item.IsTwoHanded())) {
				throw new Exception("Error: Attempted to add too much stuff "
					+ "to a set of hands."); }

			else {
				base.AddThing(item); }
		}

	}
}

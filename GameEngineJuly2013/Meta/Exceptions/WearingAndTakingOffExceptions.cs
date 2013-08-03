using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stuff.Things;

namespace Meta.Exceptions.WearingAndTakingOffExceptions
{
	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to wear a <see cref="Thing"/>
	/// that isn't <see cref="Clothing"/>.
	/// </summary>
	public class WearingSomethingBesidesClothingException : GameException
	{
		public WearingSomethingBesidesClothingException() : base("That's not something you can wear.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to wear some
	/// <see cref="Clothing"/> that they are already wearing.
	/// </summary>
	public class WearingSomethingAlreadyWornException : GameException
	{
		public WearingSomethingAlreadyWornException() : base("You're already wearing that.") { }
	}

	/// <summary>
	/// Happens when an actor tries to wear something that cannot be worn with
	/// something they're already wearing.
	/// </summary>
	public class WearingWithConflictingItemException : GameException
	{
		public WearingWithConflictingItemException(Thing item1, Thing item2) :
			base("You can't wear both " + item1.GetSpecificName() + " and " +
			item2.GetSpecificName() + " at the same time. You'll have to " +
			"take off " + item1.GetSpecificName() + " in order to put on " +
			item2.GetSpecificName() + '.') { }
	}

	/// <summary>
	/// Happens when an actor tries to wear something while also wearing
	/// something that gets in the way of wearing the new item.
	/// </summary>
	public class WearingWithSomethingBlockingException : GameException
	{
		public WearingWithSomethingBlockingException(Thing item1, Thing item2) :
			base("You can't wear both " + item1.GetSpecificName() + " and " +
			item2.GetSpecificName() + " at the same time. You'll have to " +
			"take off " + item1.GetSpecificName() + " in order to put on " +
			item2.GetSpecificName() + '.') { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to take off some
	/// <see cref="Clothing"/> that they are not wearing.
	/// </summary>
	public class RemovingSomethingNotWornException : GameException
	{
		public RemovingSomethingNotWornException() : base("You're not wearing that.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to take off some
	/// <see cref="Clothing"/> that has been flagged as cursed/unremovable.
	/// </summary>
	class RemovingingCursedUnremovableItemException : GameException
	{
		public RemovingingCursedUnremovableItemException() : base("You try to take it off, but somehow it just doesn't work. It's really weird.") { }
	}

	public class RemovingWithHandsFullException : GameException
	{
		public RemovingWithHandsFullException() : base("Your hands are full; you'll need to put something down first.") { }
	}
}

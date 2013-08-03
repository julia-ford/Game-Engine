using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meta.Exceptions.WearingExceptions
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
}

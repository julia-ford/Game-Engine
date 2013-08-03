using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meta.Exceptions.PuttingIntoExceptions
{
	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to put a <see cref="Thing"/>
	/// into a <see cref="Container"/>, but the item is already inside the
	/// container.
	/// </summary>
	public class PuttingItemAlreadyInsideException : GameException
	{
		public PuttingItemAlreadyInsideException() : base("Error: Attempted to add an item to a room or container when the item was already inside the room or container.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to put a
	/// <see cref="Container"/> inside of itself.
	/// </summary>
	public class PuttingItemIntoItselfException : GameException
	{
		public PuttingItemIntoItselfException() : base("Error: Attempted to add a container of some kind to its own contents.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to put one
	/// <see cref="Thing"/> into a second <see cref="Thing"/>, and the second
	/// <see cref="Thing"/> isn't actually a <see cref="Container"/>.
	/// </summary>
	public class PuttingIntoNonContainerException : GameException
	{
		public PuttingIntoNonContainerException() : base("That isn't a container, so you can't put things in it.") { }
	}

}

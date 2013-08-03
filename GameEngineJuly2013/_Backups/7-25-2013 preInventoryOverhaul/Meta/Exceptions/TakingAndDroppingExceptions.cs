namespace Meta.Exceptions.TakingExceptions
{
	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to take a <see cref="Thing"/>,
	/// but it is already in their <see cref="Inventory"/>
	/// </summary>
	class TakingItemAlreadyHeldException : GameException
	{
		public TakingItemAlreadyHeldException() : base("You're already carrying that.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to take a <see cref="Thing"/>
	/// that has been marked as impossible to take.
	/// </summary>
	class TakingItemFlaggedUntakeableException : GameException
	{
		public TakingItemFlaggedUntakeableException() : base("That's not something that you can take.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to take a
	/// <see cref="Thing"/> from a container, but the container is closed.
	/// </summary>
	public class TakingFromClosedContainerException : GameException
	{
		public TakingFromClosedContainerException() : base("Error: Attempted to remove an item from a closed container.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to drop a <see cref="Thing"/>
	/// that they are not carrying.
	/// </summary>
	class DroppingItemNotHeldException : GameException
	{
		public DroppingItemNotHeldException() : base("You aren't holding that.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to drop a <see cref="Thing"/>
	/// that has been flagged as cursed/undroppable.
	/// </summary>
	class DroppingCursedUndroppableItemException : GameException
	{
		public DroppingCursedUndroppableItemException() : base("You try to drop it, but somehow it just doesn't work. It's really weird.") { }
	}
}

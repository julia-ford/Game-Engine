namespace Meta.Exceptions.TakingAndDroppingExceptions
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
		public TakingFromClosedContainerException() : base("That is inside of a closed container.") { }
	}

	/// <summary>
	/// Happens when a <see cref="Person"/> tries to take a <see cref="Thing"/>
	/// when both of their hands are full, or if they try to take a two-handed
	/// <see cref="Thing"/> with one hand full.
	/// </summary>
	public class TakingWithHandsFullException : GameException
	{
		public TakingWithHandsFullException() : base("You only have two hands; you'll need to put something down before you can pick up more.") { }
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

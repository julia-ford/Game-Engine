using System;

namespace Meta.Exceptions
{
	/// <summary>
	/// A step in between Exception and my custom, specific exceptions that
	/// allows me to catch all of my game exceptions without catching other
	/// exceptions caused by buggy code.
	/// </summary>
	public class GameException : Exception
	{
		public GameException(string message) : base(message) { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to interact with a
	/// <see cref="Thing"/> that they can't see.
	/// </summary>
	public class InteractingWithUnseenThingException : GameException
	{
		public InteractingWithUnseenThingException() : base("You can't see any such thing.") { }
	}
}

namespace Meta.Exceptions.MovingExceptions
{
	/// <summary>
	/// Happens when an actor tries to move in a direction, but there's nowhere to go in that direction.
	/// </summary>
	public class MovingInInvalidDirectionException : GameException
	{
		public MovingInInvalidDirectionException() : base("You can't go that way.") { }
	}

	/// <summary>
	/// Happens when an actor tries to move in a direction, but their location is set to null.
	/// </summary>
	public class MovingDirectionallyFromNullException : GameException
	{
		public MovingDirectionallyFromNullException() : base("Uh... So your location is currently null. I'm not sure how you managed that, but it's pretty game-breaking. You should probably quit.") { }
	}
}
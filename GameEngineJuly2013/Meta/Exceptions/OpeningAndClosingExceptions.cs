using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meta.Exceptions.OpeningAndClosingExceptions
{
	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to open something that
	/// isn't <see cref="Openable"/>.
	/// <seealso cref="ClosingSomethingBesidesOpenableException"/>
	/// </summary>
	public class OpeningSomethingBesidesOpenableException : GameException
	{
		public OpeningSomethingBesidesOpenableException() : base("That isn't something you can open.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to close something that
	/// isn't <see cref="Openable"/>.
	/// <seealso cref="OpeningSomethingBesidesOpenableException"/>
	/// </summary>
	public class ClosingSomethingBesidesOpenableException : GameException
	{
		public ClosingSomethingBesidesOpenableException() : base("That isn't something you can close.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to open a
	/// <see cref="Container"/>, but the container is already open.
	/// </summary>
	public class OpeningAlreadyOpenContainerException : GameException
	{
		public OpeningAlreadyOpenContainerException() : base("It's already open.") { }
	}

	/// <summary>
	/// Happens when an <see cref="Actor"/> tries to close a
	/// <see cref="Container"/>, but the container is already closed.
	/// </summary>
	public class ClosingAlreadyClosedContinerException : GameException
	{
		public ClosingAlreadyClosedContinerException() : base("It's already closed.") { }
	}

}

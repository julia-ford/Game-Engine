using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
	/// <summary>
	/// Represents a container that can be opened, closed, and locked.
	/// </summary>
	public interface LockableOpenableContainer : Container, Openable, Lockable { }
}

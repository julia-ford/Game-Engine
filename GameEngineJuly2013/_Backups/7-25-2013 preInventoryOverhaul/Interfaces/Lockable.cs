using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
	/// <summary>
	/// Represents something that can be locked and unlocked.
	/// </summary>
	public interface Lockable
	{
		bool IsLocked();
		void Lock();
		void UnLock();
	}
}

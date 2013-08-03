using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
	/// <summary>
	/// Represents something that can be opened and closed, like a door or a chest.
	/// </summary>
	public interface Openable
	{
		/// <summary>
		/// Determine if the <see cref="Openable"/> is open or closed.
		/// </summary>
		/// <returns>true if it is open; false if it is closed</returns>
		bool IsOpen();

		/// <summary>
		/// Open the <see cref="Openable"/>.
		/// </summary>
		void Open();

		/// <summary>
		/// Close the <see cref="Openable"/>.
		/// </summary>
		void Close();
	}
}

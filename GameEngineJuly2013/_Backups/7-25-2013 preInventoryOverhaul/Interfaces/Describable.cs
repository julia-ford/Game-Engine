using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
	/// <summary>
	/// Represents anything that the game can print a name and description about.
	/// </summary>
	public interface Describable
	{
		string GetName();
		string GetDescription();
		string GetQualifiedName();
	}
}

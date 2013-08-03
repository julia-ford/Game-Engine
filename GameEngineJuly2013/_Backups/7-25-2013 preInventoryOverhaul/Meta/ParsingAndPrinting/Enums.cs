namespace Meta.ParsingAndPrinting
{
	/// <summary>
	/// Represents physical directions. Used by rooms to keep track of adjacent/adjoining rooms.
	/// </summary>
	public enum Direction { north, east, south, west, northeast, southeast, northwest, southwest, up, down };

	/// <summary>
	/// Indicates whether a given <see cref="Effect"/> changes the property it
	/// affects by setting it equal to a new value or by adding to it.
	/// </summary>
	public enum EffectType { equality, addition };

	/// <summary>
	/// Indicates whether a given <see cref="Effect"/> lasts indefinately or
	/// for a set number of turns.
	/// </summary>
	public enum EffectDuration { permanent, turns }

	public enum ClothingSlot { socks, shoes, tights, underpants, pants, skirt, undershirt, shirt, cincher, jacket, ring, bracelet, necklace, gloves, hat, glasses }
}
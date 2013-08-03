using Stuff.Things;

namespace Meta.ParsingAndPrinting
{
	public delegate void NoArgumentsDelegate();

	public delegate void OneDirectionDelegate(Direction dir);

	public delegate void OneThingDelegate(Thing item);

	public delegate void TwoThingsDelegate(Thing item1, Thing item2);
}
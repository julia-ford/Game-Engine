using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meta.ParsingAndPrinting
{
	/// <summary>
	/// Represents a verb that will appear in some text that may refer to the
	/// player, another person, or several people.
	/// </summary>
	public class VerbSet
	{
		/// <summary>
		/// the form used when referring to an NPC
		/// </summary>
		private readonly string singularForm;

		/// <summary>
		/// the form use in reference to several NPCs
		/// </summary>
		private readonly string pluralForm;

		/// <summary>
		/// the form used when talking about the player
		/// </summary>
		private readonly string secondPersonForm;

		/// <summary>
		/// Constructs a new VerbSet with the specified singular, plural, and
		/// second-person forms.
		/// </summary>
		/// <param name="singularForm">the form used when referring to an NPC</param>
		/// <param name="pluralForm">the form use in reference to several NPCs</param>
		/// <param name="secondPersonForm">the form used when talking about the player</param>
		private VerbSet(string singularForm, string pluralForm, string secondPersonForm)
		{
			this.singularForm = singularForm;
			this.pluralForm = pluralForm;
			this.secondPersonForm = secondPersonForm;
		}

		/// <summary>
		/// Uses the pronoun set to generate the appropriate form of the verb
		/// to agree with the pronouns.
		/// </summary>
		/// <param name="verb">the verb to conjugate</param>
		/// <param name="form">the form of the subject of the sentence</param>
		/// <returns>the conjugated verb</returns>
		public static string GetForm(VerbSet verb, PronounSet form)
		{
			if (form == PronounSet.GetFemaleSet() || form == PronounSet.GetMaleSet() || form == PronounSet.GetNeuterSet())
			{
				return verb.singularForm;
			}
			else if (form == PronounSet.GetPluralSet())
			{
				return verb.pluralForm;
			}
			else if (form == PronounSet.GetSecondPersonSet())
			{
				return verb.secondPersonForm;
			}
			return null;
		}

		public static readonly VerbSet ToBe      = new VerbSet("is",        "are",      "are");
		public static readonly VerbSet ToTry     = new VerbSet("tries",     "try",      "try");
		public static readonly VerbSet ToGo      = new VerbSet("goes",      "go",       "go");
		public static readonly VerbSet ToHead    = new VerbSet("heads",     "head",     "head");
		public static readonly VerbSet ToMove    = new VerbSet("moves",     "move",     "move");
		public static readonly VerbSet ToLeave   = new VerbSet("leaves",    "leave",    "leave");
		public static readonly VerbSet ToExit    = new VerbSet("exits",     "exit",     "exit");
		public static readonly VerbSet ToEnter   = new VerbSet("enters",    "enter",    "enter");
		public static readonly VerbSet ToTake    = new VerbSet("takes",     "take",     "take");
		public static readonly VerbSet ToDrop    = new VerbSet("drops",     "drop",     "drop");
		public static readonly VerbSet ToOpen    = new VerbSet("opens",     "open",     "open");
		public static readonly VerbSet ToClose   = new VerbSet("closes",    "close",    "close");
		public static readonly VerbSet ToPutOn   = new VerbSet("puts on",   "put on",   "put on");
		public static readonly VerbSet ToTakeOff = new VerbSet("takes off", "take off", "take off");
		public static readonly VerbSet ToPut     = new VerbSet("puts",      "put",      "put");

	}
}

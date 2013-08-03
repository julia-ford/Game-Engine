namespace Meta.ParsingAndPrinting
{
	/// <summary>
	/// A set of pronouns to be used for people.
	/// </summary>
	public class PronounSet
	{
		private readonly string subjectForm;
		private readonly string objectForm;
		private readonly string possessiveForm;
		private readonly string possessiveObjectForm;
		private readonly string reflexiveForm;

		/// <summary>
		/// Creates a new set of pronuns with the specified forms.
		/// </summary>
		/// <param name="subjectForm">the pronoun used as the subject of a sentence</param>
		/// <param name="objectForm">the pronoun used as a direct or indirect object</param>
		/// <param name="possessiveForm">the pronoun used to indicate possession</param>
		/// <param name="possessiveObjectForm">the possessive pronoun that is used as a direct or indirect object</param>
		/// <param name="reflexiveForm">the pronoun that ends in "-self"</param>
		private PronounSet(string subjectForm, string objectForm, string possessiveForm, string possessiveObjectForm, string reflexiveForm)
		{
			this.subjectForm = subjectForm;
			this.objectForm = objectForm;
			this.possessiveForm = possessiveForm;
			this.possessiveObjectForm = possessiveObjectForm;
			this.reflexiveForm = reflexiveForm;
		}

		private static readonly PronounSet maleSet   = new PronounSet("he", "him", "his", "his", "himself");
		private static readonly PronounSet femaleSet = new PronounSet("she", "her", "her", "hers", "herself");
		private static readonly PronounSet neuterSet = new PronounSet("it", "it", "its", "its", "itself");
		private static readonly PronounSet pluralSet = new PronounSet("they", "them", "their", "theirs", "themselves");
		private static readonly PronounSet secondPersonSet = new PronounSet("you", "you", "your", "yours", "yourself");

		/// <summary>
		/// Accessor for the set of masculine pronouns.
		/// </summary>
		/// <returns>the set of masculine pronouns</returns>
		public static PronounSet GetMaleSet()
		{
			return maleSet;
		}

		/// <summary>
		/// Accessor for the set of feminine pronouns.
		/// </summary>
		/// <returns>the set of feminine pronouns</returns>
		public static PronounSet GetFemaleSet()
		{
			return femaleSet;
		}

		/// <summary>
		/// Accessor for the set of gender-neutral pronouns.
		/// </summary>
		/// <returns>the set of gender-neutral pronouns</returns>
		public static PronounSet GetNeuterSet()
		{
			return neuterSet;
		}

		/// <summary>
		/// Accessor for the set of plural pronouns.
		/// </summary>
		/// <returns>the set of plural pronouns</returns>
		public static PronounSet GetPluralSet()
		{
			return pluralSet;
		}

		/// <summary>
		/// Accessor for the set of second-person pronouns.
		/// </summary>
		/// <returns>the set of second-person pronouns</returns>
		public static PronounSet GetSecondPersonSet()
		{
			return secondPersonSet;
		}

		/// <summary>
		/// Accessor for the subject form of the pronoun set.
		/// </summary>
		/// <returns>the subject form of the pronoun set</returns>
		public string GetSubjectForm()
		{
			return this.subjectForm;
		}

		/// <summary>
		/// Accessor for the direct/indirect object form of the pronoun set.
		/// </summary>
		/// <returns>the (in)direct object form of the pronoun set</returns>
		public string GetObjectForm()
		{
			return this.objectForm;
		}

		/// <summary>
		/// Accessor for the possessive form of the pronoun set.
		/// </summary>
		/// <returns>the possessive form of the pronoun set</returns>
		public string GetPossessiveForm()
		{
			return this.possessiveForm;
		}

		/// <summary>
		/// Accessor for the possessive direct/indirect object form of the pronoun set.
		/// </summary>
		/// <returns>the possessive (in)direct form of the pronoun set</returns>
		public string GetPossessiveObjectForm()
		{
			return this.possessiveObjectForm;
		}

		/// <summary>
		/// Accessor for the reflexive form of the pronoun set.
		/// </summary>
		/// <returns>the relfexive form of the pronoun set</returns>
		public string GetReflexiveForm()
		{
			return this.reflexiveForm;
		}
	}
}

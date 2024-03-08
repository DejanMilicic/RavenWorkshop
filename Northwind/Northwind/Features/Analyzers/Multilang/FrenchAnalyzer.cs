using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using System;
using System.Collections.Generic;
using Lucene.Net.Analysis.Tokenattributes;
using System;
using System.Collections.Generic;
using System.IO;
using System;
using Lucene.Net.Analysis.Fr;

public sealed class FrenchAnalyzer : Analyzer
{

    /*
     * Extended list of typical French stopwords.
     * @deprecated use {@link #getDefaultStopSet()} instead
     */
    // TODO make this private in 3.1
    public readonly static String[] FRENCH_STOP_WORDS = {
    "a", "afin", "ai", "ainsi", "après", "attendu", "au", "aujourd", "auquel", "aussi",
    "autre", "autres", "aux", "auxquelles", "auxquels", "avait", "avant", "avec", "avoir",
    "c", "car", "ce", "ceci", "cela", "celle", "celles", "celui", "cependant", "certain",
    "certaine", "certaines", "certains", "ces", "cet", "cette", "ceux", "chez", "ci",
    "combien", "comme", "comment", "concernant", "contre", "d", "dans", "de", "debout",
    "dedans", "dehors", "delà", "depuis", "derrière", "des", "désormais", "desquelles",
    "desquels", "dessous", "dessus", "devant", "devers", "devra", "divers", "diverse",
    "diverses", "doit", "donc", "dont", "du", "duquel", "durant", "dès", "elle", "elles",
    "en", "entre", "environ", "est", "et", "etc", "etre", "eu", "eux", "excepté", "hormis",
    "hors", "hélas", "hui", "il", "ils", "j", "je", "jusqu", "jusque", "l", "la", "laquelle",
    "le", "lequel", "les", "lesquelles", "lesquels", "leur", "leurs", "lorsque", "lui", "là",
    "ma", "mais", "malgré", "me", "merci", "mes", "mien", "mienne", "miennes", "miens", "moi",
    "moins", "mon", "moyennant", "même", "mêmes", "n", "ne", "ni", "non", "nos", "notre",
    "nous", "néanmoins", "nôtre", "nôtres", "on", "ont", "ou", "outre", "où", "par", "parmi",
    "partant", "pas", "passé", "pendant", "plein", "plus", "plusieurs", "pour", "pourquoi",
    "proche", "près", "puisque", "qu", "quand", "que", "quel", "quelle", "quelles", "quels",
    "qui", "quoi", "quoique", "revoici", "revoilà", "s", "sa", "sans", "sauf", "se", "selon",
    "seront", "ses", "si", "sien", "sienne", "siennes", "siens", "sinon", "soi", "soit",
    "son", "sont", "sous", "suivant", "sur", "ta", "te", "tes", "tien", "tienne", "tiennes",
    "tiens", "toi", "ton", "tous", "tout", "toute", "toutes", "tu", "un", "une", "va", "vers",
    "voici", "voilà", "vos", "votre", "vous", "vu", "vôtre", "vôtres", "y", "à", "ça", "ès",
    "été", "être", "ô"
  };

    /*
     * Contains the stopwords used with the {@link StopFilter}.
     */
    private readonly ISet<string> stoptable;
    /*
     * Contains words that should be indexed but not stemmed.
     */
    //TODO make this final in 3.0
    private ISet<string> excltable = Lucene.Net.Support.Compatibility.SetFactory.CreateHashSet<string>();

    private readonly Version matchVersion;

    /*
     * Returns an unmodifiable instance of the default stop-words set.
     * @return an unmodifiable instance of the default stop-words set.
     */
    public static ISet<string> GetDefaultStopSet()
    {
        return DefaultSetHolder.DEFAULT_STOP_SET;
    }

    static class DefaultSetHolder
    {
        internal static ISet<string> DEFAULT_STOP_SET = CharArraySet.UnmodifiableSet(new CharArraySet((IEnumerable<string>)FRENCH_STOP_WORDS, false));
    }

    /*
     * Builds an analyzer with the default stop words ({@link #FRENCH_STOP_WORDS}).
     */
    public FrenchAnalyzer(Version matchVersion)
        : this(matchVersion, DefaultSetHolder.DEFAULT_STOP_SET)
    {

    }

    /*
     * Builds an analyzer with the given stop words
     * 
     * @param matchVersion
     *          lucene compatibility version
     * @param stopwords
     *          a stopword set
     */
    public FrenchAnalyzer(Version matchVersion, ISet<string> stopwords)
        : this(matchVersion, stopwords, CharArraySet.EMPTY_SET)
    {
    }

    /*
     * Builds an analyzer with the given stop words
     * 
     * @param matchVersion
     *          lucene compatibility version
     * @param stopwords
     *          a stopword set
     * @param stemExclutionSet
     *          a stemming exclusion set
     */
    public FrenchAnalyzer(Version matchVersion, ISet<string> stopwords, ISet<string> stemExclutionSet)
    {
        this.matchVersion = matchVersion;
        this.stoptable = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stopwords));
        this.excltable = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stemExclutionSet));
    }


    /*
     * Builds an analyzer with the given stop words.
     * @deprecated use {@link #FrenchAnalyzer(Version, Set)} instead
     */
    public FrenchAnalyzer(Version matchVersion, params string[] stopwords)
        : this(matchVersion, StopFilter.MakeStopSet(stopwords))
    {

    }

    /*
     * Builds an analyzer with the given stop words.
     * @throws IOException
     * @deprecated use {@link #FrenchAnalyzer(Version, Set)} instead
     */
    //public FrenchAnalyzer(Version matchVersion, FileInfo stopwords)
    //    : this(matchVersion, WordlistLoader.GetWordSet(stopwords))
    //{
    //}

    /*
     * Builds an exclusionlist from an array of Strings.
     * @deprecated use {@link #FrenchAnalyzer(Version, Set, Set)} instead
     */
    public void SetStemExclusionTable(params string[] exclusionlist)
    {
        excltable = StopFilter.MakeStopSet(exclusionlist);
        PreviousTokenStream = null; // force a new stemmer to be created
    }

    /*
     * Builds an exclusionlist from a Map.
     * @deprecated use {@link #FrenchAnalyzer(Version, Set, Set)} instead
     */
    public void SetStemExclusionTable(IDictionary<string, string> exclusionlist)
    {
        excltable = Lucene.Net.Support.Compatibility.SetFactory.CreateHashSet(exclusionlist.Keys);
        PreviousTokenStream = null; // force a new stemmer to be created
    }

    /*
     * Builds an exclusionlist from the words contained in the given file.
     * @throws IOException
     * @deprecated use {@link #FrenchAnalyzer(Version, Set, Set)} instead
     */
    //public void SetStemExclusionTable(FileInfo exclusionlist)
    //{
    //    excltable = WordlistLoader.GetWordSet(exclusionlist);
    //    PreviousTokenStream = null; // force a new stemmer to be created
    //}

    /*
     * Creates a {@link TokenStream} which tokenizes all the text in the provided
     * {@link Reader}.
     *
     * @return A {@link TokenStream} built from a {@link StandardTokenizer} 
     *         filtered with {@link StandardFilter}, {@link StopFilter}, 
     *         {@link FrenchStemFilter} and {@link LowerCaseFilter}
     */
    public override sealed TokenStream TokenStream(String fieldName, TextReader reader)
    {
        TokenStream result = new StandardTokenizer(matchVersion, reader);
        result = new StandardFilter(result);
        result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(matchVersion),
                                result, stoptable);
        result = new FrenchStemFilter(result, excltable);
        // Convert to lowercase after stemming!
        result = new LowerCaseFilter(result);
        return result;
    }

    class SavedStreams
    {
        protected internal Tokenizer source;
        protected internal TokenStream result;
    };

    /*
     * Returns a (possibly reused) {@link TokenStream} which tokenizes all the 
     * text in the provided {@link Reader}.
     *
     * @return A {@link TokenStream} built from a {@link StandardTokenizer} 
     *         filtered with {@link StandardFilter}, {@link StopFilter}, 
     *         {@link FrenchStemFilter} and {@link LowerCaseFilter}
     */
    public override TokenStream ReusableTokenStream(String fieldName, TextReader reader)
    {
        SavedStreams streams = (SavedStreams)PreviousTokenStream;
        if (streams == null)
        {
            streams = new SavedStreams();
            streams.source = new StandardTokenizer(matchVersion, reader);
            streams.result = new StandardFilter(streams.source);
            streams.result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(matchVersion),
                                            streams.result, stoptable);
            streams.result = new FrenchStemFilter(streams.result, excltable);
            // Convert to lowercase after stemming!
            streams.result = new LowerCaseFilter(streams.result);
            PreviousTokenStream = streams;
        }
        else
        {
            streams.source.Reset(reader);
        }
        return streams.result;
    }
}





/*
 * Removes elisions from a {@link TokenStream}. For example, "l'avion" (the plane) will be
 * tokenized as "avion" (plane).
 * <p>
 * Note that {@link StandardTokenizer} sees " ' " as a space, and cuts it out.
 * 
 * @see <a href="http://fr.wikipedia.org/wiki/%C3%89lision">Elision in Wikipedia</a>
 */
public sealed class ElisionFilter : TokenFilter
{
    private CharArraySet articles = null;
    private ITermAttribute termAtt;

    private static char[] apostrophes = { '\'', '’' };

    public void SetArticles(ISet<string> articles)
    {
        if (articles is CharArraySet)
            this.articles = (CharArraySet)articles;
        else
            this.articles = new CharArraySet(articles, true);
    }

    /*
     * Constructs an elision filter with standard stop words
     */
    internal ElisionFilter(TokenStream input)
        : this(input, new[] { "l", "m", "t", "qu", "n", "s", "j" })
    { }

    /*
     * Constructs an elision filter with a Set of stop words
     */
    public ElisionFilter(TokenStream input, ISet<string> articles)
        : base(input)
    {
        SetArticles(articles);
        termAtt = AddAttribute<ITermAttribute>();
    }

    /*
     * Constructs an elision filter with an array of stop words
     */
    public ElisionFilter(TokenStream input, IEnumerable<string> articles)
        : base(input)
    {
        this.articles = new CharArraySet(articles, true);
        termAtt = AddAttribute<ITermAttribute>();
    }

    /*
     * Increments the {@link TokenStream} with a {@link TermAttribute} without elisioned start
     */
    public override sealed bool IncrementToken()
    {
        if (input.IncrementToken())
        {
            char[] termBuffer = termAtt.TermBuffer();
            int termLength = termAtt.TermLength();

            int minPoz = int.MaxValue;
            for (int i = 0; i < apostrophes.Length; i++)
            {
                char apos = apostrophes[i];
                // The equivalent of String.indexOf(ch)
                for (int poz = 0; poz < termLength; poz++)
                {
                    if (termBuffer[poz] == apos)
                    {
                        minPoz = Math.Min(poz, minPoz);
                        break;
                    }
                }
            }

            // An apostrophe has been found. If the prefix is an article strip it off.
            if (minPoz != int.MaxValue
                && articles.Contains(termAtt.TermBuffer(), 0, minPoz))
            {
                termAtt.SetTermBuffer(termAtt.TermBuffer(), minPoz + 1, termAtt.TermLength() - (minPoz + 1));
            }

            return true;
        }
        else
        {
            return false;
        }
    }
}




namespace Lucene.Net.Analysis.Fr
{
    /*
 * A {@link TokenFilter} that stems french words. 
 * <p>
 * It supports a table of words that should
 * not be stemmed at all. The used stemmer can be changed at runtime after the
 * filter object is created (as long as it is a {@link FrenchStemmer}).
 * </p>
 * NOTE: This stemmer does not implement the Snowball algorithm correctly,
 * especially involving case problems. It is recommended that you consider using
 * the "French" stemmer in the snowball package instead. This stemmer will likely
 * be deprecated in a future release.
 */
    public sealed class FrenchStemFilter : TokenFilter
    {

        /*
         * The actual token in the input stream.
         */
        private FrenchStemmer stemmer = null;
        private ISet<string> exclusions = null;

        private ITermAttribute termAtt;

        public FrenchStemFilter(TokenStream _in)
            : base(_in)
        {

            stemmer = new FrenchStemmer();
            termAtt = AddAttribute<ITermAttribute>();
        }


        public FrenchStemFilter(TokenStream _in, ISet<string> exclusiontable)
            : this(_in)
        {
            exclusions = exclusiontable;
        }

        /*
         * @return  Returns true for the next token in the stream, or false at EOS
         */
        public override bool IncrementToken()
        {
            if (input.IncrementToken())
            {
                String term = termAtt.Term;

                // Check the exclusion table
                if (exclusions == null || !exclusions.Contains(term))
                {
                    String s = stemmer.Stem(term);
                    // If not stemmed, don't waste the time  adjusting the token.
                    if ((s != null) && !s.Equals(term))
                        termAtt.SetTermBuffer(s);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        /*
         * Set a alternative/custom {@link FrenchStemmer} for this filter.
         */
        public void SetStemmer(FrenchStemmer stemmer)
        {
            if (stemmer != null)
            {
                this.stemmer = stemmer;
            }
        }
        /*
         * Set an alternative exclusion list for this filter.
         */
        public void SetExclusionTable(IDictionary<string, string> exclusiontable)
        {
            exclusions = Support.Compatibility.SetFactory.CreateHashSet(exclusiontable.Keys);
        }
    }
}

/*
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 *
*/




/*
 * A stemmer for French words. 
 * <p>
 * The algorithm is based on the work of
 * Dr Martin Porter on his snowball project<br>
 * refer to http://snowball.sourceforge.net/french/stemmer.html<br>
 * (French stemming algorithm) for details
 * </p>
 */

public class FrenchStemmer
{

    /*
     * Buffer for the terms while stemming them.
     */
    private StringBuilder sb = new StringBuilder();

    /*
     * A temporary buffer, used to reconstruct R2
     */
    private StringBuilder tb = new StringBuilder();

    /*
     * Region R0 is equal to the whole buffer
     */
    private String R0;

    /*
     * Region RV
     * "If the word begins with two vowels, RV is the region after the third letter,
     * otherwise the region after the first vowel not at the beginning of the word,
     * or the end of the word if these positions cannot be found."
     */
    private String RV;

    /*
     * Region R1
     * "R1 is the region after the first non-vowel following a vowel
     * or is the null region at the end of the word if there is no such non-vowel"
     */
    private String R1;

    /*
     * Region R2
     * "R2 is the region after the first non-vowel in R1 following a vowel
     * or is the null region at the end of the word if there is no such non-vowel"
     */
    private String R2;


    /*
     * Set to true if we need to perform step 2
     */
    private bool suite;

    /*
     * Set to true if the buffer was modified
     */
    private bool modified;


    /*
     * Stems the given term to a unique <tt>discriminator</tt>.
     *
     * @param term  java.langString The term that should be stemmed
     * @return java.lang.String  Discriminator for <tt>term</tt>
     */
    protected internal String Stem(String term)
    {
        if (!IsStemmable(term))
        {
            return term;
        }

        // Use lowercase for medium stemming.
        term = term.ToLower();

        // Reset the StringBuilder.
        sb.Length = 0;
        sb.Insert(0, term);

        // reset the bools
        modified = false;
        suite = false;

        sb = TreatVowels(sb);

        SetStrings();

        Step1();

        if (!modified || suite)
        {
            if (RV != null)
            {
                suite = Step2A();
                if (!suite)
                    Step2B();
            }
        }

        if (modified || suite)
            Step3();
        else
            Step4();

        Step5();

        Step6();

        return sb.ToString();
    }

    /*
     * Sets the search region Strings<br>
     * it needs to be done each time the buffer was modified
     */
    private void SetStrings()
    {
        // set the strings
        R0 = sb.ToString();
        RV = RetrieveRV(sb);
        R1 = RetrieveR(sb);
        if (R1 != null)
        {
            tb.Length = 0;
            tb.Insert(0, R1);
            R2 = RetrieveR(tb);
        }
        else
            R2 = null;
    }

    /*
     * First step of the Porter Algorithm<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     */
    private void Step1()
    {
        String[] suffix = { "ances", "iqUes", "ismes", "ables", "istes", "ance", "iqUe", "isme", "able", "iste" };
        DeleteFrom(R2, suffix);

        ReplaceFrom(R2, new String[] { "logies", "logie" }, "log");
        ReplaceFrom(R2, new String[] { "usions", "utions", "usion", "ution" }, "u");
        ReplaceFrom(R2, new String[] { "ences", "ence" }, "ent");

        String[] search = { "atrices", "ateurs", "ations", "atrice", "ateur", "ation" };
        DeleteButSuffixFromElseReplace(R2, search, "ic", true, R0, "iqU");

        DeleteButSuffixFromElseReplace(R2, new String[] { "ements", "ement" }, "eus", false, R0, "eux");
        DeleteButSuffixFrom(R2, new String[] { "ements", "ement" }, "ativ", false);
        DeleteButSuffixFrom(R2, new String[] { "ements", "ement" }, "iv", false);
        DeleteButSuffixFrom(R2, new String[] { "ements", "ement" }, "abl", false);
        DeleteButSuffixFrom(R2, new String[] { "ements", "ement" }, "iqU", false);

        DeleteFromIfTestVowelBeforeIn(R1, new String[] { "issements", "issement" }, false, R0);
        DeleteFrom(RV, new String[] { "ements", "ement" });

        DeleteButSuffixFromElseReplace(R2, new[] { "it\u00e9s", "it\u00e9" }, "abil", false, R0, "abl");
        DeleteButSuffixFromElseReplace(R2, new[] { "it\u00e9s", "it\u00e9" }, "ic", false, R0, "iqU");
        DeleteButSuffixFrom(R2, new[] { "it\u00e9s", "it\u00e9" }, "iv", true);

        String[] autre = { "ifs", "ives", "if", "ive" };
        DeleteButSuffixFromElseReplace(R2, autre, "icat", false, R0, "iqU");
        DeleteButSuffixFromElseReplace(R2, autre, "at", true, R2, "iqU");

        ReplaceFrom(R0, new String[] { "eaux" }, "eau");

        ReplaceFrom(R1, new String[] { "aux" }, "al");

        DeleteButSuffixFromElseReplace(R2, new String[] { "euses", "euse" }, "", true, R1, "eux");

        DeleteFrom(R2, new String[] { "eux" });

        // if one of the next steps is performed, we will need to perform step2a
        bool temp = false;
        temp = ReplaceFrom(RV, new String[] { "amment" }, "ant");
        if (temp == true)
            suite = true;
        temp = ReplaceFrom(RV, new String[] { "emment" }, "ent");
        if (temp == true)
            suite = true;
        temp = DeleteFromIfTestVowelBeforeIn(RV, new String[] { "ments", "ment" }, true, RV);
        if (temp == true)
            suite = true;

    }

    /*
     * Second step (A) of the Porter Algorithm<br>
     * Will be performed if nothing changed from the first step
     * or changed were done in the amment, emment, ments or ment suffixes<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     *
     * @return bool - true if something changed in the StringBuilder
     */
    private bool Step2A()
    {
        String[] search = { "\u00eemes", "\u00eetes", "iraIent", "irait", "irais", "irai", "iras", "ira",
                            "irent", "iriez", "irez", "irions", "irons", "iront",
                            "issaIent", "issais", "issantes", "issante", "issants", "issant",
                            "issait", "issais", "issions", "issons", "issiez", "issez", "issent",
                            "isses", "isse", "ir", "is", "\u00eet", "it", "ies", "ie", "i" };
        return DeleteFromIfTestVowelBeforeIn(RV, search, false, RV);
    }

    /*
     * Second step (B) of the Porter Algorithm<br>
     * Will be performed if step 2 A was performed unsuccessfully<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     */
    private void Step2B()
    {
        String[] suffix = { "eraIent", "erais", "erait", "erai", "eras", "erions", "eriez",
                            "erons", "eront","erez", "\u00e8rent", "era", "\u00e9es", "iez",
                            "\u00e9e", "\u00e9s", "er", "ez", "\u00e9" };
        DeleteFrom(RV, suffix);

        String[] search = { "assions", "assiez", "assent", "asses", "asse", "aIent",
                            "antes", "aIent", "Aient", "ante", "\u00e2mes", "\u00e2tes", "ants", "ant",
                            "ait", "a\u00eet", "ais", "Ait", "A\u00eet", "Ais", "\u00e2t", "as", "ai", "Ai", "a" };
        DeleteButSuffixFrom(RV, search, "e", true);

        DeleteFrom(R2, new String[] { "ions" });
    }

    /*
     * Third step of the Porter Algorithm<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     */
    private void Step3()
    {
        if (sb.Length > 0)
        {
            char ch = sb[sb.Length - 1];
            if (ch == 'Y')
            {
                sb[sb.Length - 1] = 'i';
                SetStrings();
            }
            else if (ch == 'ç')
            {
                sb[sb.Length - 1] = 'c';
                SetStrings();
            }
        }
    }

    /*
     * Fourth step of the Porter Algorithm<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     */
    private void Step4()
    {
        if (sb.Length > 1)
        {
            char ch = sb[sb.Length - 1];
            if (ch == 's')
            {
                char b = sb[sb.Length - 2];
                if (b != 'a' && b != 'i' && b != 'o' && b != 'u' && b != 'è' && b != 's')
                {
                    sb.Length = sb.Length - 1;
                    SetStrings();
                }
            }
        }
        bool found = DeleteFromIfPrecededIn(R2, new String[] { "ion" }, RV, "s");
        if (!found)
            found = DeleteFromIfPrecededIn(R2, new String[] { "ion" }, RV, "t");

        ReplaceFrom(RV, new String[] { "I\u00e8re", "i\u00e8re", "Ier", "ier" }, "i");
        DeleteFrom(RV, new String[] { "e" });
        DeleteFromIfPrecededIn(RV, new String[] { "\u00eb" }, R0, "gu");
    }

    /*
     * Fifth step of the Porter Algorithm<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     */
    private void Step5()
    {
        if (R0 != null)
        {
            if (R0.EndsWith("enn") || R0.EndsWith("onn") || R0.EndsWith("ett") || R0.EndsWith("ell") || R0.EndsWith("eill"))
            {
                sb.Length = sb.Length - 1;
                SetStrings();
            }
        }
    }

    /*
     * Sixth (and last!) step of the Porter Algorithm<br>
     * refer to http://snowball.sourceforge.net/french/stemmer.html for an explanation
     */
    private void Step6()
    {
        if (R0 != null && R0.Length > 0)
        {
            bool seenVowel = false;
            bool seenConson = false;
            int pos = -1;
            for (int i = R0.Length - 1; i > -1; i--)
            {
                char ch = R0[i];
                if (IsVowel(ch))
                {
                    if (!seenVowel)
                    {
                        if (ch == 'é' || ch == 'è')
                        {
                            pos = i;
                            break;
                        }
                    }
                    seenVowel = true;
                }
                else
                {
                    if (seenVowel)
                        break;
                    else
                        seenConson = true;
                }
            }
            if (pos > -1 && seenConson && !seenVowel)
                sb[pos] = 'e';
        }
    }

    /*
     * Delete a suffix searched in zone "source" if zone "from" contains prefix + search string
     *
     * @param source java.lang.String - the primary source zone for search
     * @param search java.lang.String[] - the strings to search for suppression
     * @param from java.lang.String - the secondary source zone for search
     * @param prefix java.lang.String - the prefix to add to the search string to test
     * @return bool - true if modified
     */
    private bool DeleteFromIfPrecededIn(String source, String[] search, String from, String prefix)
    {
        bool found = false;
        if (source != null)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (source.EndsWith(search[i]))
                {
                    if (from != null && from.EndsWith(prefix + search[i]))
                    {
                        sb.Length = sb.Length - search[i].Length;
                        found = true;
                        SetStrings();
                        break;
                    }
                }
            }
        }
        return found;
    }

    /*
     * Delete a suffix searched in zone "source" if the preceding letter is (or isn't) a vowel
     *
     * @param source java.lang.String - the primary source zone for search
     * @param search java.lang.String[] - the strings to search for suppression
     * @param vowel bool - true if we need a vowel before the search string
     * @param from java.lang.String - the secondary source zone for search (where vowel could be)
     * @return bool - true if modified
     */
    private bool DeleteFromIfTestVowelBeforeIn(String source, String[] search, bool vowel, String from)
    {
        bool found = false;
        if (source != null && from != null)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (source.EndsWith(search[i]))
                {
                    if ((search[i].Length + 1) <= from.Length)
                    {
                        bool test = IsVowel(sb[sb.Length - (search[i].Length + 1)]);
                        if (test == vowel)
                        {
                            sb.Length = sb.Length - search[i].Length;
                            modified = true;
                            found = true;
                            SetStrings();
                            break;
                        }
                    }
                }
            }
        }
        return found;
    }

    /*
     * Delete a suffix searched in zone "source" if preceded by the prefix
     *
     * @param source java.lang.String - the primary source zone for search
     * @param search java.lang.String[] - the strings to search for suppression
     * @param prefix java.lang.String - the prefix to add to the search string to test
     * @param without bool - true if it will be deleted even without prefix found
     */
    private void DeleteButSuffixFrom(String source, String[] search, String prefix, bool without)
    {
        if (source != null)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (source.EndsWith(prefix + search[i]))
                {
                    sb.Length = sb.Length - (prefix.Length + search[i].Length);
                    modified = true;
                    SetStrings();
                    break;
                }
                else if (without && source.EndsWith(search[i]))
                {
                    sb.Length = sb.Length - search[i].Length;
                    modified = true;
                    SetStrings();
                    break;
                }
            }
        }
    }

    /*
     * Delete a suffix searched in zone "source" if preceded by prefix<br>
     * or replace it with the replace string if preceded by the prefix in the zone "from"<br>
     * or delete the suffix if specified
     *
     * @param source java.lang.String - the primary source zone for search
     * @param search java.lang.String[] - the strings to search for suppression
     * @param prefix java.lang.String - the prefix to add to the search string to test
     * @param without bool - true if it will be deleted even without prefix found
     */
    private void DeleteButSuffixFromElseReplace(String source, String[] search, String prefix, bool without, String from, String replace)
    {
        if (source != null)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (source.EndsWith(prefix + search[i]))
                {
                    sb.Length = sb.Length - (prefix.Length + search[i].Length);
                    modified = true;
                    SetStrings();
                    break;
                }
                else if (from != null && from.EndsWith(prefix + search[i]))
                {
                    // java equivalent of replace
                    sb.Length = sb.Length - (prefix.Length + search[i].Length);
                    sb.Append(replace);

                    modified = true;
                    SetStrings();
                    break;
                }
                else if (without && source.EndsWith(search[i]))
                {
                    sb.Length = sb.Length - search[i].Length;
                    modified = true;
                    SetStrings();
                    break;
                }
            }
        }
    }

    /*
     * Replace a search string with another within the source zone
     *
     * @param source java.lang.String - the source zone for search
     * @param search java.lang.String[] - the strings to search for replacement
     * @param replace java.lang.String - the replacement string
     */
    private bool ReplaceFrom(String source, String[] search, String replace)
    {
        bool found = false;
        if (source != null)
        {
            for (int i = 0; i < search.Length; i++)
            {
                if (source.EndsWith(search[i]))
                {
                    // java equivalent for replace
                    sb.Length = sb.Length - search[i].Length;
                    sb.Append(replace);

                    modified = true;
                    found = true;
                    SetStrings();
                    break;
                }
            }
        }
        return found;
    }

    /*
     * Delete a search string within the source zone
     *
     * @param source the source zone for search
     * @param suffix the strings to search for suppression
     */
    private void DeleteFrom(String source, String[] suffix)
    {
        if (source != null)
        {
            for (int i = 0; i < suffix.Length; i++)
            {
                if (source.EndsWith(suffix[i]))
                {
                    sb.Length = sb.Length - suffix[i].Length;
                    modified = true;
                    SetStrings();
                    break;
                }
            }
        }
    }

    /*
     * Test if a char is a french vowel, including accentuated ones
     *
     * @param ch the char to test
     * @return bool - true if the char is a vowel
     */
    private bool IsVowel(char ch)
    {
        switch (ch)
        {
            case 'a':
            case 'e':
            case 'i':
            case 'o':
            case 'u':
            case 'y':
            case 'â':
            case 'à':
            case 'ë':
            case 'é':
            case 'ê':
            case 'è':
            case 'ï':
            case 'î':
            case 'ô':
            case 'ü':
            case 'ù':
            case 'û':
                return true;
            default:
                return false;
        }
    }

    /*
     * Retrieve the "R zone" (1 or 2 depending on the buffer) and return the corresponding string<br>
     * "R is the region after the first non-vowel following a vowel
     * or is the null region at the end of the word if there is no such non-vowel"<br>
     * @param buffer java.lang.StringBuilder - the in buffer
     * @return java.lang.String - the resulting string
     */
    private String RetrieveR(StringBuilder buffer)
    {
        int len = buffer.Length;
        int pos = -1;
        for (int c = 0; c < len; c++)
        {
            if (IsVowel(buffer[c]))
            {
                pos = c;
                break;
            }
        }
        if (pos > -1)
        {
            int consonne = -1;
            for (int c = pos; c < len; c++)
            {
                if (!IsVowel(buffer[c]))
                {
                    consonne = c;
                    break;
                }
            }
            if (consonne > -1 && (consonne + 1) < len)
                return buffer.ToString(consonne + 1, len - (consonne + 1));
            else
                return null;
        }
        else
            return null;
    }

    /*
     * Retrieve the "RV zone" from a buffer an return the corresponding string<br>
     * "If the word begins with two vowels, RV is the region after the third letter,
     * otherwise the region after the first vowel not at the beginning of the word,
     * or the end of the word if these positions cannot be found."<br>
     * @param buffer java.lang.StringBuilder - the in buffer
     * @return java.lang.String - the resulting string
     */
    private String RetrieveRV(StringBuilder buffer)
    {
        int len = buffer.Length;
        if (buffer.Length > 3)
        {
            if (IsVowel(buffer[0]) && IsVowel(buffer[1]))
            {
                return buffer.ToString(3, len - 3);
            }
            else
            {
                int pos = 0;
                for (int c = 1; c < len; c++)
                {
                    if (IsVowel(buffer[c]))
                    {
                        pos = c;
                        break;
                    }
                }
                if (pos + 1 < len)
                    return buffer.ToString(pos + 1, len - (pos + 1));
                else
                    return null;
            }
        }
        else
            return null;
    }



    /*
     * Turns u and i preceded AND followed by a vowel to UpperCase<br>
     * Turns y preceded OR followed by a vowel to UpperCase<br>
     * Turns u preceded by q to UpperCase<br>
     *
     * @param buffer java.util.StringBuilder - the buffer to treat
     * @return java.util.StringBuilder - the treated buffer
     */
    private StringBuilder TreatVowels(StringBuilder buffer)
    {
        for (int c = 0; c < buffer.Length; c++)
        {
            char ch = buffer[c];

            if (c == 0) // first char
            {
                if (buffer.Length > 1)
                {
                    if (ch == 'y' && IsVowel(buffer[c + 1]))
                        buffer[c] = 'Y';
                }
            }
            else if (c == buffer.Length - 1) // last char
            {
                if (ch == 'u' && buffer[c - 1] == 'q')
                    buffer[c] = 'U';
                if (ch == 'y' && IsVowel(buffer[c - 1]))
                    buffer[c] = 'Y';
            }
            else // other cases
            {
                if (ch == 'u')
                {
                    if (buffer[c - 1] == 'q')
                        buffer[c] = 'U';
                    else if (IsVowel(buffer[c - 1]) && IsVowel(buffer[c + 1]))
                        buffer[c] = 'U';
                }
                if (ch == 'i')
                {
                    if (IsVowel(buffer[c - 1]) && IsVowel(buffer[c + 1]))
                        buffer[c] = 'I';
                }
                if (ch == 'y')
                {
                    if (IsVowel(buffer[c - 1]) || IsVowel(buffer[c + 1]))
                        buffer[c] = 'Y';
                }
            }
        }

        return buffer;
    }

    /*
     * Checks a term if it can be processed correctly.
     *
     * @return bool - true if, and only if, the given term consists in letters.
     */
    private bool IsStemmable(String term)
    {
        bool upper = false;
        int first = -1;
        for (int c = 0; c < term.Length; c++)
        {
            // Discard terms that contain non-letter chars.
            if (!char.IsLetter(term[c]))
            {
                return false;
            }
            // Discard terms that contain multiple uppercase letters.
            if (char.IsUpper(term[c]))
            {
                if (upper)
                {
                    return false;
                }
                // First encountered uppercase letter, set flag and save
                // position.
                else
                {
                    first = c;
                    upper = true;
                }
            }
        }
        // Discard the term if it contains a single uppercase letter that
        // is not starting the term.
        if (first > 0)
        {
            return false;
        }
        return true;
    }
}


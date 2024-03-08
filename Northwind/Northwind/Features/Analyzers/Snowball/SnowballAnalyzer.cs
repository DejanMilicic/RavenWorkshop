/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Analysis.Tokenattributes;
using System.Text;


/// <summary>Filters <see cref="StandardTokenizer"/> with <see cref="StandardFilter"/>, {@link
/// LowerCaseFilter}, <see cref="StopFilter"/> and <see cref="SnowballFilter"/>.
/// 
/// Available stemmers are listed in <see cref="SF.Snowball.Ext"/>.  The name of a
/// stemmer is the part of the class name before "Stemmer", e.g., the stemmer in
/// <see cref="EnglishStemmer"/> is named "English".
/// 
/// <p><b>NOTE:</b> This class uses the same <see cref="Version"/>
/// dependent settings as <see cref="StandardAnalyzer"/></p>
/// </summary>
public class SnowballAnalyzer : Analyzer
{
    private System.String name;
    private ISet<string> stopSet;
    private readonly Version matchVersion;

    public SnowballAnalyzer() : this(Version.LUCENE_30, "English")
    {

    }

    /// <summary>Builds the named analyzer with no stop words. </summary>
    public SnowballAnalyzer(Version matchVersion, System.String name)
    {
        this.name = name;
        SetOverridesTokenStreamMethod<SnowballAnalyzer>();
        this.matchVersion = matchVersion;
    }

    /// <summary>Builds the named analyzer with the given stop words. </summary>
    [Obsolete("Use SnowballAnalyzer(Version, string, ISet) instead.")]
    public SnowballAnalyzer(Version matchVersion, System.String name, System.String[] stopWords)
        : this(matchVersion, name)
    {
        stopSet = StopFilter.MakeStopSet(stopWords);
    }

    /// <summary>
    /// Builds the named analyzer with the given stop words.
    /// </summary>
    public SnowballAnalyzer(Version matchVersion, string name, ISet<string> stopWords)
        : this(matchVersion, name)
    {
        stopSet = CharArraySet.UnmodifiableSet(CharArraySet.Copy(stopWords));
    }

    /// <summary>Constructs a <see cref="StandardTokenizer"/> filtered by a {@link
    /// StandardFilter}, a <see cref="LowerCaseFilter"/> and a <see cref="StopFilter"/>. 
    /// </summary>
    public override TokenStream TokenStream(System.String fieldName, System.IO.TextReader reader)
    {
        TokenStream result = new StandardTokenizer(matchVersion, reader);
        result = new StandardFilter(result);
        result = new LowerCaseFilter(result);
        if (stopSet != null)
            result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(matchVersion),
                                    result, stopSet);
        result = new SnowballFilter(result, name);
        return result;
    }

    private class SavedStreams
    {
        internal Tokenizer source;
        internal TokenStream result;
    };

    /* Returns a (possibly reused) {@link StandardTokenizer} filtered by a 
        * {@link StandardFilter}, a {@link LowerCaseFilter}, 
        * a {@link StopFilter}, and a {@link SnowballFilter} */

    public override TokenStream ReusableTokenStream(String fieldName, TextReader reader)
    {
        if (overridesTokenStreamMethod)
        {
            // LUCENE-1678: force fallback to tokenStream() if we
            // have been subclassed and that subclass overrides
            // tokenStream but not reusableTokenStream
            return TokenStream(fieldName, reader);
        }

        SavedStreams streams = (SavedStreams)PreviousTokenStream;
        if (streams == null)
        {
            streams = new SavedStreams();
            streams.source = new StandardTokenizer(matchVersion, reader);
            streams.result = new StandardFilter(streams.source);
            streams.result = new LowerCaseFilter(streams.result);
            if (stopSet != null)
                streams.result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(matchVersion),
                                                streams.result, stopSet);
            streams.result = new SnowballFilter(streams.result, name);
            PreviousTokenStream = streams;
        }
        else
        {
            streams.source.Reset(reader);
        }
        return streams.result;
    }
}

/// <summary>A filter that stems words using a Snowball-generated stemmer.
/// 
/// Available stemmers are listed in <see cref="SF.Snowball.Ext"/>.  The name of a
/// stemmer is the part of the class name before "Stemmer", e.g., the stemmer in
/// <see cref="EnglishStemmer"/> is named "English".
/// </summary>

public sealed class SnowballFilter : TokenFilter
{
    private static readonly System.Object[] EMPTY_ARGS = new System.Object[0];

    private SnowballProgram stemmer;
    private ITermAttribute termAtt;
    //private System.Reflection.MethodInfo stemMethod;

    public SnowballFilter(TokenStream input, SnowballProgram stemmer)
        : base(input)
    {
        this.stemmer = stemmer;
        termAtt = AddAttribute<ITermAttribute>();
    }

    /// <summary>Construct the named stemming filter.
    /// 
    /// </summary>
    /// <param name="input">the input tokens to stem
    /// </param>
    /// <param name="name">the name of a stemmer
    /// </param>
    public SnowballFilter(TokenStream input, System.String name) : base(input)
    {
        stemmer = new EnglishStemmer();
        termAtt = AddAttribute<ITermAttribute>();
    }

    /// <summary>Returns the next input Token, after being stemmed </summary>
    public sealed override bool IncrementToken()
    {
        if (input.IncrementToken())
        {
            String originalTerm = termAtt.Term;
            stemmer.SetCurrent(originalTerm);
            stemmer.Stem();
            String finalTerm = stemmer.GetCurrent();
            // Don't bother updating, if it is unchanged.
            if (!originalTerm.Equals(finalTerm))
                termAtt.SetTermBuffer(finalTerm);
            return true;
        }
        else
        {
            return false;
        }
    }
}

/// <summary>
/// This is the rev 500 of the snowball SVN trunk,
/// but modified:
/// made abstract and introduced abstract method stem to avoid expensive reflection in filter class
/// </summary>
public abstract class SnowballProgram
{
    protected internal SnowballProgram()
    {
        current = new System.Text.StringBuilder();
        SetCurrent("");
    }

    public abstract bool Stem();

    /// <summary> Set the current string.</summary>
    public virtual void SetCurrent(System.String value)
    {
        //// current.Replace(current.ToString(0, current.Length - 0), value_Renamed, 0, current.Length - 0);
        current.Remove(0, current.Length);
        current.Append(value);
        cursor = 0;
        limit = current.Length;
        limit_backward = 0;
        bra = cursor;
        ket = limit;
    }

    /// <summary> Get the current string.</summary>
    virtual public System.String GetCurrent()
    {
        string result = current.ToString();
        // Make a new StringBuffer.  If we reuse the old one, and a user of
        // the library keeps a reference to the buffer returned (for example,
        // by converting it to a String in a way which doesn't force a copy),
        // the buffer size will not decrease, and we will risk wasting a large
        // amount of memory.
        // Thanks to Wolfram Esser for spotting this problem.
        current = new StringBuilder();
        return result;
    }

    // current string
    protected internal System.Text.StringBuilder current;

    protected internal int cursor;
    protected internal int limit;
    protected internal int limit_backward;
    protected internal int bra;
    protected internal int ket;

    protected internal virtual void copy_from(SnowballProgram other)
    {
        current = other.current;
        cursor = other.cursor;
        limit = other.limit;
        limit_backward = other.limit_backward;
        bra = other.bra;
        ket = other.ket;
    }

    protected internal virtual bool in_grouping(char[] s, int min, int max)
    {
        if (cursor >= limit)
            return false;
        char ch = current[cursor];
        if (ch > max || ch < min)
            return false;
        ch -= (char)(min);
        if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
            return false;
        cursor++;
        return true;
    }

    protected internal virtual bool in_grouping_b(char[] s, int min, int max)
    {
        if (cursor <= limit_backward)
            return false;
        char ch = current[cursor - 1];
        if (ch > max || ch < min)
            return false;
        ch -= (char)(min);
        if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
            return false;
        cursor--;
        return true;
    }

    protected internal virtual bool out_grouping(char[] s, int min, int max)
    {
        if (cursor >= limit)
            return false;
        char ch = current[cursor];
        if (ch > max || ch < min)
        {
            cursor++;
            return true;
        }
        ch -= (char)(min);
        if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
        {
            cursor++;
            return true;
        }
        return false;
    }

    protected internal virtual bool out_grouping_b(char[] s, int min, int max)
    {
        if (cursor <= limit_backward)
            return false;
        char ch = current[cursor - 1];
        if (ch > max || ch < min)
        {
            cursor--;
            return true;
        }
        ch -= (char)(min);
        if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
        {
            cursor--;
            return true;
        }
        return false;
    }

    protected internal virtual bool in_range(int min, int max)
    {
        if (cursor >= limit)
            return false;
        char ch = current[cursor];
        if (ch > max || ch < min)
            return false;
        cursor++;
        return true;
    }

    protected internal virtual bool in_range_b(int min, int max)
    {
        if (cursor <= limit_backward)
            return false;
        char ch = current[cursor - 1];
        if (ch > max || ch < min)
            return false;
        cursor--;
        return true;
    }

    protected internal virtual bool out_range(int min, int max)
    {
        if (cursor >= limit)
            return false;
        char ch = current[cursor];
        if (!(ch > max || ch < min))
            return false;
        cursor++;
        return true;
    }

    protected internal virtual bool out_range_b(int min, int max)
    {
        if (cursor <= limit_backward)
            return false;
        char ch = current[cursor - 1];
        if (!(ch > max || ch < min))
            return false;
        cursor--;
        return true;
    }

    protected internal virtual bool eq_s(int s_size, System.String s)
    {
        if (limit - cursor < s_size)
            return false;
        int i;
        for (i = 0; i != s_size; i++)
        {
            if (current[cursor + i] != s[i])
                return false;
        }
        cursor += s_size;
        return true;
    }

    protected internal virtual bool eq_s_b(int s_size, System.String s)
    {
        if (cursor - limit_backward < s_size)
            return false;
        int i;
        for (i = 0; i != s_size; i++)
        {
            if (current[cursor - s_size + i] != s[i])
                return false;
        }
        cursor -= s_size;
        return true;
    }

    protected internal virtual bool eq_v(System.Text.StringBuilder s)
    {
        return eq_s(s.Length, s.ToString());
    }

    protected internal virtual bool eq_v_b(System.Text.StringBuilder s)
    {
        return eq_s_b(s.Length, s.ToString());
    }

    protected internal virtual int find_among(Among[] v, int v_size)
    {
        int i = 0;
        int j = v_size;

        int c = cursor;
        int l = limit;

        int common_i = 0;
        int common_j = 0;

        bool first_key_inspected = false;

        while (true)
        {
            int k = i + ((j - i) >> 1);
            int diff = 0;
            int common = common_i < common_j ? common_i : common_j; // smaller
            Among w = v[k];
            int i2;
            for (i2 = common; i2 < w.s_size; i2++)
            {
                if (c + common == l)
                {
                    diff = -1;
                    break;
                }
                diff = current[c + common] - w.s[i2];
                if (diff != 0)
                    break;
                common++;
            }
            if (diff < 0)
            {
                j = k;
                common_j = common;
            }
            else
            {
                i = k;
                common_i = common;
            }
            if (j - i <= 1)
            {
                if (i > 0)
                    break; // v->s has been inspected
                if (j == i)
                    break; // only one item in v

                // - but now we need to go round once more to get
                // v->s inspected. This looks messy, but is actually
                // the optimal approach.

                if (first_key_inspected)
                    break;
                first_key_inspected = true;
            }
        }
        while (true)
        {
            Among w = v[i];
            if (common_i >= w.s_size)
            {
                cursor = c + w.s_size;
                if (w.method == null)
                    return w.result;
                bool res;
                try
                {
                    System.Object resobj = w.method.Invoke(w.methodobject, (System.Object[])new System.Object[0]);
                    // {{Aroush}} UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
                    res = resobj.ToString().Equals("true");
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    res = false;
                    // FIXME - debug message
                }
                catch (System.UnauthorizedAccessException)
                {
                    res = false;
                    // FIXME - debug message
                }
                cursor = c + w.s_size;
                if (res)
                    return w.result;
            }
            i = w.substring_i;
            if (i < 0)
                return 0;
        }
    }

    // find_among_b is for backwards processing. Same comments apply
    protected internal virtual int find_among_b(Among[] v, int v_size)
    {
        int i = 0;
        int j = v_size;

        int c = cursor;
        int lb = limit_backward;

        int common_i = 0;
        int common_j = 0;

        bool first_key_inspected = false;

        while (true)
        {
            int k = i + ((j - i) >> 1);
            int diff = 0;
            int common = common_i < common_j ? common_i : common_j;
            Among w = v[k];
            int i2;
            for (i2 = w.s_size - 1 - common; i2 >= 0; i2--)
            {
                if (c - common == lb)
                {
                    diff = -1;
                    break;
                }
                diff = current[c - 1 - common] - w.s[i2];
                if (diff != 0)
                    break;
                common++;
            }
            if (diff < 0)
            {
                j = k;
                common_j = common;
            }
            else
            {
                i = k;
                common_i = common;
            }
            if (j - i <= 1)
            {
                if (i > 0)
                    break;
                if (j == i)
                    break;
                if (first_key_inspected)
                    break;
                first_key_inspected = true;
            }
        }
        while (true)
        {
            Among w = v[i];
            if (common_i >= w.s_size)
            {
                cursor = c - w.s_size;
                if (w.method == null)
                    return w.result;

                bool res;
                try
                {
                    System.Object resobj = w.method.Invoke(w.methodobject, (System.Object[])new System.Object[0]);
                    // {{Aroush}} UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
                    res = resobj.ToString().Equals("true");
                }
                catch (System.Reflection.TargetInvocationException)
                {
                    res = false;
                    // FIXME - debug message
                }
                catch (System.UnauthorizedAccessException)
                {
                    res = false;
                    // FIXME - debug message
                }
                cursor = c - w.s_size;
                if (res)
                    return w.result;
            }
            i = w.substring_i;
            if (i < 0)
                return 0;
        }
    }

    /* to replace chars between c_bra and c_ket in current by the
    * chars in s.
    */
    protected internal virtual int replace_s(int c_bra, int c_ket, System.String s)
    {
        int adjustment = s.Length - (c_ket - c_bra);
        if (current.Length > bra)
            current.Replace(current.ToString(bra, ket - bra), s, bra, ket - bra);
        else
            current.Append(s);
        limit += adjustment;
        if (cursor >= c_ket)
            cursor += adjustment;
        else if (cursor > c_bra)
            cursor = c_bra;
        return adjustment;
    }

    protected internal virtual void slice_check()
    {

    }

    protected internal virtual void slice_from(System.String s)
    {
        slice_check();
        replace_s(bra, ket, s);
    }

    protected internal virtual void slice_from(System.Text.StringBuilder s)
    {
        slice_from(s.ToString());
    }

    protected internal virtual void slice_del()
    {
        slice_from("");
    }

    protected internal virtual void insert(int c_bra, int c_ket, System.String s)
    {
        int adjustment = replace_s(c_bra, c_ket, s);
        if (c_bra <= bra)
            bra += adjustment;
        if (c_bra <= ket)
            ket += adjustment;
    }

    protected internal virtual void insert(int c_bra, int c_ket, System.Text.StringBuilder s)
    {
        insert(c_bra, c_ket, s.ToString());
    }

    /* Copy the slice into the supplied StringBuffer */
    protected internal virtual System.Text.StringBuilder slice_to(System.Text.StringBuilder s)
    {
        slice_check();
        int len = ket - bra;
        //// s.Replace(s.ToString(0, s.Length - 0), current.ToString(bra, ket), 0, s.Length - 0);
        s.Remove(0, s.Length);
        s.Append(current.ToString(bra, len));
        return s;
    }

    protected internal virtual System.Text.StringBuilder assign_to(System.Text.StringBuilder s)
    {
        //// s.Replace(s.ToString(0, s.Length - 0), current.ToString(0, limit), 0, s.Length - 0);
        s.Remove(0, s.Length);
        s.Append(current.ToString(0, limit));
        return s;
    }

    /*
    extern void debug(struct SN_env * z, int number, int line_count)
    {   int i;
    int limit = SIZE(z->p);
    //if (number >= 0) printf("%3d (line %4d): '", number, line_count);
    if (number >= 0) printf("%3d (line %4d): [%d]'", number, line_count,limit);
    for (i = 0; i <= limit; i++)
    {   if (z->lb == i) printf("{");
    if (z->bra == i) printf("[");
    if (z->c == i) printf("|");
    if (z->ket == i) printf("]");
    if (z->l == i) printf("}");
    if (i < limit)
    {   int ch = z->p[i];
    if (ch == 0) ch = '#';
    printf("%c", ch);
    }
    }
    printf("'\n");
    }*/
}


/// <summary> Generated class implementing code defined by a snowball script.</summary>
public class EnglishStemmer : SnowballProgram
{

    public EnglishStemmer()
    {
        InitBlock();
    }
    private void InitBlock()
    {
        a_0 = new Among[] { new Among("gener", -1, -1, "", this) };
        a_1 = new Among[] { new Among("ied", -1, 2, "", this), new Among("s", -1, 3, "", this), new Among("ies", 1, 2, "", this), new Among("sses", 1, 1, "", this), new Among("ss", 1, -1, "", this), new Among("us", 1, -1, "", this) };
        a_2 = new Among[] { new Among("", -1, 3, "", this), new Among("bb", 0, 2, "", this), new Among("dd", 0, 2, "", this), new Among("ff", 0, 2, "", this), new Among("gg", 0, 2, "", this), new Among("bl", 0, 1, "", this), new Among("mm", 0, 2, "", this), new Among("nn", 0, 2, "", this), new Among("pp", 0, 2, "", this), new Among("rr", 0, 2, "", this), new Among("at", 0, 1, "", this), new Among("tt", 0, 2, "", this), new Among("iz", 0, 1, "", this) };
        a_3 = new Among[] { new Among("ed", -1, 2, "", this), new Among("eed", 0, 1, "", this), new Among("ing", -1, 2, "", this), new Among("edly", -1, 2, "", this), new Among("eedly", 3, 1, "", this), new Among("ingly", -1, 2, "", this) };
        a_4 = new Among[] { new Among("anci", -1, 3, "", this), new Among("enci", -1, 2, "", this), new Among("ogi", -1, 13, "", this), new Among("li", -1, 16, "", this), new Among("bli", 3, 12, "", this), new Among("abli", 4, 4, "", this), new Among("alli", 3, 8, "", this), new Among("fulli", 3, 14, "", this), new Among("lessli", 3, 15, "", this), new Among("ousli", 3, 10, "", this), new Among("entli", 3, 5, "", this), new Among("aliti", -1, 8, "", this), new Among("biliti", -1, 12, "", this), new Among("iviti", -1, 11, "", this), new Among("tional", -1, 1, "", this), new Among("ational", 14, 7, "", this), new Among("alism", -1, 8, "", this), new Among("ation", -1, 7, "", this), new Among("ization", 17, 6, "", this), new Among("izer", -1, 6, "", this), new Among("ator", -1, 7, "", this), new Among("iveness", -1, 11, "", this), new Among("fulness", -1, 9, "", this), new Among("ousness", -1, 10, "", this) };
        a_5 = new Among[] { new Among("icate", -1, 4, "", this), new Among("ative", -1, 6, "", this), new Among("alize", -1, 3, "", this), new Among("iciti", -1, 4, "", this), new Among("ical", -1, 4, "", this), new Among("tional", -1, 1, "", this), new Among("ational", 5, 2, "", this), new Among("ful", -1, 5, "", this), new Among("ness", -1, 5, "", this) };
        a_6 = new Among[] { new Among("ic", -1, 1, "", this), new Among("ance", -1, 1, "", this), new Among("ence", -1, 1, "", this), new Among("able", -1, 1, "", this), new Among("ible", -1, 1, "", this), new Among("ate", -1, 1, "", this), new Among("ive", -1, 1, "", this), new Among("ize", -1, 1, "", this), new Among("iti", -1, 1, "", this), new Among("al", -1, 1, "", this), new Among("ism", -1, 1, "", this), new Among("ion", -1, 2, "", this), new Among("er", -1, 1, "", this), new Among("ous", -1, 1, "", this), new Among("ant", -1, 1, "", this), new Among("ent", -1, 1, "", this), new Among("ment", 15, 1, "", this), new Among("ement", 16, 1, "", this) };
        a_7 = new Among[] { new Among("e", -1, 1, "", this), new Among("l", -1, 2, "", this) };
        a_8 = new Among[] { new Among("succeed", -1, -1, "", this), new Among("proceed", -1, -1, "", this), new Among("exceed", -1, -1, "", this), new Among("canning", -1, -1, "", this), new Among("inning", -1, -1, "", this), new Among("earring", -1, -1, "", this), new Among("herring", -1, -1, "", this), new Among("outing", -1, -1, "", this) };
        a_9 = new Among[] { new Among("andes", -1, -1, "", this), new Among("atlas", -1, -1, "", this), new Among("bias", -1, -1, "", this), new Among("cosmos", -1, -1, "", this), new Among("dying", -1, 3, "", this), new Among("early", -1, 9, "", this), new Among("gently", -1, 7, "", this), new Among("howe", -1, -1, "", this), new Among("idly", -1, 6, "", this), new Among("lying", -1, 4, "", this), new Among("news", -1, -1, "", this), new Among("only", -1, 10, "", this), new Among("singly", -1, 11, "", this), new Among("skies", -1, 2, "", this), new Among("skis", -1, 1, "", this), new Among("sky", -1, -1, "", this), new Among("tying", -1, 5, "", this), new Among("ugly", -1, 8, "", this) };
    }

    private Among[] a_0;
    private Among[] a_1;
    private Among[] a_2;
    private Among[] a_3;
    private Among[] a_4;
    private Among[] a_5;
    private Among[] a_6;
    private Among[] a_7;
    private Among[] a_8;
    private Among[] a_9;

    private static readonly char[] g_v = new char[] { (char)(17), (char)(65), (char)(16), (char)(1) };
    private static readonly char[] g_v_WXY = new char[] { (char)(1), (char)(17), (char)(65), (char)(208), (char)(1) };
    private static readonly char[] g_valid_LI = new char[] { (char)(55), (char)(141), (char)(2) };

    private bool B_Y_found;
    private int I_p2;
    private int I_p1;

    protected internal virtual void copy_from(EnglishStemmer other)
    {
        B_Y_found = other.B_Y_found;
        I_p2 = other.I_p2;
        I_p1 = other.I_p1;
        base.copy_from(other);
    }

    private bool r_prelude()
    {
        int v_1;
        int v_2;
        int v_3;
        int v_4;
        // (, line 23
        // unset Y_found, line 24
        B_Y_found = false;
        // do, line 25
        v_1 = cursor;
        do
        {
            // (, line 25
            // [, line 25
            bra = cursor;
            // literal, line 25
            if (!(eq_s(1, "y")))
            {
                goto lab0_brk;
            }
            // ], line 25
            ket = cursor;
            if (!(in_grouping(g_v, 97, 121)))
            {
                goto lab0_brk;
            }
            // <-, line 25
            slice_from("Y");
            // set Y_found, line 25
            B_Y_found = true;
        }
        while (false);

    lab0_brk:;

        cursor = v_1;
        // do, line 26
        v_2 = cursor;
        do
        {
            // repeat, line 26
            while (true)
            {
                v_3 = cursor;
                do
                {
                    // (, line 26
                    // goto, line 26
                    while (true)
                    {
                        v_4 = cursor;
                        do
                        {
                            // (, line 26
                            if (!(in_grouping(g_v, 97, 121)))
                            {
                                goto lab5_brk;
                            }
                            // [, line 26
                            bra = cursor;
                            // literal, line 26
                            if (!(eq_s(1, "y")))
                            {
                                goto lab5_brk;
                            }
                            // ], line 26
                            ket = cursor;
                            cursor = v_4;
                            goto golab4_brk;
                        }
                        while (false);

                    lab5_brk:;

                        cursor = v_4;
                        if (cursor >= limit)
                        {
                            goto lab3_brk;
                        }
                        cursor++;
                    }

                golab4_brk:;

                    // <-, line 26
                    slice_from("Y");
                    // set Y_found, line 26
                    B_Y_found = true;
                    goto replab2;
                }
                while (false);

            lab3_brk:;

                cursor = v_3;
                goto replab2_brk;

            replab2:;
            }

        replab2_brk:;

        }
        while (false);

    lab1_brk:;

        cursor = v_2;
        return true;
    }

    private bool r_mark_regions()
    {
        int v_1;
        int v_2;
        // (, line 29
        I_p1 = limit;
        I_p2 = limit;
        // do, line 32
        v_1 = cursor;
        do
        {
            // (, line 32
            // or, line 36
            do
            {
                v_2 = cursor;
                do
                {
                    // among, line 33
                    if (find_among(a_0, 1) == 0)
                    {
                        goto lab2_brk;
                    }
                    goto lab1_brk;
                }
                while (false);

            lab2_brk:;

                cursor = v_2;
                // (, line 36
                // gopast, line 36
                while (true)
                {
                    do
                    {
                        if (!(in_grouping(g_v, 97, 121)))
                        {
                            goto lab4_brk;
                        }
                        goto golab3_brk;
                    }
                    while (false);

                lab4_brk:;

                    if (cursor >= limit)
                    {
                        goto lab0_brk;
                    }
                    cursor++;
                }

            golab3_brk:;

                // gopast, line 36
                while (true)
                {
                    do
                    {
                        if (!(out_grouping(g_v, 97, 121)))
                        {
                            goto lab6_brk;
                        }
                        goto golab5_brk;
                    }
                    while (false);

                lab6_brk:;

                    if (cursor >= limit)
                    {
                        goto lab0_brk;
                    }
                    cursor++;
                }

            golab5_brk:;

            }
            while (false);

        lab1_brk:;

            // setmark p1, line 37
            I_p1 = cursor;
            // gopast, line 38
            while (true)
            {
                do
                {
                    if (!(in_grouping(g_v, 97, 121)))
                    {
                        goto lab8_brk;
                    }
                    goto golab7_brk;
                }
                while (false);

            lab8_brk:;

                if (cursor >= limit)
                {
                    goto lab0_brk;
                }
                cursor++;
            }

        golab7_brk:;

            // gopast, line 38
            while (true)
            {
                do
                {
                    if (!(out_grouping(g_v, 97, 121)))
                    {
                        goto lab10_brk;
                    }
                    goto golab9_brk;
                }
                while (false);

            lab10_brk:;

                if (cursor >= limit)
                {
                    goto lab0_brk;
                }
                cursor++;
            }

        golab9_brk:;

            // setmark p2, line 38
            I_p2 = cursor;
        }
        while (false);

    lab0_brk:;

        cursor = v_1;
        return true;
    }

    private bool r_shortv()
    {
        int v_1;
        // (, line 44
        // or, line 46
        do
        {
            v_1 = limit - cursor;
            do
            {
                // (, line 45
                if (!(out_grouping_b(g_v_WXY, 89, 121)))
                {
                    goto lab1_brk;
                }
                if (!(in_grouping_b(g_v, 97, 121)))
                {
                    goto lab1_brk;
                }
                if (!(out_grouping_b(g_v, 97, 121)))
                {
                    goto lab1_brk;
                }
                goto lab0_brk;
            }
            while (false);

        lab1_brk:;

            cursor = limit - v_1;
            // (, line 47
            if (!(out_grouping_b(g_v, 97, 121)))
            {
                return false;
            }
            if (!(in_grouping_b(g_v, 97, 121)))
            {
                return false;
            }
            // atlimit, line 47
            if (cursor > limit_backward)
            {
                return false;
            }
        }
        while (false);

    lab0_brk:;

        return true;
    }

    private bool r_R1()
    {
        if (!(I_p1 <= cursor))
        {
            return false;
        }
        return true;
    }

    private bool r_R2()
    {
        if (!(I_p2 <= cursor))
        {
            return false;
        }
        return true;
    }

    private bool r_Step_1a()
    {
        int among_var;
        int v_1;
        // (, line 53
        // [, line 54
        ket = cursor;
        // substring, line 54
        among_var = find_among_b(a_1, 6);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 54
        bra = cursor;
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 55
                // <-, line 55
                slice_from("ss");
                break;

            case 2:
                // (, line 57
                // or, line 57
                do
                {
                    v_1 = limit - cursor;
                    do
                    {
                        // (, line 57
                        // next, line 57
                        if (cursor <= limit_backward)
                        {
                            goto lab1_brk;
                        }
                        cursor--;
                        // atlimit, line 57
                        if (cursor > limit_backward)
                        {
                            goto lab1_brk;
                        }
                        // <-, line 57
                        slice_from("ie");
                        goto lab0_brk;
                    }
                    while (false);

                lab1_brk:;

                    cursor = limit - v_1;
                    // <-, line 57
                    slice_from("i");
                }
                while (false);

            lab0_brk:;

                break;

            case 3:
                // (, line 58
                // next, line 58
                if (cursor <= limit_backward)
                {
                    return false;
                }
                cursor--;
                // gopast, line 58
                while (true)
                {
                    do
                    {
                        if (!(in_grouping_b(g_v, 97, 121)))
                        {
                            goto lab3_brk;
                        }
                        goto golab2_brk;
                    }
                    while (false);

                lab3_brk:;

                    if (cursor <= limit_backward)
                    {
                        return false;
                    }
                    cursor--;
                }

            golab2_brk:;

                // delete, line 58
                slice_del();
                break;
        }
        return true;
    }

    private bool r_Step_1b()
    {
        int among_var;
        int v_1;
        int v_3;
        int v_4;
        // (, line 63
        // [, line 64
        ket = cursor;
        // substring, line 64
        among_var = find_among_b(a_3, 6);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 64
        bra = cursor;
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 66
                // call R1, line 66
                if (!r_R1())
                {
                    return false;
                }
                // <-, line 66
                slice_from("ee");
                break;

            case 2:
                // (, line 68
                // test, line 69
                v_1 = limit - cursor;
                // gopast, line 69
                while (true)
                {
                    do
                    {
                        if (!(in_grouping_b(g_v, 97, 121)))
                        {
                            goto lab1_brk;
                        }
                        goto golab0_brk;
                    }
                    while (false);

                lab1_brk:;

                    if (cursor <= limit_backward)
                    {
                        return false;
                    }
                    cursor--;
                }

            golab0_brk:;

                cursor = limit - v_1;
                // delete, line 69
                slice_del();
                // test, line 70
                v_3 = limit - cursor;
                // substring, line 70
                among_var = find_among_b(a_2, 13);
                if (among_var == 0)
                {
                    return false;
                }
                cursor = limit - v_3;
                switch (among_var)
                {

                    case 0:
                        return false;

                    case 1:
                        // (, line 72
                        // <+, line 72
                        {
                            int c = cursor;
                            insert(cursor, cursor, "e");
                            cursor = c;
                        }
                        break;

                    case 2:
                        // (, line 75
                        // [, line 75
                        ket = cursor;
                        // next, line 75
                        if (cursor <= limit_backward)
                        {
                            return false;
                        }
                        cursor--;
                        // ], line 75
                        bra = cursor;
                        // delete, line 75
                        slice_del();
                        break;

                    case 3:
                        // (, line 76
                        // atmark, line 76
                        if (cursor != I_p1)
                        {
                            return false;
                        }
                        // test, line 76
                        v_4 = limit - cursor;
                        // call shortv, line 76
                        if (!r_shortv())
                        {
                            return false;
                        }
                        cursor = limit - v_4;
                        // <+, line 76
                        {
                            int c = cursor;
                            insert(cursor, cursor, "e");
                            cursor = c;
                        }
                        break;
                }
                break;
        }
        return true;
    }

    private bool r_Step_1c()
    {
        int v_1;
        int v_2;
        // (, line 82
        // [, line 83
        ket = cursor;
        // or, line 83
        do
        {
            v_1 = limit - cursor;
            do
            {
                // literal, line 83
                if (!(eq_s_b(1, "y")))
                {
                    goto lab1_brk;
                }
                goto lab0_brk;
            }
            while (false);

        lab1_brk:;

            cursor = limit - v_1;
            // literal, line 83
            if (!(eq_s_b(1, "Y")))
            {
                return false;
            }
        }
        while (false);

    lab0_brk:;

        // ], line 83
        bra = cursor;
        if (!(out_grouping_b(g_v, 97, 121)))
        {
            return false;
        }
        // not, line 84
        {
            v_2 = limit - cursor;
            do
            {
                // atlimit, line 84
                if (cursor > limit_backward)
                {
                    goto lab2_brk;
                }
                return false;
            }
            while (false);

        lab2_brk:;

            cursor = limit - v_2;
        }
        // <-, line 85
        slice_from("i");
        return true;
    }

    private bool r_Step_2()
    {
        int among_var;
        // (, line 88
        // [, line 89
        ket = cursor;
        // substring, line 89
        among_var = find_among_b(a_4, 24);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 89
        bra = cursor;
        // call R1, line 89
        if (!r_R1())
        {
            return false;
        }
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 90
                // <-, line 90
                slice_from("tion");
                break;

            case 2:
                // (, line 91
                // <-, line 91
                slice_from("ence");
                break;

            case 3:
                // (, line 92
                // <-, line 92
                slice_from("ance");
                break;

            case 4:
                // (, line 93
                // <-, line 93
                slice_from("able");
                break;

            case 5:
                // (, line 94
                // <-, line 94
                slice_from("ent");
                break;

            case 6:
                // (, line 96
                // <-, line 96
                slice_from("ize");
                break;

            case 7:
                // (, line 98
                // <-, line 98
                slice_from("ate");
                break;

            case 8:
                // (, line 100
                // <-, line 100
                slice_from("al");
                break;

            case 9:
                // (, line 101
                // <-, line 101
                slice_from("ful");
                break;

            case 10:
                // (, line 103
                // <-, line 103
                slice_from("ous");
                break;

            case 11:
                // (, line 105
                // <-, line 105
                slice_from("ive");
                break;

            case 12:
                // (, line 107
                // <-, line 107
                slice_from("ble");
                break;

            case 13:
                // (, line 108
                // literal, line 108
                if (!(eq_s_b(1, "l")))
                {
                    return false;
                }
                // <-, line 108
                slice_from("og");
                break;

            case 14:
                // (, line 109
                // <-, line 109
                slice_from("ful");
                break;

            case 15:
                // (, line 110
                // <-, line 110
                slice_from("less");
                break;

            case 16:
                // (, line 111
                if (!(in_grouping_b(g_valid_LI, 99, 116)))
                {
                    return false;
                }
                // delete, line 111
                slice_del();
                break;
        }
        return true;
    }

    private bool r_Step_3()
    {
        int among_var;
        // (, line 115
        // [, line 116
        ket = cursor;
        // substring, line 116
        among_var = find_among_b(a_5, 9);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 116
        bra = cursor;
        // call R1, line 116
        if (!r_R1())
        {
            return false;
        }
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 117
                // <-, line 117
                slice_from("tion");
                break;

            case 2:
                // (, line 118
                // <-, line 118
                slice_from("ate");
                break;

            case 3:
                // (, line 119
                // <-, line 119
                slice_from("al");
                break;

            case 4:
                // (, line 121
                // <-, line 121
                slice_from("ic");
                break;

            case 5:
                // (, line 123
                // delete, line 123
                slice_del();
                break;

            case 6:
                // (, line 125
                // call R2, line 125
                if (!r_R2())
                {
                    return false;
                }
                // delete, line 125
                slice_del();
                break;
        }
        return true;
    }

    private bool r_Step_4()
    {
        int among_var;
        int v_1;
        // (, line 129
        // [, line 130
        ket = cursor;
        // substring, line 130
        among_var = find_among_b(a_6, 18);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 130
        bra = cursor;
        // call R2, line 130
        if (!r_R2())
        {
            return false;
        }
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 133
                // delete, line 133
                slice_del();
                break;

            case 2:
                // (, line 134
                // or, line 134
                do
                {
                    v_1 = limit - cursor;
                    do
                    {
                        // literal, line 134
                        if (!(eq_s_b(1, "s")))
                        {
                            goto lab1_brk;
                        }
                        goto lab0_brk;
                    }
                    while (false);

                lab1_brk:;

                    cursor = limit - v_1;
                    // literal, line 134
                    if (!(eq_s_b(1, "t")))
                    {
                        return false;
                    }
                }
                while (false);

            lab0_brk:;

                // delete, line 134
                slice_del();
                break;
        }
        return true;
    }

    private bool r_Step_5()
    {
        int among_var;
        int v_1;
        int v_2;
        // (, line 138
        // [, line 139
        ket = cursor;
        // substring, line 139
        among_var = find_among_b(a_7, 2);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 139
        bra = cursor;
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 140
                // or, line 140
                do
                {
                    v_1 = limit - cursor;
                    do
                    {
                        // call R2, line 140
                        if (!r_R2())
                        {
                            goto lab1_brk;
                        }
                        goto lab0_brk;
                    }
                    while (false);

                lab1_brk:;

                    cursor = limit - v_1;
                    // (, line 140
                    // call R1, line 140
                    if (!r_R1())
                    {
                        return false;
                    }
                    // not, line 140
                    {
                        v_2 = limit - cursor;
                        do
                        {
                            // call shortv, line 140
                            if (!r_shortv())
                            {
                                goto lab2_brk;
                            }
                            return false;
                        }
                        while (false);

                    lab2_brk:;

                        cursor = limit - v_2;
                    }
                }
                while (false);
            lab0_brk:;
                // delete, line 140
                slice_del();
                break;

            case 2:
                // (, line 141
                // call R2, line 141
                if (!r_R2())
                {
                    return false;
                }
                // literal, line 141
                if (!(eq_s_b(1, "l")))
                {
                    return false;
                }
                // delete, line 141
                slice_del();
                break;
        }
        return true;
    }

    private bool r_exception2()
    {
        // (, line 145
        // [, line 147
        ket = cursor;
        // substring, line 147
        if (find_among_b(a_8, 8) == 0)
        {
            return false;
        }
        // ], line 147
        bra = cursor;
        // atlimit, line 147
        if (cursor > limit_backward)
        {
            return false;
        }
        return true;
    }

    private bool r_exception1()
    {
        int among_var;
        // (, line 157
        // [, line 159
        bra = cursor;
        // substring, line 159
        among_var = find_among(a_9, 18);
        if (among_var == 0)
        {
            return false;
        }
        // ], line 159
        ket = cursor;
        // atlimit, line 159
        if (cursor < limit)
        {
            return false;
        }
        switch (among_var)
        {

            case 0:
                return false;

            case 1:
                // (, line 163
                // <-, line 163
                slice_from("ski");
                break;

            case 2:
                // (, line 164
                // <-, line 164
                slice_from("sky");
                break;

            case 3:
                // (, line 165
                // <-, line 165
                slice_from("die");
                break;

            case 4:
                // (, line 166
                // <-, line 166
                slice_from("lie");
                break;

            case 5:
                // (, line 167
                // <-, line 167
                slice_from("tie");
                break;

            case 6:
                // (, line 171
                // <-, line 171
                slice_from("idl");
                break;

            case 7:
                // (, line 172
                // <-, line 172
                slice_from("gentl");
                break;

            case 8:
                // (, line 173
                // <-, line 173
                slice_from("ugli");
                break;

            case 9:
                // (, line 174
                // <-, line 174
                slice_from("earli");
                break;

            case 10:
                // (, line 175
                // <-, line 175
                slice_from("onli");
                break;

            case 11:
                // (, line 176
                // <-, line 176
                slice_from("singl");
                break;
        }
        return true;
    }

    private bool r_postlude()
    {
        int v_1;
        int v_2;
        // (, line 192
        // Boolean test Y_found, line 192
        if (!(B_Y_found))
        {
            return false;
        }
        // repeat, line 192
        while (true)
        {
            v_1 = cursor;
            do
            {
                // (, line 192
                // goto, line 192
                while (true)
                {
                    v_2 = cursor;
                    do
                    {
                        // (, line 192
                        // [, line 192
                        bra = cursor;
                        // literal, line 192
                        if (!(eq_s(1, "Y")))
                        {
                            goto lab3_brk;
                        }
                        // ], line 192
                        ket = cursor;
                        cursor = v_2;
                        goto golab2_brk;
                    }
                    while (false);

                lab3_brk:;

                    cursor = v_2;
                    if (cursor >= limit)
                    {
                        goto lab1_brk;
                    }
                    cursor++;
                }
            golab2_brk:;

                // <-, line 192
                slice_from("y");
                goto replab0;
            }
            while (false);

        lab1_brk:;

            cursor = v_1;
            goto replab0_brk;

        replab0:;
        }

    replab0_brk:;

        return true;
    }

    public override bool Stem()
    {
        int v_1;
        int v_2;
        int v_3;
        int v_4;
        int v_5;
        int v_6;
        int v_7;
        int v_8;
        int v_9;
        int v_10;
        int v_11;
        int v_12;
        int v_13;
        // (, line 194
        // or, line 196
        do
        {
            v_1 = cursor;
            do
            {
                // call exception1, line 196
                if (!r_exception1())
                {
                    goto lab1_brk;
                }
                goto lab0_brk;
            }
            while (false);

        lab1_brk:;

            cursor = v_1;
            // (, line 196
            // test, line 198
            v_2 = cursor;
            // hop, line 198
            {
                int c = cursor + 3;
                if (0 > c || c > limit)
                {
                    return false;
                }
                cursor = c;
            }
            cursor = v_2;
            // do, line 199
            v_3 = cursor;
            do
            {
                // call prelude, line 199
                if (!r_prelude())
                {
                    goto lab2_brk;
                }
            }
            while (false);

        lab2_brk:;

            cursor = v_3;
            // do, line 200
            v_4 = cursor;
            do
            {
                // call mark_regions, line 200
                if (!r_mark_regions())
                {
                    goto lab3_brk;
                }
            }
            while (false);

        lab3_brk:;

            cursor = v_4;
            // backwards, line 201
            limit_backward = cursor; cursor = limit;
            // (, line 201
            // do, line 203
            v_5 = limit - cursor;
            do
            {
                // call Step_1a, line 203
                if (!r_Step_1a())
                {
                    goto lab4_brk;
                }
            }
            while (false);

        lab4_brk:;

            cursor = limit - v_5;
            // or, line 205
            do
            {
                v_6 = limit - cursor;
                do
                {
                    // call exception2, line 205
                    if (!r_exception2())
                    {
                        goto lab6_brk;
                    }
                    goto lab5_brk;
                }
                while (false);

            lab6_brk:;

                cursor = limit - v_6;
                // (, line 205
                // do, line 207
                v_7 = limit - cursor;
                do
                {
                    // call Step_1b, line 207
                    if (!r_Step_1b())
                    {
                        goto lab7_brk;
                    }
                }
                while (false);

            lab7_brk:;

                cursor = limit - v_7;
                // do, line 208
                v_8 = limit - cursor;
                do
                {
                    // call Step_1c, line 208
                    if (!r_Step_1c())
                    {
                        goto lab8_brk;
                    }
                }
                while (false);

            lab8_brk:;

                cursor = limit - v_8;
                // do, line 210
                v_9 = limit - cursor;
                do
                {
                    // call Step_2, line 210
                    if (!r_Step_2())
                    {
                        goto lab9_brk;
                    }
                }
                while (false);

            lab9_brk:;

                cursor = limit - v_9;
                // do, line 211
                v_10 = limit - cursor;
                do
                {
                    // call Step_3, line 211
                    if (!r_Step_3())
                    {
                        goto lab10_brk;
                    }
                }
                while (false);

            lab10_brk:;

                cursor = limit - v_10;
                // do, line 212
                v_11 = limit - cursor;
                do
                {
                    // call Step_4, line 212
                    if (!r_Step_4())
                    {
                        goto lab11_brk;
                    }
                }
                while (false);

            lab11_brk:;

                cursor = limit - v_11;
                // do, line 214
                v_12 = limit - cursor;
                do
                {
                    // call Step_5, line 214
                    if (!r_Step_5())
                    {
                        goto lab12_brk;
                    }
                }
                while (false);

            lab12_brk:;

                cursor = limit - v_12;
            }
            while (false);

        lab5_brk:;

            cursor = limit_backward; // do, line 217
            v_13 = cursor;
            do
            {
                // call postlude, line 217
                if (!r_postlude())
                {
                    goto lab13_brk;
                }
            }
            while (false);

        lab13_brk:;

            cursor = v_13;
        }
        while (false);

    lab0_brk:;

        return true;
    }
}

public class Among
{
    public Among(System.String s, int substring_i, int result, System.String methodname, SnowballProgram methodobject)
    {
        this.s_size = s.Length;
        this.s = s;
        this.substring_i = substring_i;
        this.result = result;
        this.methodobject = methodobject;
        if (methodname.Length == 0)
        {
            this.method = null;
        }
        else
        {
            try
            {
                this.method = methodobject.GetType().GetMethod(methodname, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly, null, new System.Type[0], null);
            }
            catch (System.MethodAccessException)
            {
                // FIXME - debug message
                this.method = null;
            }
        }
    }

    public int s_size; /* search string */
    public System.String s; /* search string */
    public int substring_i; /* index to longest matching substring */
    public int result; /* result of the lookup */
    public System.Reflection.MethodInfo method; /* method to use if substring matches */
    public SnowballProgram methodobject; /* object to invoke method on */
}
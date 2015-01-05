#region license

// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moroco;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Tests.Mocks;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryExecuterTests {
        public class TestDocument {
            [SolrUniqueKey]
            public int Id { get; set; }

            public string OtherField { get; set; }
        }

        [Test]
        public void Execute() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            var conn = new MockConnection(q);
            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();

            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), null);
            Assert.AreEqual(1, serializer.serialize.Calls);
        }

        [Test]
        public void Sort() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["sort"] = "id asc";
            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub(queryString);
            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Expect(1);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                OrderBy = new[] { new SortOrder("id") }
            });
            parser.parse.Verify();
        }

        [Test]
        public void SortMultipleWithOrders() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["sort"] = "id asc,name desc";
            var conn = new MockConnection(q);

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var querySerializer = new SolrQuerySerializerStub(queryString);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                OrderBy = new[] {
                        new SortOrder("id", Order.ASC),
                        new SortOrder("name", Order.DESC)
                    }
            });
        }

        [Test]
        public void ResultFields() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["fl"] = "id,name";
            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub(queryString);

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                Fields = new[] { "id", "name" },
            });
        }

        [Test]
        public void Facets() {
            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["facet"] = "true";
            q["facet.field"] = "Id";
            q["facet.query"] = "id:[1 TO 5]";
            var conn = new MockConnection(q);
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, facetQuerySerializer, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetQuery(new SolrQuery("id:[1 TO 5]")),
                    }
                }
            });
        }

        [Test]
        public void MultipleFacetFields() {
            var conn = new MockConnection(new[] {
                KV.Create("q", ""),
                KV.Create("rows", SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString()),
                KV.Create("facet", "true"),
                KV.Create("facet.field", "Id"),
                KV.Create("facet.field", "OtherField"),
            });
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(serializer, new DefaultFieldSerializer());

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, facetQuerySerializer, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetFieldQuery("OtherField"),
                    }
                }
            });
        }

        [Test]
        public void Highlighting() {
            const string highlightedField = "field1";
            const string afterTerm = "after";
            const string beforeTerm = "before";
            const int snippets = 3;
            const string alt = "alt";
            const int fragsize = 7;
            const string query = "mausch";
            var highlightQuery = new SolrQuery(query);
            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["hl"] = "true";
            q["hl.q"] = query;
            q["hl.fl"] = highlightedField;
            q["hl.snippets"] = snippets.ToString();
            q["hl.fragsize"] = fragsize.ToString();
            q["hl.requireFieldMatch"] = "true";
            q["hl.alternateField"] = alt;
            q["hl.tag.pre"] = beforeTerm;
            q["hl.tag.post"] = afterTerm;
            q["hl.regex.slop"] = "4.12";
            q["hl.regex.pattern"] = "\\.";
            q["hl.regex.maxAnalyzedChars"] = "8000";
            q["hl.usePhraseHighlighter"] = "true";
            q["hl.useFastVectorHighlighter"] = "true";
            q["hl.highlightMultiTerm"] = "true";
            q["hl.mergeContiguous"] = "true";
            q["hl.maxAnalyzedChars"] = "12";
            q["hl.maxAlternateFieldLength"] = "22";
            q["hl.fragmenter"] = "regex";

            var conn = new MockConnection(q);
            var querySerializer = new DefaultQuerySerializer(new MSolrFieldSerializer());

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] { highlightedField },
                    AfterTerm = afterTerm,
                    BeforeTerm = beforeTerm,
                    Query = highlightQuery,
                    Snippets = snippets,
                    AlternateField = alt,
                    Fragsize = fragsize,
                    RequireFieldMatch = true,
                    RegexSlop = 4.12,
                    RegexPattern = "\\.",
                    RegexMaxAnalyzedChars = 8000,
                    UsePhraseHighlighter = true,
                    UseFastVectorHighlighter = true,
                    MergeContiguous = true,
                    MaxAnalyzedChars = 12,
                    HighlightMultiTerm = true,
                    MaxAlternateFieldLength = 22,
                    Fragmenter = SolrHighlightFragmenter.Regex
                }
            });
        }

        [Test]
        public void HighlightingWithoutFieldsOutputsPrePost() {
            const string afterTerm = "after";
            const string beforeTerm = "before";

            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["hl"] = "true";
            q["hl.tag.pre"] = beforeTerm;
            q["hl.tag.post"] = afterTerm;
            q["hl.useFastVectorHighlighter"] = "true";

            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub("");

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Highlight = new HighlightingParameters {
                    AfterTerm = afterTerm,
                    BeforeTerm = beforeTerm,
                    UseFastVectorHighlighter = true,
                }
            });
        }


        [Test]
        public void HighlightingWithFastVectorHighlighter() {
            var e = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = e.GetHighlightingParameters(new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] {"a"},
                    AfterTerm = "after",
                    BeforeTerm = "before",
                    UseFastVectorHighlighter = true,
                }
            });
            Assert.AreEqual("true", p["hl.useFastVectorHighlighter"]);
            Assert.AreEqual("before", p["hl.tag.pre"]);
            Assert.AreEqual("after", p["hl.tag.post"]);
            Assert.IsFalse(p.ContainsKey("hl.simple.pre"));
            Assert.IsFalse(p.ContainsKey("hl.simple.post"));
        }

        [Test]
        public void FilterQuery() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var conn = new MockConnection(new[] {
                KV.Create("q", "*:*"),
                KV.Create("rows", "10"),
                KV.Create("fq", "id:0"),
                KV.Create("fq", "id:2"),
            });

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null) {
                DefaultRows = 10,
            };
            queryExecuter.Execute(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] {
                    new SolrQuery("id:0"),
                    new SolrQuery("id:2"),
                },
            });
        }

        [Test]
        public void SpellChecking() {
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = queryExecuter.GetSpellCheckingParameters(new QueryOptions {
                SpellCheck = new SpellCheckingParameters {
                    Query = "hell",
                    Build = true,
                    Collate = true,
                    Count = 4,
                    Dictionary = "spanish",
                    OnlyMorePopular = true,
                    Reload = true,
                },
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("spellcheck", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.q", "hell"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.build", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.collate", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.count", "4"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.dictionary", "spanish"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.onlyMorePopular", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.reload", "true"));
        }

        [Test]
        public void TermsSingleField() {
            var p = SolrQueryExecuter<TestDocument>.GetTermsParameters(new QueryOptions {
                Terms = new TermsParameters("text") {
                    Limit = 10,
                    Lower = "lower",
                    LowerInclude = true,
                    MaxCount = 10,
                    MinCount = 0,
                    Prefix = "pre",
                    Raw = true,
                    Regex = "regex",
                    RegexFlag = new[] { RegexFlag.CanonEq, RegexFlag.CaseInsensitive },
                    Sort = TermsSort.Count,
                    Upper = "upper",
                    UpperInclude = true
                },
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("terms", "true"));
            CollectionAssert.Contains(p, KV.Create("terms.fl", "text"));
            CollectionAssert.Contains(p, KV.Create("terms.lower", "lower"));
            CollectionAssert.Contains(p, KV.Create("terms.lower.incl", "true"));
            CollectionAssert.Contains(p, KV.Create("terms.maxcount", "10"));
            CollectionAssert.Contains(p, KV.Create("terms.mincount", "0"));
            CollectionAssert.Contains(p, KV.Create("terms.prefix", "pre"));
            CollectionAssert.Contains(p, KV.Create("terms.raw", "true"));
            CollectionAssert.Contains(p, KV.Create("terms.regex", "regex"));
            CollectionAssert.Contains(p, KV.Create("terms.regex.flag", RegexFlag.CanonEq.ToString()));
            CollectionAssert.Contains(p, KV.Create("terms.regex.flag", RegexFlag.CaseInsensitive.ToString()));
            CollectionAssert.Contains(p, KV.Create("terms.sort", "count"));
            CollectionAssert.Contains(p, KV.Create("terms.upper", "upper"));
            CollectionAssert.Contains(p, KV.Create("terms.upper.incl", "true"));
        }

        [Test]
        public void TermsMultipleFields() {
            var p = SolrQueryExecuter<TestDocument>.GetTermsParameters(new QueryOptions {
                Terms = new TermsParameters(new List<string> { "text", "text2", "text3" }) {
                    Limit = 10,
                    Lower = "lower",
                    LowerInclude = true,
                    MaxCount = 10,
                    MinCount = 0,
                    Prefix = "pre",
                    Raw = true,
                    Regex = "regex",
                    RegexFlag = new[] { RegexFlag.CanonEq, RegexFlag.CaseInsensitive },
                    Sort = TermsSort.Count,
                    Upper = "upper",
                    UpperInclude = true
                },
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("terms", "true"));
            CollectionAssert.Contains(p, KV.Create("terms.fl", "text"));
            CollectionAssert.Contains(p, KV.Create("terms.fl", "text2"));
            CollectionAssert.Contains(p, KV.Create("terms.fl", "text3"));
            CollectionAssert.Contains(p, KV.Create("terms.lower", "lower"));
            CollectionAssert.Contains(p, KV.Create("terms.lower.incl", "true"));
            CollectionAssert.Contains(p, KV.Create("terms.maxcount", "10"));
            CollectionAssert.Contains(p, KV.Create("terms.mincount", "0"));
            CollectionAssert.Contains(p, KV.Create("terms.prefix", "pre"));
            CollectionAssert.Contains(p, KV.Create("terms.raw", "true"));
            CollectionAssert.Contains(p, KV.Create("terms.regex", "regex"));
            CollectionAssert.Contains(p, KV.Create("terms.regex.flag", RegexFlag.CanonEq.ToString()));
            CollectionAssert.Contains(p, KV.Create("terms.regex.flag", RegexFlag.CaseInsensitive.ToString()));
            CollectionAssert.Contains(p, KV.Create("terms.sort", "count"));
            CollectionAssert.Contains(p, KV.Create("terms.upper", "upper"));
            CollectionAssert.Contains(p, KV.Create("terms.upper.incl", "true"));
        }

        [Test]
        public void GetTermVectorParameterOptions_All() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.All).ToList();
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("tv.all", r[0]);
        }

        [Test]
        public void GetTermVectorParameterOptions_All_indirect() {
            const TermVectorParameterOptions o = 
                TermVectorParameterOptions.DocumentFrequency 
                | TermVectorParameterOptions.TermFrequency 
                | TermVectorParameterOptions.Positions 
                | TermVectorParameterOptions.Offsets 
                | TermVectorParameterOptions.TermFrequency_InverseDocumentFrequency;
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(o).ToList();
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("tv.all", r[0]);
        }

        [Test]
        public void GetTermVectorParameterOptions_Tf() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.TermFrequency).ToList();
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("tv.tf", r[0]);
        }

        [Test]
        public void GetTermVectorParameterOptions_Df() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.DocumentFrequency).ToList();
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("tv.df", r[0]);
        }

        [Test]
        public void GetTermVectorParameterOptions_default() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.Default).ToList();
            Assert.AreEqual(0, r.Count);
        }

        [Test]
        public void GetTermVectorParameterOptions_TfDf() {
            const TermVectorParameterOptions o =
                TermVectorParameterOptions.DocumentFrequency
                | TermVectorParameterOptions.TermFrequency;
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(o).ToList();
            Assert.AreEqual(2, r.Count);
            CollectionAssert.Contains(r, "tv.df");
            CollectionAssert.Contains(r, "tv.tf");
        }

        [Test]
        public void GetTermVectorParameterOptions_offsets() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.Offsets).ToList();
            Assert.AreEqual(1, r.Count);
            Assert.AreEqual("tv.offsets", r[0]);
        }

        [Test]
        public void GetTermVectorParameterOptions_tfidf() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.TermFrequency_InverseDocumentFrequency).ToList();
            Assert.AreEqual(3, r.Count);
            Assert.Contains("tv.df", r);
            CollectionAssert.Contains(r, "tv.tf");
            CollectionAssert.Contains(r, "tv.tf_idf");
        }

		[Test]
		public void TermVector() {
            var p = SolrQueryExecuter<TestDocument>.GetTermVectorQueryOptions(new QueryOptions {
				TermVector = new TermVectorParameters {
                    Fields = new[] {"text"},
                    Options = TermVectorParameterOptions.All,
				},
			}).ToList();
			CollectionAssert.Contains(p, KV.Create("tv", "true"));
			CollectionAssert.Contains(p, KV.Create("tv.all", "true"));
			CollectionAssert.Contains(p, KV.Create("tv.fl", "text"));
		}

        [Test]
        public void GetAllParameters_with_spelling() {
            var querySerializer = new SolrQuerySerializerStub("*:*");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(SolrQuery.All, new QueryOptions {
                SpellCheck = new SpellCheckingParameters {
                    Query = "hell",
                    Build = true,
                    Collate = true,
                    Count = 4,
                    Dictionary = "spanish",
                    OnlyMorePopular = true,
                    Reload = true,
                },
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("spellcheck", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.q", "hell"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.build", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.collate", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.count", "4"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.dictionary", "spanish"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.onlyMorePopular", "true"));
            CollectionAssert.Contains(p, KV.Create("spellcheck.reload", "true"));
        }

        [Test]
        public void MoreLikeThis() {
            var querySerializer = new SolrQuerySerializerStub("apache");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("apache"), new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] { "manu", "cat" }) {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                },
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("mlt", "true"));
            CollectionAssert.Contains(p, KV.Create("mlt.mindf", "1"));
            CollectionAssert.Contains(p, KV.Create("mlt.fl", "manu,cat"));
            CollectionAssert.Contains(p, KV.Create("mlt.mintf", "1"));
            CollectionAssert.Contains(p, KV.Create("q", "apache"));
        }

        [Test]
        public void GetMoreLikeThisParameters() {
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = queryExecuter.GetMoreLikeThisParameters(
                new MoreLikeThisParameters(new[] { "field1", "field2" }) {
                    Boost = true,
                    Count = 10,
                    QueryFields = new[] { "qf1", "qf2" },
                    MaxQueryTerms = 2,
                    MaxTokens = 3,
                    MaxWordLength = 4,
                    MinDocFreq = 5,
                    MinTermFreq = 6,
                    MinWordLength = 7,
                }).ToList();
            CollectionAssert.Contains(p, KV.Create("mlt", "true"));
            CollectionAssert.Contains(p, KV.Create("mlt.boost", "true"));
            CollectionAssert.Contains(p, KV.Create("mlt.count", "10"));
            CollectionAssert.Contains(p, KV.Create("mlt.maxqt", "2"));
            CollectionAssert.Contains(p, KV.Create("mlt.maxntp", "3"));
            CollectionAssert.Contains(p, KV.Create("mlt.maxwl", "4"));
            CollectionAssert.Contains(p, KV.Create("mlt.mindf", "5"));
            CollectionAssert.Contains(p, KV.Create("mlt.mintf", "6"));
            CollectionAssert.Contains(p, KV.Create("mlt.minwl", "7"));
            CollectionAssert.Contains(p, KV.Create("mlt.fl", "field1,field2"));
            CollectionAssert.Contains(p, KV.Create("mlt.qf", "qf1,qf2"));
        }

        [Test]
        public void GetAllParameters_mlt_with_field_query() {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var qe = new SolrQueryExecuter<TestDocument>(null, null, serializer, null, null);
            var p = qe.GetAllMoreLikeThisHandlerParameters(
                new SolrMoreLikeThisHandlerQuery(new SolrQueryByField("id", "1234")),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] { "one", "three" }) {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    }) {
                        Start = 0,
                        Rows = 5,
                        Fields = new[] { "one", "two", "three" },
                    }).ToList();
            CollectionAssert.Contains(p, KV.Create("q", "id:(1234)"));
            CollectionAssert.Contains(p, KV.Create("start", "0"));
            CollectionAssert.Contains(p, KV.Create("rows", "5"));
            CollectionAssert.Contains(p, KV.Create("fl", "one,two,three"));
            CollectionAssert.Contains(p, KV.Create("mlt.fl", "one,three"));
            CollectionAssert.Contains(p, KV.Create("mlt.match.include", "false"));
            CollectionAssert.Contains(p, KV.Create("mlt.match.offset", "5"));
            CollectionAssert.Contains(p, KV.Create("mlt.interestingTerms", "none"));
        }

        [Test]
        public void GetAllParameters_mlt_with_stream_body_query() {
            var qe = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = qe.GetAllMoreLikeThisHandlerParameters(
                new SolrMoreLikeThisHandlerStreamBodyQuery("one two three"),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] { "one", "three" })
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                    {
                        Start = 0,
                        Rows = 5,
                        Fields = new[] { "one", "two", "three" },
                    }).ToList();
            CollectionAssert.Contains(p, KV.Create("stream.body", "one two three"));
        }

        [Test]
        public void GetAllParameters_mlt_with_stream_url_query() {
            var qe = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = qe.GetAllMoreLikeThisHandlerParameters(
                new SolrMoreLikeThisHandlerStreamUrlQuery("http://wiki.apache.org/solr/MoreLikeThisHandler"),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] { "one", "three" })
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                    {
                        Start = 0,
                        Rows = 5,
                        Fields = new[] { "one", "two", "three" },
                    }).ToList();
            CollectionAssert.Contains(p, KV.Create("stream.url", "http://wiki.apache.org/solr/MoreLikeThisHandler"));
        }

        [Test]
        public void FacetFieldOptions() {
            var querySerializer = new SolrQuerySerializerStub("q");
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, null);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, facetQuerySerializer, null);
            var facetOptions = queryExecuter.GetFacetFieldOptions(
                new FacetParameters {
                    Queries = new List<ISolrFacetQuery> {
                        new SolrFacetQuery(new SolrQuery("q")),
                    },
                    Prefix = "pref",
                    EnumCacheMinDf = 123,
                    Limit = 100,
                    MinCount = 5,
                    Missing = true,
                    Offset = 55,
                    Sort = true,
                    Threads = 5
                }).ToDictionary(x => x.Key, x => x.Value);
            Assert.AreEqual("pref", facetOptions["facet.prefix"]);
            Assert.AreEqual("123", facetOptions["facet.enum.cache.minDf"]);
            Assert.AreEqual("100", facetOptions["facet.limit"]);
            Assert.AreEqual("5", facetOptions["facet.mincount"]);
            Assert.AreEqual("true", facetOptions["facet.missing"]);
            Assert.AreEqual("55", facetOptions["facet.offset"]);
            Assert.AreEqual("true", facetOptions["facet.sort"]);
            Assert.AreEqual("5", facetOptions["facet.threads"]);
        }

        [Test]
        public void StatsOptions() {
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var statsOptions = queryExecuter.GetStatsQueryOptions(new QueryOptions {
                Stats = new StatsParameters()
                    .AddField("popularity")
                    .AddFieldWithFacet("price", "inStock")
                    .AddFieldWithFacets("afield", "facet1", "facet2")
                    .AddFacet("globalfacet")
            }).ToList();
            Assert.AreEqual(8, statsOptions.Count);
            CollectionAssert.Contains(statsOptions, KV.Create("stats", "true"));
            CollectionAssert.Contains(statsOptions, KV.Create("stats.field", "popularity"));
            CollectionAssert.Contains(statsOptions, KV.Create("stats.field", "price"));
            CollectionAssert.Contains(statsOptions, KV.Create("f.price.stats.facet", "inStock"));
            CollectionAssert.Contains(statsOptions, KV.Create("stats.field", "afield"));
            CollectionAssert.Contains(statsOptions, KV.Create("f.afield.stats.facet", "facet1"));
            CollectionAssert.Contains(statsOptions, KV.Create("f.afield.stats.facet", "facet2"));
            CollectionAssert.Contains(statsOptions, KV.Create("stats.facet", "globalfacet"));
        }

        [Test]
        public void ExtraParams() {
            var querySerializer = new SolrQuerySerializerStub("123123");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("123123"), new QueryOptions {
                ExtraParams = new Dictionary<string, string> {
                            {"qt", "geo"},
                            {"lat", "40.75141843299745"},
                            {"long", "-74.0093994140625"},
                            {"radius", "1"},
                        }
            }).ToDictionary(x => x.Key, x => x.Value);
            Assert.AreEqual("123123", p["q"]);
            Assert.AreEqual("geo", p["qt"]);
            Assert.AreEqual("1", p["radius"]);
        }

        [Test]
        public void GetClusteringParameters() {
            var querySerializer = new SolrQuerySerializerStub("apache");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("apache"), new QueryOptions {
                Clustering = new ClusteringParameters {
                    Title = "headline",
                    FragSize = 10,
                    LexicalResources = "fakedir",
                    ProduceSummary = true,
                    Algorithm = "org.carrot2.clustering.lingo.LingoClusteringAlgorithm",
                    Url = "none",
                    Collection = false,
                    Engine = "default",
                    SubClusters = false,
                    Snippet = "synopsis",
                    NumDescriptions = 20
                },
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("carrot.title", "headline"));
            CollectionAssert.Contains(p, KV.Create("clustering.engine", "default"));
            CollectionAssert.Contains(p, KV.Create("clustering.collection", "false"));
            CollectionAssert.Contains(p, KV.Create("carrot.algorithm", "org.carrot2.clustering.lingo.LingoClusteringAlgorithm"));
            CollectionAssert.Contains(p, KV.Create("carrot.url", "none"));
            CollectionAssert.Contains(p, KV.Create("carrot.snippet", "synopsis"));
            CollectionAssert.Contains(p, KV.Create("carrot.produceSummary", "true"));
            CollectionAssert.Contains(p, KV.Create("carrot.fragSize", "10"));
            CollectionAssert.Contains(p, KV.Create("carrot.numDescriptions", "20"));
            CollectionAssert.Contains(p, KV.Create("carrot.outputSubClusters", "false"));
            CollectionAssert.Contains(p, KV.Create("carrot.lexicalResourcesDir", "fakedir"));
        }

        [Test]
        public void GetCursormarkWithDefaultSetup()
        {
            var e = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = e.GetCommonParameters(new CommonQueryOptions {
                StartOrCursor = StartOrCursor.Cursor.Start
            });

            Assert.AreEqual("cursorMark", p.First().Key);
            Assert.AreEqual("*", p.First().Value);
        }

        [Test]
        public void GetCursormarkWithMarkSet()
        {
            var e = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = e.GetCommonParameters(new CommonQueryOptions {
                StartOrCursor = new StartOrCursor.Cursor("AoEoZTQ3YmY0NDM=")
            });

            Assert.AreEqual("cursorMark", p.First().Key);
            Assert.AreEqual("AoEoZTQ3YmY0NDM=", p.First().Value);
        }

        [Test]
        public void GetCollapseExpandParameters() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var e = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = e.GetAllParameters(SolrQuery.All, new QueryOptions {
                Rows = 1,
                CollapseExpand = new CollapseExpandParameters("somefield", null, null, null),
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("fq", "{!collapse field=somefield}"));
        }

        [Test]
        public void GetCollapseExpandParameters_min_policy() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var e = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var max = new CollapseExpandParameters.MinOrMax.Max("maxfield");
            var policy = CollapseExpandParameters.NullPolicyType.Collapse;
            var p = e.GetAllParameters(SolrQuery.All, new QueryOptions {
                Rows = 1,
                CollapseExpand = new CollapseExpandParameters("somefield", null, max, policy),
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("fq", "{!collapse field=somefield nullPolicy=collapse max=maxfield}"));
        }

        [Test]
        public void GetCollapseExpandParameters_Expand() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var e = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var expand = new ExpandParameters(
                sort: new SortOrder("sortField", Order.ASC),
                rows: 100,
                query: new SolrQuery("aquery"),
                filterQuery: null);

            var p = e.GetAllParameters(SolrQuery.All, new QueryOptions {
                Rows = 1,
                CollapseExpand = new CollapseExpandParameters("somefield", expand, null, null),
            }).ToList();
            CollectionAssert.Contains(p, KV.Create("fq", "{!collapse field=somefield}"));
            CollectionAssert.Contains(p, KV.Create("expand.sort", "sortField asc"));
            CollectionAssert.Contains(p, KV.Create("expand.rows", "100"));
            CollectionAssert.Contains(p, KV.Create("expand.q", "aquery"));
        }
    }
}
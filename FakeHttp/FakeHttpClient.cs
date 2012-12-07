using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using FakeHttp.Exceptions;
using FakeHttp.Rules;

namespace FakeHttp
{
    public class FakeHttpClient : HttpClient, IRuleSet<HttpRequestMessage, HttpResponseMessage>, IRuleSet<HttpRequestMessage, CancellationToken, HttpResponseMessage>
    {
        private readonly List<IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>> _rules;
        private readonly Queue<Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>> _queue;

        #region Constructors
        public FakeHttpClient() : this(new List<IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>>(), new Queue<Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>>()) { }

        private FakeHttpClient(List<IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>> rules, Queue<Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>> queue)
            : this((request, cancellationToken) =>
            {
                var response = queue.Count > 0 ? queue.Dequeue() : null;
                if (response == null)
                {
                    var rule = rules.FirstOrDefault(r => r.Predicate(request, cancellationToken));
                    if (rule != null)
                    {
                        response = rule.Selector;
                    }
                }
                if (response == null)
                {
                    throw new FakeNotSetupException("The fake was not setup to provide a response for this request.");
                }
                return response(request, cancellationToken);
            })
        {
            _rules = rules;
            _queue = queue;
        }

        private FakeHttpClient(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler) : base(new InterceptingMessageHandler(handler)) { }

        #endregion

        #region RuleSet Implementations
        public void AddRule(Func<HttpRequestMessage, bool> predicate, Func<HttpRequestMessage, HttpResponseMessage> selector)
        {
            _rules.Add(new IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>((req, tok) => predicate(req), (req, tok) => selector(req)));
        }

        public void AddRule(Func<HttpRequestMessage, HttpResponseMessage> selector)
        {
            AddRule(req => true, selector);
        }

        public void AddRule(Func<HttpRequestMessage, bool> predicate, HttpResponseMessage output)
        {
            AddRule(predicate, req => output);
        }

        public void AddRule(Func<HttpRequestMessage, CancellationToken, bool> predicate, Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> selector)
        {
            _rules.Add(new IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>(predicate, selector));
        }

        public void AddRule(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> selector)
        {
            AddRule((req, tok) => true, selector);
        }

        public void AddRule(Func<HttpRequestMessage, CancellationToken, bool> predicate, HttpResponseMessage output)
        {
            AddRule(predicate, (req, tok) => output);
        }
        #endregion

        #region Queue Methods

        public void Enqueue(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
        {
            _queue.Enqueue(handler);
        }

        public void Enqueue(HttpResponseMessage response)
        {
            Enqueue((req, tok) => response);
        }

        #endregion

    }
}

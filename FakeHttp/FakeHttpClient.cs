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
        private readonly IList<HttpRequestMessage> _history;

        #region Constructors
        // default constructor passes blank rules and queue to the 2nd constructor
        public FakeHttpClient() : this(
            new List<IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>>(),
            new Queue<Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>>(),
            new List<HttpRequestMessage>()) { }

        // this one takes the rules and queue, and creates a handler for them, and invokes that constructor with the handler
        private FakeHttpClient(
            List<IfThen<HttpRequestMessage, CancellationToken, HttpResponseMessage>> rules,
            Queue<Func<HttpRequestMessage, CancellationToken, HttpResponseMessage>> queue,
            IList<HttpRequestMessage> history)
            : this((request, cancellationToken) =>
            {
                history.Add(request);
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
            _history = history;
        }

        // this one takes the handler and sets up the base with it.
        private FakeHttpClient(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler)
            : base(new InterceptingMessageHandler(handler))
        {

        }

        public IList<HttpRequestMessage> History { get { return _history; } }

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

        public void AddRule(HttpResponseMessage output)
        {
            AddRule(req => output);
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

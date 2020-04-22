// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MultipleHostedService;

namespace Test.TestHelpers
{
    public class StringProvider
    {
        public StringProvider()
        {
            MyString = Guid.NewGuid().ToString();
        }

        public string MyString { get; }
    }

    public class ListenToStrings
    {
        private readonly ConcurrentStack<string> _strings;

        public ListenToStrings(ConcurrentStack<string> strings)
        {
            _strings = strings;
        }

        public void AddGuid(string myString)
        {
            _strings.Push(myString);
        }
    }

    public class TestScopedTask : IBackgroundTaskToCall
    {
        private readonly StringProvider _stringProvider;
        private readonly ListenToStrings _listener;

        public TestScopedTask(StringProvider stringProvider, ListenToStrings listener)
        {
            _stringProvider = stringProvider;
            _listener = listener;
        }

        public Task MethodToRunAsync(CancellationToken cancellationToken)
        {
            _listener.AddGuid(_stringProvider.MyString);
            return Task.CompletedTask;
        }
    }
}
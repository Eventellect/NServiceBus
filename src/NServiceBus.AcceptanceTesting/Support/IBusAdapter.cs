﻿namespace NServiceBus.AcceptanceTesting.Support
{
    using System;
    using System.Threading.Tasks;

    public class IBusAdapter : IBus
    {
        ISendOnlyBus sendOnlyBus;

        public IBusAdapter(ISendOnlyBus sendOnlyBus)
        {
            this.sendOnlyBus = sendOnlyBus;
        }

        public void Dispose()
        {
            sendOnlyBus.Dispose();
        }

        public Task PublishAsync(object message, PublishOptions options)
        {
            return sendOnlyBus.PublishAsync(message, options);
        }

        public Task PublishAsync<T>(Action<T> messageConstructor, PublishOptions options)
        {
            return sendOnlyBus.PublishAsync(messageConstructor, options);
        }

        public void Send(object message, SendOptions options)
        {
            sendOnlyBus.Send(message, options);
        }

        public void Send<T>(Action<T> messageConstructor, SendOptions options)
        {
            sendOnlyBus.Send(messageConstructor, options);
        }

        [Obsolete("", true)]
        ICallback ISendOnlyBus.Send(Address address, object message)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        ICallback ISendOnlyBus.Send<T>(Address address, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        public ICallback Send(string destination, string correlationId, object message)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        ICallback ISendOnlyBus.Send(Address address, string correlationId, object message)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        public ICallback Send<T>(string destination, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        ICallback ISendOnlyBus.Send<T>(Address address, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public Task ReplyAsync(object message, ReplyOptions options)
        {
            throw new NotImplementedException();
        }

        public Task ReplyAsync<T>(Action<T> messageConstructor, ReplyOptions options)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type eventType, SubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type eventType, UnsubscribeOptions options)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        public void Return<T>(T errorEnum)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        public ICallback Defer(TimeSpan delay, object message)
        {
            throw new NotImplementedException();
        }

        [Obsolete("", true)]
        public ICallback Defer(DateTime processAt, object message)
        {
            throw new NotImplementedException();
        }

        public void HandleCurrentMessageLater()
        {
            throw new NotImplementedException();
        }

        public void ForwardCurrentMessageTo(string destination)
        {
            throw new NotImplementedException();
        }

        public void DoNotContinueDispatchingCurrentMessageToHandlers()
        {
            throw new NotImplementedException();
        }

        public IMessageContext CurrentMessageContext { get; private set; }
    }
}

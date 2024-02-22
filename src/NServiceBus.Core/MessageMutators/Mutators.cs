namespace NServiceBus.Features;

using System.Collections.Generic;
using MessageMutator;

class Mutators : Feature
{
    public Mutators()
    {
        EnableByDefault();
    }

    protected internal override void Setup(FeatureConfigurationContext context)
    {
        var registry = context.Settings.GetOrDefault<RegisteredMutators>() ?? new RegisteredMutators();

        context.Pipeline.Register("MutateIncomingTransportMessage", new MutateIncomingTransportMessageBehavior(registry.IncomingTransportMessage), "Executes IMutateIncomingTransportMessages");
        context.Pipeline.Register("MutateIncomingMessages", new MutateIncomingMessageBehavior(registry.IncomingMessage), "Executes IMutateIncomingMessages");
        context.Pipeline.Register("MutateOutgoingMessages", new MutateOutgoingMessageBehavior(registry.OutgoingMessage), "Executes IMutateOutgoingMessages");
        context.Pipeline.Register("MutateOutgoingTransportMessage", new MutateOutgoingTransportMessageBehavior(registry.OutgoingTransportMessage), "Executes IMutateOutgoingTransportMessages");
    }

    public class RegisteredMutators
    {
        public readonly List<IMutateIncomingMessages> IncomingMessage = [];
        public readonly List<IMutateOutgoingMessages> OutgoingMessage = [];
        public readonly List<IMutateIncomingTransportMessages> IncomingTransportMessage = [];
        public readonly List<IMutateOutgoingTransportMessages> OutgoingTransportMessage = [];
    }
}
namespace NServiceBus.AcceptanceTests.Diagnostics
{
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using AcceptanceTesting;
    using EndpointTemplates;
    using NUnit.Framework;
    using NServiceBus.AcceptanceTesting.Customization;

    [NonParallelizable] // Ensure only activities for the current test are captured
    public class When_sending_replies : NServiceBusAcceptanceTest
    {
        [Test]
        public async Task Should_create_outgoing_message_span()
        {
            using var activityListener = TestingActivityListener.SetupNServiceBusDiagnosticListener();
            var context = await Scenario.Define<Context>()
                .WithEndpoint<TestEndpoint>(b => b
                    .When(s => s.SendLocal(new IncomingMessage())))
                .Done(c => c.OutgoingMessageReceived)
                .Run();

            Assert.AreEqual(activityListener.CompletedActivities.Count, activityListener.StartedActivities.Count, "all activities should be completed");

            var outgoingMessageActivities = activityListener.CompletedActivities.GetOutgoingActivities();
            Assert.AreEqual(2, outgoingMessageActivities.Count, "2 messages are being sent");
            var replyMessage = outgoingMessageActivities[1];

            Assert.AreEqual("reply", replyMessage.DisplayName);
            Assert.IsNotNull(replyMessage.ParentId, "reply should have ambient span");
            var destination = Conventions.EndpointNamingConvention(typeof(TestEndpoint));

            replyMessage.VerifyUniqueTags();
            var replyMessageTags = replyMessage.Tags.ToImmutableDictionary();
            // TODO: Verify whether we want to keep this. messaging.message_id is from the spec
            replyMessageTags.VerifyTag("NServiceBus.MessageId", context.OutgoingMessageId);
            replyMessageTags.VerifyTag("messaging.message_id", context.OutgoingMessageId); // TODO: should be set by the transport? If set by NSB, this should be the transport message id?
            replyMessageTags.VerifyTag("messaging.conversation_id", context.MessageConversationId);
            replyMessageTags.VerifyTag("messaging.operation", "send");
            replyMessageTags.VerifyTag("messaging.destination_kind", "queue");
            replyMessageTags.VerifyTag("messaging.destination", destination);
            // NOTE: Payload size is zero and the tag is not added
            replyMessageTags.VerifyTag("messaging.message_payload_size_bytes", "218");
        }

        class Context : ScenarioContext
        {
            public string MessageConversationId { get; set; }
            public string OutgoingMessageId { get; set; }
            public bool OutgoingMessageReceived { get; set; }
        }

        class TestEndpoint : EndpointConfigurationBuilder
        {
            public TestEndpoint() => EndpointSetup<DefaultServer>();

            class MessageHandler : IHandleMessages<IncomingMessage>,
                IHandleMessages<OutgoingReply>
            {
                Context testContext;

                public MessageHandler(Context testContext) => this.testContext = testContext;

                public Task Handle(IncomingMessage message, IMessageHandlerContext context)
                {
                    return context.Reply(new OutgoingReply());
                }

                public Task Handle(OutgoingReply message, IMessageHandlerContext context)
                {
                    testContext.MessageConversationId = context.MessageHeaders[Headers.ConversationId];
                    testContext.OutgoingMessageId = context.MessageId;
                    testContext.OutgoingMessageReceived = true;
                    return Task.CompletedTask;
                }
            }
        }

        public class IncomingMessage : IMessage
        {
        }

        public class OutgoingReply : IMessage
        {
        }
    }
}